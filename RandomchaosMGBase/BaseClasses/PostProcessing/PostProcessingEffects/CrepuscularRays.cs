using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RandomchaosMGBase.BaseClasses.PostProcessing
{
    public class CrepuscularRays : BasePostProcessingEffect
    {
        public LightSourceMask lsMask;
        public LightSceneMask mask;
        public LightRay rays;
        public BrightPass bp;
        public SceneBlend blend;

        public Vector3 lightSource
        {
            set
            {
                lsMask.lighSourcePos = value;
                rays.lighSourcePos = value;
            }
        }

        public Texture lightTexture
        {
            get { return lsMask.lishsourceTexture; }
            set { lsMask.lishsourceTexture = value; }
        }

        public float LightSourceSize
        {
            set { lsMask.lightSize = value; }
        }

        public float Density
        {
            get { return rays.Density; }
            set { rays.Density = value; }
        }

        public float Decay
        {
            get { return rays.Decay; }
            set { rays.Decay = value; }
        }

        public float Weight
        {
            get { return rays.Weight; }
            set { rays.Weight = value; }
        }

        public float Exposure
        {
            get { return rays.Exposure; }
            set { rays.Exposure = value; }
        }

        public float BrightThreshold
        {
            get { return bp.BloomThreshold; }
            set { bp.BloomThreshold = value; }
        }


        public CrepuscularRays(Game game, Vector3 lightSourcePos, string lightSourceImage, float lightSourceSize, float density, float decay, float weight, float exposure, float brightThreshold)
            : base(game)
        {
            lsMask = new LightSourceMask(game, lightSourcePos, lightSourceImage, lightSourceSize);
            mask = new LightSceneMask(game, lightSourcePos);
            rays = new LightRay(game, lightSourcePos, density, decay, weight, exposure);
            bp = new BrightPass(game, brightThreshold);
            blend = new SceneBlend(game);

            AddPostProcess(lsMask);
            AddPostProcess(mask);
            AddPostProcess(rays);
            //AddPostProcess(bp);
            AddPostProcess(blend);
        }

    }
}
