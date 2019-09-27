using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

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
        
        public float AnimationSpeed { get { return animSpeed; } set { animSpeed = value; } }
        public float FlameOffSet { get { return flameOffset; } set { flameOffset = value; } }

        public RCLayeredFire(Game game, int layers) : base(game,null)
        {
            flameLayers = layers;
            flames = new List<Base3DQuad>();

            

            for (int f = 0; f < flameLayers; f++)
            {
                flames.Add(new Base3DQuad(game, "Shaders/VolumetricFire"));
                flames[f].Transform.Parent = this;
                flames[f].ColorAsset = "Textures/flame";
                flames[f].Rotation = Rotation;
                flames[f].Position = new Vector3(0, 0, -.01f * ((float)f*.5f));
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

        Texture3D test = null;

        public override void SetEffect(GameTime gameTime, Effect effect)
        {
            foreach (Base3DQuad flame in flames)
            {
                Effect flameEffect = flame.Effect;

                if (flameEffect.Parameters["flameTexture"] != null)
                    flameEffect.Parameters["flameTexture"].SetValue(Game.Content.Load<Texture2D>("Textures/flame"));
                if (flameEffect.Parameters["ticks"] != null)
                    flameEffect.Parameters["ticks"].SetValue(tick += animSpeed);

                if (test == null)
                {
                    test = new Texture3D(Game.GraphicsDevice, 32, 32, 32, false, SurfaceFormat.Rgba1010102);
                    Color[] c = new Color[32 * 32 * 32];
                    
                    Color[] tc = new Color[32 * 32];
                    for (int l = 0; l < 32; l++)
                    {
                        Texture2D tl = Game.Content.Load<Texture2D>($"Textures/Noise1/{l}");
                        tl.GetData(tc);

                        Array.Copy(tc, 0, c, l * 32, tc.Length);
                    }

                    test.SetData(c);
                }
                if (flameEffect.Parameters["noiseTexture"] != null)
                    flameEffect.Parameters["noiseTexture"].SetValue(test);

                if (flameEffect.Parameters["Index"] != null)
                    flameEffect.Parameters["Index"].SetValue(flameOffset + (flames.IndexOf(flame) *.5f) *.1f);

                if (flameEffect.Parameters["noiseFreq"] != null)
                    flameEffect.Parameters["noiseFreq"].SetValue(.10f);

                if (flameEffect.Parameters["noiseStrength"] != null)
                    flameEffect.Parameters["noiseStrength"].SetValue(1.0f);
                if (flameEffect.Parameters["timeScale"] != null)
                    flameEffect.Parameters["timeScale"].SetValue(1.0f);
                if (flameEffect.Parameters["noiseScale"] != null)
                    flameEffect.Parameters["noiseScale"].SetValue(new Vector3(1.0f, 1f, 1.0f));
                if (flameEffect.Parameters["noiseAnim"] != null)
                    flameEffect.Parameters["noiseAnim"].SetValue(new Vector3( 0.0f, -0.1f, 0.0f ));
                if (flameEffect.Parameters["flameColor"] != null)
                    flameEffect.Parameters["flameColor"].SetValue(new Vector4(0.2f, 0.2f, 0.2f, 1.0f));
                if (flameEffect.Parameters["flameScale"] != null)
                    flameEffect.Parameters["flameScale"].SetValue(new Vector3(1, -1.0f, 1));
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
