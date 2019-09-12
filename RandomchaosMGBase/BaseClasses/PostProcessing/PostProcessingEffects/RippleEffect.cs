using Microsoft.Xna.Framework;

namespace RandomchaosMGBase.BaseClasses.PostProcessing
{
    public class RippleEffect : BasePostProcessingEffect
    {
        public Ripple r;

        public RippleEffect(Game game) : base(game)
        {
            r = new Ripple(game);

            AddPostProcess(r);
        }
    }
}
