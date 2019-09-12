using Microsoft.Xna.Framework;

namespace RandomchaosMGBase.BaseClasses.PostProcessing
{
    public class RadialBlurEffect : BasePostProcessingEffect
    {
        public RadialBlur rb;

        public RadialBlurEffect(Game game, float scale) : base(game)
        {
            rb = new RadialBlur(game, scale);

            AddPostProcess(rb);
        }
    }
}
