using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Lorlandia.Camera
{
    public class Camera
    {
        public Matrix Projection { get; protected set; }
        public Matrix View { get; protected set; }

        protected float yaw;
        protected float pitch;

        public virtual void Update()
        { }

        public virtual void HandleInput(float elapsed_time, GamePadState g_state, KeyboardState k_state)
        { }
    }
}
