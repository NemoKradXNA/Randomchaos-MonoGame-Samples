using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RandomchaosMGBase.BaseClasses.PostProcessing;

namespace RandomchaosMG2DCrepuscularRays.PostProcessing
{
    public class LightRay2D : BasePostProcess
    {
        public Vector2 lighSourcePos;
        public float Density = .5f;
        public float Decay = .95f;
        public float Weight = 1.0f;
        public float Exposure = .15f;

        public LightRay2D(Game game, Vector2 sourcePos, float density, float decay, float weight, float exposure)
            : base(game)
        {
            lighSourcePos = sourcePos;

            Density = density;
            Decay = decay;
            Weight = weight;
            Exposure = exposure;
            UsesVertexShader = true;
        }


        public override void Draw(GameTime gameTime)
        {
            if (effect == null)
                effect = Game.Content.Load<Effect>("Shaders/PostProcessing/LigthRays2D");

            effect.CurrentTechnique = effect.Techniques["LightRayFX"];

            effect.Parameters["Density"].SetValue(Density);
            effect.Parameters["Decay"].SetValue(Decay);
            effect.Parameters["Weight"].SetValue(Weight);
            effect.Parameters["Exposure"].SetValue(Exposure);

            effect.Parameters["lightPosition"].SetValue(lighSourcePos);

            // Set Params.
            base.Draw(gameTime);

        }
    }
}
