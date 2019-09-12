using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using RandomchaosMGBase.BaseClasses;
using RandomchaosMGBase.BaseClasses.PostProcessing;
using RandomchaosMGBase.InputManagers;

namespace RandomchaosMGPostProcessingSandBox
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        RenderTarget2D depthBuffer;
        RenderTarget2D backBuffer;

        KeyboardStateManager kbm;
        Base3DCamera camera;

        #region Scene Objects
        BaseSkyBox skyBox;

        Base3DObject cube;
        Base3DObject[] bigShip;
        Base3DObject landShark;
        Base3DObject earth;
        #endregion

        #region Post Processing Elements
        PostProcessingManager ppManager;

        BloomEffect bloom;
        CrepuscularRays GodRays;
        DepthOfFieldEffect dof;
        FogEffect fog;
        HeatHazeEffect haze;
        RadialBlurEffect radialBlur;
        RippleEffect ripple;
        SunEffect sun;
        SepiaEffect sepia;
        GreyScaleEffect greyScale;
        InvertColorEffect invert;
        #endregion

        Vector3 LightPosition = new Vector3(50, 50, 1000);

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.GraphicsProfile = GraphicsProfile.HiDef;

            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            
            IsMouseVisible = true;

            kbm = new KeyboardStateManager(this);

            camera = new Base3DCamera(this, .5f, 20000);
            camera.Position = new Vector3(0, 0, 0);
            Components.Add(camera);
            Services.AddService(typeof(Base3DCamera), camera);

            skyBox = new BaseSkyBox(this, "Textures/SkyBox/NebulaBlue");
            Components.Add(skyBox);

            cube = new Base3DObject(this, "Models/cube");
            cube.Position = new Vector3(0, 0, -10);
            cube.ColorAsset = "Textures/h2mcpCube";
            cube.BumpAsset = "Textures/h2mcpCubeNormal";
            cube.LightPosition = LightPosition;
            Components.Add(cube);

            bigShip = new Base3DObject[10];
            for (int s = 0; s < 10; s++)
            {
                bigShip[s] = new Base3DObject(this, "Models/bigship1");
                bigShip[s].Position = new Vector3(20 + (s * 12), 0, -10 - (s * 12));
                bigShip[s].ColorAsset = "Textures/WindmillTopColor";
                bigShip[s].BumpAsset = "Textures/WindmillTopNormal";
                bigShip[s].LightPosition = LightPosition;
                Components.Add(bigShip[s]);
            }

            landShark = new Base3DObject(this, "Models/landShark");
            landShark.Position = new Vector3(-20, 0, -10);

            landShark.ColorAsset = "Textures/Speeder_diff";
            landShark.BumpAsset = "Textures/Speeder_bump";
            landShark.LightPosition = LightPosition;
            Components.Add(landShark);

            earth = new Base3DObject(this, "Models/sphere");
            earth.Position = new Vector3(0, 0, -200);
            earth.ColorAsset = "Textures/Earth_Diffuse";
            earth.BumpAsset = "Textures/Earth_NormalMap";
            earth.LightPosition = LightPosition;
            Components.Add(earth);

            ppManager = new PostProcessingManager(this);

            fog = new FogEffect(this, 50, 100, Color.DarkSlateGray);
            fog.Enabled = false;
            ppManager.AddEffect(fog);

            GodRays = new CrepuscularRays(this, LightPosition, "Textures/flare", 1500, .99f, .99f, .5f, .12f, .25f);
            GodRays.Enabled = false;
            ppManager.AddEffect(GodRays);

            sun = new SunEffect(this, LightPosition);
            sun.Enabled = false;
            ppManager.AddEffect(sun);

            bloom = new BloomEffect(this, 1.25f, 1f, 1f, 1f, .25f, 4f);
            bloom.Enabled = false;
            ppManager.AddEffect(bloom);

            dof = new DepthOfFieldEffect(this, 5, 30);
            dof.Enabled = false;
            ppManager.AddEffect(dof);            

            haze = new HeatHazeEffect(this, "Textures/bumpmap", false);
            haze.Enabled = false;
            ppManager.AddEffect(haze);

            radialBlur = new RadialBlurEffect(this, 0.009f);
            radialBlur.Enabled = false;
            ppManager.AddEffect(radialBlur);

            ripple = new RippleEffect(this);
            ripple.Enabled = false;
            ppManager.AddEffect(ripple);

            sepia = new SepiaEffect(this);
            sepia.Enabled = false;
            ppManager.AddEffect(sepia);

            greyScale = new GreyScaleEffect(this);
            greyScale.Enabled = false;
            ppManager.AddEffect(greyScale);

            invert = new InvertColorEffect(this);
            invert.Enabled = false;
            ppManager.AddEffect(invert);
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

            backBuffer = new RenderTarget2D(GraphicsDevice,
                                            GraphicsDevice.Viewport.Width,
                                            GraphicsDevice.Viewport.Height,
                                            false,
                                            SurfaceFormat.Color,
                                            DepthFormat.Depth24);

            depthBuffer = new RenderTarget2D(GraphicsDevice,
                                            GraphicsDevice.Viewport.Width,
                                            GraphicsDevice.Viewport.Height,
                                            false, SurfaceFormat.Single,
                                            DepthFormat.Depth24);
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
            kbm.PreUpdate(gameTime);
            base.Update(gameTime);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || kbm.KeyDown(Keys.Escape))
                Exit();

            earth.Rotate(Vector3.Up, .0001f);

            // Camera controls..
            float speedTran = .1f;
            float speedRot = .01f;

            if (kbm.KeyDown(Keys.W) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y > 0)
                camera.Translate(Vector3.Forward * speedTran);
            if (kbm.KeyDown(Keys.S) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y < 0)
                camera.Translate(Vector3.Backward * speedTran);
            if (kbm.KeyDown(Keys.A) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X < 0)
                camera.Translate(Vector3.Left * speedTran);
            if (kbm.KeyDown(Keys.D) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X > 0)
                camera.Translate(Vector3.Right * speedTran);

            if (kbm.KeyDown(Keys.Left) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X < 0)
                camera.Rotate(Vector3.Up, speedRot);
            if (kbm.KeyDown(Keys.Right) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X > 0)
                camera.Rotate(Vector3.Up, -speedRot);
            if (kbm.KeyDown(Keys.Up) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y > 0)
                camera.Rotate(Vector3.Right, speedRot);
            if (kbm.KeyDown(Keys.Down) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y < 0)
                camera.Rotate(Vector3.Right, -speedRot);

            if (kbm.KeyPress(Keys.F1))
                bloom.Enabled = !bloom.Enabled;
            if (kbm.KeyPress(Keys.F2))
                GodRays.Enabled = !GodRays.Enabled;
            if (kbm.KeyPress(Keys.F3))
                dof.Enabled = !dof.Enabled;
            if (kbm.KeyPress(Keys.F4))
                fog.Enabled = !fog.Enabled;
            if (kbm.KeyPress(Keys.F5))
                haze.Enabled = !haze.Enabled;
            if (kbm.KeyPress(Keys.F6))
                radialBlur.Enabled = !radialBlur.Enabled;
            if (kbm.KeyPress(Keys.F7))
                ripple.Enabled = !ripple.Enabled;
            if (kbm.KeyPress(Keys.F8))
                sun.Enabled = !sun.Enabled;
            if (kbm.KeyPress(Keys.F9))
                sepia.Enabled = !sepia.Enabled;
            if (kbm.KeyPress(Keys.F10))
                greyScale.Enabled = !greyScale.Enabled;
            if (kbm.KeyPress(Keys.F11))
                invert.Enabled = !invert.Enabled;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Set up RT's
            GraphicsDevice.SetRenderTargets(backBuffer, depthBuffer);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);

            //Resolve targets.
            GraphicsDevice.SetRenderTarget(null);

            ppManager.Draw(gameTime, backBuffer, depthBuffer);

            spriteBatch.Begin();
            spriteBatch.DrawString(Content.Load<SpriteFont>("Fonts/hudFont"), "Post Processing", new Vector2(8, 8), Color.Gold);
            spriteBatch.DrawString(Content.Load<SpriteFont>("Fonts/hudFont"), $"[F1 ] - Bloom On: {bloom.Enabled}", new Vector2(8, 20), Color.Gold);
            spriteBatch.DrawString(Content.Load<SpriteFont>("Fonts/hudFont"), $"[F2 ] - God Rays On: {GodRays.Enabled}", new Vector2(8, 32), Color.Gold);
            spriteBatch.DrawString(Content.Load<SpriteFont>("Fonts/hudFont"), $"[F3 ] - Depth of Field On: {dof.Enabled}", new Vector2(8, 44), Color.Gold);
            spriteBatch.DrawString(Content.Load<SpriteFont>("Fonts/hudFont"), $"[F4 ] - Fog On: {fog.Enabled}", new Vector2(8, 56), Color.Gold);
            spriteBatch.DrawString(Content.Load<SpriteFont>("Fonts/hudFont"), $"[F5 ] - Heat Haze On: {haze.Enabled}", new Vector2(8, 68), Color.Gold);
            spriteBatch.DrawString(Content.Load<SpriteFont>("Fonts/hudFont"), $"[F6 ] - Radial Blur On: {radialBlur.Enabled}", new Vector2(8, 80), Color.Gold);
            spriteBatch.DrawString(Content.Load<SpriteFont>("Fonts/hudFont"), $"[F7 ] - Ripple On: {ripple.Enabled}", new Vector2(8, 92), Color.Gold);
            spriteBatch.DrawString(Content.Load<SpriteFont>("Fonts/hudFont"), $"[F8 ] - Sun On: {sun.Enabled}", new Vector2(8, 104), Color.Gold);
            spriteBatch.DrawString(Content.Load<SpriteFont>("Fonts/hudFont"), $"[F9 ] - Sepia On: {sepia.Enabled}", new Vector2(8, 118), Color.Gold);
            spriteBatch.DrawString(Content.Load<SpriteFont>("Fonts/hudFont"), $"[F10] - Grey Scale On: {greyScale.Enabled}", new Vector2(8, 130), Color.Gold);
            spriteBatch.DrawString(Content.Load<SpriteFont>("Fonts/hudFont"), $"[F11] - Invert Color On: {invert.Enabled}", new Vector2(8, 142), Color.Gold);
            spriteBatch.End();
        }
        public void SaveJpg(Texture2D texture, string name)
        {
            FileStream s = new FileStream(name, FileMode.Create);
            texture.SaveAsJpeg(s, texture.Width, texture.Height);
            s.Close();
        }
    }
}
