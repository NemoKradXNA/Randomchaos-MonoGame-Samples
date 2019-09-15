using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RandomchaosMGBase.BaseClasses.PostProcessing
{
    public class DeRezed : BasePostProcess
    {
        public int NumberOfTiles { get; set; }

        public DeRezed(Game game, int numberOfTiles) : base(game)
        {
            NumberOfTiles = numberOfTiles;
        }

        public override void Draw(GameTime gameTime)
        {
            if (effect == null)
            {
                effect = Game.Content.Load<Effect>("Shaders/PostProcessing/DeRezed");
            }

            effect.Parameters["numberOfTiles"].SetValue(NumberOfTiles);

            // Set Params.
            base.Draw(gameTime);
        }
    }
}
