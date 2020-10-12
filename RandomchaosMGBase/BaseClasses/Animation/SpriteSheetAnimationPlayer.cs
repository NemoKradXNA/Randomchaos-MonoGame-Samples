using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;


namespace RandomchaosMGBase.BaseClasses.Animation
{
    public class SpriteSheetAnimationPlayer
    {
        public TimeSpan AnimationOffSet { get; set; }

        protected bool _IsPlaying = false;
        public bool IsPlaying { get { return _IsPlaying; } }

        public Vector2 CurrentCell { get; set; }

        public int CurrentKeyframe { get; set; }

        public float ClipLerpValue
        {
            get
            {
                if (currentClip != null)
                    return (float)CurrentKeyframe / currentClip.Keyframes.Count;
                else
                    return 0;
            }
        }

        protected SpriteSheetAnimationClip currentClip;
        public SpriteSheetAnimationClip CurrentClip
        {
            get { return currentClip; }
        }


        /// <summary>
        /// Gets the current play position.
        /// </summary>
        TimeSpan currentTime;
        public TimeSpan CurrentTime
        {
            get { return currentTime; }
        }

        public Dictionary<string, SpriteSheetAnimationClip> Clips { get; set; }

        public SpriteSheetAnimationPlayer(Dictionary<string, SpriteSheetAnimationClip> clips = null, TimeSpan animationOffSet = new TimeSpan())
        {
            AnimationOffSet = animationOffSet;
            Clips = clips;
        }

        public void StartClip(string name, int frame = 0)
        {
            StartClip(Clips[name]);
        }

        public void StartClip(SpriteSheetAnimationClip clip, int frame = 0)
        {
            if (clip != null && clip != currentClip)
            {
                currentTime = TimeSpan.Zero + AnimationOffSet;
                CurrentKeyframe = frame;

                currentClip = clip;

                _IsPlaying = true;
            }
        }

        public void StopClip()
        {
            if (currentClip != null && IsPlaying)
                _IsPlaying = false;
        }

        public void Update(TimeSpan time)
        {
            if (currentClip != null)
                GetCurrentCell(time);
        }

        public void Update(float lerp)
        {
            if (currentClip != null)
                GetCurrentCell(lerp);
        }

        protected void GetCurrentCell(float lerp)
        {
            CurrentKeyframe = (int)MathHelper.Lerp(0, currentClip.Keyframes.Count - 1, lerp);
            CurrentCell = currentClip.Keyframes[CurrentKeyframe].Cell;
        }

        protected void GetCurrentCell(TimeSpan time)
        {
            time += currentTime;

            // If we reached the end, loop back to the start.
            while (time >= currentClip.Duration)
                time -= currentClip.Duration;

            if ((time < TimeSpan.Zero) || (time >= currentClip.Duration))
                throw new ArgumentOutOfRangeException("time");

            if (time < currentTime)
            {
                if (currentClip.Looped)
                    CurrentKeyframe = 0;
                else
                {

                    CurrentKeyframe = currentClip.Keyframes.Count - 1;

                    StopClip();
                }
            }

            currentTime = time;

            // Read key frame matrices.
            IList<SpriteSheetKeyFrame> keyframes = currentClip.Keyframes;

            while (CurrentKeyframe < keyframes.Count)
            {
                SpriteSheetKeyFrame keyframe = keyframes[CurrentKeyframe];

                // Stop when we've read up to the current time position.
                if (keyframe.Time > currentTime)
                    break;

                // Use this key frame.
                CurrentCell = keyframe.Cell;

                CurrentKeyframe++;
            }
        }

    }
}
