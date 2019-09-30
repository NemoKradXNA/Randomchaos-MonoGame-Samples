using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RandomchaosMGFire
{
    public class OldSchoolFireBase  : DrawableGameComponent
    {
        protected Color[] palette = new Color[256];

        protected Texture2D paletteTexture;
        protected Texture2D fireMap;

        protected Random rand;

        protected Point Resolution = new Point(400, 240);
        protected float oxygen = .25f;
        protected int minFuel = 0;
        protected int maxFuel = 255;

        public bool ShowDebug { get; set; }

        public float Oxygen
        {
            get { return 1 - oxygen; }
            set
            {
                if (1 - value >= 0)
                    oxygen = 1 - value;
            }
        }
        public int MinFuel
        {
            get { return minFuel + 1; }
            set
            {
                if (value - 1 < maxFuel && value - 1 >= 0)
                    minFuel = value - 1;
            }
        }
        public int MaxFuel
        {
            get { return maxFuel; }
            set
            {
                if (value >= minFuel && value <= 255)
                    maxFuel = value;
            }
        }

        protected SpriteBatch spriteBatch
        {
            get { return ((Game1)Game).spriteBatch; }
        }

        public OldSchoolFireBase(Game game) : base(game)
        {
            BuildPalette();
            rand = new Random(DateTime.Now.Millisecond);
        }

        public OldSchoolFireBase(Game game, Point resolution) : base(game)
        {
            BuildPalette();
            rand = new Random(DateTime.Now.Millisecond);
            Resolution = resolution;
        }
        protected void BuildPalette()
        {
            for (int x = 0; x < palette.Length; x++)
                palette[x] = new Color(255, Math.Min(255, x * 2), x / 3, x * 5);
            //palette[x] = new Color(x / 3, Math.Min(255, x * 2), 255, x * 5);
            //palette[x] = new Color(x / 3, 255, Math.Min(255, x * 2), x * 5);
            //palette[x] = new Color(Math.Min(255, x * 2), 255, x / 3, x * 5);
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            fireMap = new Texture2D(GraphicsDevice, Resolution.X, Resolution.Y);

            paletteTexture = new Texture2D(GraphicsDevice, 256, 1);
            paletteTexture.SetData<Color>(palette);
        }

        public virtual void GenerateFireMap(int mapHeight, int mapWidth)
        {
            // Put code here to gen the fire map...
        }

        public override void Draw(GameTime gameTime)
        {
            if (!Enabled)
                return;

            base.Draw(gameTime);

            if (ShowDebug)
                RenderDebugTextures(paletteTexture);
        }

        public virtual void RenderDebugTextures(params Texture2D[] textures)
        {
            int s = 256;
            int t = 0;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            foreach (Texture2D texture in textures)
            {
                spriteBatch.Draw(texture, new Rectangle(GraphicsDevice.PresentationParameters.BackBufferWidth - (s * (t+1)), 0, s, s), Color.White);
                t++;
            }
            spriteBatch.End();
        }

        public virtual void SaveToFile(Texture2D texture, string name)
        {
            FileStream s = new FileStream(name, FileMode.Create);
            texture.SaveAsJpeg(s, fireMap.Width, fireMap.Height);
            s.Close();
        }
    }
}
