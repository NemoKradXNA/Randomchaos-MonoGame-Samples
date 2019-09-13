using Microsoft.Xna.Framework;

namespace RandomchaosMGBase.BaseClasses.PostProcessing
{
    public class HeatHazeEffect : BasePostProcessingEffect
    {
        public BumpmapDistort distort;

        public bool High
        {
            get { return distort.High; }
            set { distort.High = value; }
        }

        public HeatHazeEffect(Game game, string bumpasset, bool high) : base(game)
        {
            distort = new BumpmapDistort(game, bumpasset, high);

            AddPostProcess(distort);
        }
    }
}
