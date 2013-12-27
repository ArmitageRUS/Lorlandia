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
        Texture2D grassTexture;
        //Texture2D charTexture;
        Model characterModel;
        Model toolModel;
        AnimationPlayer modelPlayer;
        SpherePrimitive sphere;
        Collision collision;
        Camera.Camera camera;
        Matrix headYaw;
        Vector3 SphereOffset = Vector3.Zero;
        //float lower_model_point = 0;
        const float constSpeed = 13.0f;
        int headBone = 0;
        int leftPlamBone = 0;

        Vector3[] terrainPoligons;
        
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
            grassTexture = Content.Load<Texture2D>("Grass");
            LoadToolModel();
            LoadCharModel();
            LoadTerrain();
            sphere = new SpherePrimitive(device);
            SetUpCamera();
            collision = new Collision();
            // TODO: use this.Content to load your game content here
        }

        private void LoadTerrain()
        {
            VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[6];

            vertices[0] = new VertexPositionNormalTexture(new Vector3(-10, 0, -10), Vector3.Up, new Vector2(-1, -1));
            vertices[1] = new VertexPositionNormalTexture(new Vector3(10, 0, -10), Vector3.Up, new Vector2(1, -1));
            vertices[2] = new VertexPositionNormalTexture(new Vector3(-10, 0, 10), Vector3.Up, new Vector2(-1, 1));
            vertices[3] = new VertexPositionNormalTexture(new Vector3(10, 0, 10), Vector3.Up, new Vector2(1, 1));
            vertices[4] = new VertexPositionNormalTexture(new Vector3(-10, 0, 10), Vector3.Up, new Vector2(-1, 1));
            vertices[5] = new VertexPositionNormalTexture(new Vector3(10, 0, -10), Vector3.Up, new Vector2(1, -1));

            terrainPoligons = new Vector3[4];
            terrainPoligons[0] = new Vector3(-10, 0, -10);
            terrainPoligons[1] = new Vector3(10, 0, -10);
            terrainPoligons[2] = new Vector3(10, 0, 10);
            terrainPoligons[3] = new Vector3(-10, 0, 10);

            vertexBuffer = new VertexBuffer(device, VertexPositionNormalTexture.VertexDeclaration, 6, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionNormalTexture>(vertices);
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
            SkinnedModel.SkinnedModel model = characterModel.Tag as SkinnedModel.SkinnedModel;
            headBone = model.BoneNames.IndexOf("Head");
            leftPlamBone = model.BoneNames.IndexOf("L_palm");
            modelPlayer = new AnimationPlayer(model);
            modelPlayer.StartClip("ArmatureAction");
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
            camera = new ArcBallCamera(device.Viewport.AspectRatio, 1.0f, 500.0f, new Vector3(0,2,0));
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
            camera.Update();
            Dictionary<int, Matrix> headTransform = new Dictionary<int, Matrix>();
            headTransform.Add(headBone, headYaw);
            modelPlayer.Update(gameTime.ElapsedGameTime, Matrix.Identity, headTransform);
            base.Update(gameTime);
        }
        private void ProcessInput(float elapsedMiliseconds)
        {
            GamePadState g_state = GamePad.GetState(PlayerIndex.One);
            KeyboardState k_state = Keyboard.GetState();
            MouseState m_state = Mouse.GetState();
            camera.HandleInput(elapsedMiliseconds, g_state, k_state, m_state);
            float headRotate = -g_state.Triggers.Left + g_state.Triggers.Right;
            headYaw = Matrix.CreateRotationZ(MathHelper.ToRadians(45.0f*headRotate));
            output = "Pitch"+camera.Pitch.ToString();

            /*sphere move*/
            if (k_state.IsKeyDown(Keys.NumPad8)) SphereOffset += Vector3.Up * elapsedMiliseconds;
            if (k_state.IsKeyDown(Keys.NumPad2)) SphereOffset -= Vector3.Up * elapsedMiliseconds;
            if (k_state.IsKeyDown(Keys.NumPad4)) SphereOffset += Vector3.Left * elapsedMiliseconds;
            if (k_state.IsKeyDown(Keys.NumPad6)) SphereOffset -= Vector3.Left * elapsedMiliseconds;
            Collision.Intersection intersect = collision.ClassifySphere(terrainPoligons, sphere.Centre, 5);
            switch (intersect)
            { 
                case Collision.Intersection.Front:
                    sphere.color = Color.Blue;
                    break;
                case Collision.Intersection.Inside:
                    sphere.color = Color.Red;
                    break;
                case Collision.Intersection.Behind:
                    sphere.color = Color.Black;
                    break;
                default:
                    sphere.color = Color.White;
                    break;
            }
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
            //DrawTerrain();
            //DrawCharacter();
            //DrawTool();
            sphere.Draw(Matrix.CreateTranslation(SphereOffset), camera.View, camera.Projection);
            // TODO: Add your drawing code here
            base.Draw(gameTime);
        }

        private void DrawTerrain()
        { 
            effect.CurrentTechnique = effect.Techniques["Textured"];
            effect.Parameters["World"].SetValue(Matrix.Identity);
            effect.Parameters["View"].SetValue(camera.View);
            effect.Parameters["Projection"].SetValue(camera.Projection);
            effect.Parameters["xTexture"].SetValue(grassTexture);
            device.SetVertexBuffer(vertexBuffer);
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawPrimitives(PrimitiveType.TriangleList, 0, 2); 
            }
        }

        private void DrawCharacter()
        {
            Matrix[] characterTransforms = new Matrix[characterModel.Bones.Count];
            characterModel.CopyAbsoluteBoneTransformsTo(characterTransforms);
            foreach (ModelMesh mesh in characterModel.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    effect.CurrentTechnique = effect.Techniques["Skinned"];
                    //effect.Parameters["World"].SetValue(characterTransforms[mesh.ParentBone.Index] * worldMatrix);
                    effect.Parameters["View"].SetValue(camera.View);
                    effect.Parameters["Projection"].SetValue(camera.Projection);
                    effect.Parameters["BoneTransforms"].SetValue(modelPlayer.SkinnedTransforms);
                    effect.Parameters["LightPosition"].SetValue(new Vector3(0.0f, 100.0f, 10.0f));
                }
                mesh.Draw();
            }
        }

        private void DrawTool()
        {
            Matrix[] toolTransforms = new Matrix[toolModel.Bones.Count];
            toolModel.CopyAbsoluteBoneTransformsTo(toolTransforms);
            foreach (ModelMesh mesh in toolModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = Matrix.CreateRotationZ(MathHelper.PiOver2)*modelPlayer.WorldTransforms[leftPlamBone] * toolTransforms[mesh.ParentBone.Index];
                    effect.View = camera.View;
                    effect.Projection = camera.Projection;
                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
        }
    }
}
