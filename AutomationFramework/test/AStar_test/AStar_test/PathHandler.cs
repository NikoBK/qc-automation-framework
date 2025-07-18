using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Roy_T.AStar.Grids;
using Roy_T.AStar.Primitives;
using Roy_T.AStar.Paths;
using RoySize = Roy_T.AStar.Primitives.Size;
using DrawSize = System.Drawing.Size;

namespace AStar_test
{
    public class PathHandler
    {
        public void pathing(int xbotID, int[] goalPoint, Grid grid)
        {
            int[] currentPoint = Routines.GetXbotGridPoint(xbotID);
            PathFinder pathFinder = new PathFinder();

            var path = pathFinder.FindPath(new GridPosition(currentPoint[0], currentPoint[1]), new GridPosition(goalPoint[0], goalPoint[1]), grid);

            Console.WriteLine($"type: {path.Type}, distance: {path.Distance}, duration {path.Duration}");

            Console.WriteLine($"Edge count: {path.Edges.Count}");

            PointF point = new PointF();
            double travelTime = 0;

            DateTime startTime = new DateTime();

            for (int i = 0; i < path.Edges.Count; i++)
            {
                Console.WriteLine($"X: {path.Edges[i].End.Position.X}, Y: {path.Edges[i].End.Position.Y}");

                point.X = path.Edges[i].End.Position.X + 0.06f;
                point.Y = path.Edges[i].End.Position.Y + 0.06f;

                if (i == 0)
                {
                    startTime = DateTime.Now;
                }
                travelTime += MoveBots.MoveBot(xbotID, new PointF(point.X, point.Y));
            }
            Console.WriteLine($"ACOPOS estimated travel time: {travelTime}");

            bool botIdle = false;
            while (!botIdle)
            {
                botIdle = Routines.XbotIdle(xbotID);
            }
            TimeSpan timeElapsed = DateTime.Now - startTime;
            Console.WriteLine($"{timeElapsed}");
        }
    }
}
