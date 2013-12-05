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
        IndexBuffer indexBuffer;
        GraphicsDevice device;

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
            VertexPositionNormal[] vertices = new VertexPositionNormal[6];

            //top
            vertices[0] = new VertexPositionNormal(new Vector3(-5, 5, 7), Vector3.Up); //top_right_front
            vertices[1] = new VertexPositionNormal(new Vector3(-5, 5, -7), Vector3.Up); //top_rigth_back
            vertices[2] = new VertexPositionNormal(new Vector3(5, 5, -7), Vector3.Up); //top_left_back
            vertices[3] = new VertexPositionNormal(new Vector3(5, 5, 7), Vector3.Up); //top_left_front
            //down
            vertices[4] = new VertexPositionNormal(new Vector3(-5, -5, 7), Vector3.Up); //dawn_left_front
            vertices[5] = new VertexPositionNormal(new Vector3(-5, -5, -7), Vector3.Up); //dawn_left_back
            vertices[6] = new VertexPositionNormal(new Vector3(5, -5, -7), Vector3.Up); //dawn_right_back
            vertices[7] = new VertexPositionNormal(new Vector3(5, -5, 7), Vector3.Up); //dawn_right_front

            vertexBuffer = new VertexBuffer(device, VertexPositionNormal.VertexDeclaration, 6, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionNormal>(vertices);
            ushort[] indices = new ushort[12];
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
            indices[25] = 2;
            indices[26] = 4;
            indices[27] = 3;
            indices[28] = 4;
            indices[29] = 2;
            //right
            indices[30] = 0;
            indices[31] = 1;
            indices[32] = 7;
            indices[33] = 6;
            indices[34] = 7;
            indices[35] = 1;
            indexBuffer = new IndexBuffer(device, typeof(ushort), indices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData<ushort>(indices);
            // TODO: use this.Content to load your game content here
            
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

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
