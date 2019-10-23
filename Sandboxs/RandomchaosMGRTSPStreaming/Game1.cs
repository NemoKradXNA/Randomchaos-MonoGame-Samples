using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using RandomchaosRTSPLib;

namespace RandomchaosMGRTSPStreaming
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public string stream1URL = "rtsp://b1.dnsdojo.com:1935/live/sys6.stream";
        public string stream2URL = "rtsp://b1.dnsdojo.com:1935/live/sys1.stream";
        public string stream3URL = "rtsp://b1.dnsdojo.com:1935/live/sys2.stream";
        public string stream4URL = "rtsp://b1.dnsdojo.com:1935/live/sys7.stream";

        RTSPStreamer streamer1;
        RTSPStreamer streamer2;
        RTSPStreamer streamer3;
        RTSPStreamer streamer4;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.GraphicsProfile = GraphicsProfile.HiDef;

            int w = 1920;
            int h = 1080;

            graphics.PreferredBackBufferWidth = w;
            graphics.PreferredBackBufferHeight = h;

            IsMouseVisible = true;

            streamer1 = new RTSPStreamer(this, new Rectangle(0, 0, w/2, h/2));
            streamer1.StartStream(stream1URL);
            Components.Add(streamer1);

            streamer2 = new RTSPStreamer(this, new Rectangle(w/2, 0, w / 2, h / 2));
            streamer2.StartStream(stream2URL);
            Components.Add(streamer2);

            streamer3 = new RTSPStreamer(this, new Rectangle(0, h / 2, w / 2, h / 2));
            streamer3.StartStream(stream3URL);
            Components.Add(streamer3);

            streamer4 = new RTSPStreamer(this, new Rectangle(w / 2, h / 2, w / 2, h / 2));
            streamer4.StartStream(stream4URL);
            Components.Add(streamer4);
        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Services.AddService(typeof(SpriteBatch), spriteBatch);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Immediate);
            base.Draw(gameTime);
            spriteBatch.End();
        }

        protected void RenderStream(Texture2D texture, string text, Rectangle rect)
        {
            spriteBatch.Draw(texture, rect, Color.White);
            spriteBatch.DrawString(Content.Load<SpriteFont>("Fonts/hudFont"), text, new Vector2(rect.X, rect.Y), Color.Gold);
        }
    }
}
