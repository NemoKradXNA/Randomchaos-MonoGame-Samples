using Microsoft.Xna.Framework;

namespace RandomchaosMGBase.BaseClasses.PostProcessing
{
    public class FXAAEffect : BasePostProcessingEffect
    {
        protected FXAA fxaa;

        public float ContrastThreshold { get { return fxaa.ContrastThreshold; } set { fxaa.ContrastThreshold = value; } }
        public float RelativeThreshold { get { return fxaa.RelativeThreshold; } set { fxaa.RelativeThreshold = value; } }
        public float SubpixelBlending { get { return fxaa.SubpixelBlending; } set { fxaa.SubpixelBlending = value; } }

        public FXAAEffect(Game game, float contrastThreshold = 0.0312f, float relativeThreshold = 0.063f, float subpixelBlending = 1f) : base(game)
        {
            fxaa = new FXAA(game,contrastThreshold, relativeThreshold, subpixelBlending);

            AddPostProcess(fxaa);
        }
    }
}
