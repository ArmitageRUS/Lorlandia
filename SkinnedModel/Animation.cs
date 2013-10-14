using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace SkinnedModel
{
    public class Keyframe
    {
        [ContentSerializer]
        public Int32 Bone { get; private set; }
        [ContentSerializer]
        public TimeSpan Time { get; private set; }
        [ContentSerializer]
        public Matrix Transform { get; private set; }

        public Keyframe(int boneIndex, TimeSpan time, Matrix boneTransform)
        {
            Bone = boneIndex;
            Time = time;
            Transform = boneTransform;
        }

        private Keyframe()
        { 
        
        }
    }

    public class AnimationClip
    {
        [ContentSerializer]
        public TimeSpan Duration { get; private set; }
        [ContentSerializer]
        public List<Keyframe> Keyframes { get; private set; }
        
        public AnimationClip(TimeSpan duration, List<Keyframe> keyframes)
        {
            Duration = duration;
            Keyframes = keyframes;
        }

        private AnimationClip() 
        { 
        
        }
    }

    public class AnimationPlayer
    {
        Matrix[] boneTransforms;
        Matrix[] worldTransforms;
        Matrix[] skinnedTransforms;

        public Matrix[] WorldTransforms 
        { 
            get
            {
                return worldTransforms;
            }
            private set
            {
                worldTransforms = value;
            } 
        }
        public Matrix[] SkinnedTransforms 
        {
            get
            {
                return skinnedTransforms;
            }
            private set 
            {
                skinnedTransforms = value;
            } 
        }

        int currentKeyFrame=0;
        TimeSpan totalElapsedTime;

        AnimationClip currentClip;

        SkinnedModel model;

        public AnimationPlayer(SkinnedModel model)
        {
            boneTransforms = new Matrix[model.BindPose.Count];
            worldTransforms = new Matrix[model.BindPose.Count];
            skinnedTransforms = new Matrix[model.BindInversePose.Count];
            this.model = model;
        }

        public void StartClip(string clip)
        {

            if (!model.Animations.TryGetValue(clip, out currentClip))
                throw new ArgumentException("There is no clip with such name");
            totalElapsedTime = TimeSpan.Zero;
            model.BindPose.CopyTo(boneTransforms);
        }

        public void Update(TimeSpan elapsed_time, Matrix model_transform, Dictionary<int, Matrix> boneManipulate)
        {
            UpdateBoneTransforms(elapsed_time, boneManipulate);
            UpdateWorldTransform(model_transform);
            UpdateSkinnedTransform();
        }

        private void UpdateBoneTransforms(TimeSpan elapsed_time, Dictionary<int, Matrix> boneManipulate)
        {
            elapsed_time += totalElapsedTime;
            while (elapsed_time >= currentClip.Duration) elapsed_time -= currentClip.Duration;
            if (elapsed_time < totalElapsedTime)
            {
                currentKeyFrame = 0;
                model.BindPose.CopyTo(boneTransforms);
            }
            totalElapsedTime = elapsed_time;
            List<Keyframe> frames = currentClip.Keyframes;
            while (currentKeyFrame < frames.Count)
            {
                Keyframe frame = frames[currentKeyFrame];
                if (frame.Time > totalElapsedTime) break;
                boneTransforms[frame.Bone] = frame.Transform;
                currentKeyFrame++;
            }
            if (boneManipulate != null)
            {
                foreach (KeyValuePair<int, Matrix> transform in boneManipulate)
                {
                    boneTransforms[transform.Key] = transform.Value * boneTransforms[transform.Key];
                }
            }
        }

        private void UpdateWorldTransform(Matrix model_transform)
        {
            worldTransforms[0] = boneTransforms[0] * model_transform;
            for (int bone = 1; bone < worldTransforms.Length; bone++)
            {
                worldTransforms[bone] = boneTransforms[bone] * worldTransforms[model.SkeletonHierarchy[bone]];                
            }
        }

        private void UpdateSkinnedTransform()
        { 
            for(int bone = 0; bone < skinnedTransforms.Length; bone++)
            {
                skinnedTransforms[bone] = model.BindInversePose[bone] * worldTransforms[bone]; 
            }
        }
    }

}
