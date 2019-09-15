using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RandomchaosMGBase.BaseClasses.PostProcessing
{
    public class ColorFilter : BasePostProcess
    {
        public float Burn { get; set; }
        public float Saturation { get; set; }
        public float Bright { get; set; }
        public Color Color { get; set; }

        public ColorFilter(Game game, Color color, float burn, float saturation, float bright) : base(game)
        {
            Burn = burn;
            Saturation = saturation;
            Bright = burn;
            Color = color;
        }

        public override void Draw(GameTime gameTime)
        {
            if (effect == null)
            {
                effect = Game.Content.Load<Effect>("Shaders/PostProcessing/ColourFilter");
            }            

            effect.Parameters["burn"].SetValue(Burn);
            effect.Parameters["saturation"].SetValue(Saturation);
            effect.Parameters["bright"].SetValue(Bright);
            effect.Parameters["color"].SetValue(Color.ToVector3());

            // Set Params.
            base.Draw(gameTime);
        }
    }
}
