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
        static void Main(string[] args)
        {
            PathHandler pathHandler = new PathHandler();
            GridData gridData = new GridData();

            float testfloat = 9.89731f;
            TimeSpan test = new TimeSpan(0, 0, 0, (int)Decimal.Truncate((Decimal)testfloat), (int)(testfloat % 1));
            Console.WriteLine($"Test converting float to TimeSpan: {testfloat} --> {test}");
            Console.WriteLine($"{(int)(testfloat % 1 *1000)}");

            if (Routines.RunStartUpRoutine())
            {
                int[] xbotIDs = Routines.GetIds();

                var grid = gridData.Data();
                int[] goalPoint0 = new int[2];
                int[] goalPoint1 = new int[2];
                int[] goalPoint2 = new int[2];
                int[] goalPoint3 = new int[2];
                goalPoint0[0] = 0;
                goalPoint0[1] = 3;
                goalPoint1[0] = 4;
                goalPoint1[1] = 7;
                goalPoint2[0] = 3;
                goalPoint2[1] = 0;
                goalPoint3[0] = 4;
                goalPoint3[1] = 7;

                pathHandler.pathing(xbotIDs[0], goalPoint0, grid);
                pathHandler.pathing(xbotIDs[1], goalPoint1, grid);
                //pathHandler.pathing(xbotIDs[2], goalPoint1, grid);
                //pathHandler.pathing(xbotIDs[3], goalPoint3, grid);

                Console.WriteLine($"Desired end point: {goalPoint1[0]}, {goalPoint1[1]}");
                Console.WriteLine($"End point reached: {Routines.GetXbotGridPoint(xbotIDs[0])[0]}, {Routines.GetXbotGridPoint(xbotIDs[0])[1]}");
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
