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
        string output = "����� ����...";
        VertexBuffer vertexBuffer;
        GraphicsDevice device;
        Effect effect;
        Texture2D grassTexture;
        //Texture2D charTexture;
        Model characterModel;
        Model toolModel;
        AnimationPlayer modelPlayer;
        SpherePrimitive sphere;
        Camera.Camera camera;
        Matrix worldMatrix;
        //Matrix viewMatrix;
        //Matrix projectionMatrix;
        Matrix headYaw;
        Vector3 cameraPosition;
        float yaw = 0f; //around UP axis
        float pitch = 0f; //arounf RIGHT axis
        //float lower_model_point = 0;
        const float constSpeed = 13.0f;
        int headBone = 0;
        int leftPlamBone = 0;
        
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
            Window.Title = "Lorlandia � alpha";
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
            //charTexture = Content.Load<Texture2D>("Dobrochan_TEXURE");
            LoadToolModel();
            LoadCharModel();
            LoadTerrain();
            sphere = new SpherePrimitive(device);
            SetUpCamera();
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
            //projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, device.Viewport.AspectRatio, 1.0f, 500.0f);
            //cameraPosition = new Vector3(0, 10, 20);
            //pitch = -(float)Math.PI / 6.0f;
            //UpdateCamera();
            camera = new FirstPersonCamera(device.Viewport.AspectRatio, 1.0f, 500.0f, new Vector3(0, 10, 20), device);
            camera.Pitch = -MathHelper.Pi / 6.0f;
        }

        private void AddCameraPosition(Vector3 movement)
        {
            Matrix rotation = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0);
            Vector3 rotatedVector = Vector3.Transform(movement,rotation);
            cameraPosition += rotatedVector*constSpeed;
            UpdateCamera();
        }

        private void UpdateCamera()
        {
            Matrix rotation = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0);
            Vector3 cameraForward = Vector3.Transform(Vector3.Forward, rotation);
            Vector3 cameraUp = Vector3.Transform(Vector3.Up, rotation);
            //viewMatrix = Matrix.CreateLookAt(cameraPosition, cameraPosition+cameraForward, cameraUp);
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
            DrawTerrain();
            DrawCharacter();
            DrawTool();
            sphere.Draw(Matrix.Identity, camera.View, camera.Projection, Color.White);
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