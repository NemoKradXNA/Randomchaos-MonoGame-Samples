using System;
using System.Collections.Generic;

namespace RandomchaosMGBase.BaseClasses.Animation
{
    public class SpriteSheetAnimationClip
    {
        public string Name { get; set; }
        public bool Looped { get; set; }
        public TimeSpan Duration { get; set; }

        public List<SpriteSheetKeyFrame> Keyframes { get; set; }

        public SpriteSheetAnimationClip() { }

        public SpriteSheetAnimationClip(string name, TimeSpan duration, List<SpriteSheetKeyFrame> keyframes, bool looped = true)
        {
            Name = name;
            Duration = duration;
            Keyframes = keyframes;
            Looped = looped;
        }

        public SpriteSheetAnimationClip(SpriteSheetAnimationClip clip)
        {
            Name = clip.Name;
            Duration = clip.Duration;

            SpriteSheetKeyFrame[] frames = new SpriteSheetKeyFrame[clip.Keyframes.Count];
            clip.Keyframes.CopyTo(frames, 0);

            Keyframes = new List<SpriteSheetKeyFrame>();
            Keyframes.AddRange(frames);

            Looped = clip.Looped;
        }
    }
}
