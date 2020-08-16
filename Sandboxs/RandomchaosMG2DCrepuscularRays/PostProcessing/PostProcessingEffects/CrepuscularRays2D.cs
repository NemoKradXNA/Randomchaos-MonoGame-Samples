using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RandomchaosMGBase.BaseClasses.PostProcessing;


namespace RandomchaosMG2DCrepuscularRays.PostProcessing.PostProcessingEffects
{
    public class CrepuscularRays2D : BasePostProcessingEffect
    {
        public LightSourceMask2D lsMask;
        public LightRay2D rays;

        public Vector2 lightSource
        {
            set
            {
                lsMask.lighSourcePos = value;
                rays.lighSourcePos = value;
            }
            get
            {
                return rays.lighSourcePos;
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
            get { return lsMask.lightSize; }
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


        public CrepuscularRays2D(Game game, Vector2 lightScreenSourcePos, string lightSourceImage, float lightSourceSize, float density, float decay, float weight, float exposure)
            : base(game)
        {
            lsMask = new LightSourceMask2D(game, lightScreenSourcePos, lightSourceImage, lightSourceSize);
            rays = new LightRay2D(game, lightScreenSourcePos, density, decay, weight, exposure);
            
            AddPostProcess(lsMask);
            AddPostProcess(rays);
        }

    }
}
