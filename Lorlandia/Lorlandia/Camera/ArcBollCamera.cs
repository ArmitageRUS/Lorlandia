using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Lorlandia.Camera
{
    class ArcBallCamera:Camera
    {
        Vector3 forward = Vector3.Forward;
        Vector3 up = Vector3.Up;

        public Vector3 target;
        public Vector3 position;

        public float Yaw
        {
            get
            {
                return yaw;
            }
            set
            {
                yaw = value % MathHelper.TwoPi;
            }
        }

        public float Pitch
        {
            get
            {
                return pitch;
            }

            set
            {
                if (value > -0.01f) pitch = -0.01f;
                else if (value < -(MathHelper.Pi - 0.01f)) pitch = -(MathHelper.Pi - 0.01f);
                else pitch = value;
            }
        }

        public ArcBallCamera(float aspect_ration, float near_plane, float far_plane, Vector3 target)
        {
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect_ration, near_plane, far_plane);
            this.target = target;
        }

        public override void HandleInput(float elapsed_time, GamePadState g_state, KeyboardState k_state)
        { 
            
        }

        public override void Update()
        {
            Vector3 cameraPosition = new Vector3(0, 15, 0);

            cameraPosition = Vector3.Transform(cameraPosition, Matrix.CreateRotationX(Pitch));
            cameraPosition = Vector3.Transform(cameraPosition, Matrix.CreateRotationY(Yaw));

            position = cameraPosition+target;
            this.TransformBaseVectors();
            View = Matrix.CreateLookAt(this.position,this.target, this.up);
        }

        public void TransformBaseVectors()
        {
            Vector3 newForward = target - position;
            newForward.Normalize();
            forward = newForward;
            Vector3 referenceVector = Vector3.UnitY;
            Vector3 right = Vector3.Cross(forward, referenceVector);
            up = Vector3.Cross(right, forward);
        }
    }
}
