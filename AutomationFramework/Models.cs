
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
        Insulin,
        SampleA,
        Water
    }

    public class Vial
    {
        public VialType? Type { get; set; }
        public int? Volume { get; set; }

        public Vial(VialType type, int vol)
        {
            Type = type;
            Volume = vol;
        }
    }

    public class VialRack
    {
        public int rackId; // Maybe create a function for generating a rack id?
        public Dictionary<string, Vial> rack = new Dictionary<string, Vial>();
        private static readonly string[] _validPositions = _GenerateValidPositions();
        public string? protocolPath;
        public Mover mover;

        public VialRack(Dictionary<string, Vial> initialVials, string path, Mover newMover, int id)
        {
            foreach (var vial in initialVials)
            {
                bool ret = AddVial(vial.Key, vial.Value); // vial.Key is the position, and vial.Value is objectype Vial.
                if (!ret)
                {
                    Console.WriteLine($"Failed to add vial at positon {vial.Key}");
                }
            }
            protocolPath = path;
            mover = newMover;
            rackId = id;
        }

        public bool AddVial(string position, Vial vial)
        {
            if (!_IsValidPosition(position))
            {
                return false;
            }

            rack[position] = vial;
            return true;
        }

        private static string[] _GenerateValidPositions()
        {
            var positions = new List<string>();
            for (char col = 'A'; col <= 'E'; col++)
            {
                for (int row = 1; row <= 4; row++)
                {
                    positions.Add($"{col}{row}");
                }
            }
            return positions.ToArray();
        }

        private static bool _IsValidPosition(string pos)
        {
            return Array.Exists(_validPositions, p => p == pos);
        }
    }

    public class Mover
    {
        public int Id { get; }
        public float X { get; private set; }
        public float Y { get; private set; }

        private XBotCommands? cmds;
        public Mover(int id, XBotCommands commands) {
            Id = id;
            cmds = commands;
        }

        /// <summary>
        /// Return a boolean that represents whether or not the mover is currently idle.
        /// </summary>
        public bool IsIdle() {
            return cmds.GetXbotStatus(Id).XBOTState == XBOTSTATE.XBOT_IDLE;
        }

        /// <summary>
        /// Moves a <see="Mover"> to a vector2 defined position in planar (2D) environment.
        /// </summary>
        public async Task MoverToPosition(Vector2 pos,
            ushort cmdLabel = 0, POSITIONMODE posMode = POSITIONMODE.ABSOLUTE, LINEARPATHTYPE pathType = LINEARPATHTYPE.DIRECT,
            double finalSpdMetersPs = 0, double maxSpdMetersPs = 0.5, double maxAccelerationMetersPs2 = 10)
        {
            Console.WriteLine($"Shuttle {Id} is moving!");
            cmds.LinearMotionSI(cmdLabel, Id, posMode, pathType, pos.X, pos.Y, finalSpdMetersPs, maxSpdMetersPs, maxAccelerationMetersPs2);
            Console.WriteLine("finished moving");

            await Task.Delay(1000); // Buffer time to get the mover moving.
            Console.WriteLine("time delay of 1s passed");
        }
    }

    public class Station
    {
        public string? Name { get; private set; }
        public MachineState State { get; private set; }
        public Vector2 Entrance;
        public Vector2 Exit;
        public VialRack? rack;
        public Logger? logger;

        public virtual void Start(string name, Vector2 entrance, Vector2 exit) {
            Name = name;
            Entrance = entrance;
            Exit = exit;
            logger = new Logger(name);
            logger.Log($"Created station object");
            State = MachineState.IDLE;
        }

        public void ChangeState(MachineState newState)
        {
            if (State == newState)
            {
                logger.Log($"Already in state: {newState}");
                return;
            }

            MachineState previousState = State;

            switch (newState)
            {
                case MachineState.STARTING:
                    logger.Log($"Changing state from {previousState} to STARTING...");
                    State = MachineState.STARTING;
                    break;

                case MachineState.IDLE:
                    logger.Log($"Changing state from {previousState} to IDLE...");
                    State = MachineState.IDLE;
                    break;

                case MachineState.RUNNING:
                    logger.Log($"Changing state from {previousState} to RUNNING...");
                    State = MachineState.RUNNING;
                    break;

                case MachineState.STOPPED:
                    logger.Log($"Changing state from {previousState} to STOPPED...");
                    State = MachineState.STOPPED;
                    break;

                case MachineState.FAULT:
                    logger.Log($"Changing state from {previousState} to FAULT...");
                    State = MachineState.FAULT;
                    break;

                default:
                    logger.Log($"Unknown state requested: {newState}. Current state remains {State}.");
                    break;
            }
        }
    }

    public class Chunk
    {
        public int Id { get; private set; }
        public List<Tile> Tiles = new List<Tile>();

        public Chunk(int id) {
            Id = id;
        }
    }

    public class Tile
    {
        public int Id { get; private set; }

        public Tile(int id) {
            Id = id;
        }
    }
}
