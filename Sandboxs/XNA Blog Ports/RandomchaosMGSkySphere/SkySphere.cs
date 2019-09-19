using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RandomchaosMGBase.BaseClasses;

namespace RandomchaosMGSkySphere
{
    public class SkySphere : Base3DObject
    {
        public float tod = 12; // Mid day
        
        public Vector3 SunPos = new Vector3(0, 1, 0);

        string modelAsset;
        string textureAsset;

        TextureCube environ;
        Texture2D[] cloudSystem;
        string[] cloudSystemAsset;

        public Color MorningTint = Color.Gold;
        public Color EveningTint = Color.Red;

        private float cloudAnim = .5f;
        /// <summary>
        /// value should be from 0 - 1. Recommend .0001 - .00005
        /// </summary>
        public float cloudAnimSpeed = .0001f;
        public float cloudIntensity = .5f;

        public Color NightColor = Color.Navy;
        public Color SkyColor = Color.LightSkyBlue;

        public bool ReaTime = true;

        public SkySphere(Game game, string modelAsset, string textureAsset, string[] cloudSystems) : base(game,modelAsset)
        {
            this.modelAsset = modelAsset;
            this.textureAsset = textureAsset;

            Position = new Vector3(0, 0, 0);
            Rotation = new Quaternion(0, 0, 0, 1);
            Scale = new Vector3(900, 900, 900);

            cloudSystemAsset = cloudSystems;
        }

        public string GetTime()
        {
            string retVal = string.Format("{0:00}:{1:00}:{2:00}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            if (!ReaTime)
            {
                retVal = tod.ToString();

                string hr = ((int)tod).ToString();
                string tmn = "0";
                string umn = "0";
                string tsc = "0";
                string usc = "0";

                if (retVal.IndexOf(".") != -1)
                {
                    tmn = tod.ToString().Substring(tod.ToString().IndexOf(".") + 1, 1);
                    if (retVal.Substring(retVal.IndexOf(".") + 1).Length > 1)
                        umn = tod.ToString().Substring(tod.ToString().IndexOf(".") + 2, 1);

                    if (retVal.Substring(retVal.IndexOf(".") + 1).Length > 2)
                        tsc = tod.ToString().Substring(tod.ToString().IndexOf(".") + 3, 1);
                    if (retVal.Substring(retVal.IndexOf(".") + 1).Length > 3)
                        usc = tod.ToString().Substring(tod.ToString().IndexOf(".") + 4, 1);
                }

                int imn = (int)((Convert.ToDecimal(tmn) / 10) * 60) + (int)((Convert.ToDecimal(umn) / 100) * 60);
                int isc = (int)((Convert.ToDecimal(tsc) / 10) * 60) + (int)((Convert.ToDecimal(usc) / 100) * 60);

                retVal = string.Format("{0:00}:{1:00}:{2:00}", hr, imn, isc);
            }

            return retVal;
        }


        public override void Initialize()
        {
            base.Initialize();

            Effect = Game.Content.Load<Effect>("Shaders/dynamicSkybox");
            environ = Game.Content.Load<TextureCube>(textureAsset);

            cloudSystem = new Texture2D[cloudSystemAsset.Length];
            for (int c = 0; c < cloudSystem.Length; c++)
                cloudSystem[c] = Game.Content.Load<Texture2D>(cloudSystemAsset[c]);
        }

        public override void Update(GameTime gameTime)
        {
            long div = 10000;
            //div = 1000;

            if (!ReaTime)
                tod += ((float)gameTime.ElapsedGameTime.Milliseconds / div);
            else
                tod = ((float)DateTime.Now.Hour) + ((float)DateTime.Now.Minute) / 60 + (((float)DateTime.Now.Second) / 60) / 60;

            if (tod >= 24)
                tod = 0;

            // Calculate the position of the sun based on the time of day.
            float x = 0;
            float y = 0;
            float z = 0;

            if (tod <= 12)
            {
                y = tod / 12;
                x = 12 - tod;
            }
            else
            {
                y = (24 - tod) / 12;
                x = 12 - tod;
            }

            x /= 10;
            SunPos = new Vector3(-x, y, z);

            cloudAnim += cloudAnimSpeed;
            if (cloudAnim > 1)
                cloudAnim = 0;

            base.Update(gameTime);
        }

        

        public override void Draw(GameTime gameTime)
        {
            Matrix World = Matrix.CreateScale(Scale) *
                            Matrix.CreateFromQuaternion(Rotation) *
                            Matrix.CreateTranslation(camera.Position);

            Game.GraphicsDevice.BlendState = BlendState.Opaque;

            Effect.Parameters["World"].SetValue(World);
            Effect.Parameters["View"].SetValue(camera.View);
            Effect.Parameters["Projection"].SetValue(camera.Projection);

            Effect.Parameters["surfaceTexture"].SetValue(environ);

            Effect.Parameters["NightColor"].SetValue(NightColor.ToVector4());
            Effect.Parameters["SkyColor"].SetValue(SkyColor.ToVector4());
            Effect.Parameters["MorningTint"].SetValue(MorningTint.ToVector4());
            Effect.Parameters["EveningTint"].SetValue(EveningTint.ToVector4());



            Effect.Parameters["EyePosition"].SetValue(camera.Position);

            Effect.Parameters["timeOfDay"].SetValue(tod);
            Effect.Parameters["cloudTimer"].SetValue(cloudAnim);
            Effect.Parameters["cloudIntensity"].SetValue(cloudIntensity);

            // Pass the cloud system you want to use.
            Effect.Parameters["cloud"].SetValue(cloudSystem[3]);

            for (int pass = 0; pass < Effect.CurrentTechnique.Passes.Count; pass++)
            {
                for (int msh = 0; msh < mesh.Meshes.Count; msh++)
                {
                    ModelMesh meshSky = mesh.Meshes[msh];
                    for (int prt = 0; prt < meshSky.MeshParts.Count; prt++)
                        meshSky.MeshParts[prt].Effect = Effect;
                    meshSky.Draw();
                }
            }
        }
    }
}
