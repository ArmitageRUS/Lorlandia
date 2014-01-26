using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Lorlandia.Camera
{
    public class ArcBallCamera:Camera
    {
        Vector3 forward = Vector3.Forward;
        Vector3 up = Vector3.Up;

        float zoom = 15.0f;

        //public Vector3 target;
        Vector3 target_offset;
        public Vector3 position;

        MouseState old_state;

        public override float Yaw
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

        public override float Pitch
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

        public ArcBallCamera(float aspect_ration, float near_plane, float far_plane, Vector3 target, Vector3 target_offset, MouseState m_state)
        {
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect_ration, near_plane, far_plane);
            old_state = m_state;
            this.target_offset = target_offset;
            base.target = target;
        }

        public override void HandleInput(float elapsed_time, GamePadState g_state, KeyboardState k_state, MouseState m_state)
        {
            Yaw -= g_state.ThumbSticks.Right.X * elapsed_time;
            Pitch += g_state.ThumbSticks.Right.Y * elapsed_time;
            if (g_state.DPad.Up == ButtonState.Pressed) zoom -= 15f * elapsed_time;
            if (g_state.DPad.Down == ButtonState.Pressed) zoom += 15f * elapsed_time;

            if (old_state != m_state)
            {
                if (m_state.MiddleButton == ButtonState.Pressed)
                {
                    float x_diff = m_state.X - old_state.X;
                    float y_diff = m_state.Y - old_state.Y;
                    Yaw -= x_diff * elapsed_time * 0.25f;
                    Pitch += y_diff * elapsed_time * 0.25f;
                    zoom -= (m_state.ScrollWheelValue - old_state.ScrollWheelValue) * elapsed_time;
                }
                old_state = m_state;
            }

            //if (k_state.IsKeyDown(Keys.A)) Yaw -= 1.05f * elapsed_time;
            //if (k_state.IsKeyDown(Keys.D)) Yaw += 1.05f * elapsed_time;
            //if (k_state.IsKeyDown(Keys.W)) Pitch += 1.05f * elapsed_time;
            //if (k_state.IsKeyDown(Keys.S)) Pitch -= 1.05f * elapsed_time;
            if (k_state.IsKeyDown(Keys.Subtract)) zoom += 15f * elapsed_time;
            if (k_state.IsKeyDown(Keys.Add)) zoom -= 15f * elapsed_time;
        }

        public override void Update()
        {
            Vector3 cameraPosition = new Vector3(0, zoom, 0);

            cameraPosition = Vector3.Transform(cameraPosition, Matrix.CreateRotationX(Pitch));
            cameraPosition = Vector3.Transform(cameraPosition, Matrix.CreateRotationY(Yaw));

            position = cameraPosition+target;
            this.TransformBaseVectors();
            View = Matrix.CreateLookAt(this.position,base.target+target_offset, this.up);
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
