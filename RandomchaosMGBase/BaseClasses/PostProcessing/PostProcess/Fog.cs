using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RandomchaosMGBase.BaseClasses.PostProcessing
{
    public class Fog : BasePostProcess
    {
        public float FogDistance;
        public float FogRange;
        public Color FogColor;

        public Fog(Game game, float distance, float range, Color color) : base(game)
        {
            FogDistance = distance;
            FogRange = range;
            FogColor = color;
        }

        public override void Draw(GameTime gameTime)
        {
            if (effect == null)
                effect = Game.Content.Load<Effect>("Shaders/PostProcessing/Fog");

            //effect.Parameters["screenTexture"].SetValue(orgBuffer);
            effect.Parameters["depthMap"].SetValue(DepthBuffer);

            effect.Parameters["camMin"].SetValue(camera.Viewport.MinDepth);
            effect.Parameters["camMax"].SetValue(camera.Viewport.MaxDepth);
            effect.Parameters["fogDistance"].SetValue(FogDistance);
            effect.Parameters["fogRange"].SetValue(FogRange);
            effect.Parameters["fogColor"].SetValue(FogColor.ToVector4());

            // Set Params.
            base.Draw(gameTime);
        }
    }
}
