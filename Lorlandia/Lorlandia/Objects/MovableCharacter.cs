using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SkinnedModel;

namespace Lorlandia.Objects
{
    class MovableCharacter
    {
        Vector3 direction;
        Vector3 position = Vector3.Zero;
        public AnimationPlayer animationPlayer;
        int headBone = 0;
        int leftPlamBone = 0;
        Model model;
        Matrix mainTransform=Matrix.Identity;
        Dictionary<int, Matrix> boneManipulate;
        public Matrix headTransform
        {
            set 
            {
                if (boneManipulate == null)
                {
                    boneManipulate = new Dictionary<int, Matrix>();
                    boneManipulate.Add(headBone, value);
                }
                else
                {
                    if (boneManipulate.ContainsKey(headBone)) boneManipulate[headBone] = value;
                    else boneManipulate.Add(headBone, value);
                }
            }
        }

        public Matrix LeftPalm
        {
            get
            {
                if (animationPlayer != null)
                {
                    return animationPlayer.WorldTransforms[leftPlamBone];
                }
                else return Matrix.Identity;
            }

            private set{}
        }

        public Int32 HeadBone
        {
            get
            {
                return headBone;
            }
            private set
            {
                headBone = value;
            }
        }

        public Int32 LeftPalmBone
        {
            get
            {
                return leftPlamBone;
            }
            private set
            {
                leftPlamBone = value;
            }
        }

        public MovableCharacter(Model characterModel)
        {
            SkinnedModel.SkinnedModel skinnedModel = characterModel.Tag as SkinnedModel.SkinnedModel;
            animationPlayer = new AnimationPlayer(skinnedModel);
            headBone = skinnedModel.BoneNames.IndexOf("Head");
            leftPlamBone = skinnedModel.BoneNames.IndexOf("L_palm");
            model = characterModel;
        }

        public void Draw(Matrix view, Matrix projection)
        {
            Matrix[] characterTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(characterTransforms);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    effect.CurrentTechnique = effect.Techniques["Skinned"];
                    //effect.Parameters["World"].SetValue(characterTransforms[mesh.ParentBone.Index] * worldMatrix);
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(projection);
                    effect.Parameters["BoneTransforms"].SetValue(animationPlayer.SkinnedTransforms);
                    effect.Parameters["LightPosition"].SetValue(new Vector3(0.0f, 500.0f, 10.0f));
                }
                mesh.Draw();
            }
        }

        public void HandleInput(float elapsed_time, GamePadState g_state, KeyboardState k_state, MouseState m_state)
        {
            float headRotate = -g_state.Triggers.Left + g_state.Triggers.Right;
            headTransform = Matrix.CreateRotationZ(MathHelper.ToRadians(45.0f * headRotate));
            direction = new Vector3(g_state.ThumbSticks.Left.X, 0, g_state.ThumbSticks.Left.Y);
            if (direction.Length() > 0.0f)
            {
                double angle = Math.Atan2(direction.X, direction.Z);
                mainTransform = Matrix.CreateRotationY((float)angle);
            }
            
            //Yaw -= g_state.ThumbSticks.Right.X * elapsed_time;
            //Pitch += g_state.ThumbSticks.Right.Y * elapsed_time;
            //if (g_state.DPad.Up == ButtonState.Pressed) zoom -= 15f * elapsed_time;
            //if (g_state.DPad.Down == ButtonState.Pressed) zoom += 15f * elapsed_time;
            //if (k_state.IsKeyDown(Keys.A)) Yaw -= 1.05f * elapsed_time;
            //if (k_state.IsKeyDown(Keys.D)) Yaw += 1.05f * elapsed_time;
            //if (k_state.IsKeyDown(Keys.W)) Pitch += 1.05f * elapsed_time;
            //if (k_state.IsKeyDown(Keys.S)) Pitch -= 1.05f * elapsed_time;
            //if (k_state.IsKeyDown(Keys.Subtract)) zoom += 15f * elapsed_time;
            //if (k_state.IsKeyDown(Keys.Add)) zoom -= 15f * elapsed_time;
        }

        public void Update(TimeSpan elapsed_time)
        {
            animationPlayer.Update(elapsed_time, mainTransform, boneManipulate);
        }
    }
}
