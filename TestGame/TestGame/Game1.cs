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

namespace TestGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        VertexBuffer vertexBuffer;
        VertexBuffer triangleBuffer;
        IndexBuffer indexBuffer;
        GraphicsDevice device;
        Color color = Color.Black;
        Camera camera;

        static RasterizerState Wire = new RasterizerState { FillMode = FillMode.WireFrame, CullMode = CullMode.None };

        public Game1()
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
            LoadBox();
            Loadtriangle();
            SetUpCamera();
            // TODO: use this.Content to load your game content here

        }

        private void LoadBox()
        { 
            VertexPositionNormal[] vertices = new VertexPositionNormal[8];
            //top
            vertices[0] = new VertexPositionNormal(new Vector3(-5, 5, 7), Vector3.Up); //top_left_front
            vertices[1] = new VertexPositionNormal(new Vector3(-5, 5, -7), Vector3.Up); //top_left_back
            vertices[2] = new VertexPositionNormal(new Vector3(5, 5, -7), Vector3.Up); //top_right_back
            vertices[3] = new VertexPositionNormal(new Vector3(5, 5, 7), Vector3.Up); //top_right_front
            //down
            vertices[4] = new VertexPositionNormal(new Vector3(-5, -5, 7), Vector3.Up); //dawn_left_front
            vertices[5] = new VertexPositionNormal(new Vector3(-5, -5, -7), Vector3.Up); //dawn_left_back
            vertices[6] = new VertexPositionNormal(new Vector3(5, -5, -7), Vector3.Up); //dawn_right_back
            vertices[7] = new VertexPositionNormal(new Vector3(5, -5, 7), Vector3.Up); //dawn_right_front

            vertexBuffer = new VertexBuffer(device, VertexPositionNormal.VertexDeclaration, 8, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionNormal>(vertices);
            ushort[] indices = new ushort[36];
            //top
            indices[0] = 0;
            indices[1] = 2;
            indices[2] = 3;
            indices[3] = 2;
            indices[4] = 0;
            indices[5] = 1;
            //dawn
            indices[6] = 4;
            indices[7] = 7;
            indices[8] = 6;
            indices[9] = 6;
            indices[10] = 5;
            indices[11] = 4;
            //front
            indices[12] = 4;
            indices[13] = 3;
            indices[14] = 7;
            indices[15] = 3;
            indices[16] = 4;
            indices[17] = 0;
            //back
            indices[18] = 2;
            indices[19] = 1;
            indices[20] = 6;
            indices[21] = 5;
            indices[22] = 6;
            indices[23] = 1;
            //left
            indices[24] = 5;
            indices[25] = 1;
            indices[26] = 4;
            indices[27] = 0;
            indices[28] = 4;
            indices[29] = 1;
            //right
            indices[30] = 6;
            indices[31] = 7;
            indices[32] = 2;
            
            indices[33] = 3;
            indices[34] = 2;
            indices[35] = 7;
            indexBuffer = new IndexBuffer(device, typeof(ushort), indices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData<ushort>(indices);
        }
        private void Loadtriangle()
        {
            VertexPositionColor[] vertices = new VertexPositionColor[3];
            //top
            vertices[0] = new VertexPositionColor(new Vector3(3, 6, 9), Color.Red);
            vertices[1] = new VertexPositionColor(new Vector3(6, 7.5f, 8), Color.Red);
            vertices[2] = new VertexPositionColor(new Vector3(5, 8, 7.5f), Color.Red);

            triangleBuffer = new VertexBuffer(device, VertexPositionColor.VertexDeclaration, 3, BufferUsage.WriteOnly);
            triangleBuffer.SetData<VertexPositionColor>(vertices);

            Vector3 E2 = new Vector3(5, 8, 7.5f) - new Vector3(3, 6, 9);
            Vector3 E1 = new Vector3(6, 7.5f, 8) - new Vector3(3, 6, 9);
            Vector3 normal_v = Vector3.Cross(E2, E1);
            float L = Vector3.Dot(normal_v, new Vector3(3, 6, 9));
            //float R = 5* + 7 + 5;
        }
        private void SetUpCamera()
        {
            Mouse.SetPosition(device.Viewport.Width / 2, device.Viewport.Height / 2);
            //camera = new FirstPersonCamera(device.Viewport.AspectRatio, 1.0f, 500.0f, new Vector3(0, 10, 20), device);
            camera = new ArcBallCamera(device.Viewport.AspectRatio, 1.0f, 500.0f, new Vector3(0, 2, 0));
            camera.Pitch = -MathHelper.Pi / 3.0f;
            camera.Yaw = MathHelper.Pi;
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            float elapsedMiliseconds = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
            // TODO: Add your update logic here
            GamePadState g_state = GamePad.GetState(PlayerIndex.One);
            KeyboardState k_state = Keyboard.GetState();
            MouseState m_state = Mouse.GetState();
            camera.HandleInput(elapsedMiliseconds, g_state, k_state, m_state);
            camera.Update();
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            BasicEffect effect = new BasicEffect(device);
            //effect.EnableDefaultLighting();
            GraphicsDevice.Clear(Color.CornflowerBlue);
            effect.World = Matrix.Identity;
            effect.View = camera.View;
            effect.Projection = camera.Projection;
            effect.DiffuseColor = color.ToVector3();
            effect.Alpha = color.A / 255.0f;
            device.DepthStencilState = DepthStencilState.Default;
            device.BlendState = BlendState.AlphaBlend;
            device.RasterizerState = Wire;
            device.SetVertexBuffer(vertexBuffer);
            device.Indices = indexBuffer;
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 8, 0, 12);
            }
            device.SetVertexBuffer(triangleBuffer);
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawPrimitives(PrimitiveType.TriangleList, 0, 1);
            }

            device.RasterizerState = RasterizerState.CullCounterClockwise;
            device.BlendState = BlendState.Opaque;
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
