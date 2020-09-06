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

        public float NoiseFrequency { get; set; }
        public float NoiseStrength { get; set; }
        public float TimeScale { get; set; }
        public Vector3 NoiseScale { get; set; }
        public Vector3 NoiseAnimation { get; set; }
        public Vector3 FlameScale { get; set; }
        public Vector3 FlameTranslation { get; set; }

        public RCLayeredFire(Game game, int layers) : base(game,null, null)
        {
            flameLayers = layers;
            flames = new List<Base3DQuad>();

            NoiseFrequency = .10f;
            NoiseStrength = 1.0f;
            TimeScale = 1.0f;
            NoiseScale = new Vector3(1.0f, 1f, 0);
            NoiseAnimation = new Vector3(0.0f, -0.1f, 0.0f);
            FlameScale = new Vector3(1, -1.0f, 1);
            FlameTranslation = new Vector3(0.0f, 0.0f, 0.0f);

            for (int f = 0; f < flameLayers; f++)
            {
                flames.Add(new Base3DQuad(game, "Shaders/VolumetricFire"));
                flames[f].Transform.Parent = this;
                flames[f].ColorAsset = "Textures/flame";
                flames[f].Rotation = Rotation;
                flames[f].Position = new Vector3(0, 0, .01f * f);// ((float)f*.5f));

                flames[f].Transform.SetParent(this);
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
            // Look at the camera.
            //Transform.LookAtLockRotation(camera.Transform.Position, 1, Vector3.Backward, new Vector3(1, 0, 1));
            
            Transform.Update();

            foreach (Base3DQuad flame in flames)
                flame.Update(gameTime);
        }

        Texture3D noiseTxture3D = null;

        public override void SetEffect(GameTime gameTime, Effect effect)
        {
            foreach (Base3DQuad flame in flames)
            {
                Effect flameEffect = flame.Effect;

                if (flameEffect == null)
                    continue;

                if (flameEffect.Parameters["flameTexture"] != null)
                    flameEffect.Parameters["flameTexture"].SetValue(Game.Content.Load<Texture2D>("Textures/flame"));
                if (flameEffect.Parameters["ticks"] != null)
                    flameEffect.Parameters["ticks"].SetValue((float)gameTime.TotalGameTime.TotalSeconds * animSpeed);

                if (noiseTxture3D == null)
                {
                    noiseTxture3D = new Texture3D(Game.GraphicsDevice, 32, 32, 32, false, SurfaceFormat.Rgba1010102);
                    Color[] c = new Color[32 * 32 * 32];
                    
                    Color[] tc = new Color[32 * 32];
                    for (int l = 0; l < 32; l++)
                    {
                        Texture2D tl = Game.Content.Load<Texture2D>($"Textures/Noise1/{l}");
                        tl.GetData(tc);

                        Array.Copy(tc, 0, c, l * 32, tc.Length);
                    }

                    noiseTxture3D.SetData(c);
                }

                if (flameEffect.Parameters["noiseTexture"] != null)
                    flameEffect.Parameters["noiseTexture"].SetValue(noiseTxture3D);

                if (flameEffect.Parameters["Index"] != null)
                    flameEffect.Parameters["Index"].SetValue(flameOffset + (flames.IndexOf(flame) *.5f) *.1f);

                if (flameEffect.Parameters["noiseFreq"] != null)
                    flameEffect.Parameters["noiseFreq"].SetValue(NoiseFrequency);

                if (flameEffect.Parameters["noiseStrength"] != null)
                    flameEffect.Parameters["noiseStrength"].SetValue(NoiseStrength);
                if (flameEffect.Parameters["timeScale"] != null)
                    flameEffect.Parameters["timeScale"].SetValue(TimeScale);
                if (flameEffect.Parameters["noiseScale"] != null)
                    flameEffect.Parameters["noiseScale"].SetValue(NoiseScale);
                if (flameEffect.Parameters["noiseAnim"] != null)
                    flameEffect.Parameters["noiseAnim"].SetValue(NoiseAnimation);
                if (flameEffect.Parameters["flameColor"] != null)
                    flameEffect.Parameters["flameColor"].SetValue(Color.ToVector4());
                if (flameEffect.Parameters["flameScale"] != null)
                    flameEffect.Parameters["flameScale"].SetValue(FlameScale);
                 if (flameEffect.Parameters["flameTrans"] != null)
                    flameEffect.Parameters["flameTrans"].SetValue(FlameTranslation);
            }
        }

        public override void Draw(GameTime gameTime, Effect effect)
        {
            if (!Enabled)
                return;

            SetEffect(gameTime, effect);

            foreach (Base3DQuad flame in flames)
            {
                //flame.Effect.Parameters["Index"].SetValue(flameOffset + ((float)(flames.IndexOf(flame)+1) / flames.Count) );
                flame.Effect.Parameters["Index"].SetValue(flameOffset + (flames.IndexOf(flame) * .5f) * .1f);
                flame.Effect.Parameters["worldId"].SetValue(Matrix.CreateScale(Scale) * Matrix.Identity * Matrix.CreateTranslation(Position));
                flame.Draw(gameTime);
            }
        }
    }
}
