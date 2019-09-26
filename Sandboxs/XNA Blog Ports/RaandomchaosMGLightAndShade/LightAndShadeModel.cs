using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RandomchaosMGBase.BaseClasses;

namespace RandomchaosMGLightAndShade
{
    public class LightAndShadeModel : Base3DObject
    {
        public float AmbientIntensity { get; set; }
        public Color AmbientColor { get; set; }
        public Color Tint { get; set; }
        
        public List<LightModel> Lights { get; set; }

        public bool MultiLight { get; set; }

        public string GlowAsset { get; set; }
        public string ReflectionAsset { get; set; }

        public string CloudMap { get; set; }
        public string WaterRipples { get; set; }
        public string AtmosAsset { get; set; }

        public float cloudSpeed { get; set; }
        public float cloudHeight { get; set; }
        public float cloudShadowIntensity { get; set; }

        public LightAndShadeModel(Game game, string modelAssetName, string effectAsset) : base(game, modelAssetName, effectAsset)
        {
            Lights = new List<LightModel>() { null, null, null };
        }

        public LightAndShadeModel(Game game, string modelAssetName, string effecAsset, string diffuseTextureAsset, string glowMap = null, string bumpMap = null, string reflectionMap = null, string cloudMap = null, string waterRipples = null, string atmosAsset = null) : this(game, modelAssetName, effecAsset)
        {
            ColorAsset = diffuseTextureAsset;
            GlowAsset = glowMap;
            BumpAsset = bumpMap;
            ReflectionAsset = reflectionMap;

            CloudMap = cloudMap;
            WaterRipples = waterRipples;
            AtmosAsset = atmosAsset;

            cloudSpeed = -.0025f;
            cloudHeight = .005f;
            cloudShadowIntensity = .5f;

        Tint = Color.White;
        }

        public override void SetEffect(GameTime gameTime, Effect effect)
        {
            base.SetEffect(gameTime, effect);


            if (effect.Parameters["AmbientIntensity"] != null)
                effect.Parameters["AmbientIntensity"].SetValue(AmbientIntensity);

            if(effect.Parameters["AmbientColor"] != null)
                effect.Parameters["AmbientColor"].SetValue(AmbientColor.ToVector4());

            if (effect.Parameters["itw"] != null)
                effect.Parameters["itw"].SetValue(Matrix.Invert(Matrix.Transpose(World)));

            if (MultiLight)
            {
                // Calculate the light direction in relation to me.
                if (effect.Parameters["LightDirection"] != null)
                    effect.Parameters["LightDirection"].SetValue(Lights.Select(p => p.Position - Position).ToArray());

                if (effect.Parameters["DiffuseIntensity"] != null)
                    effect.Parameters["DiffuseIntensity"].SetValue(Lights.Select(p => p.LightIntensity).ToArray());
                if (effect.Parameters["DiffuseColor"] != null)
                    effect.Parameters["DiffuseColor"].SetValue(Lights.Select(p => p.Color.ToVector4()).ToArray());

                if (effect.Parameters["SpecularColor"] != null)
                    effect.Parameters["SpecularColor"].SetValue(Lights.Select(p => p.SpecularColor.ToVector4()).ToArray());
                if (effect.Parameters["SpecularIntensity"] != null)
                    effect.Parameters["SpecularIntensity"].SetValue(Lights.Select(p => p.SpecularIntensity).ToArray());
            }
            else
            {
                if (effect.Parameters["LightDirection"] != null)
                    effect.Parameters["LightDirection"].SetValue(Lights[0].Position - Position);

                if (effect.Parameters["DiffuseIntensity"] != null)
                    effect.Parameters["DiffuseIntensity"].SetValue(Lights[0].LightIntensity);
                if (effect.Parameters["DiffuseColor"] != null)
                    effect.Parameters["DiffuseColor"].SetValue(Lights[0].Color.ToVector4());

                if (effect.Parameters["SpecularColor"] != null)
                    effect.Parameters["SpecularColor"].SetValue(Lights[0].SpecularColor.ToVector4());
                if (effect.Parameters["SpecularIntensity"] != null)
                    effect.Parameters["SpecularIntensity"].SetValue(Lights[0].SpecularIntensity);
            }

            if (effect.Parameters["CameraPosition"] != null)
                effect.Parameters["CameraPosition"].SetValue(camera.Position);

            if (effect.Parameters["ColorMap"] != null)
                effect.Parameters["ColorMap"].SetValue(Game.Content.Load<Texture2D>(ColorAsset));
            if (effect.Parameters["GlowMap"] != null)
                effect.Parameters["GlowMap"].SetValue(Game.Content.Load<Texture2D>(GlowAsset));
            if (effect.Parameters["BumpMap"] != null)
                effect.Parameters["BumpMap"].SetValue(Game.Content.Load<Texture2D>(BumpAsset));
            if (effect.Parameters["ReflectionMap"] != null)
                effect.Parameters["ReflectionMap"].SetValue(Game.Content.Load<Texture2D>(ReflectionAsset));

            if (effect.Parameters["worldIT"] != null)
                effect.Parameters["worldIT"].SetValue(Matrix.Invert(Matrix.Transpose(World)));

            if (effect.Parameters["viewI"] != null)
                effect.Parameters["viewI"].SetValue(Matrix.Invert(camera.View));

            if (effect.Parameters["cubeMap"] != null)
                effect.Parameters["cubeMap"].SetValue(Game.Content.Load<TextureCube>(ColorAsset));

            if (effect.Parameters["tint"] != null)
                effect.Parameters["tint"].SetValue(Tint.ToVector4());

            if (effect.Parameters["fresnelTex"] != null)
                effect.Parameters["fresnelTex"].SetValue(Game.Content.Load<Texture2D>(GlowAsset));

            if (effect.Parameters["EyePosition"] != null)
                effect.Parameters["EyePosition"].SetValue(camera.Position);

            if(effect.Parameters["time"] != null)
                effect.Parameters["time"].SetValue((float)gameTime.TotalGameTime.TotalSeconds * 3);

            
            if(effect.Parameters["CloudMap"] != null)
                effect.Parameters["CloudMap"].SetValue(Game.Content.Load<Texture2D>(CloudMap));

            if(effect.Parameters["WaveMap"] != null)
                effect.Parameters["WaveMap"].SetValue(Game.Content.Load<Texture2D>(WaterRipples));

            if(effect.Parameters["AtmosMap"] != null)
                effect.Parameters["AtmosMap"].SetValue(Game.Content.Load<Texture2D>(AtmosAsset));

            if(effect.Parameters["cloudSpeed"] != null)
                effect.Parameters["cloudSpeed"].SetValue(cloudSpeed);

            if(effect.Parameters["cloudHeight"] != null)
                effect.Parameters["cloudHeight"].SetValue(cloudHeight);

            if(effect.Parameters["cloudShadowIntensity"] != null)
                effect.Parameters["cloudShadowIntensity"].SetValue(cloudShadowIntensity);

            
        }

        public void SetLightParams(int idx, LightModel light)
        {
            Lights[idx] = light;
        }
    }
}
