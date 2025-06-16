using System.Numerics;
using PMCLIB;

namespace AutomationFramework
{
    /// <summary>
    /// MES: Manufacturing Execution System.
    /// <para>
    /// On startup the MES is responseable for handling threads, queue generation, tasks
    /// </para>
    /// </summary>
    public class MES
    {
        public Dictionary<int, Station> Stations { get; private set; }
        public Dictionary<int, Mover> Movers { get; private set; }

        public TrafficPlanner TrafficPlanner { get; private set; }
        public TransportController TransportController { get; private set; }

        private static XBotCommands _xbotCmd = new XBotCommands();

        public Logger logger = new Logger("MES");

        public MES() {
            //Stations = new Dictionary<int, Station>();
            //Movers = new Dictionary<int, Mover>();

            //TrafficPlanner = new TrafficPlanner();
            //TransportController = new TransportController();
        }

        public async Task Test() { //TODO for testing, remove eventually or refactor to start
            // test coord #1: X: 650, Y:900
            // test coord #2: X: 50, Y:550
            _ = TransportController.MoverToPosition(1, Constants.TestPos1);
            await Task.Delay(2000); // Simulate time passing (test buffer)
            //_ = TransportController.MoverToPosition(1, new Vector2(0.050f, 0.550f));
            //_ = TransportController.MoverToPosition(3, new Vector2(0.860f, 0.450f)); //TODO use better coords

            await Task.Delay(1500); // let everything finish
        }

        public void OT2_test()
        {
            LiquidHandler ot2 = new LiquidHandler("169.254.122.228");

            Dictionary<string, Vial> initialVials = new Dictionary<string, Vial>
            {
                { "A1", new Vial(VialType.SampleA, 1500) },
                { "C1", new Vial(VialType.Water, 1000) },
                { "A4", new Vial(VialType.Empty, 0) },
                { "C4", new Vial(VialType.Empty, 0) }
            };
            string path = "C:\\Users\\Nicolai\\qc-automation-framework\\AutomationFramework\\protocols\\recipe_1.py";
            Mover mover = new Mover(1, _xbotCmd);
            VialRack rack = new VialRack(initialVials, path, mover, 1);

            ot2.RecieveRack(rack);

            while (ot2.State != MachineState.STOPPED)
            {

            }
        }

        public void capper_test()
        {
            Vector2 StartPos = new Vector2(0.660f, 0.840f);
            Dictionary<string, Vial> initialVials = new Dictionary<string, Vial>
            {
                { "A1", new Vial(VialType.SampleA, 1500) },
                { "C1", new Vial(VialType.Water, 1000) },
                { "A4", new Vial(VialType.Empty, 0) },
                { "C4", new Vial(VialType.Empty, 0) }
            };
            string path = "C:\\Users\\Nicolai\\qc-automation-framework\\AutomationFramework\\protocols\\recipe_1.py";
            Mover mover = new Mover(2, _xbotCmd);
            VialRack rack = new VialRack(initialVials, path, mover, 1);

            DeCapper capper = new DeCapper();

            while (capper.State != MachineState.STOPPED)
            {
                logger.Log("Going to start pos");
                _xbotCmd.LinearMotionSI(0, rack.rackId, POSITIONMODE.ABSOLUTE, LINEARPATHTYPE.YTHENX, StartPos.X, StartPos.Y, 0, 0.5, 10);

                logger.Log("Moving to Capper Entrance");
                _xbotCmd.LinearMotionSI(0, rack.rackId, POSITIONMODE.ABSOLUTE, LINEARPATHTYPE.YTHENX, capper.Entrance.X, capper.Entrance.Y, 0, 0.5, 10);

                // RUN CAPPING
                capper.RecieveRack(rack);

                while(capper.State == MachineState.RUNNING)
                {

                }

                logger.Log("Capping done. Moving back to start pos");
                _xbotCmd.LinearMotionSI(0, rack.rackId, POSITIONMODE.ABSOLUTE, LINEARPATHTYPE.YTHENX, StartPos.X, StartPos.Y, 0, 0.5, 10);

                capper.ChangeState(MachineState.STOPPED);
            }
        }

        public void MixTest()
        {
            Vector2 StartPos = new Vector2(0.600f, 0.120f);
            Dictionary<string, Vial> initialVials = new Dictionary<string, Vial>
            {
                { "A1", new Vial(VialType.SampleA, 1500) },
                { "C1", new Vial(VialType.Water, 1000) },
                { "A4", new Vial(VialType.Empty, 0) },
                { "C4", new Vial(VialType.Empty, 0) }
            };
            string path = "C:\\Users\\Nicolai\\qc-automation-framework\\AutomationFramework\\protocols\\recipe_1.py";
            Mover mover = new Mover(2, _xbotCmd);
            int id = 1;
            VialRack rack = new VialRack(initialVials, path, mover, id);

            VialMixer mixer = new VialMixer();

            while (mixer.State != MachineState.STOPPED)
            {
                logger.Log("Going to start pos");
                _xbotCmd.LinearMotionSI(0, rack.rackId, POSITIONMODE.ABSOLUTE, LINEARPATHTYPE.XTHENY, StartPos.X, StartPos.Y, 0, 0.5, 10);

                logger.Log("Moving to Mixer Entrance");
                _xbotCmd.LinearMotionSI(0, rack.rackId, POSITIONMODE.ABSOLUTE, LINEARPATHTYPE.YTHENX, mixer.Entrance.X, mixer.Entrance.Y, 0, 0.5, 10);

                logger.Log($"Giving rack with id {rack.rackId} to mixer and awaiting mixing process");
                mixer.RecieveRack(rack);
                logger.Log($"Mixing station state: {mixer.State}");

                while(mixer.State == MachineState.RUNNING)
                {

                }

                logger.Log("Mixing done. Moving back to start pos");
                _xbotCmd.LinearMotionSI(0, rack.rackId, POSITIONMODE.ABSOLUTE, LINEARPATHTYPE.XTHENY, StartPos.X, StartPos.Y, 0, 0.5, 10);

                mixer.ChangeState(MachineState.STOPPED);
            }
        }
    }
}
