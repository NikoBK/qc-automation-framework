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

        public MES() {
            Stations = new Dictionary<int, Station>();
            Movers = new Dictionary<int, Mover>();
        }

        /// <summary>
        /// Start the MES system, see <cref="MES"> on how it operates after start.
        /// </summary>
        public void Start() {
            // TODO: Implement something here.
            Console.WriteLine("MES Started!");
        }
    }
}
