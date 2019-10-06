using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using RandomchaosMGBase.BaseClasses;
using RandomchaosMGBase.InputManagers;
using RandomchaosMGDeferredLighting.Lights;
using RandomchaosMGBase.BaseClasses.Primatives;
using RandomchaosMGBase;

using Plane = RandomchaosMGBase.BaseClasses.Primatives.Plane;

namespace RandomchaosMGDeferredLighting
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        DeferredLightingManager lightingManager;

        KeyboardStateManager kbm;
        

        SpriteFont infoFont;
        SpriteFont debugFont;

        DeferredConeLight coneLight;

        protected DeferredLightingCamera camera;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.GraphicsProfile = GraphicsProfile.HiDef;

            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;

            IsMouseVisible = true;

            kbm = new KeyboardStateManager(this);

            lightingManager = new DeferredLightingManager(this);

            camera = new DeferredLightingCamera(this, .5f, 20000);
            camera.Position = new Vector3(0, 1, 0);
            Components.Add(camera);
            Services.AddService(typeof(Base3DCamera), camera);

            BaseSkyBox skyBox = new BaseSkyBox(this, "Textures/SkyBox/HMcubemap");
            skyBox.effectAsset = "Shaders/DeferredRender/SkyBoxShader";
            Components.Add(skyBox);

            Plane plane = new Plane(this, "Shaders/DeferredRender/DeferredModelRender");
            plane.ColorAsset = "Textures/cobbles";
            plane.BumpAsset = "Textures/cobblesNormal";
            plane.OcclusionAsset = "Textures/cobbleOcclusion";
            plane.SpecularAsset = "Textures/cobblesSpecular";
            plane.Scale = Vector3.One * 2;
            plane.UVMultiplier = Vector2.One * 5;
            plane.Position = new Vector3(0, -1, -20);     
            Components.Add(plane);

            Base3DObject bunny = new Base3DObject(this, "Models/bunny", "Shaders/DeferredRender/DeferredModelRender");
            bunny.Position = new Vector3(0, -1f, -20);
            Components.Add(bunny);

            Cube cube = new Cube(this, "Shaders/DeferredRender/DeferredModelRender");
            cube.ColorAsset = "Textures/xnauguk";
            cube.Position = new Vector3(-3, -.5f, -20);
            Components.Add(cube);

            Base3DObject sphere = new Base3DObject(this, "Models/sphere", "Shaders/DeferredRender/DeferredModelRender");
            sphere.Position = new Vector3(3, -0, -20);
            sphere.ColorAsset = "Textures/brick";
            sphere.BumpAsset = "Textures/brickNormal";
            sphere.OcclusionAsset = "Textures/brickOcclusion";
            sphere.SpecularAsset = "Textures/brickSpecular";
            sphere.UVMultiplier = Vector2.One * 2;
            Components.Add(sphere);

            DeferredDirectionalLight directionalLight = new DeferredDirectionalLight(this, new Vector3(10, 10, 10), Color.AliceBlue, 1, true);
            directionalLight.Transform.LookAt(Vector3.Zero, 1, Vector3.Forward);
            Components.Add(directionalLight);
            lightingManager.AddLight(directionalLight);

           
            DeferredPointLight pointLight = new DeferredPointLight(this, new Vector3(0, 1, -20), Color.Red, 5, 1, false);
            Components.Add(pointLight);
            lightingManager.AddLight(pointLight);

            coneLight = new DeferredConeLight(this, new Vector3(-10, 5, -25), Color.Gold, 1, MathHelper.ToRadians(45), 5, true);
            coneLight.Transform.LookAt(new Vector3(0, 0, -20), 1, Vector3.Forward);
            Components.Add(coneLight);
            lightingManager.AddLight(coneLight);

            DeferredConeLight coneLight2 = new DeferredConeLight(this, new Vector3(0, 20, -20), Color.Green, 1, MathHelper.ToRadians(25), 50, true);
            coneLight2.Transform.LookAt(new Vector3(0, 0, -20), 1, Vector3.Forward);
            Components.Add(coneLight2);
            lightingManager.AddLight(coneLight2);

            Cube sandBlock = new Cube(this, "Shaders/DeferredRender/DeferredModelRender");
            sandBlock.ColorAsset = "Textures/sand";
            sandBlock.BumpAsset = "Textures/sandNormal";
            sandBlock.Position = new Vector3(-3, 1.5f, -20);
            sandBlock.Rotate(Vector3.Up + Vector3.Forward + Vector3.Left, 15);
            
            Components.Add(sandBlock);

            // Shadow casters and receivers..
            lightingManager.AddShadowCaster(bunny);
            lightingManager.AddShadowCaster(cube);
            lightingManager.AddShadowCaster(sphere);
            lightingManager.AddShadowCaster(sandBlock);
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

            infoFont = Content.Load<SpriteFont>("Fonts/font");
            debugFont = Content.Load<SpriteFont>("Fonts/AGENCYR");

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


            if (kbm.KeyDown(Keys.R))
                lightingManager.deferredDirectionalShadowMapMod += .000001f;
            if (kbm.KeyDown(Keys.F))
                lightingManager.deferredDirectionalShadowMapMod -= .000001f;

            if (kbm.KeyDown(Keys.T))
                lightingManager.deferredConeShadowMapMod += .000001f;
            if (kbm.KeyDown(Keys.G))
                lightingManager.deferredConeShadowMapMod -= .000001f;

            if (kbm.KeyPress(Keys.Space))
            {
                foreach (BaseLight light in lightingManager.ShadowLights)
                    light.HardShadows = !light.HardShadows;
            }

            if (kbm.KeyPress(Keys.F1))
            {
                foreach (BaseLight light in lightingManager.ShadowLights)
                    light.CastShadow = !light.CastShadow;
            }

            lightingManager.deferredDirectionalShadowMapMod = MathHelper.Clamp(lightingManager.deferredDirectionalShadowMapMod, 0, 1);
            lightingManager.deferredConeShadowMapMod = MathHelper.Clamp(lightingManager.deferredConeShadowMapMod, 0, 1);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            lightingManager.PreSceneDrawSetUp(gameTime);

            // Draw all the items in the scene, they need to use the deferred Model Render shader.
            base.Draw(gameTime);

            lightingManager.PostSceneDraw(gameTime);

            #region Debug & HUD
            // Debug, show shadow map...
            if (lightingManager.DebugShadowMaps)
            {
                if (lightingManager.ShadowLights != null && lightingManager.ShadowLights.Count > 0 && lightingManager.ShadowCasters != null && lightingManager.ShadowCasters.Count > 0)
                {
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
                    // We have lights to cast shadows, objects that will block light, and objects to receive the shadows...
                    int s = 512;
                    int m = 0;
                    foreach (BaseLight light in lightingManager.ShadowLights)
                    {
                        spriteBatch.Draw(light.ShadowMap, new Rectangle(s * m++, 0, s, s), Color.White);
                    }
                    spriteBatch.End();
                }
            }

            // Deferred Render debug
            if (lightingManager.DebugLighting) // && GameComponentHelper.InEditor)
            {
                int w = GraphicsDevice.Viewport.Width / 5;
                int h = GraphicsDevice.Viewport.Height / 5;

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);

                //spriteBatch.Draw(t, new Rectangle(0, 0, w * 6, h + 2), Color.White);
                spriteBatch.Draw(camera.RenderTarget, new Rectangle(1, 1, w, h), Color.White);
                spriteBatch.Draw(camera.SpecularGlowReflectionMap, new Rectangle(w + 2, 1, w, h), Color.White);
                spriteBatch.Draw(camera.NormalBuffer, new Rectangle((w * 2) + 3, 1, w, h), Color.White);

                GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                spriteBatch.Draw(camera.DepthBuffer, new Rectangle((w * 3) + 5, 1, w, h), Color.White);
                spriteBatch.Draw(lightingManager.lightMap, new Rectangle((w * 4) + 7, 1, w, h), Color.White);

                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                spriteBatch.DrawString(debugFont, "Color Map", new Vector2((w / 2) - 100, h), Color.White);
                spriteBatch.DrawString(debugFont, "SGR Map", new Vector2(w + 2 + (w / 2) - 100, h), Color.White);
                spriteBatch.DrawString(debugFont, "Bump Map", new Vector2((w * 2) + (w / 2) - 100, h), Color.White);
                spriteBatch.DrawString(debugFont, "Depth Map", new Vector2((w * 3) + (w / 2) - 100, h), Color.White);
                spriteBatch.DrawString(debugFont, "Light Map", new Vector2((w * 4) + (w / 2) - 100, h), Color.White);
                spriteBatch.End();
            }

            int top = 8;
            int lineHeight = infoFont.LineSpacing;

            if (lightingManager.DebugLighting || lightingManager.DebugShadowMaps)
                top += (GraphicsDevice.Viewport.Height / 5) + debugFont.LineSpacing;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            spriteBatch.DrawString(infoFont, $"[WASD] - Translate Camera", new Vector2(8, top), Color.Gold);
            spriteBatch.DrawString(infoFont, $"[Arrow Keys] - Rotate Camera", new Vector2(8, top + lineHeight), Color.Gold);
            spriteBatch.DrawString(infoFont, $"[R/F] - Directional Shadow Offset +- {lightingManager.deferredDirectionalShadowMapMod} ", new Vector2(8, top + lineHeight * 2), Color.Gold);
            spriteBatch.DrawString(infoFont, $"[T/G] - Cone Shadow Offset +- {lightingManager.deferredConeShadowMapMod} ", new Vector2(8, top + lineHeight * 3), Color.Gold);
            spriteBatch.DrawString(infoFont, $"[SPC] - Toggle Hord/Soft Shadows", new Vector2(8, top + lineHeight * 4), Color.Gold);
            spriteBatch.DrawString(infoFont, $"[F1] - Toggle Shadows", new Vector2(8, top + lineHeight * 5), Color.Gold);
            spriteBatch.End();
            #endregion
        }
    }
}
