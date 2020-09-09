using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RandomchaosMGBase.BaseClasses.PostProcessing
{
    public class FXAA  : BasePostProcess
    {
        public float ContrastThreshold { get; set; }
        public float RelativeThreshold { get; set; }
        public float SubpixelBlending { get; set; }

        public bool RenderHalfScreen { get; set; }

        public float Vline { get; set; }

        public enum Technique : int
        {
            BasicLuminance,
            LinearRgbLuminance,
            Contrast,
            ContrastSkipLow,
            ContrastSkipHigh,
            ContrastSkipBoth,
            ContrastSkip,
            BlendFactor,
            BlendFactorHorizontal,
            Blending,
            EdgeLumincense,
            FXAA,
            MAX_DO_NOT_USE
        }

        public Technique TechniqueUsed { get; set; }

        public FXAA(Game game, float contrastThreshold = 0.0312f, float relativeThreshold = 0.063f, float subpixelBlending = 1f, Technique techniqueUsed = Technique.FXAA) : base(game)
        {
            ContrastThreshold = contrastThreshold;
            RelativeThreshold = relativeThreshold;
            SubpixelBlending = subpixelBlending;
            TechniqueUsed = techniqueUsed;
            Vline = .5f;
        }

        public override void Draw(GameTime gameTime)
        {
            if (effect == null)
            {
                effect = Game.Content.Load<Effect>("Shaders/PostProcessing/FXAA");
            }

            effect.CurrentTechnique = effect.Techniques[$"{TechniqueUsed}"];
            effect.Parameters["width"].SetValue(Game.GraphicsDevice.PresentationParameters.BackBufferWidth);
            effect.Parameters["height"].SetValue(Game.GraphicsDevice.PresentationParameters.BackBufferHeight);

            effect.Parameters["_ContrastThreshold"].SetValue(ContrastThreshold);
            effect.Parameters["_RelativeThreshold"].SetValue(RelativeThreshold);
            effect.Parameters["_SubpixelBlending"].SetValue(SubpixelBlending);
            effect.Parameters["RenderHalfScreen"].SetValue(RenderHalfScreen);
            effect.Parameters["VLine"].SetValue(Vline);

            base.Draw(gameTime);
        }
    }
}
