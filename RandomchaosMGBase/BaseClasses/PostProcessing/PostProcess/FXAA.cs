using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RandomchaosMGBase.BaseClasses.PostProcessing
{
    public class FXAA  : BasePostProcess
    {
        public float ContrastThreshold { get; set; }
        public float RelativeThreshold { get; set; }
        public float SubpixelBlending { get; set; }

        public FXAA(Game game, float contrastThreshold = 0.0312f, float relativeThreshold = 0.063f, float subpixelBlending = 1f) : base(game)
        {
            ContrastThreshold = contrastThreshold;
            RelativeThreshold = relativeThreshold;
            SubpixelBlending = subpixelBlending;
        }

        public override void Draw(GameTime gameTime)
        {
            if (effect == null)
            {
                effect = Game.Content.Load<Effect>("Shaders/PostProcessing/FXAA");
            }

            effect.Parameters["width"].SetValue(Game.GraphicsDevice.PresentationParameters.BackBufferWidth);
            effect.Parameters["height"].SetValue(Game.GraphicsDevice.PresentationParameters.BackBufferHeight);

            effect.Parameters["_ContrastThreshold"].SetValue(ContrastThreshold);
            effect.Parameters["_RelativeThreshold"].SetValue(RelativeThreshold);
            effect.Parameters["_SubpixelBlending"].SetValue(SubpixelBlending);

            base.Draw(gameTime);
        }
    }
}
