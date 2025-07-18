using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Roy_T.AStar.Grids;
using Roy_T.AStar.Primitives;
using Roy_T.AStar.Paths;

namespace AStar_test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PathHandler pathHandler = new PathHandler();
            GridData gridData = new GridData();

            if (Routines.RunStartUpRoutine())
            {
                int[] xbotIDs = Routines.GetIds();

                var grid = gridData.Data();
                int[] goalPoint = new int[2];
                goalPoint[0] = 5;
                goalPoint[1] = 0;

                pathHandler.pathing(xbotIDs[0], goalPoint, grid);

                Console.WriteLine($"{Routines.GetXbotGridPoint(xbotIDs[0])[0]}, {Routines.GetXbotGridPoint(xbotIDs[0])[1]}");
            }
            else
            {
                Console.WriteLine("Program aborted...");
            }
            /*
            int[] xbotIDs = Routines.GetIds();

            var grid = gridData.Data();
            int[] goalPoint = new int[2];
            goalPoint[0] = 0;
            goalPoint[1] = 5;

            pathHandler.pathing(xbotIDs[0], goalPoint, grid);

            Console.WriteLine($"{Routines.GetXbotGridPoint(xbotIDs[0])[0]}, {Routines.GetXbotGridPoint(xbotIDs[0])[1]}");
            */
        }
    }
}
