using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lorlandia.Physics
{
    class QuadGridColision
    {
        public Intersection GetCurrentHeight(Vector3[,] vertex_grid, Vector3 position, out VertexPositionColor[] vertives)
        {
            vertives = new VertexPositionColor[4];
            Intersection intersection = Intersection.None;
            //height = 0.0f;
            int grid_width = vertex_grid.GetLength(0) - 1;
            int grid_height = vertex_grid.GetLength(1) - 1;
            Vector3 first = vertex_grid[0, 0];
            Vector3 last = vertex_grid[grid_width, grid_height];
            if (position.X >= first.X && position.X <= last.X && position.Z >= first.Z && position.Z <= last.Z)
            {
                intersection = Intersection.Inside;
                int cellX = (int)Math.Floor(first.X - position.X);
                int cellY = (int)Math.Floor(first.Z - position.Z);

            }
            return intersection;
        }
    }
}
