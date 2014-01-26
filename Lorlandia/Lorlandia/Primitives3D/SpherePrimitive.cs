using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lorlandia.Primitives3D
{
    public class SpherePrimitive:GeometricPrimitive
    {
        public SpherePrimitive(GraphicsDevice device)
            : this(device, 5, 8, true)
        { }

        //Vector3 _centre;

        //public Vector3 Centre
        //{
        //    get 
        //    {
        //        return Vector3.Transform(_centre, base.world);  
        //    }
        //    private set
        //    {
        //        _centre = value;
        //    }
        //}

        public SpherePrimitive(GraphicsDevice device, float radius, int tessellation, bool quadrangle)
        {
            base.device = device;
            Centre = Vector3.Zero;
            base.quadrangle = quadrangle;
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

            #region TRIANGLE_LIST
            /* FOR TRIANGLE LIST
            for (int ibottom = 0; ibottom < horizontalSegments; ibottom++)
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
            */
            
            #endregion
            
            //for quadrangle
            AddIndex(0);
            for (int i = 0; i < verticalSegments-1; i++)
            {
                for (int j = 0; j <= horizontalSegments; j++)
                {
                    AddIndex(i *horizontalSegments+ (j % (horizontalSegments))+1);
                }
            }
            AddIndex(vertices.Count-1);
            //another ark
            bool up = true;
            for (int i = 0; i < horizontalSegments; i++)
            {
                if (up)
                {
                    for (int j = verticalSegments-1; j > 0; j--)AddIndex(j * horizontalSegments - i);
                    AddIndex(0);
                }
                else
                {
                    for (int j = 1; j < verticalSegments; j++)AddIndex(j * horizontalSegments - i);
                    AddIndex(vertices.Count - 1);
                }
                up = !up;
            }
            
            InitializePrimitive();
        }
    }
}
