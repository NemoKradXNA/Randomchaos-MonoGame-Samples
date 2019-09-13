using Microsoft.Xna.Framework;

namespace RandomchaosMGBase.BaseClasses.PostProcessing
{
    public class RadialBlurEffect : BasePostProcessingEffect
    {
        public RadialBlur rb;

        public float Scale
        {
            get { return rb.Scale; }
            set { rb.Scale = value; }
        }

        public RadialBlurEffect(Game game, float scale) : base(game)
        {
            rb = new RadialBlur(game, scale);

            AddPostProcess(rb);
        }
    }
}
