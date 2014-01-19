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
        public Vector3 GetCurrentHeight(Vector3[,] vertex_grid, Vector3 position)
        {
            Vector3 new_position = position;
            //height = 0.0f;
            int grid_width = vertex_grid.GetLength(0) - 1;
            int grid_height = vertex_grid.GetLength(1) - 1;
            Vector3 first = vertex_grid[0, 0];
            Vector3 last = vertex_grid[grid_width, grid_height];
            if (new_position.X < first.X) new_position.X = first.X;
            else if (new_position.X > last.X) new_position.X = last.X;
            if (new_position.Z < first.Z) new_position.Z = first.Z;
            else if (new_position.Z > last.Z) new_position.Z = last.Z;

            int cellX = (int)Math.Floor(new_position.X - first.X);
            int cellY = (int)Math.Floor(new_position.Z - first.Z);

            float dX = new_position.X - cellX;
            float dY = new_position.Z - cellY;

            Vector3 V0 = vertex_grid[cellX, cellY];
            Vector3 V1 = Vector3.Zero;
            Vector3 V2 = Vector3.Zero;
            if (dX > dY)
            {
                V1 = vertex_grid[cellX+1, cellY];
                V2 = vertex_grid[cellX+1, cellY+1];
           }
           else
           {
                V1 = vertex_grid[cellX+1, cellY+1];
                V2 = vertex_grid[cellX, cellY+1];
           }
           Vector3 normal = Vector3.Cross(V1-V0, V2-V0);
           //normal.Normalize();
           new_position.Y = V0.Y + (normal.X * dX + normal.Z * dY) / -normal.Y;
            
            return new_position;
        }
    }
}
