using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RandomchaosMGBase.BaseClasses.PostProcessing
{
    public class RadialBlur : BasePostProcess
    {
        public float Scale;

        public RadialBlur(Game game, float scale) : base(game)
        {
            Scale = scale;
        }

        public override void Draw(GameTime gameTime)
        {
            if (effect == null)
                effect = Game.Content.Load<Effect>("Shaders/PostProcessing/RadialBlur");


            effect.Parameters["radialBlurScaleFactor"].SetValue(Scale);
            effect.Parameters["windowSize"].SetValue(new Vector2(BackBuffer.Height, BackBuffer.Width));

            // Set Params.
            base.Draw(gameTime);
        }
    }
}
