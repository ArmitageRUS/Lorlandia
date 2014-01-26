using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Lorlandia.Primitives3D;
using Lorlandia.Camera;

namespace Lorlandia
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class TestGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        GraphicsDevice device;
        Camera.Camera camera;
        MouseState mouse_state;
        SpriteBatch spriteBatch;
        PlanePrimitive plane;

        public TestGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            graphics.PreferredBackBufferWidth = 1440;
            graphics.PreferredBackBufferHeight = 900;
            graphics.ApplyChanges();
            device = graphics.GraphicsDevice;    
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            SetUpCamera();
            plane = new PlanePrimitive(device, 5, 5, 5.0f);
            // TODO: use this.Content to load your game content here

        }

        private void SetUpCamera()
        {
            Mouse.SetPosition(device.Viewport.Width / 2, device.Viewport.Height / 2);
            mouse_state = Mouse.GetState();
            //camera = new FirstPersonCamera(device.Viewport.AspectRatio, 1.0f, 500.0f, new Vector3(0, 10, 20), device);
            camera = new ArcBallCamera(device.Viewport.AspectRatio, 1.0f, 500.0f, new Vector3(0, 0, 0), Vector3.Zero, mouse_state);
            //camera.Pitch = -MathHelper.Pi / 3.0f;
            //camera.Yaw = MathHelper.Pi;
        }
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();
            float elapsedMiliseconds = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
            // TODO: Add your update logic here
            GamePadState g_state = GamePad.GetState(PlayerIndex.One);
            KeyboardState k_state = Keyboard.GetState();
            MouseState m_state = Mouse.GetState();
            camera.HandleInput(elapsedMiliseconds, g_state, k_state, m_state);
            camera.Update();
            plane.Update();
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            plane.Draw(camera.View, camera.Projection);
            base.Draw(gameTime);
        }
    }
}
