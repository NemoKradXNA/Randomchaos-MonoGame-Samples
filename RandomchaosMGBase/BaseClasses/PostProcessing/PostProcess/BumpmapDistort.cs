using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RandomchaosMGBase.BaseClasses.PostProcessing
{
    public class BumpmapDistort : BasePostProcess
    {
        private double elapsedTime = 0;
        public bool High = true;
        public string BumpAsset;

        public BumpmapDistort(Game game, string bumpAsset, bool high) : base(game)
        {
            BumpAsset = bumpAsset;
            High = high;
        }

        public override void Draw(GameTime gameTime)
        {
            if (effect == null)
            {
                effect = Game.Content.Load<Effect>("Shaders/PostProcessing/BumpMapDistort");
            }

            if (High)
                effect.CurrentTechnique = effect.Techniques["High"];
            else
                effect.CurrentTechnique = effect.Techniques["Low"];

            elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;

            if (elapsedTime >= 10.0f)
                elapsedTime = 0.0f;

            effect.Parameters["Offset"].SetValue((float)elapsedTime * .1f);
            effect.Parameters["Bumpmap"].SetValue(Game.Content.Load<Texture2D>(BumpAsset));

            effect.Parameters["halfPixel"].SetValue(HalfPixel);

            // Set Params.
            base.Draw(gameTime);
        }
    }
}
