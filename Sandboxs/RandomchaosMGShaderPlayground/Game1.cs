using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using RandomchaosMGBase;
using RandomchaosMGBase.BaseClasses;
using RandomchaosMGBase.InputManagers;


namespace RandomchaosMGShaderPlayground
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        KeyboardStateManager kbm;
        MouseStateManager msm;
        Base3DCamera camera;

        #region Scene Objects
        BaseSkyBox skyBox;

        Base3DObjectCrossHatch bunny;
        HatchedCube cube;
        Base3DObjectCrossHatch earth;
        Base3DObjectCrossHatch cube2;

        HatchedPlane ground;

        Vector3 LightPosition = new Vector3(100, 500, 1000);
        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.GraphicsProfile = GraphicsProfile.HiDef;

            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;

            IsMouseVisible = true;

            msm = new MouseStateManager(this);
            kbm = new KeyboardStateManager(this);

            camera = new Base3DCamera(this, .5f, 20000);
            camera.Position = new Vector3(0, 0, 0);
            camera.ClearColor = Color.Ivory;
            Components.Add(camera);
            Services.AddService(typeof(Base3DCamera), camera);

            skyBox = new BaseSkyBox(this, "Textures/SkyBox/HMcubemap");
            //Components.Add(skyBox);

            ground = new HatchedPlane(this);
            ground.Position = new Vector3(0, -1.5f, -10);
            ground.UVMultiplier = Vector2.One * .75f;
            Components.Add(ground);

            cube = new HatchedCube(this);
            cube.Position = new Vector3(-2, -1, -10);
            cube.UVMultiplier = Vector2.One * .1f;
            cube.LightPosition = LightPosition;
            Components.Add(cube);

            cube2 = new Base3DObjectCrossHatch(this, "Models/Cube");
            cube2.Position = new Vector3(-2, 1, -10);
            cube2.Rotate(Vector3.Left + Vector3.Up, 45);
            cube2.ColorAsset = "Textures/h2mcpCube";
            cube2.BumpAsset = "Textures/h2mcpCubeNormal";
            cube2.LightPosition = LightPosition;
            Components.Add(cube2);

            bunny = new Base3DObjectCrossHatch(this, "Models/bunny");
            bunny.Position = new Vector3(0, -1.5f, -10);
            bunny.LightPosition = LightPosition;
            Components.Add(bunny);

            earth = new Base3DObjectCrossHatch(this, "Models/sphere");
            earth.Position = new Vector3(2, -1, -10);
            earth.ColorAsset = "Textures/Earth_Diffuse";
            earth.BumpAsset = "Textures/Earth_NormalMap";
            earth.LightPosition = LightPosition;
            Components.Add(earth);
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

            // TODO: use this.Content to load your game content here
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
            msm.PreUpdate(gameTime);
            kbm.PreUpdate(gameTime);
            base.Update(gameTime);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || kbm.KeyDown(Keys.Escape))
                Exit();

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


            float cdv = .1f;
            if (kbm.KeyDown(Keys.R))
            {
                cube.HatchDencity += cdv;
                bunny.HatchDencity += cdv;
                earth.HatchDencity += cdv;
            }

            if (kbm.KeyDown(Keys.F))
            {
                cube.HatchDencity -= cdv;
                bunny.HatchDencity -= cdv;
                earth.HatchDencity -= cdv;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(camera.ClearColor);

            // TODO: Add your drawing code here

            base.Draw(gameTime);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            spriteBatch.DrawString(Content.Load<SpriteFont>("Fonts/hudFont"), "Camera Controls: [WASD - Translate] [Arrow Keys - Rotate]", new Vector2(8, 8), Color.Gold);
            spriteBatch.DrawString(Content.Load<SpriteFont>("Fonts/hudFont"), $"Hatching Density [R/F] +- {cube.HatchDencity}", new Vector2(8, 22), Color.Gold);
            spriteBatch.DrawString(Content.Load<SpriteFont>("Fonts/hudFont"), "ESC - Exit", new Vector2(8, 36), Color.Gold);
            spriteBatch.End();
        }
    }
}
