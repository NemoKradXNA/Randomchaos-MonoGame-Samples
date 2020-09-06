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
    public class GPUOldSchoolFire : OldSchoolFireBase
    {

        protected RenderTarget2D rt;
        
        protected Color[] rtc;

        protected Effect effect;

        public GPUOldSchoolFire(Game game) : base(game) { }
        public GPUOldSchoolFire(Game game, Point resolution) : base(game, resolution) { }

        protected override void LoadContent()
        {
            base.LoadContent();

           
            int[] i = new int[Resolution.X * Resolution.Y];
            fireMap.SetData(i);

            effect = Game.Content.Load<Effect>("Shaders/OldSchoolFire");

            rt = new RenderTarget2D(GraphicsDevice, Resolution.X, Resolution.Y);
            rtc = new Color[Resolution.X * Resolution.Y];
        }

        public override void Update(GameTime gameTime)
        {
            if (!Enabled)
                return;

            base.Update(gameTime);

        }

        public override void GenerateFireMap(int mapWidth, int mapHeight)
        {
            float[] d = new float[mapWidth * mapHeight];

            fireMap.GetData<float>(d);

            for (int x = 0; x < fireMap.Width; x++)
            {
                for (int y = fireMap.Height - 1; y < fireMap.Height; y++)
                    d[x + (y * fireMap.Width)] = MathHelper.Clamp((float)rand.NextDouble(), minFuel / 255f, maxFuel / 255f);
            }

            fireMap.SetData(d);
        }

        public override void Draw(GameTime gameTime)
        {
            if (!Enabled)
                return;

            GenerateFireMap(fireMap.Width, fireMap.Height);

            effect.Parameters["fireMap"].SetValue(fireMap);

            effect.Parameters["oxygen"].SetValue(oxygen);
            effect.Parameters["paletteMap"].SetValue(paletteTexture);
            effect.Parameters["width"].SetValue((float)Resolution.X);
            effect.Parameters["height"].SetValue((float)Resolution.Y);

            // Capture fireMap data.
            GraphicsDevice.SetRenderTarget(rt);
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            effect.CurrentTechnique = effect.Techniques["UpdateFire"];
            effect.CurrentTechnique.Passes[0].Apply();

            spriteBatch.Draw(fireMap, new Rectangle(0, 0, Resolution.X, Resolution.Y), Color.White);
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);

            // Get the data from the rt and pack it back into the map.
            rt.GetData(rtc);
            //GraphicsDevice.Textures[1] = null;
            fireMap.SetData(rtc);

            // Re draw with color passing in the firemap data
            GraphicsDevice.Clear(Color.Black);
            //spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);

            effect.CurrentTechnique = effect.Techniques["ColorFire"];
            effect.CurrentTechnique.Passes[0].Apply();

            spriteBatch.Draw(fireMap, new Rectangle(0, 0, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight), Color.White);

            spriteBatch.End();

            // Debug
            if(ShowDebug)
                RenderDebugTextures(paletteTexture, fireMap);
        }
       
    }
}
