using PMCLIB;
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
        private static XBotCommands _xbotCommand = new XBotCommands();

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
                MoveBot(xbotID, point);
                Console.WriteLine($"Genrated path: {point}");
            }


        }

        public void MoveBot(int xbot, PointF Pos)
        {
            MotionRtn rtnVal = _xbotCommand.LinearMotionSI(0, xbot, 0, 0, Pos.X, Pos.Y, 0, 0.2, 2);

            //return rtnVal.TravelTimeSecs;
        }
        //TODO implement some kind of path planning here.
    }
}
