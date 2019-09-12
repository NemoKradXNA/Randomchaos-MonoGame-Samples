using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RandomchaosMGBase.BaseClasses.PostProcessing
{
    public class Ripple : BasePostProcess
    {
        float divisor = .5f;
        float distortion = 2.5f;

        public Ripple(Game game) : base(game)
        { }

        public override void Draw(GameTime gameTime)
        {
            if (effect == null)
                effect = Game.Content.Load<Effect>("Shaders/PostProcessing/Ripple");

            if (divisor > 1.25f)
                divisor = .4f;

            divisor += (float)gameTime.ElapsedGameTime.TotalSeconds * 0.5f;

            effect.Parameters["wave"].SetValue(MathHelper.Pi / divisor);
            effect.Parameters["distortion"].SetValue(distortion);
            effect.Parameters["centerCoord"].SetValue(new Vector2(.5f, .5f));

            // Set Params.
            base.Draw(gameTime);
        }
    }
}
