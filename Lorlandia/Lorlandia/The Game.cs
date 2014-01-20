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
using SkinnedModel;
using Lorlandia.Primitives3D;
using Lorlandia.Camera;
using Lorlandia.Physics;
using Lorlandia.Terrain;
using Lorlandia.Objects;

namespace Lorlandia
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont Font1;
        Vector2 FontPos;
        string output = "такие дела...";
        VertexBuffer vertexBuffer;
        GraphicsDevice device;
        Effect effect;
        Model characterModel;
        Model toolModel;
        SpherePrimitive sphere;
        BoxPrimitive box;
        Collision collision;
        Camera.Camera camera;
        Matrix headYaw;
        HightMapTerrain terrain;
        Vector3 SphereOffset = Vector3.Zero;
        MovableCharacter protagonist;
        //float lower_model_point = 0;
        const float constSpeed = 13.0f;

        VertexBuffer test_buffer;
        IndexBuffer test_ibuffer;
        BasicEffect test_effect;

        
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
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            Window.Title = "Lorlandia Ч alpha";
            device = graphics.GraphicsDevice;

            test_effect = new BasicEffect(device);
            
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
            Font1 = Content.Load<SpriteFont>("SpriteFont1");
            FontPos = new Vector2(10,10);
            effect = Content.Load<Effect>("MainEffects");
            
            LoadToolModel();
            LoadCharModel();
            LoadTerrain();
            sphere = new SpherePrimitive(device);
            box = new BoxPrimitive(device);
            SetUpCamera();
            collision = new Collision();
            // TODO: use this.Content to load your game content here
        }

        private void LoadTerrain()
        {
            Texture2D grassTexture = Content.Load<Texture2D>("Grass");
            Texture2D hightMap = Content.Load<Texture2D>("heightmap");
            terrain = new HightMapTerrain(hightMap, grassTexture, effect, device);
            terrain.SetUpGeometry();
        }

        private void LoadToolModel()
        { 
            toolModel = Content.Load<Model>("baseballbat");
            //foreach (ModelMesh mesh in toolModel.Meshes)
            //{
            //    foreach (ModelMeshPart part in mesh.MeshParts)
            //    {
            //        part.Effect = effect.Clone();
            //    }
            //}
        }

        private void LoadCharModel()
        {
            characterModel = Content.Load<Model>("Dobrochar._Armature_begin");
            protagonist = new MovableCharacter(characterModel);
            protagonist.animationPlayer.StartClip("ArmatureAction");
            
            //foreach (ModelMesh mesh in characterModel.Meshes)
            //{
            //    foreach (ModelMeshPart part in mesh.MeshParts )
            //    {
            //        short[] indices = new short[part.IndexBuffer.IndexCount];
            //        float[] vertices = new float[part.NumVertices*part.VertexBuffer.VertexDeclaration.VertexStride/4];
            //        part.IndexBuffer.GetData<short>(indices);
            //        part.VertexBuffer.GetData<float>(vertices);
            //        for(int i = part.StartIndex;i<part.StartIndex+part.PrimitiveCount*3;i++)
            //        {
            //            int index = (part.VertexOffset+indices[i])*part.VertexBuffer.VertexDeclaration.VertexStride/4;
            //            //Vector3 point_position = new Vector3(vertices[index],vertices[index+1],vertices[index+2]);
            //            if (vertices[index + 1] < lower_model_point) 
            //                lower_model_point = vertices[index + 1];
            //        }
            //        part.Effect = effect.Clone();
            //        int stride = part.VertexBuffer.VertexDeclaration.VertexStride;
            //    }
            //}
        }
        
        private void SetUpCamera()
        {
            Mouse.SetPosition(device.Viewport.Width / 2, device.Viewport.Height / 2);
            //camera = new FirstPersonCamera(device.Viewport.AspectRatio, 1.0f, 500.0f, new Vector3(0, 10, 20), device);
            MouseState m_state = Mouse.GetState();
            camera = new ArcBallCamera(device.Viewport.AspectRatio, 1.0f, 500.0f, protagonist.position, new Vector3(0,2,0), m_state);
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            // TODO: Add your update logic here
            ProcessInput((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f);
            sphere.Update(Matrix.CreateTranslation(SphereOffset));
            box.Update(Matrix.CreateTranslation(SphereOffset));
            protagonist.Update(gameTime.ElapsedGameTime);
            terrain.Collision(ref protagonist.position);
            camera.target = protagonist.position;
            camera.Update();
            base.Update(gameTime);
        }
        private void ProcessInput(float elapsedMiliseconds)
        {
            GamePadState g_state = GamePad.GetState(PlayerIndex.One);
            KeyboardState k_state = Keyboard.GetState();
            MouseState m_state = Mouse.GetState();
            camera.HandleInput(elapsedMiliseconds, g_state, k_state, m_state);
            protagonist.HandleInput(elapsedMiliseconds, g_state, k_state, m_state);

            output = "Pitch"+camera.Pitch.ToString();

            /*sphere move*/
            if (k_state.IsKeyDown(Keys.NumPad8)) SphereOffset += Vector3.Up * elapsedMiliseconds;
            if (k_state.IsKeyDown(Keys.NumPad2)) SphereOffset -= Vector3.Up * elapsedMiliseconds;
            if (k_state.IsKeyDown(Keys.NumPad4)) SphereOffset += Vector3.Left * elapsedMiliseconds;
            if (k_state.IsKeyDown(Keys.NumPad6)) SphereOffset -= Vector3.Left * elapsedMiliseconds;
            
            //Intersection intersect = collision.ClassifySphere(terrainPoligons, sphere.Centre, 5);
            //switch (intersect)
            //{ 
            //    case Intersection.Front:
            //        sphere.color = Color.Blue;
            //        break;
            //    case Intersection.Inside:
            //        sphere.color = Color.Red;
            //        break;
            //    case Intersection.Behind:
            //        sphere.color = Color.Black;
            //        break;
            //    default:
            //        sphere.color = Color.White;
            //        break;
            //}
        }
        
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1.0f, 0);
            spriteBatch.Begin();
            Vector2 FontOrigin = Font1.MeasureString(output) / 2;
            spriteBatch.DrawString(Font1, output, FontPos, Color.Black, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
            spriteBatch.End();
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            terrain.Draw(camera.View, camera.Projection);
            protagonist.Draw(camera.View, camera.Projection);
            DrawTool();

            //sphere.Draw(camera.View, camera.Projection);
            //box.Draw(camera.View, camera.Projection);
            // TODO: Add your drawing code here
            base.Draw(gameTime);
        }

        private void DrawTool()
        {
            Matrix[] toolTransforms = new Matrix[toolModel.Bones.Count];
            toolModel.CopyAbsoluteBoneTransformsTo(toolTransforms);
            foreach (ModelMesh mesh in toolModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = Matrix.CreateRotationZ(MathHelper.PiOver2)*protagonist.LeftPalm * toolTransforms[mesh.ParentBone.Index];
                    effect.View = camera.View;
                    effect.Projection = camera.Projection;
                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
        }
    }
}
