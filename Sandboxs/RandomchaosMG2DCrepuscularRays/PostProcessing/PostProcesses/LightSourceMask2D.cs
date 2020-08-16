using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RandomchaosMGBase.BaseClasses.PostProcessing;

namespace RandomchaosMG2DCrepuscularRays.PostProcessing
{
    public class LightSourceMask2D : BasePostProcess
    {
        public Texture lishsourceTexture;
        public Vector2 lighSourcePos;
        public string lightSourceasset;
        public float lightSize = 1500;

        public LightSourceMask2D(Game game, Vector2 sourcePos, string lightSourceasset, float lightSize)
            : base(game)
        {
            UsesVertexShader = true;
            lighSourcePos = sourcePos;
            this.lightSourceasset = lightSourceasset;
            this.lightSize = lightSize;
        }

        public override void Draw(GameTime gameTime)
        {

            if (effect == null)
            {
                effect = Game.Content.Load<Effect>("Shaders/PostProcessing/LightSourceMask2D");
                lishsourceTexture = Game.Content.Load<Texture2D>(lightSourceasset);
            }

            effect.CurrentTechnique = effect.Techniques["LightSourceMask"];

            effect.Parameters["flare"].SetValue(lishsourceTexture);

            effect.Parameters["SunSize"].SetValue(lightSize);
            effect.Parameters["lightPosition"].SetValue(lighSourcePos);

            // Set Params.
            base.Draw(gameTime);

        }
    }
}
