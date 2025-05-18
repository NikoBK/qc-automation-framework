using System.Collections;
using Roy_T.AStar.Grids;
using Roy_T.AStar.Primitives;
using Roy_T.AStar.Paths;

namespace AutomationFramework
{
    /// <summary>
    /// Responsible for path generation, motion planning and execution on movements along paths through the
    /// <see="TransportController">
    /// </summary>
    public class TrafficPlanner
    {
        // Maps chunk labels to their positions in the chunk grid (2x2 tiles on the acopos6d)
        static readonly Dictionary<string, (int chunkX, int chunkY)> ChunkLayout = new() {
            ["1"] = (0, 0), ["2"] = (1, 0), ["3"] = (2, 0), ["4"] = (3, 0),
            ["5"] = (0, 1), ["6"] = (1, 1), ["7"] = (2, 1),
            ["8"] = (0, 2), ["x"] = (1, 2), ["9"] = (2, 2),
            ["a"] = (0, 3), ["b"] = (1, 3), ["c"] = (2, 3),
        };

        public TransportController TransportController { get; private set; }

        public TrafficPlanner(TransportController transportController) {
            TransportController = transportController;
            Initialize();
        }

        private void Initialize()
        {
            int cols = 8; // 4 chunks max width * 2 tiles
            int rows = 8; // 4 chunks tall * 2 tiles

            var gridSize = new GridSize(cols, rows);
            var grid = Grid.CreateGridWithLateralConnections(
                gridSize,
                new Roy_T.AStar.Primitives.Size(Distance.FromMeters(1), Distance.FromMeters(1)),
                                                             Velocity.FromMetersPerSecond(1)
            );

            // Disconnect all tiles not part of a valid chunk (except obstacle "x")
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    var pos = new GridPosition(x, y);
                    if (!IsInAnyChunk(pos) || IsInChunk(pos, ChunkLayout["x"]))
                    {
                        grid.DisconnectNode(pos);
                        grid.RemoveDiagonalConnectionsIntersectingWithNode(pos);
                    }
                }
            }

            // Define obstacle tiles (chunk "x")
            var obstacles = GetChunkTiles(ChunkLayout["x"]);
            foreach (var pos in obstacles)
            {
                grid.DisconnectNode(pos);
                grid.RemoveDiagonalConnectionsIntersectingWithNode(pos);
            }

            var start = new GridPosition(0, 0);    // chunk "1"
            var end = new GridPosition(5, 7);      // chunk "c"

            var path = new PathFinder().FindPath(start, end, grid);
            var pathSet = new HashSet<(int, int)>();
            foreach (var edge in path.Edges)
            {
                pathSet.Add(((int)edge.Start.Position.X, (int)edge.Start.Position.Y));
            }

            Console.WriteLine($"pathset has: {pathSet.Count} indices");

            Console.WriteLine("\nA* Pathfinding Grid (12 chunks / 48 tiles):\n");
            PrintAsciiGrid(cols, rows, start, end, pathSet, obstacles);

            Console.WriteLine("traffic planner initialized");
            Console.WriteLine("Initiating mover 1 on path...");

            Task.Run(async () => { await TransportController.MoverOnPath(1, pathSet); }); //TODO make this assigned id dynamic
        }

        private bool IsInChunk(GridPosition pos, (int cx, int cy) chunk)
        {
            foreach (var tile in GetChunkTiles(chunk))
            {
                if (tile.X == pos.X && tile.Y == pos.Y)
                    return true;
            }
            return false;
        }


        private List<GridPosition> GetChunkTiles((int cx, int cy) chunk)
        {
            int x = chunk.cx * 2;
            int y = chunk.cy * 2;
            return new List<GridPosition>
            {
                new GridPosition(x, y),
                new GridPosition(x + 1, y),
                new GridPosition(x, y + 1),
                new GridPosition(x + 1, y + 1)
            };
        }

        private void PrintAsciiGrid(int cols, int rows, GridPosition start, GridPosition end, HashSet<(int, int)> pathSet, List<GridPosition> obstacles)
        {
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                    Console.Write("+---");
                Console.WriteLine("+");

                for (int x = 0; x < cols; x++)
                {
                    Console.Write("| ");
                    var pos = new GridPosition(x, y);
                    if (!IsInAnyChunk(pos))
                    {
                        Console.Write("  "); // outside layout
                    }
                    else if (x == start.X && y == start.Y)
                        Console.Write("S ");
                    else if (x == end.X && y == end.Y)
                        Console.Write("E ");
                    else if (obstacles.Contains(pos))
                        Console.Write("# ");
                    else if (pathSet.Contains((x, y)))
                        Console.Write("* ");
                    else
                        Console.Write(". ");
                }
                Console.WriteLine("|");
            }

            for (int x = 0; x < cols; x++)
                Console.Write("+---");
            Console.WriteLine("+");

            Console.WriteLine("\nLegend: S = Start, E = End, * = Path, # = Obstacle, . = Free, (blank) = outside layout");
        }

        private bool IsInAnyChunk(GridPosition pos)
        {
            foreach (var chunk in ChunkLayout.Values)
            {
                foreach (var tile in GetChunkTiles(chunk))
                {
                    if (tile.X == pos.X && tile.Y == pos.Y)
                        return true;
                }
            }
            return false;
        }
    }
}
