using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RandomchaosMGBase.BaseClasses;

namespace RandomchaosMGShaderPlayground
{
    /// <summary>
    /// Shader source: http://kylehalladay.com/blog/tutorial/2017/02/21/Pencil-Sketch-Effect.html
    /// </summary>
    public class Base3DObjectCrossHatch : Base3DObject
    {
        public float HatchDencity { get; set; }
        public Base3DObjectCrossHatch(Game1 game, string model) : base(game, model, "Shaders/crosshatch") { HatchDencity = 10; }

        public override void SetEffect(GameTime gameTime, Effect effect)
        {
            base.SetEffect(gameTime, effect);

            effect.Parameters["hatch1Map"].SetValue(Game.Content.Load<Texture2D>("Textures/Crosshatch/hatchrgb0"));
            effect.Parameters["hatch0Map"].SetValue(Game.Content.Load<Texture2D>("Textures/Crosshatch/hatchrgb1"));
            effect.Parameters["hatchingDensity"].SetValue(HatchDencity);
        }
    }
}
