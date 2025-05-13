using Roy_T.AStar.Grids;
using Roy_T.AStar.Paths;
using Roy_T.AStar.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoySize = Roy_T.AStar.Primitives.Size;
using DrawSize = System.Drawing.Size;

namespace AStar_test
{
    public class GridData
    {
        public Grid Data()
        {
            var gridSize = new GridSize(columns: 8, rows: 8);
            var cellSize = new RoySize(Distance.FromMeters(0.120f), Distance.FromMeters(0.120f));
            var traversalVelocity = Velocity.FromMetersPerSecond(0.2f);
            var grid = Grid.CreateGridWithLateralConnections(gridSize, cellSize, traversalVelocity);

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

            return grid;
        }
    }
}
