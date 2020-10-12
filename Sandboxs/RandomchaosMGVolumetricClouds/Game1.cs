using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using RandomchaosMGBase.Noise;
using RandomchaosMGBase.BaseClasses;

namespace RandomchaosMGVolumetricClouds
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Base3DCamera camera;

        InstancedCloudManager cloudManager;


        int[] allCloudSprites = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

        float speedTran = 1f;
        float speedRot = .01f;

        enum SkyType
        {
            SpotClouds,
            CloudySky,
            CloudField,
            CloudSplatter
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Content.RootDirectory = "Content";

            camera = new Base3DCamera(this, .1f, 20000);
            Components.Add(camera);
            Services.AddService(typeof(Base3DCamera), camera);

            //////////////////////////////////////////////////////////////////
            // REMEMBER THE CloudSpriteSheetOrg texture is property of MS   //
            // Do not comercialy use this texture!                          //
            //////////////////////////////////////////////////////////////////
            cloudManager = new InstancedCloudManager(this, "Shaders/InstancedCloudShader", "Textures/CloudSpriteSheetOrg");
            Components.Add(cloudManager);

            Random rnd = new Random(DateTime.Now.Millisecond);

            SkyType skyType = SkyType.CloudySky;

            float x, y, z;
            float d = 1;

            switch (skyType)
            {
                case SkyType.CloudSplatter:
                    float boxSize = 250;
                    Vector3 flatBase = new Vector3(10, 1, 5);

                    for (int c = 0; c < 100; c++)
                    {
                        d = 1;
                        x = MathHelper.Lerp(-boxSize, boxSize, (float)rnd.NextDouble());
                        y = MathHelper.Lerp(-boxSize / 2, boxSize, (float)rnd.NextDouble());
                        z = MathHelper.Lerp(-boxSize, boxSize, (float)rnd.NextDouble());

                        if (y < 200)
                            d = .75f;
                        if (y < 0)
                            d = .5f;

                        cloudManager.AddCloud(25, new Vector3(x, y, z), 40, flatBase, flatBase * 5, d, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9);
                    }
                    break;
                case SkyType.CloudField:
                    Vector3 cloudDim1 = new Vector3(500, 20, 500);

                    cloudManager.AddCloud(2000, Vector3.Zero, 60, cloudDim1, cloudDim1, .25f, 0, 1, 2, 3, 4);
                    cloudManager.AddCloud(2000, new Vector3(0, 30, 0), 60, cloudDim1, cloudDim1, .5f, 3, 4, 5, 6, 7, 8);
                    cloudManager.AddCloud(2000, new Vector3(0, 60, 0), 60, cloudDim1, cloudDim1, .75f, 7, 8, 9, 10, 11);
                    cloudManager.AddCloud(2000, new Vector3(0, 90, 0), 60, cloudDim1, cloudDim1, 1f, 0, 1, 2, 3, 4, 12, 13, 14, 15);

                    break;

                case SkyType.CloudySky:
                    Vector3 episode1PlayArea = new Vector3(1000, 1000, 1000);

                    // Outer large clouds                    
                    cloudManager.AddCloud(50, Vector3.Zero, 2500, new Vector3(4000, 4000, 4000), new Vector3(2000, 2000, 2000), .75f, allCloudSprites);

                    // clouds inplay
                    flatBase = new Vector3(50, 5, 25);

                    for (int c = 0; c < 50; c++)
                    {
                        d = 1;

                        x = MathHelper.Lerp(-episode1PlayArea.X, episode1PlayArea.X, (float)rnd.NextDouble());
                        y = MathHelper.Lerp(-episode1PlayArea.Y, episode1PlayArea.Y, (float)rnd.NextDouble());
                        z = MathHelper.Lerp(-episode1PlayArea.Z, episode1PlayArea.Z, (float)rnd.NextDouble());

                        if (y < 200)
                            d = .8f;
                        if (y < 0 && y > -500)
                            d = .75f;
                        if (y < -500)
                            d = .5f;

                        cloudManager.AddCloud(25, new Vector3(x, y, z), 300, flatBase, flatBase * 5, d, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9);

                    }
                    break;
                case SkyType.SpotClouds:
                    // Randomly place some clouds in the scene.
                    flatBase = new Vector3(20, 2, 20);

                    // Cloud are avolume
                    boxSize = 800;
                    for (int c = 0; c < 300; c++)
                    {
                        // Place the cloud randomly in the area.
                        x = MathHelper.Lerp(-boxSize, boxSize, (float)rnd.NextDouble());
                        y = MathHelper.Lerp(0, boxSize / 1, (float)rnd.NextDouble());
                        z = MathHelper.Lerp(-boxSize, boxSize, (float)rnd.NextDouble());

                        cloudManager.AddCloud(25, new Vector3(x, y, z), 64, flatBase, flatBase * 5, .75f, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9);
                    }
                    break;
            }
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

        Texture2D testNoise;

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            testNoise = new Texture2D(GraphicsDevice, 128, 128);

            INoise noise = new PerlinNoise(0, 8);
            //noise = new ValueNoise(0, 8);
            //noise = new SimplexNoise(0, 8);
            //noise = new VoronoiNoise(0, 8);
            //noise = new WorleyNoise(0, 8,1.0f);

            FractalNoiseGenerator fractal = new FractalNoiseGenerator(noise, 4, 1);

            Color[] pixels = new Color[testNoise.Width * testNoise.Height];

            for (int y = 0; y < testNoise.Height; y++)
            {
                for (int x = 0; x < testNoise.Width; x++)
                {
                    float fx = x / (testNoise.Width - 1.0f);
                    float fy = y / (testNoise.Height - 1.0f);

                    float l = fractal.Sample2D(fx, fy);
                    pixels[x + (y * testNoise.Width)] = Color.Lerp(Color.Black, Color.White, l);
                }
            }

            testNoise.SetData(pixels);            
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
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.W))
                camera.Translate(Vector3.Forward * speedTran);
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                camera.Translate(Vector3.Backward * speedTran);
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                camera.Translate(Vector3.Left * speedTran);
            if (Keyboard.GetState().IsKeyDown(Keys.D))
                camera.Translate(Vector3.Right * speedTran);

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                camera.Rotate(Vector3.Up, speedRot);
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                camera.Rotate(Vector3.Up, -speedRot);
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                camera.Rotate(Vector3.Right, speedRot);
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                camera.Rotate(Vector3.Right, -speedRot);

            base.Update(gameTime);

            cloudManager.Rotate(Vector3.Up, .00005f);
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

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);

            spriteBatch.Draw(testNoise, new Rectangle(0, 0, 256, 256), new Rectangle(0, 0, 128, 128), Color.White);

            spriteBatch.End();
        }
    }
}
