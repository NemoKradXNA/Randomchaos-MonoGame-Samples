using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RandomchaosRTSPLib;

namespace RandomchaosMGRTSPStreaming
{
    public class RTSPStreamer : DrawableGameComponent
    {
        protected SpriteBatch spriteBatch { get { return Game.Services.GetService<SpriteBatch>(); } }
        public string URL { get; set; }

        RTSPStream rtspStream;
        Texture2D stream1Texture;

        public Rectangle RenderRectangle;

        public RTSPStreamer(Game game, Rectangle rect) : base(game)
        {
            RenderRectangle = rect;
            rtspStream = new RTSPStream();
        }

        public void StartStream(string url, int threadSleep = 1, bool forceTCP = true, int tcpTimeout = 0, int maxFrameLoss = 0)
        {
            URL = url;
            rtspStream.StartStream(URL, threadSleep, forceTCP, tcpTimeout, maxFrameLoss);
        }

        public override void Update(GameTime gameTime)
        {
            if (rtspStream.IsThreadRunning && rtspStream.StreamReady)
                TextureFormatter.GetTexture2DFromBitmap(Game.GraphicsDevice, rtspStream.GetStreamTexture(), ref stream1Texture);
        }

        public override void Draw(GameTime gameTime)
        {
            if (rtspStream.IsThreadRunning && rtspStream.StreamReady)
            {
                if(stream1Texture != null)
                    spriteBatch.Draw(stream1Texture, RenderRectangle, Color.White);

                if (string.IsNullOrEmpty(rtspStream.ErrorMessage))
                    DrawText(URL);
                else
                    DrawText(rtspStream.ErrorMessage);
            }
            else
            {
                if (rtspStream.IsThreadRunning)
                    DrawText("Connecting...");
            }
        }

        public void DrawText(string text)
        {
            spriteBatch.DrawString(Game.Content.Load<SpriteFont>("Fonts/hudFont"), text, new Vector2(RenderRectangle.X, RenderRectangle.Y + 12), Color.Gold);
        }

        public void StopStream()
        {
            rtspStream.StopStream();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            rtspStream.StopStream();
        }
    }
}
