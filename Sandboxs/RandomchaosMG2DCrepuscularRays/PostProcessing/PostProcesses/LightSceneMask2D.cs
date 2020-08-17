using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RandomchaosMGBase.BaseClasses.PostProcessing
{
    public class LightSceneMask2D : BasePostProcess
    {
        Vector2 lighSourcePos;
        public LightSceneMask2D(Game game, Vector2 sourcePos)
            : base(game)
        {
            lighSourcePos = sourcePos;
            UsesVertexShader = true;
        }

        public override void Draw(GameTime gameTime)
        {
            if (effect == null)
                effect = Game.Content.Load<Effect>("Shaders/PostProcessing/LightSceneMask2D");

            effect.CurrentTechnique = effect.Techniques["LightSourceSceneMask"];

            effect.Parameters["depthMap"].SetValue(orgBuffer);

            //effect.Parameters["lightPosition"].SetValue(lighSourcePos);


            // Set Params.
            base.Draw(gameTime);

        }
    }
}
