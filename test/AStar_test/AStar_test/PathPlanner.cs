using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Roy_T.AStar_time_expanded.Grids;
using Roy_T.AStar_time_expanded.Primitives;
using Roy_T.AStar_time_expanded.Paths;
using RoySize = Roy_T.AStar_time_expanded.Primitives.Size;
using PMCLIB;

namespace AStar_test
{
    public class PathPlanner
    {
        private static SystemCommands _systemCommand = new SystemCommands();
        private static XBotCommands _xbotCommand = new XBotCommands();
        public List<PointF> Pathing(int xbotID, Point goalPoint)
        {
            PathFinder pathFinder = new PathFinder();

            List<PointF> pathPoints = new List<PointF>();

            int[] currentPoint = GetXbotGridPoint(xbotID);

            Path path = pathFinder.FindPath(new GridPosition(currentPoint[0], currentPoint[1]), new GridPosition(goalPoint.X, goalPoint.Y), GridData.grid);

            //Console.WriteLine($"type: {path.Type}, distance: {path.Distance}, duration {path.Duration}");

            //Console.WriteLine($"Edge count: {path.Edges.Count}");

            foreach (var edge in path.Edges)
            {
                float pointX = edge.End.Position.X + 0.06f;
                float pointY = edge.End.Position.Y + 0.06f;

                pathPoints.Add(new PointF(pointX, pointY));
            }
            return pathPoints;
        }
        private static int[] GetXbotGridPoint(int xbot)
        {
            int[] point = new int[2];
            int[] IDs = GetIds();
            int xbotIndex = Array.IndexOf(IDs, xbot);
            AllXBotInfo xbotInfo = _xbotCommand.GetAllXbotInfo(ALLXBOTSFEEDBACKOPTION.POSITION);

            XBotInfo xbotPos = xbotInfo.AllXbotInfoList[xbotIndex];

            point[0] = (int)Math.Round((xbotPos.XPos - 0.06) / 120 * 1000);
            point[1] = (int)Math.Round((xbotPos.YPos - 0.06) / 120 * 1000);

            return point;
        }
        private static int[] GetIds()
        {
            XBotIDs xBot_IDs = _xbotCommand.GetXBotIDS();
            int[] xBotIDs = xBot_IDs.XBotIDsArray;

            return xBotIDs;
        }
    }
}
