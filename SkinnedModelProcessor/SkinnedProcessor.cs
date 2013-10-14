using System;

using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using SkinnedModel;

// TODO: replace these with the processor input and output types.
using TInput = Microsoft.Xna.Framework.Content.Pipeline.Graphics.NodeContent;
using TOutput = Microsoft.Xna.Framework.Content.Pipeline.Processors.ModelContent;

namespace SkinnedModelProcessor
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to apply custom processing to content data, converting an object of
    /// type TInput to TOutput. The input and output types may be the same if
    /// the processor wishes to alter data without changing its type.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    ///
    /// TODO: change the ContentProcessor attribute to specify the correct
    /// display name for this processor.
    /// </summary>
    [ContentProcessor(DisplayName = "SkinnedModelProcessor.CustomContentProcessor")]
    public class SkinnedProcessor : ModelProcessor//ContentProcessor<TInput, TOutput>
    {
        [DefaultValue(0.0f)]
        public float InitialRotX { get; set; }
        [DefaultValue(0.0f)]
        public float InitialRotY { get; set; }
        [DefaultValue(0.0f)]
        public float InitialRotZ { get; set; }

        public override TOutput Process(TInput input, ContentProcessorContext context)
        {
            ValidateMesh(input, context);
            BoneContent skeleton = MeshHelper.FindSkeleton(input);
            if (skeleton == null) throw new InvalidContentException("no skeleton");
            FlatterenMesh(input);
            InitialRotate(input, InitialRotX, InitialRotY, InitialRotZ);
            IList<BoneContent> bones = MeshHelper.FlattenSkeleton(skeleton);
            List<Matrix> bindPose = new List<Matrix>();
            List<string> boneNames = new List<string>();
            List<Matrix> inverseBindPose = new List<Matrix>();
            List<int> skeletonHierarchy = new List<int>();
            foreach(BoneContent bone in bones)
            {
                bindPose.Add(bone.Transform);
                boneNames.Add(bone.Name);
                inverseBindPose.Add(Matrix.Invert(bone.AbsoluteTransform));
                skeletonHierarchy.Add(bones.IndexOf(bone.Parent as BoneContent));
            }
            Dictionary<string, AnimationClip> animations = ProcessAnimations(skeleton.Animations, bones, context);
            SkinnedModel.SkinnedModel s_model = new SkinnedModel.SkinnedModel(bindPose, boneNames, inverseBindPose, skeletonHierarchy, animations);
            //ModelProcessor processor = new ModelProcessor();
            TOutput model = base.Process(input, context);//processor.Process(input, context);
            model.Tag = s_model;
            return model;
        }
        static void FlatterenMesh(TInput node)
        {
            foreach (TInput child in node.Children)
            {
                if (child is MeshContent)
                {
                    MeshHelper.TransformScene(child, child.Transform);
                    child.Transform = Matrix.Identity;
                    FlatterenMesh(child);
                    
                }
            }
        }

        static void InitialRotate(TInput input, float rotX, float rotY, float rotZ)
        {
            float yaw = MathHelper.ToRadians(rotY);
            float pitch = MathHelper.ToRadians(rotX);
            float roll = MathHelper.ToRadians(rotZ);
            Matrix transformScene = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
            MeshHelper.TransformScene(input, transformScene);
        }
        static void ValidateMesh(TInput node, ContentProcessorContext context, bool bone=false)
        {
            MeshContent mesh = node as MeshContent;
            if (mesh != null && !bone)
            {
                if (!MeshHasSkinning(mesh)) mesh.Parent.Children.Remove(mesh);
            }
            else if (node is BoneContent) bone = true;

            foreach (TInput child in node.Children)
            {
                ValidateMesh(child, context, bone);
            }
        }
        static bool MeshHasSkinning(MeshContent mesh)
        {
            foreach (GeometryContent geometry in mesh.Geometry)if (!geometry.Vertices.Channels.Contains(VertexChannelNames.Weights())) return false;
            return true;
        }

        static Dictionary<string, AnimationClip> ProcessAnimations(AnimationContentDictionary animations, IList<BoneContent> bones, ContentProcessorContext context)
        {
            Dictionary<string, int> namedBones = new Dictionary<string, int>();
            for (int i = 0; i < bones.Count; i++)
            {
                namedBones.Add(bones[i].Name, i);
            }

            Dictionary<string, AnimationClip> clips = new Dictionary<string, AnimationClip>();
            foreach (KeyValuePair<string, AnimationContent> animation in animations)
            {
                clips.Add(animation.Key, ProcessAnimation(animation.Value, namedBones, context));
            }
            if (clips.Count == 0) throw new InvalidContentException("has no animations");
            return clips;
        }

        static AnimationClip ProcessAnimation(AnimationContent animation, Dictionary<string, int> named_bones, ContentProcessorContext context)
        {
            List<Keyframe> keyframes = new List<Keyframe>();
            
            foreach (KeyValuePair<string, AnimationChannel> channel in animation.Channels)
            { 
                int bone_index;
                if (!named_bones.TryGetValue(channel.Key, out bone_index))throw new InvalidContentException("animation out of skeleton");
                //context.Logger.LogWarning(null, null, "keyframes: {0}", channel.Value.Count);
                foreach (AnimationKeyframe keyframe in channel.Value)
                {
                    keyframes.Add(new Keyframe(bone_index, keyframe.Time, keyframe.Transform));
                }
            }
            keyframes.Sort(CompareKeyframe);
            if (keyframes.Count == 0) throw new InvalidContentException("no movement");
            if (animation.Duration<=TimeSpan.Zero) throw new InvalidContentException("incorrect duration");
            return new AnimationClip(animation.Duration, keyframes);
        }

        static int CompareKeyframe(Keyframe a, Keyframe b)
        {
            return a.Time.CompareTo(b.Time);
        }

        //[DefaultValue(MaterialProcessorDefaultEffect.SkinnedEffect)]   
        //public override MaterialProcessorDefaultEffect DefaultEffect
        //{
        //    get { return MaterialProcessorDefaultEffect.SkinnedEffect; }
        //    set { }
        //}

        protected override MaterialContent  ConvertMaterial(MaterialContent material, ContentProcessorContext context)
        {
            BasicMaterialContent basicMaterial = material as BasicMaterialContent;
            if (basicMaterial != null)
            {
                EffectMaterialContent customMaterial = new EffectMaterialContent();
                customMaterial.Effect = new ExternalReference<EffectContent>("MainEffects.fx");
                customMaterial.Textures.Add("xTexture", new ExternalReference<TextureContent>("Dobrochan_TEXURE.png"));
                return base.ConvertMaterial(customMaterial, context);
            }
            else
            {
                context.Logger.LogImportantMessage(null, null, "bad meterial");
                return base.ConvertMaterial(material, context);
            }
            
        }
    }
}