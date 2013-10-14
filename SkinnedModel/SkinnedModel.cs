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

namespace SkinnedModel
{
    public class SkinnedModel
    {
        [ContentSerializer]
        public List<Matrix> BindPose { get; private set; }
        [ContentSerializer]
        public List<string> BoneNames { get; private set; }
        [ContentSerializer]
        public List<Matrix> BindInversePose { get; private set; }
        [ContentSerializer]
        public List<Int32> SkeletonHierarchy { get; private set; }
        [ContentSerializer]
        public Dictionary<string, AnimationClip> Animations { get; private set; }

        public SkinnedModel(List<Matrix> bindPose, List<string> names, List<Matrix> bindInversePose, List<Int32> skeletonHierarchy, Dictionary<string, AnimationClip> animations)
        {
            BindPose = bindPose;
            BindInversePose = bindInversePose;
            SkeletonHierarchy = skeletonHierarchy;
            Animations = animations;
            BoneNames = names;
        }

        private SkinnedModel()
        {
        }
    }
}
