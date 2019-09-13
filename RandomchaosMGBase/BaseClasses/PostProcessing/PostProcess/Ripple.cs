using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RandomchaosMGBase.BaseClasses.PostProcessing
{
    public class Ripple : BasePostProcess
    {
        protected float divisor = .5f;
        protected float distortion = 2.5f;
        protected Vector2 pos = new Vector2(.5f, .5f);


        public float Distortion
        {
            get { return distortion; }
            set { distortion = value; }
        }

        public Vector2 ScreenPosition
        {
            get { return pos; }
            set { pos = value; }
        }

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
            effect.Parameters["centerCoord"].SetValue(pos);

            // Set Params.
            base.Draw(gameTime);
        }
    }
}
