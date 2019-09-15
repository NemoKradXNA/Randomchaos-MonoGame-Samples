using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RandomchaosMGBase.BaseClasses.PostProcessing
{
    public class ScanLines  : BasePostProcess
    {

        public float NoiseIntensity { get; set; }
        public float LineIntensity { get; set; }
        public float LineCount { get; set; }

        public ScanLines(Game game, float noiseIntensity, float lineIntensity, float lineCount) : base(game)
        {
            NoiseIntensity = noiseIntensity;
            LineCount = lineCount;
            LineIntensity = lineIntensity;
        }

        public override void Draw(GameTime gameTime)
        {
            if (effect == null)
            {
                effect = Game.Content.Load<Effect>("Shaders/PostProcessing/ScanLines");
            }

            effect.Parameters["Timer"].SetValue((float)gameTime.TotalGameTime.TotalMilliseconds);
            effect.Parameters["fNintensity"].SetValue(NoiseIntensity);
            effect.Parameters["fSintensity"].SetValue(LineIntensity);
            effect.Parameters["fScount"].SetValue(LineCount);

            // Set Params.
            base.Draw(gameTime);
        }
    }
}
