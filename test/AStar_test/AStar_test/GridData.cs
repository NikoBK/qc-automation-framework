using Roy_T.AStar_time_expanded.Grids;
using Roy_T.AStar_time_expanded.Paths;
using Roy_T.AStar_time_expanded.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoySize = Roy_T.AStar_time_expanded.Primitives.Size;

namespace AStar_test
{
    public class GridData
    {
        public static Grid grid;

        public void InitGrid(int columns = 8, int rows = 8, float cellDistance = 0.120f, float velocity = 0.1678f)
        {
            var gridSize = new GridSize(columns, rows);
            var cellSize = new RoySize(Distance.FromMeters(cellDistance), Distance.FromMeters(cellDistance));
            var traversalVelocity = Velocity.FromMetersPerSecond(velocity);
            grid = Grid.CreateGridWithLateralConnections(gridSize, cellSize, traversalVelocity);
            
            grid.DisconnectNode(new GridPosition(6, 0));
            grid.DisconnectNode(new GridPosition(7, 0));
            grid.DisconnectNode(new GridPosition(6, 1));
            grid.DisconnectNode(new GridPosition(7, 1));
            grid.DisconnectNode(new GridPosition(6, 2));
            grid.DisconnectNode(new GridPosition(7, 2));
            grid.DisconnectNode(new GridPosition(6, 3));
            grid.DisconnectNode(new GridPosition(7, 3));
            grid.DisconnectNode(new GridPosition(6, 4));
            grid.DisconnectNode(new GridPosition(7, 4));
            grid.DisconnectNode(new GridPosition(6, 5));
            grid.DisconnectNode(new GridPosition(7, 5));
            grid.DisconnectNode(new GridPosition(2, 2));
            grid.DisconnectNode(new GridPosition(2, 3));
            grid.DisconnectNode(new GridPosition(3, 2));
            grid.DisconnectNode(new GridPosition(3, 3));
            grid.DisconnectNode(new GridPosition(2, 4));
            grid.DisconnectNode(new GridPosition(3, 4));
            grid.DisconnectNode(new GridPosition(2, 5));
            grid.DisconnectNode(new GridPosition(3, 5));
        }
    }
}
