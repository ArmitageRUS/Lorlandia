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
    public class MovableCharacter
    {
        Vector3 direction;
        public Vector3 position = Vector3.Zero;
        public AnimationPlayer animationPlayer;
        int headBone = 0;
        int leftPlamBone = 0;
        float angle=0;
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

        //public Vector3 Position
        //{
        //    get { return position; }
        //}

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
            direction = new Vector3(-g_state.ThumbSticks.Left.X, 0, g_state.ThumbSticks.Left.Y);
            if (direction.Length() > 0.0f)
            {
                angle = (float)Math.Atan2(direction.X, direction.Z);
                position += (direction*elapsed_time*2.0f);
                mainTransform = Matrix.CreateRotationY(angle)*Matrix.CreateTranslation(position);
            }
            float angle_offset = 0.0f;
            if (k_state.IsKeyDown(Keys.Left)) angle_offset = elapsed_time * 1.5f;
            if (k_state.IsKeyDown(Keys.Right)) angle_offset = -elapsed_time * 1.5f;
            if (angle_offset != 0)
            {
                angle += angle_offset;
                //mainTransform = Matrix.CreateRotationY(angle);
            }
            if (k_state.IsKeyDown(Keys.Up))
            {
                Vector3 position_offset = Vector3.Transform(Vector3.Backward, Matrix.CreateRotationY(angle));
                position += (position_offset * elapsed_time*2.0f);
               
            }
             mainTransform = Matrix.CreateRotationY(angle)*Matrix.CreateTranslation(position);
        }

        public void Update(TimeSpan elapsed_time)
        {
            animationPlayer.Update(elapsed_time, mainTransform, boneManipulate);
        }
    }
}
