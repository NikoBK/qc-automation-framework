using System.Collections;

namespace automationframework
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

        public TransportManager TransportManager { get; private set; }

        public MES() {
            Stations = new Dictionary<int, Station>();
            Movers = new Dictionary<int, Mover>();

            TransportManager = new TransportManager();
        }

        /// <summary>
        /// Start the MES system, see <cref="MES"> on how it operates after start.
        /// </summary>
        public void Start() {
            // TODO: Implement something here.
            TransportManager.MoveMoverToStation(0, 1); // NOTE: this is just a test
            Console.WriteLine("MES Started!");
        }
    }
}
