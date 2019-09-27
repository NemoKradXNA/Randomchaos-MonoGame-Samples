using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RandomchaosMGBase.BaseClasses;

namespace RandomchaosMGFire
{
    public class RCLayeredFire : Base3DObject
    {
        protected List<Base3DQuad> flames;

        protected int flameLayers;
        protected float animSpeed;
        protected float tick;
        protected float flameOffset;
        
        protected float AnimationSpeed { get { return animSpeed; } set { animSpeed = value; } }
        protected float FlameOffSet { get { return flameOffset; } set { flameOffset = value; } }

        public RCLayeredFire(Game game, int layers) : base(game,null)
        {
            flameLayers = layers;
            flames = new List<Base3DQuad>();

            int totalFlames = flameLayers * 2;

            for (int f = 0; f < totalFlames; f++)
            {
                flames.Add(new Base3DQuad(game, "Shaders/VolumetricFire"));
                flames[f].ColorAsset = "Textures/flame";
                flames[f].Rotation = Rotation;


                if (flameLayers > totalFlames / 2)
                    flames[f].Rotate(new Vector3(0, 0, 1), MathHelper.Pi);
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            foreach (Base3DQuad flame in flames)
                flame.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            Transform.Update();

            foreach (Base3DQuad flame in flames)
                flame.Update(gameTime);
        }

        public override void SetEffect(GameTime gameTime, Effect effect)
        {
            foreach (Base3DQuad flame in flames)
            {
                Effect flameEffect = flame.Effect;

                if (flameEffect.Parameters["flameTexture"] != null)
                    flameEffect.Parameters["flameTexture"].SetValue(Game.Content.Load<Texture2D>("Textures/flame"));
                if (flameEffect.Parameters["ticks"] != null)
                    flameEffect.Parameters["ticks"].SetValue(tick += animSpeed);

                throw new Exception("Don't run this project, it is still under construction, trying to figure out why 3D textures won't import :/");
                if (flameEffect.Parameters["noiseTexture"] != null)
                    flameEffect.Parameters["noiseTexture"].SetValue(Game.Content.Load<Texture3D>("Textures/noise1"));

                if (flameEffect.Parameters["Index"] != null)
                    flameEffect.Parameters["Index"].SetValue(flameOffset + (float)Convert.ToDouble((flames.IndexOf(flame)/2)) / 10);

                if (flameEffect.Parameters["noiseFreq"] != null)
                    flameEffect.Parameters["noiseFreq"].SetValue(.10f);

                if (flameEffect.Parameters["noiseStrength"] != null)
                    flameEffect.Parameters["noiseStrength"].SetValue(1.0f);
                if (flameEffect.Parameters["timeScale"] != null)
                    flameEffect.Parameters["timeScale"].SetValue(1.0f);
                if (flameEffect.Parameters["noiseScale"] != null)
                    flameEffect.Parameters["noiseScale"].SetValue(new Vector3(1.0f, 1.0f, 1.0f));
                if (flameEffect.Parameters["noiseAnim"] != null)
                    flameEffect.Parameters["noiseAnim"].SetValue(new Vector3( 0.0f, -0.1f, 0.0f ));
                if (flameEffect.Parameters["flameColor"] != null)
                    flameEffect.Parameters["flameColor"].SetValue(new Vector4(0.2f, 0.2f, 0.2f, 1.0f));
                if (flameEffect.Parameters["flameScale"] != null)
                    flameEffect.Parameters["flameScale"].SetValue(new Vector3(1.0f, -1.0f, 1.0f));
                 if (flameEffect.Parameters["flameTrans"] != null)
                    flameEffect.Parameters["flameTrans"].SetValue(new Vector3( 0.0f, 0.0f, 0.0f));
            }
        }

        public override void Draw(GameTime gameTime, Effect effect)
        {
            SetEffect(gameTime, effect);

            foreach (Base3DQuad flame in flames)
                flame.Draw(gameTime);
        }
    }
}
