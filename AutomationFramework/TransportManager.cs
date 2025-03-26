using PMCLIB;

namespace automationframework
{
    public class TransportManager
    {
        public SystemCommands _sysCmd = new SystemCommands();
        public XBotCommands _xbotCmd = new XBotCommands();

        public TransportManager() {
            Console.WriteLine("Transport Manager initialized");
        }

        public void MoveMoverToStation(int moverId, int stationId) {
            //TODO implement something here
        }
    }
}
