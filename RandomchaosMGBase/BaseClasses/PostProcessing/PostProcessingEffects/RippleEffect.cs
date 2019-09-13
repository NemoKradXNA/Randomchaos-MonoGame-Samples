using Microsoft.Xna.Framework;

namespace RandomchaosMGBase.BaseClasses.PostProcessing
{
    public class RippleEffect : BasePostProcessingEffect
    {
        public Ripple r;


        public float Distortion
        {
            get { return r.Distortion; }
            set { r.Distortion = value; }
        }

        public Vector2 ScreenPosition
        {
            get { return r.ScreenPosition; }
            set { r.ScreenPosition = value; }
        }

        public RippleEffect(Game game) : base(game)
        {
            r = new Ripple(game);

            AddPostProcess(r);
        }
    }
}
