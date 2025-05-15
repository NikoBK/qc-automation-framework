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
            for (int i = 0; i < 4; i++) {
                _ = TransportController.MoverToPosition(2, new Vector2(0.650f, 0.875f), PMCLIB.LINEARPATHTYPE.XTHENY, 0.5, 7);
                await Task.Delay(2000);
                _ = TransportController.MoverToPosition(2, new Vector2(0.650f, 0.100f), PMCLIB.LINEARPATHTYPE.XTHENY, 0.5, 7);
                await Task.Delay(2000);
                _ = TransportController.MoverToPosition(2, new Vector2(0.170f, 0.100f), PMCLIB.LINEARPATHTYPE.XTHENY, 0.5, 7);
                await Task.Delay(2000);
                _ = TransportController.MoverToPosition(2, new Vector2(0.170f, 0.875f), PMCLIB.LINEARPATHTYPE.XTHENY, 0.5, 7);
                await Task.Delay(2000);
            }
            _ = TransportController.MoverToPosition(2, Constants.LiquidHandlerPos, PMCLIB.LINEARPATHTYPE.XTHENY, 0.5, 7);
            await Task.Delay(1500); // Let everything finish
        }
    }
}
