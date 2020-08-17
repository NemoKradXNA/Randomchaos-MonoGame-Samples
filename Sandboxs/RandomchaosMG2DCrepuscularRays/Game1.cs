using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using RandomchaosMG2DCrepuscularRays.PostProcessing.PostProcessingEffects;
using RandomchaosMGBase.BaseClasses.PostProcessing;

namespace RandomchaosMG2DCrepuscularRays
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        RenderTarget2D scene;
        PostProcessingManager ppm;
        CrepuscularRays2D rays;

        Vector2 rayStartingPos = Vector2.One * .5f;

        List<string> backgroundAssets = new List<string>();
        List<float> bgSpeeds = new List<float>();

        List<Vector2> bgPos = new List<Vector2>();
        List<Vector2> bgPos2 = new List<Vector2>();
        List<Vector2> bgPos2Base = new List<Vector2>();


        List<string> clouds = new List<string>();
        List<float> cloudSpeed = new List<float>();
        List<Vector2> cloudsPos = new List<Vector2>();

        Random rnd;
        int bgCnt;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.GraphicsProfile = GraphicsProfile.HiDef;

            //graphics.IsFullScreen = true;

            graphics.PreferredBackBufferHeight = 900;
            graphics.PreferredBackBufferWidth = 1440;

            //graphics.PreferredBackBufferHeight = 600;
            //graphics.PreferredBackBufferWidth = 800;

            rnd = new Random(DateTime.Now.TimeOfDay.Milliseconds);

            ppm = new PostProcessingManager(this);

            rays = new CrepuscularRays2D(this, Vector2.One * .5f, "Textures/flare", 1, .97f, .97f, .5f, .12f, 0);

            ppm.AddEffect(rays);
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

            spriteBatch = new SpriteBatch(GraphicsDevice);
            Services.AddService(spriteBatch.GetType(), spriteBatch);

            rays.lightSource = rayStartingPos;

            scene = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.None);

            AddBackground("Textures/Backgrounds/BGForeGround_1_1", 2f);
            AddBackground("Textures/Backgrounds/BGForeGround_1_2", 3f);
            AddBackground("Textures/Backgrounds/BGForeGround_1_3", 4f);

            for (int c = 0; c < 3; c++)
                AddCloud("Textures/Backgrounds/Cloud1", (float)(rnd.NextDouble() * 5) + .1f);
            for (int c = 0; c < 3; c++)
                AddCloud("Textures/Backgrounds/Cloud2", (float)(rnd.NextDouble() * 5) + .1f);

        }

        public virtual void AddCloud(string asset, float speed)
        {
            clouds.Add(asset);
            cloudSpeed.Add(speed);

            
            Vector2 p = new Vector2();

            p.X = rnd.Next(0, GraphicsDevice.Viewport.Width);
            p.Y = rnd.Next(0, GraphicsDevice.Viewport.Height / 4);

            cloudsPos.Add(p);

            
        }

        public virtual void AddBackground(string bgAsset, float speed)
        {
            backgroundAssets.Add(bgAsset);
            bgSpeeds.Add(speed);
            bgPos.Add(Vector2.Zero);
            bgCnt++;

            bgPos2.Add(new Vector2(GraphicsDevice.Viewport.Width, 0));
            bgPos2Base.Add(new Vector2(GraphicsDevice.Viewport.Width, 0));
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

            if (Keyboard.GetState().IsKeyDown(Keys.F1))
                rays.lightTexture = Content.Load<Texture2D>("Textures/flare");
            if (Keyboard.GetState().IsKeyDown(Keys.F2))
                rays.lightTexture = Content.Load<Texture2D>("Textures/flare2");
            if (Keyboard.GetState().IsKeyDown(Keys.F3))
                rays.lightTexture = Content.Load<Texture2D>("Textures/flare3");
            if (Keyboard.GetState().IsKeyDown(Keys.F4))
                rays.lightTexture = Content.Load<Texture2D>("Textures/flare4");

            if (Keyboard.GetState().IsKeyDown(Keys.W))
                rays.lightSource = new Vector2(rays.lightSource.X, rays.lightSource.Y - .01f);
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                rays.lightSource = new Vector2(rays.lightSource.X, rays.lightSource.Y + .01f);
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                rays.lightSource = new Vector2(rays.lightSource.X - .01f, rays.lightSource.Y);
            if (Keyboard.GetState().IsKeyDown(Keys.D))
                rays.lightSource = new Vector2(rays.lightSource.X + .01f, rays.lightSource.Y);

            if (Keyboard.GetState().IsKeyDown(Keys.Q))
                rays.Exposure += .01f;
            if (Keyboard.GetState().IsKeyDown(Keys.Z))
                rays.Exposure -= .01f;

            if (Keyboard.GetState().IsKeyDown(Keys.E))
                rays.LightSourceSize += .01f;
            if (Keyboard.GetState().IsKeyDown(Keys.C))
                rays.LightSourceSize -= .01f;

            if (Keyboard.GetState().IsKeyDown(Keys.R))
                rays.Density += .01f;
            if (Keyboard.GetState().IsKeyDown(Keys.V))
                rays.Density -= .01f;

            if (Keyboard.GetState().IsKeyDown(Keys.T))
                rays.Decay += .01f;
            if (Keyboard.GetState().IsKeyDown(Keys.B))
                rays.Decay -= .01f;

            if (Keyboard.GetState().IsKeyDown(Keys.Y))
                rays.Weight += .01f;
            if (Keyboard.GetState().IsKeyDown(Keys.N))
                rays.Weight -= .01f;

            if (Keyboard.GetState().IsKeyDown(Keys.U))
                rays.BrightThreshold = MathHelper.Min(.99f, rays.BrightThreshold + .01f);
            if (Keyboard.GetState().IsKeyDown(Keys.M))
                rays.BrightThreshold = MathHelper.Max(0, rays.BrightThreshold - .01f);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Give me a shout if you need any help implementing it :D

            // Draw the stuff that is in front of the rays source
            GraphicsDevice.SetRenderTarget(scene);
            GraphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            // Draw sky texture
            spriteBatch.Draw(Content.Load<Texture2D>("Textures/Backgrounds/Sky1"), new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);

            // Draw clouds
            for (int c = 0; c < clouds.Count; c++)
                spriteBatch.Draw(Content.Load<Texture2D>(clouds[c]), cloudsPos[c], Color.White);

            // Draw BG
            for (int bg = 0; bg < bgCnt; bg++)
            {
                spriteBatch.Draw(Content.Load<Texture2D>(backgroundAssets[bg]), new Rectangle((int)bgPos[bg].X, (int)bgPos[bg].Y, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), new Rectangle(0, 0, Content.Load<Texture2D>(backgroundAssets[bg]).Width, Content.Load<Texture2D>(backgroundAssets[bg]).Height), Color.White);
                spriteBatch.Draw(Content.Load<Texture2D>(backgroundAssets[bg]), new Rectangle((int)bgPos2[bg].X, (int)bgPos[bg].Y, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), new Rectangle(0, 0, Content.Load<Texture2D>(backgroundAssets[bg]).Width, Content.Load<Texture2D>(backgroundAssets[bg]).Height), Color.White);
            }
            // Draw mouse icon mask.
            spriteBatch.Draw(Content.Load<Texture2D>("Textures/mousemask"), new Rectangle(Mouse.GetState().X - 64, Mouse.GetState().Y - 64, 128, 128), Color.White);
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);

            GraphicsDevice.Clear(Color.Gold);
            // Apply the post processing manager (just the rays in this one)
            ppm.Draw(gameTime, scene, null);

            // Move the clouds
            for (int c = 0; c < clouds.Count; c++)
            {
                cloudsPos[c] += new Vector2(cloudSpeed[c]*.25f,0);

                if (cloudsPos[c].X > GraphicsDevice.Viewport.Width)
                {
                    cloudsPos[c] = new Vector2(-300, rnd.Next(0, GraphicsDevice.Viewport.Height / 4));
                    cloudSpeed[c] = ((float)rnd.NextDouble() * 5) + .1f;
        }
            }

            // Move the backgrounds..
            for (int b = 0; b < bgCnt; b++)
            {
                bgPos[b] -= new Vector2(bgSpeeds[b], 0);
                bgPos2[b] -= new Vector2(bgSpeeds[b], 0);

                if (bgPos[b].X < -bgPos2Base[b].X)
                    bgPos[b] = new Vector2(bgPos2[b].X + bgPos2Base[b].X, 0);

                if (bgPos2[b].X < -bgPos2Base[b].X)
                    bgPos2[b] = new Vector2(bgPos[b].X + bgPos2Base[b].X, 0);
            }

            // Render command keys.
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            spriteBatch.DrawString(Content.Load<SpriteFont>("Fonts/font"), "F1 - F4 change light source texture", new Vector2(8, Content.Load<SpriteFont>("Fonts/font").LineSpacing * 1), Color.Gold);
            spriteBatch.DrawString(Content.Load<SpriteFont>("Fonts/font"), string.Format("WASD move light source [{0}]", rays.lightSource), new Vector2(8, Content.Load<SpriteFont>("Fonts/font").LineSpacing * 2), Color.Gold);
            spriteBatch.DrawString(Content.Load<SpriteFont>("Fonts/font"), string.Format("Q/Z Exposure up/down [{0}]", rays.Exposure), new Vector2(8, Content.Load<SpriteFont>("Fonts/font").LineSpacing * 3), Color.Gold);
            spriteBatch.DrawString(Content.Load<SpriteFont>("Fonts/font"), string.Format("E/C Size up/down [{0}]", rays.LightSourceSize), new Vector2(8, Content.Load<SpriteFont>("Fonts/font").LineSpacing * 4), Color.Gold);
            spriteBatch.DrawString(Content.Load<SpriteFont>("Fonts/font"), string.Format("R/V Density up/down [{0}]", rays.Density), new Vector2(8, Content.Load<SpriteFont>("Fonts/font").LineSpacing * 5), Color.Gold);
            spriteBatch.DrawString(Content.Load<SpriteFont>("Fonts/font"), string.Format("T/B Decay up/down [{0}]", rays.Decay), new Vector2(8, Content.Load<SpriteFont>("Fonts/font").LineSpacing * 6), Color.Gold);
            spriteBatch.DrawString(Content.Load<SpriteFont>("Fonts/font"), string.Format("Y/N Weight up/down [{0}]", rays.Weight), new Vector2(8, Content.Load<SpriteFont>("Fonts/font").LineSpacing * 7), Color.Gold);
            spriteBatch.DrawString(Content.Load<SpriteFont>("Fonts/font"), string.Format("U/M Bright Pass Threshold up/down [{0}]", rays.BrightThreshold), new Vector2(8, Content.Load<SpriteFont>("Fonts/font").LineSpacing * 8), Color.Gold);
            spriteBatch.End();
        }
    }
}
