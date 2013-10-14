using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Lorlandia.Camera
{
    class FirstPersonCamera:Camera
    {
        const float constSpeed = 13.0f;
        public Vector3 cameraPosition;
        public Vector3 movement;
        public float Yaw 
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
        public float Pitch 
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

        public FirstPersonCamera(float aspect_ration, float near_plane, float far_plane, Vector3 initial_position)
        {
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect_ration, near_plane, far_plane);
            cameraPosition = initial_position;
        }

        public override void Update()
        {
            Matrix rotation = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0);
            Vector3 rotatedVector = Vector3.Transform(movement, rotation);
            cameraPosition += rotatedVector * constSpeed;

            //Matrix rotation = Matrix.CreateFromYawPitchRoll(Yaw, Pitch, 0);
            Vector3 cameraForward = Vector3.Transform(Vector3.Forward, rotation);
            Vector3 cameraUp = Vector3.Transform(Vector3.Up, rotation);
            View = Matrix.CreateLookAt(cameraPosition, cameraPosition + cameraForward, cameraUp);
        }
    }
}
