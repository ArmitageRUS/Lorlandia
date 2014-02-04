using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lorlandia.Primitives3D
{
    public class PlanePrimitive:GeometricPrimitive
    {
        int xSegments = 0;
        int ySegments = 0;
        float scale = 1.0f;
        Matrix world = Matrix.Identity;

        public PlanePrimitive(GraphicsDevice device, int x_segments, int y_segments, float scale)
        {
            base.device = device;
            base.quadrangle = true;
            this.xSegments = x_segments;
            this.ySegments = y_segments;
            this.scale = scale;
            CreateGeometry();
            world = Matrix.CreateTranslation(new Vector3(-(xSegments / 2.0f), 0.0f, -(ySegments / 2.0f))) *Matrix.CreateScale(scale);
        }

        void CreateGeometry()
        {
            for (int z = 0; z <= ySegments; z++)
            {
                for (int x = 0; x <= xSegments; x++)
                {
                    Vector3 point = new Vector3(x, 0.0f, z);
                    AddVertex(point, Vector3.Up);
                }
            }
            
            bool x_revers = true;
            for (int z = 0; z <= ySegments; z++)
            {
                if (x_revers) //-->
                {
                    AddIndex(z * (xSegments+1));
                    AddIndex(z * (xSegments+1) + xSegments);
                }
                else // <--
                {
                    AddIndex(z * (xSegments + 1)+xSegments);
                    AddIndex(z * (xSegments+1));
                }
                x_revers = !x_revers;
            }
            bool z_reverse = true;
            /*
             |
             v
             */

            if (!x_revers)//actually TRUE
            {
                for (int x = xSegments; x >= 0; x--)
                {
                    if (z_reverse)
                    {
                        AddIndex(ySegments * (xSegments + 1) + x);
                        AddIndex(x);
                    }
                    else
                    {
                        AddIndex(x);
                        AddIndex(ySegments * (xSegments + 1) + x);
                    }
                    z_reverse = !z_reverse;
                }
            }
            /*
             ^
             |
             */
            else
            {
                for (int x = 0; x <= xSegments; x++)
                {
                    if (z_reverse)
                    {
                        AddIndex(ySegments * (xSegments + 1) + x);
                        AddIndex(x);
                    }
                    else
                    {
                        AddIndex(x);
                        AddIndex(ySegments * (xSegments + 1) + x);
                    }
                    z_reverse = !z_reverse;
                }
            }
            InitializePrimitive();
        }

        //void AddIndex(bool reverse, int counter)
        //{
        //    if (!reverse)
        //    {
        //        AddIndex(xSegments * (ySegments - 1) + counter);
        //        AddIndex(counter);
        //    }
        //    else
        //    {
        //        AddIndex(counter);
        //        AddIndex(xSegments * (ySegments - 1) + counter);
        //    }
        //}

        public void Update()
        {
            base.Update(world);
        }
    }
}
