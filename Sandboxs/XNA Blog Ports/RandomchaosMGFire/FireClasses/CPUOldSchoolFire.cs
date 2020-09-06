using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RandomchaosMGFire
{
    public class CPUOldSchoolFire : OldSchoolFireBase
    {
        protected Texture2D fireTexture;
        protected Color[] fireColor;
        protected int[,] fmap;
        

        public CPUOldSchoolFire(Game game) : base(game) { }
        public CPUOldSchoolFire(Game game, Point resolution) : base(game, resolution) { }
        
        protected override void LoadContent()
        {
            base.LoadContent();

            fireTexture = new Texture2D(GraphicsDevice, Resolution.X, Resolution.Y);
            fireColor = new Color[fireTexture.Width * fireTexture.Height];
            
            fmap = new int[fireTexture.Width, fireTexture.Height];
        }

        public override void GenerateFireMap(int mapWidth, int mapHeight)
        {
            int w = mapWidth;
            int h = mapHeight;

            int[] fm = new int[w * h];

            for (int x = 0; x < w; x++)
            {
                fmap[x, (h - 1)] = rand.Next(minFuel, maxFuel);
            }

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    //fmap[x, y] = ((fmap[(x - 1 + w) % w, (y + 1) % h]
                    //+ fmap[(x) % w, (y + 1) % h]
                    //+ fmap[(x + 1) % w, (y + 1) % h]
                    //+ fmap[(x) % w, (y + 2) % h])
                    //* 32) / 129;

                    //fmap[x, y] = ((fmap[(x - 1 + w) % w, (y + 1) % h]
                    //+ fmap[(x) % w, (y + 2) % h]
                    //+ fmap[(x + 1) % w, (y + 1) % h]
                    //+ fmap[(x) % w, (y + 3) % h])
                    //* 64) / 257;

                    fmap[x, y] = (int)(((fmap[(x - 1 + w) % w, (y + 1) % h]
                    + fmap[(x) % w, (y + 2) % h]
                    + fmap[(x + 1) % w, (y + 1) % h]
                    + fmap[(x) % w, (y + 2) % h])
                    * 1) / (4 + oxygen));

                    //// Water Ripple
                    //fmap[x, y] = (int)(((fmap[(x - 1 + w) % w, (y + 1) % h]
                    //+ fmap[(x) % w, (y + 1) % h]
                    //+ fmap[(x + 1) % w, (y + 1) % h]
                    //+ 0)
                    //* 1) / (3 + oxygen));
                    fm[x + (y * w)] = fmap[x, y];
                }
            }

            fireMap.SetData(fm);
        }

        public override void Update(GameTime gameTime)
        {
            if (!Enabled)
                return;

            base.Update(gameTime);

            int w = fmap.GetLength(0);
            int h = fmap.GetLength(1);

            GenerateFireMap(w, h);

            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    fireColor[x + (y * w)] = palette[fmap[x, y]];

            fireTexture.SetData(fireColor);
        }

        public override void Draw(GameTime gameTime)
        {
            if (!Enabled)
                return;

            if (fireTexture != null)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
                spriteBatch.Draw(fireTexture, new Rectangle(0, 0, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight), Color.White);
                spriteBatch.End();

                // Debug
                if (ShowDebug)
                    RenderDebugTextures(paletteTexture, fireMap);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            fireTexture.Dispose();
        }
    }
}
