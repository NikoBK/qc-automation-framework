using PMCLIB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace AStar_test
{
    class MoveBots
    {
        private static SystemCommands _systemCommand = new SystemCommands();
        private static XBotCommands _xbotCommand = new XBotCommands();

        public static double MoveBot(int xbot, PointF Pos)
        {
            //Console.WriteLine($"X: {Pos.X}, Y: {Pos.Y}");

            MotionRtn rtnVal = _xbotCommand.LinearMotionSI(0, xbot, 0, 0, Pos.X, Pos.Y, 0, 0.2, 2);

            return rtnVal.TravelTimeSecs;

        }
    }
}
