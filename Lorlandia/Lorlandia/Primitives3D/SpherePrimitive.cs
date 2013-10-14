using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lorlandia.Primitives3D
{
    class SpherePrimitive:GeometricPrimitive
    {
        public SpherePrimitive(GraphicsDevice device)
            : this(device, 5, 8)
        { }

        public SpherePrimitive(GraphicsDevice device, float radius, int tessellation)
        {
            base.device = device;

            if (tessellation < 4) tessellation = 4;
            int verticalSegments = tessellation;
            int horizontalSegments = tessellation * 2;

            AddVertex(Vector3.Down * radius, Vector3.Down);
            for (int v = 1; v < verticalSegments; v++)
            {
                float dy = (float)Math.Sin(v*Math.PI/verticalSegments - MathHelper.PiOver2);
                float dxz = (float)Math.Cos(v * Math.PI / verticalSegments - MathHelper.PiOver2);
                for (int h = 0; h < horizontalSegments; h++)
                {
                    float dz = (float)Math.Sin(h * MathHelper.TwoPi / horizontalSegments)*dxz;
                    float dx = (float)Math.Cos(h * MathHelper.TwoPi / horizontalSegments)*dxz;
                    Vector3 vertex = new Vector3(dx, dy, dz);
                    AddVertex(vertex * radius, vertex);
                }
            }
            AddVertex(Vector3.Up * radius, Vector3.Up);
            
            for(int ibottom = 0;ibottom<horizontalSegments;ibottom++)
            {
                AddIndex(0);
                AddIndex((ibottom + 2) % horizontalSegments);
                AddIndex(ibottom + 1);
            }

            for (int i = 0; i < verticalSegments - 2; i++)
            {
                for (int j = 0; j < horizontalSegments; j++)
                {
                    AddIndex(horizontalSegments * i + j + 1);
                    AddIndex(horizontalSegments * i + (j + 1) % horizontalSegments + 1);
                    AddIndex(horizontalSegments * (i+1) + j+1);

                    AddIndex(horizontalSegments * (i + 1) + (j + 1) % horizontalSegments + 1);
                    AddIndex(horizontalSegments * (i + 1) + j + 1);
                    AddIndex(horizontalSegments * i + (j + 1) % horizontalSegments + 1);
                }
            }

                for (int itop = 0; itop < horizontalSegments; itop++)
                {
                    AddIndex(LastVertex);
                    AddIndex(LastVertex - (1 + itop));
                    AddIndex(LastVertex - (2 + itop) % horizontalSegments);
                }
            InitializePrimitive();
        }
    }
}
