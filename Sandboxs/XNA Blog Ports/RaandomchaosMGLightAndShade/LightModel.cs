using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RandomchaosMGBase.BaseClasses;

namespace RandomchaosMGLightAndShade
{
    public class LightModel : Base3DObject
    {
        public Color SpecularColor { get; set; }
        public float SpecularIntensity { get; set; }
        public float LightIntensity { get; set; }

        public LightModel(Game game, Color color) : base(game, "Models/sphere", null)
        {
            Color = color;
            Scale = Vector3.One * .5f;
            LightIntensity = 1;
        }

        public LightModel(Game game, Color color, Color specularColor) : this(game, color)
        {
            SpecularColor = specularColor;
            SpecularIntensity = 1;
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            Effect = new BasicEffect(GraphicsDevice);
        }

        public override void SetEffect(GameTime gameTime, Effect effect)
        {
            base.SetEffect(gameTime, effect);

            ((BasicEffect)effect).World = World;
            ((BasicEffect)effect).View = camera.View;
            ((BasicEffect)effect).Projection = camera.Projection;
            ((BasicEffect)effect).LightingEnabled = false;

            ((BasicEffect)effect).DiffuseColor = Color.ToVector3();
            ((BasicEffect)effect).AmbientLightColor = Color.ToVector3();
        }
    }
}
