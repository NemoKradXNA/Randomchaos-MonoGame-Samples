using Microsoft.Xna.Framework;

namespace RandomchaosMGBase.BaseClasses.PostProcessing
{
    public class FogEffect : BasePostProcessingEffect
    {
        public Fog fog;

        public float FogDistance
        {
            get { return fog.FogDistance; }
            set { fog.FogDistance = MathHelper.Clamp(value, camera.Viewport.MinDepth, camera.Viewport.MaxDepth); }
        }

        public float FogRange
        {
            get { return fog.FogRange; }
            set { fog.FogRange = MathHelper.Clamp(value, camera.Viewport.MinDepth, camera.Viewport.MaxDepth); }
        }

        public FogEffect(Game game, float distance, float range, Color color) : base(game)
        {
            fog = new Fog(game, distance, range, color);

            AddPostProcess(fog);
        }
    }
}
