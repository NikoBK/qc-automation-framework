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

            TrafficPlanner = new TrafficPlanner();
            TransportController = new TransportController();
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
    }
}
