using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lorlandia.Primitives3D
{
    class BoxPrimitive:GeometricPrimitive
    {
        Vector3 Upward=Vector3.Up;
        Vector3 Rightward=Vector3.Left;
        Vector3 Forward=Vector3.Forward;
        
        float eUp;
        float eRight;
        float eForward;

        public BoxPrimitive(GraphicsDevice device, float e_up, float e_right, float e_forward)
        {
            base.device = device;
            eUp = e_up;
            eRight = e_right;
            eForward = e_forward;
            quadrangle = true;
            CalculateVertices();
        }

        public BoxPrimitive(GraphicsDevice device)
            : this(device, 1.0f, 1.0f, 2.0f)
        { }

        public override void Update(Matrix world)
        {
            Upward = world.Up;
            Rightward = world.Right;
            Forward = world.Forward;
            base.Update(world);
            //CalculateVertices();
        }

        private void CalculateVertices()
        {
            /*0*/Vector3 down_left_back = Rightward * -eRight + Upward * -eUp + Forward * -eForward;
            /*1*/Vector3 down_left_front = Rightward * -eRight + Upward * -eUp + Forward * eForward;
            /*2*/Vector3 down_right_back = Rightward * eRight + Upward * -eUp + Forward * -eForward;
            /*3*/Vector3 down_right_front = Rightward * eRight + Upward * -eUp + Forward * eForward;
            /*4*/Vector3 up_left_back = Rightward * -eRight + Upward * eUp + Forward * -eForward;
            /*5*/Vector3 up_left_front = Rightward * -eRight + Upward * eUp + Forward * eForward;
            /*6*/Vector3 up_right_back = Rightward * eRight + Upward * eUp + Forward * -eForward;
            /*7*/Vector3 up_right_front = Rightward * eRight + Upward * eUp + Forward * eForward;

            AddVertex(down_left_back + Centre, Vector3.Normalize(down_left_back));
            AddVertex(down_left_front + Centre, Vector3.Normalize(down_left_front));
            AddVertex(down_right_back + Centre, Vector3.Normalize(down_right_back));
            AddVertex(down_right_front + Centre, Vector3.Normalize(down_right_front));
            AddVertex(up_left_back + Centre, Vector3.Normalize(up_left_back));
            AddVertex(up_left_front + Centre, Vector3.Normalize(up_left_front));
            AddVertex(up_right_back + Centre, Vector3.Normalize(up_right_back));
            AddVertex(up_right_front + Centre, Vector3.Normalize(up_right_front));

            AddIndex(0);
            AddIndex(4);
            AddIndex(6);
            AddIndex(2);
            AddIndex(0);
            AddIndex(1);
            AddIndex(5);
            AddIndex(7);
            AddIndex(3);
            AddIndex(1);
            AddIndex(5);
            AddIndex(4);
            AddIndex(6);
            AddIndex(7);
            AddIndex(3);
            AddIndex(2);
            InitializePrimitive();
        }
    }
}
