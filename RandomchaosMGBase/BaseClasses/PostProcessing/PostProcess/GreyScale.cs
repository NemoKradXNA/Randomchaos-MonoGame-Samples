using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace RandomchaosMGBase.BaseClasses.PostProcessing
{
    public class GreyScale : BasePostProcess
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="game"></param>
        public GreyScale(Game game) : base(game) { }

        /// <summary>
        /// Draw
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            if (effect == null)
                effect = Game.Content.Load<Effect>("Shaders/PostProcessing/GreyScale");

            effect.Parameters["screen"].SetValue(BackBuffer);

            base.Draw(gameTime);
        }
    }
}
