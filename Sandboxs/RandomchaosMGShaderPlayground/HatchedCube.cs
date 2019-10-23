using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RandomchaosMGShaderPlayground
{
    public class HatchedCube : RandomchaosMGBase.BaseClasses.Primatives.Cube
    {
        public float HatchDencity { get; set; }
        public HatchedCube(Game1 game) : base(game, "Shaders/crosshatch") { HatchDencity = 10; }

        public override void SetEffect(GameTime gameTime, Effect effect)
        {
            base.SetEffect(gameTime, effect);

            effect.Parameters["hatch1Map"].SetValue(Game.Content.Load<Texture2D>("Textures/Crosshatch/hatchrgb0"));
            effect.Parameters["hatch0Map"].SetValue(Game.Content.Load<Texture2D>("Textures/Crosshatch/hatchrgb1"));
            effect.Parameters["hatchingDensity"].SetValue(HatchDencity);
        }
    }
}
