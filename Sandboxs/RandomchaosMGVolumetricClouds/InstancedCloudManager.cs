using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RandomchaosMGBase.BaseClasses;

namespace RandomchaosMGVolumetricClouds
{
    public class InstancedCloudManager : DrawableGameComponent
    {
        Base3DParticleInstancer clouds;

        List<Base3DParticleInstance> whisps = new List<Base3DParticleInstance>();

        Random rnd = new Random(DateTime.Now.Millisecond);

        public Base3DCamera camera
        {
            get { return ((Base3DCamera)Game.Services.GetService(typeof(Base3DCamera))); }
        }

        public InstancedCloudManager(Game game, string shaderAsset, string cloudAsset)
            : base(game)
        {
            clouds = new Base3DParticleInstancer(game, shaderAsset);
            //////////////////////////////////////////////////////////////////
            // REMEMBER THE CloudSpriteSheetOrg texture is property of MS   //
            // Do not comercialy use this texture!                          //
            //////////////////////////////////////////////////////////////////
            clouds.TextureAsset = cloudAsset;
            clouds.thisBlendState = BlendState.NonPremultiplied;
        }

        public void AddCloud(int whispCount, Vector3 position, float size, Vector3 min, Vector3 max, float colorMod, params int[] whispRange)
        {
            int si = 0;
            float scaleMod = Vector3.Distance(-min, max) / 4.5f;

            for (int w = 0; w < whispCount; w++)
            {
                float x = MathHelper.Lerp(-min.X, max.X, (float)rnd.NextDouble());
                float y = MathHelper.Lerp(-min.Y, max.Y, (float)rnd.NextDouble());
                float z = MathHelper.Lerp(-min.Z, max.Z, (float)rnd.NextDouble());

                if (si >= whispRange.Length)
                    si = 0;

                whisps.Add(new Base3DParticleInstance(Game, position + new Vector3(x, y, z), Vector3.One * size, new Vector3(whispRange[si++] / 100f, 1, (rnd.Next(7, 10) / 10f) * colorMod), clouds));
            }
        }

        public void AddCloud(int whispCount, Vector3 position, float size, float radius, float colorMod, params int[] whispRange)
        {
            int si = 0;
            float scaleMod = Vector3.Distance(position, position * radius) / 4.5f;

            for (int w = 0; w < whispCount; w++)
            {
                float x = MathHelper.Lerp(-radius, radius, (float)rnd.NextDouble());
                float y = MathHelper.Lerp(-radius, radius, (float)rnd.NextDouble());
                float z = MathHelper.Lerp(-radius, radius, (float)rnd.NextDouble());

                if (si >= whispRange.Length)
                    si = 0;

                whisps.Add(new Base3DParticleInstance(Game, position + new Vector3(x, y, z), Vector3.One * size, new Vector3(whispRange[si++] / 100f, 1, (rnd.Next(7, 10) / 10f) * colorMod), clouds));
            }
        }

        public class distData : IComparer<distData>
        {
            public float dist;
            public Base3DParticleInstance idx;

            public distData()
            { }

            public distData(Base3DParticleInstance idx, float dist)
            {
                this.idx = idx;
                this.dist = dist;
            }

            public int Compare(distData x, distData y)
            {
                return (int)(y.dist - x.dist);
            }
            public float Distance(Vector3 v1, Vector3 v2)
            {
                Vector3 unit = v2 - v1;
                float distance = unit.Length();

                return distance *= distance;
            }

        }

        private void SortClouds()
        {
            List<distData> bbDists = new List<distData>();

            for (int p = 0; p < whisps.Count; p++)
            {
                float dist = (new distData()).Distance(clouds.instanceTransformMatrices[whisps[p]].Translation, camera.Position);
                bbDists.Add(new distData(whisps[p], dist));

            }

            bbDists.Sort(new distData());

            // Reorder the matrix list.
            clouds.instanceTransformMatrices.Clear();
            for (int p = 0; p < bbDists.Count; p++)
                clouds.instanceTransformMatrices.Add(bbDists[p].idx, bbDists[p].idx.World);

            clouds.CalcVertexBuffer();
        }

        /// <summary>
        /// Method to translate object
        /// </summary>
        /// <param name="distance"></param>
        public void TranslateOO(Vector3 distance)
        {
            clouds.TranslateOO(distance);
        }
        public void TranslateAA(Vector3 distance)
        {
            clouds.TranslateAA(distance);
        }

        /// <summary>
        /// Method to rotate object
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        public void Rotate(Vector3 axis, float angle)
        {
            clouds.Rotate(axis, angle);
        }

        public override void Initialize()
        {
            clouds.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            SortClouds();
            clouds.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            clouds.Draw(gameTime);
        }
    }
}
