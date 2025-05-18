using System.Numerics;

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

        public MES() {
            Stations = new Dictionary<int, Station>();
            Movers = new Dictionary<int, Mover>();

            TransportController = new TransportController();
            TrafficPlanner = new TrafficPlanner(TransportController);
        }
    }
}
