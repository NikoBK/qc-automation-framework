using PMCLIB;
using System.Numerics;

namespace AutomationFramework
{
    public class TransportController
    {
        private static SystemCommands _sysCmd = new SystemCommands();
        private static XBotCommands _xbotCmd = new XBotCommands();

        public TransportController() {
            Console.WriteLine("Transport Controller initialized");
            bool started = RunStartupRoutine();
            if (started) {
                Console.WriteLine("Startup routine returned success");
            }
            else {
                Console.WriteLine("Startup routine returned failure");
            }
        }

        public void MoveMoverToStation(int moverId, int stationId) {
            //TODO implement something here
        }

        public void MoverToPosition(int moverId, Vector2 pos,
            ushort cmdLabel = 0, POSITIONMODE posMode = POSITIONMODE.ABSOLUTE, LINEARPATHTYPE pathType = LINEARPATHTYPE.DIRECT,
                double finalSpdMetersPs = 0, double maxSpdMetersPs = 0.5, double maxAccelerationMetersPs2 = 0)
        {
            _xbotCmd.LinearMotionSI(cmdLabel, moverId, posMode, pathType, pos.X, pos.Y, finalSpdMetersPs, maxSpdMetersPs, maxAccelerationMetersPs2);
        }


        public static int[] GetIds()
        {
            XBotIDs xBot_IDs = _xbotCmd.GetXBotIDS();
            int[] xBotIDs = xBot_IDs.XBotIDsArray;
            return xBotIDs;
        }

        public static bool RunStartupRoutine(int expectedXbotCount = 0)
        {
            #region Connect to PMC
            //first, we attempt to connect to the PMC through ethernet
            //this method will return true if the connection is successful, or false if a PMC is not found
            Console.WriteLine("Connecting to the Planar Motor Controller...");
            bool isConnectedToPMC = _sysCmd.AutoSearchAndConnectToPMC();   //this will automatically search for and connect to the PMC
            //bool isConnectedToPMC = _sysCmd.ConnectToSpecificPMC("192.168.10.100");   //this will connect to a PMC at the specified IP address
            if (isConnectedToPMC == false)   //check if the connection to the PMC is successful
            {
                Console.WriteLine("Failed to connect to the Planar Motor Controller");
                return false;
            }
            #endregion Connect to PMC

            #region Gain Mastership
            //attempt to gain mastership of the system. Only 1 program will have the mastership of the system at a time.
            Console.WriteLine("Gaining Mastership...");
            PMCRTN rtnVal = _sysCmd.GainMastership();    //sends the gain mastership command
            if (rtnVal != PMCRTN.ALLOK) //the PMCRTN.ALLOK value indicates mastership has been granted to us successfully
            {
                Console.WriteLine("Failed to gain mastership of the system. Error: " + rtnVal.ToString());
                return false;
            }
            //PMCRTN rtnVal = PMCRTN.ALLOK;
            #endregion Gain Mastership

            #region Check PMC Status and bring it into operation
            //check if the PMC is in operation
            PMCSTATUS pmcStat = _sysCmd.GetPMCStatus(); //send the get PMC status command

            //if the PMC is not in the Operation State, then we run the following code to bring it into the operation state
            if (pmcStat != PMCSTATUS.PMC_FULLCTRL && pmcStat != PMCSTATUS.PMC_INTELLIGENTCTRL)
            {
                //we now check for the PMC State
                bool isPMCinOperation = false; //check if PMC is in operation
                bool attemptedActivation = false;    //safety counter to avoid infinite loop in the while loop below
                while (isPMCinOperation == false)
                {
                    //get the PMC status, then filter PMC states to see if it in a transition state
                    pmcStat = _sysCmd.GetPMCStatus(); //send the get PMC status command
                    switch (pmcStat)
                    {
                        //PMC is in transition states
                        case PMCSTATUS.PMC_ACTIVATING:
                        case PMCSTATUS.PMC_BOOTING:
                        case PMCSTATUS.PMC_DEACTIVATING:
                        case PMCSTATUS.PMC_ERRORHANDLING:
                            isPMCinOperation = false;
                            System.Threading.Thread.Sleep(1000);    //if the system is in a transition state, then delay 1 second before reading the PMC state again
                            break;
                            //PMC is in a stable state but is not in operation
                        case PMCSTATUS.PMC_ERROR:
                        case PMCSTATUS.PMC_INACTIVE:
                            isPMCinOperation = false;
                            //if the system is not in the Operation (Full control) state, then we run the Activate XBOTs command to bring it into the operation state
                            if (attemptedActivation == false)
                            {
                                //Activate XBOTs everywhere (in all zones) on the flyway
                                Console.WriteLine("Activate all xbots");
                                rtnVal = _xbotCmd.ActivateXBOTS();
                                attemptedActivation = true;  //only attempt to send the Activate xbots command once
                                //check the return value to see if the command was accepted
                                if (rtnVal != PMCRTN.ALLOK)
                                {
                                    Console.WriteLine("Failed to Activate XBOTs. Error: " + rtnVal.ToString());
                                    return false;
                                }
                            }
                            else
                            {
                                //failed to Activate XBOTs successfully, quit the program
                                Console.WriteLine("Attempted but failed to Activate XBOTs.");
                                return false;
                            }
                            break;
                            //PMC is now in operation
                        case PMCSTATUS.PMC_FULLCTRL:
                        case PMCSTATUS.PMC_INTELLIGENTCTRL:
                            isPMCinOperation = true;
                            break;
                            //unknown or unexpected PMC state. to be safe we can exit the program.
                        default:
                            //unexpected PMC state, quit the program
                            Console.WriteLine("Unexpected PMC State. Error: " + pmcStat.ToString());
                            return false;
                    }
                }
            }
            #endregion Check PMC Status and bring it into operation

            #region Check XBOT Count
            //PMC is the the operation state, we can proceed to check the xbot count
            XBotIDs xBot_IDs = _xbotCmd.GetXBotIDS();   //this command retrieves the XBOT IDs from the PMC. The return value contains the xbot count, and an array with all the XBOT IDs
            if (xBot_IDs.PmcRtn == PMCRTN.ALLOK)
            {
                //if the user wants us to check the xbot count, then we will
                if (expectedXbotCount > 0)
                {
                    if (xBot_IDs.XBotCount != expectedXbotCount)
                    {
                        Console.WriteLine("Incorrect number of XBOTs. Detected XBOT count: " + xBot_IDs.XBotCount.ToString());
                        return false;
                    }
                }
            }
            else
            {
                Console.WriteLine("Failed to get xbot IDs. Error: " + rtnVal.ToString());
                return false;
            }
            #endregion Check XBOT Count

            #region Stop any existing XBOT motions
            //we will run the stop xbot motion command first, to bring any xbots that are in motion to a stop.
            //the stop motion command will also clear any remaining commands in the XBOT's motion buffer
            _xbotCmd.StopMotion(0); //sending 0 for XBOT id here means all XBOTs will be stopped
            #endregion

            #region check xbot states and Levitate XBOTs
            //now we check the XBOT states to see if we need to levitate them
            bool areXBOTsLevitated = false; //we need to wait until the XBOTs are levitated
            bool attemptedLevitation = false;    //safety counter to avoid infinite loop in the while loop below
            bool areXBOTsInTransitionState = false;  //check if xbots are in transition states

            //use the follow while loop to make sure all xbots are levitated
            while (areXBOTsLevitated == false)
            {
                areXBOTsLevitated = true;
                areXBOTsInTransitionState = false;
                //check the status of all xbots
                for (int i = 0; i < xBot_IDs.XBotCount; i++)
                {
                    //this command retrieves the status of 1 XBOT. for simplificity of reading, we did not check the return code from the command.
                    XBotStatus currXbotStatus = _xbotCmd.GetXbotStatus(xBot_IDs.XBotIDsArray[i]);
                    //we obtain the xbot state from the return value
                    XBOTSTATE currXbotState = currXbotStatus.XBOTState;

                    switch (currXbotState)
                    {
                        //xbots are activated but still need to be levitated
                        case XBOTSTATE.XBOT_LANDED:
                            areXBOTsLevitated = false;
                            break;

                            //transition states, wait for XBOT to leave these states
                        case XBOTSTATE.XBOT_STOPPING:
                        case XBOTSTATE.XBOT_DISCOVERING:
                        case XBOTSTATE.XBOT_MOTION: //xbot motion is not technically a transition state, but the xbot enters the motion state in order to go from landed to levitated or from disabled to levitated
                            areXBOTsLevitated = false;
                            areXBOTsInTransitionState = true;
                            break;

                            //this particular XBOT is ready to work, continue to check remaining xbots
                        case XBOTSTATE.XBOT_IDLE:
                        case XBOTSTATE.XBOT_STOPPED:

                            /*xbot state ready to move*/
                            break;

                            //XBOT in motion states, the stop motion command didn't work, report error
                        case XBOTSTATE.XBOT_WAIT:
                        case XBOTSTATE.XBOT_OBSTACLE_DETECTED:
                        case XBOTSTATE.XBOT_HOLDPOSITION:
                            Console.WriteLine("Error. Failed to stop XBOT motion. XBot State: " + currXbotState.ToString());
                            return false;
                        case XBOTSTATE.XBOT_DISABLED:
                            Console.WriteLine("Error. Cannot levitate from this XBOT state. XBot State: " + currXbotState.ToString());
                            return false;

                            //unexpected XBOT states, report error
                        default:
                            Console.WriteLine("Error. Unexpected XBOT state. XBot State: " + currXbotState.ToString());
                            return false;
                    }
                }

                //if none of the xbots are in transition states, and one or more of them are not levitated, then we run the levitation command
                if (areXBOTsLevitated == false && areXBOTsInTransitionState == false)
                {
                    //see if we have attempted to activate the xbot already
                    if (attemptedLevitation == false)
                    {
                        //levitate all XBOTs on the flyway
                        Console.WriteLine("Levitating XBOTs...");
                        //this is the levitation control command. sending 0 for xbot ID means all xbots, and the option is levitate.
                        rtnVal = _xbotCmd.LevitationCommand(0, LEVITATEOPTIONS.LEVITATE);
                        attemptedLevitation = true; //so we don't attempt levitation multiple times
                        if (rtnVal != PMCRTN.ALLOK)
                        {
                            Console.WriteLine("Failed to levitate XBOTs. Error: " + rtnVal.ToString());
                            return false;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Attempted but failed to levitate XBOTs. Error: " + rtnVal.ToString());
                        return false;
                    }
                }
            }
            //All xbots are levitated and ready to go
            Console.WriteLine("All XBOTs levitated and ready to go!");
            #endregion check xbot states and Levitate XBOTs

            //all parts passed, return success
            return true;
        }
    }
}
