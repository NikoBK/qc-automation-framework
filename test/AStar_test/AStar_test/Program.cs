using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace AStar_test
{
    internal class Program
    {
        private static Dictionary<int, Point> goalPoints = new Dictionary<int, Point>();
        static void Main(string[] args)
        {
            PathPlanner pathPlanner = new PathPlanner();
            GridData gridData = new GridData();
            MoveBots moveBots = new MoveBots();

            if (Routines.RunStartUpRoutine())
            {
                int[] xbotIDs = Routines.GetIds();
                gridData.InitGrid();

                for (int i = 0; i < xbotIDs.Length; i++)
                {
                    goalPoints.Add(xbotIDs[i], new Point(5, i));
                }

                foreach (var bot in xbotIDs)
                {
                    List<PointF> path = pathPlanner.Pathing(bot, goalPoints[bot]);

                    foreach (var point in path)
                    {
                        moveBots.MoveBot(bot, point);
                    }
                }

                //Console.WriteLine($"Desired end point: {goalPoint[0]}, {goalPoint[1]}");
                //Console.WriteLine($"End point reached: {Routines.GetXbotGridPoint(xbotIDs[0])[0]}, {Routines.GetXbotGridPoint(xbotIDs[0])[1]}");
            }
            else
            {
                Console.WriteLine("Program aborted...");
            }
        }
    }
}
