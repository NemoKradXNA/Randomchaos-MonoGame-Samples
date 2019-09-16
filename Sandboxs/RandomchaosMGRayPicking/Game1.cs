using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using RandomchaosMGBase;
using RandomchaosMGBase.BaseClasses;
using RandomchaosMGBase.InputManagers;

namespace RandomchaosMGRayPicking
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

        Base3DObject bunny;
        Base3DObject cube;
        Base3DObject earth;

        Base3DObject selected;

        List<Base3DObject> sceneObjects = new List<Base3DObject>();

        Vector3 LightPosition = new Vector3(50, 50, 1000);
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
            Components.Add(camera);
            Services.AddService(typeof(Base3DCamera), camera);

            skyBox = new BaseSkyBox(this, "Textures/SkyBox/HMcubemap");
            Components.Add(skyBox);

            cube = new Base3DObject(this, "Models/cube");
            cube.Name = "Cube";
            cube.Position = new Vector3(-2, -1, -10);
            cube.ColorAsset = "Textures/h2mcpCube";
            cube.BumpAsset = "Textures/h2mcpCubeNormal";
            cube.LightPosition = LightPosition;
            cube.BoundingBox = new BoundingBox(-Vector3.One*.5f, Vector3.One*.5f);
            cube.BoundingSphere = new BoundingSphere(Vector3.Zero, .75f);            
            Components.Add(cube);
            sceneObjects.Add(cube);

            bunny = new Base3DObject(this, "Models/bunny");
            bunny.Name = "Bunny";
            bunny.NoTangentData = true;
            bunny.NoTexCoords = true;
            bunny.Position = new Vector3(0, -1.5f, -10);
            bunny.LightPosition = LightPosition;
            bunny.BoundingBox = new BoundingBox(new Vector3(-1,0,-.6f), new Vector3(.7f,1.65f,.7f));
            bunny.BoundingSphere = new BoundingSphere(new Vector3(-.2f,.75f,0), 1);            
            Components.Add(bunny);
            sceneObjects.Add(bunny);

            earth = new Base3DObject(this, "Models/sphere");
            earth.Name = "Globe";
            earth.Position = new Vector3(2, -1, -10);
            earth.ColorAsset = "Textures/Earth_Diffuse";
            earth.BumpAsset = "Textures/Earth_NormalMap";
            earth.LightPosition = LightPosition;
            earth.BoundingBox = new BoundingBox(-Vector3.One, Vector3.One);
            earth.BoundingSphere = new BoundingSphere(Vector3.Zero, 1);            
            Components.Add(earth);
            sceneObjects.Add(earth);

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

            // Do picking
            if (msm.LeftClicked)
            {
                float minD = float.MaxValue;

                foreach (Base3DObject obj in sceneObjects)
                {
                    BoundingBox tbb = GameComponentHelper.TransformedBoundingBoxAA(obj.BoundingBox, obj.Position, obj.Scale);
                    float d = GameComponentHelper.RayPicking(msm.ScreenPoint, tbb, camera);
                    
                    if (d < minD)
                    {
                        minD = d;
                        if (selected != null)
                        {
                            selected.DrawBoxBounds = false;
                        }
                        selected = obj;
                        selected.DrawBoxBounds = true;
                    }
                }

                if (minD == float.MaxValue)
                {
                    if (selected != null)
                        selected.DrawBoxBounds = false;

                    selected = null;
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            spriteBatch.DrawString(Content.Load<SpriteFont>("Fonts/hudFont"), "Camera Controls: [WASD - Translate] [Arrow Keys - Rotate] [Left Mouse Button to Pick]", new Vector2(8, 8), Color.Gold);
            spriteBatch.DrawString(Content.Load<SpriteFont>("Fonts/hudFont"), string.Format("Selected Object: {0}", selected == null ? "None" : selected.Name), new Vector2(8, 22), Color.Gold);
            spriteBatch.DrawString(Content.Load<SpriteFont>("Fonts/hudFont"), "ESC - Exit", new Vector2(8, 36), Color.Gold);
            spriteBatch.End();
        }
    }
}
