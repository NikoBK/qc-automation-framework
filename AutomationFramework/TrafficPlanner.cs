using System.Collections;
using System.Drawing;

namespace AutomationFramework
{
    /// <summary>
    /// Responsible for path generation, motion planning and execution on movements along paths through the
    /// <see="TransportController">
    /// </summary>
    public class TrafficPlanner
    {
        PathPlanner pathPlanner;
        GridData gridData;

        public TrafficPlanner() {
            pathPlanner = new PathPlanner();
            gridData = new GridData();
            Console.WriteLine("traffic planner initialized!");
        }

        public void Initialize() {
            gridData.InitGrid();
        }

        public void GeneratePath(int xbotID, Point goalPoint)
        {
            List<PointF> path = pathPlanner.Pathing(xbotID, goalPoint);

            foreach (var point in path)
            {
                Console.WriteLine($"Genrated path: {point}");
            }
        }

        //TODO implement some kind of path planning here.
    }
}
