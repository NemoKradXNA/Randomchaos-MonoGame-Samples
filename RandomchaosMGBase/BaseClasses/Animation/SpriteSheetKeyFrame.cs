using System;

using Microsoft.Xna.Framework;

namespace RandomchaosMGBase.BaseClasses.Animation
{
    public class SpriteSheetKeyFrame 
    {
        public Vector2 Cell { get; set; }
        public TimeSpan Time { get; set; }

        public SpriteSheetKeyFrame() { }
        public SpriteSheetKeyFrame(Vector2 cell, TimeSpan time)
        {
            Cell = cell;
            Time = time;
        }
    }
}
