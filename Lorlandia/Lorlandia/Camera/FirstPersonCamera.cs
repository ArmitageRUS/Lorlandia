using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Lorlandia.Camera
{
    class FirstPersonCamera:Camera
    {
        const float constSpeed = 13.0f;
        public Vector3 cameraPosition;
        public Vector3 movement;
        MouseState archiveState;

        public override float Yaw 
        {
            get
            {
                return yaw;
            }
            set
            {
                yaw = value;
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
                pitch = value;
            }
        
        }

        public FirstPersonCamera(float aspect_ration, float near_plane, float far_plane, Vector3 initial_position, GraphicsDevice device)
        {
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect_ration, near_plane, far_plane);
            cameraPosition = initial_position;
            base.device = device;
            archiveState = Mouse.GetState();
        }

        public override void Update()
        {
            Matrix rotation = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0);
            Vector3 rotatedVector = Vector3.Transform(movement, rotation);
            cameraPosition += rotatedVector * constSpeed;
            
            Vector3 cameraForward = Vector3.Transform(Vector3.Forward, rotation);
            Vector3 cameraUp = Vector3.Transform(Vector3.Up, rotation);
            View = Matrix.CreateLookAt(cameraPosition, cameraPosition + cameraForward, cameraUp);
        }

        public override void HandleInput(float elapsed_time, GamePadState g_state, KeyboardState k_state, MouseState m_state)
        {
            Vector3 move_direction = Vector3.Zero;
            if (g_state.Buttons.LeftStick == ButtonState.Pressed) move_direction.Y = 0.05f;
            if (g_state.Buttons.RightStick == ButtonState.Pressed) move_direction.Y = -0.05f;
            Yaw -= g_state.ThumbSticks.Left.X * elapsed_time;
            Pitch += g_state.ThumbSticks.Left.Y * elapsed_time;
            move_direction.X = g_state.ThumbSticks.Right.X * elapsed_time;
            move_direction.Z -= g_state.ThumbSticks.Right.Y * elapsed_time;
            if (k_state.IsKeyDown(Keys.Space)) move_direction.Y = 1.1f * elapsed_time;
            if (k_state.IsKeyDown(Keys.LeftControl)) move_direction.Y = -1.1f * elapsed_time;
            if (k_state.IsKeyDown(Keys.Left)) Yaw += 1.2f * elapsed_time;
            if (k_state.IsKeyDown(Keys.Right)) Yaw -= 1.2f * elapsed_time;
            if (k_state.IsKeyDown(Keys.Up)) Pitch -= 1.2f * elapsed_time;
            if (k_state.IsKeyDown(Keys.Down)) Pitch += 1.2f * elapsed_time;
            if (k_state.IsKeyDown(Keys.W)) move_direction.Z -= 1.2f * elapsed_time;
            if (k_state.IsKeyDown(Keys.S)) move_direction.Z += 1.2f * elapsed_time;
            if (k_state.IsKeyDown(Keys.A)) move_direction.X -= 1.2f * elapsed_time;
            if (k_state.IsKeyDown(Keys.D)) move_direction.X += 1.2f * elapsed_time;
            if (archiveState != m_state)
            {
                int xdiff = m_state.X - archiveState.X;
                int ydiff = m_state.Y - archiveState.Y;
                Pitch -= ydiff*0.3f * elapsed_time;
                Yaw -= xdiff*0.3f * elapsed_time;
                //archiveState = m_state;
                Mouse.SetPosition(device.Viewport.Width / 2, device.Viewport.Height / 2);
            }
            movement = move_direction;
        }
    }
}
