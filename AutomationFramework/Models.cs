
using PMCLIB;
using System.Numerics;

namespace AutomationFramework
{
    public enum MachineState
    {
        IDLE = 0,          // Machine is waiting for command
        STARTING,      // Preparing for operation
        RUNNING,       // Machine is running
        PAUSED,        // Machine is paused
        STOPPED,       // Machine has been stopped
        FAULT         // Machine is in fault (error state)
    }

    public enum VialType {
        Empty = 0,
        Insulin
    }

    public class Vial
    {
        public VialRack? Rack { get; set; }
        public VialType? Type { get; set; }
    }

    public class VialRack
    {
        public Mover? Mover;
    }

    public class Mover
    {
        public int Id { get; }
        public float X { get; private set; }
        public float Y { get; private set; }

        public Mover(int id) {
            Id = id;
        }

        /// <summary>
        /// Return a boolean that represents whether or not the mover is currently idle.
        /// </summary>
        public bool IsIdle(XBotCommands cmds) {
            return cmds.GetXbotStatus(Id).XBOTState == XBOTSTATE.XBOT_IDLE;
        }

        /// <summary>
        /// Moves a <see="Mover"> to a vector2 defined position in planar (2D) environment.
        /// </summary>
        public async Task MoverToPosition(XBotCommands cmd, Vector2 pos,
            ushort cmdLabel = 0, POSITIONMODE posMode = POSITIONMODE.ABSOLUTE, LINEARPATHTYPE pathType = LINEARPATHTYPE.DIRECT,
            double finalSpdMetersPs = 0, double maxSpdMetersPs = 0.5, double maxAccelerationMetersPs2 = 10)
        {
            Console.WriteLine($"Shuttle {Id} is moving!");
            cmd.LinearMotionSI(cmdLabel, Id, posMode, pathType, pos.X, pos.Y, finalSpdMetersPs, maxSpdMetersPs, maxAccelerationMetersPs2);
            Console.WriteLine("finished moving");

            await Task.Delay(1000); // Buffer time to get the mover moving.
            Console.WriteLine("time delay of 1s passed");
        }
    }

    public class Station
    {
        public string? Name { get; private set; }
        public MachineState State { get; private set; }

        public virtual void Start(string name) {
            Name = name;
            State = MachineState.IDLE;
        }
    }
}
