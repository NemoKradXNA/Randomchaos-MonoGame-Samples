using Microsoft.Xna.Framework;

namespace RandomchaosMGBase.BaseClasses.PostProcessing
{
    public class BloomEffect : BasePostProcessingEffect
    {
        BrightPass bp;
        GaussBlurV gbv;
        GaussBlurH gbh;
        Bloom b;

        public float BloomIntensity
        {
            get { return b.BloomIntensity; }
            set { b.BloomIntensity = value; }
        }

        public float BlurAmount
        {
            get
            {
                return gbv.BlurAmount;
            }
            set
            {
                gbv.BlurAmount = value;
                gbh.BlurAmount = value;
            }
        }

        public BloomEffect(Game game, float intensity, float saturation, float baseIntensity, float baseSatration, float threshold, float blurAmount) : base(game)
        {
            bp = new BrightPass(game, threshold);
            gbv = new GaussBlurV(game, blurAmount);
            gbh = new GaussBlurH(game, blurAmount);
            b = new Bloom(game, intensity, saturation, baseIntensity, baseSatration);

            AddPostProcess(bp);
            AddPostProcess(gbv);
            AddPostProcess(gbh);
            AddPostProcess(b);
        }
    }
}
