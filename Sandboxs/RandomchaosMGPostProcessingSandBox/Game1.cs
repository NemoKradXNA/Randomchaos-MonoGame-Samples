using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using RandomchaosMGBase.BaseClasses;
using RandomchaosMGBase.BaseClasses.PostProcessing;
using RandomchaosMGBase.InputManagers;


// source of bunny mesh https://casual-effects.com/data/
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
        Base3DObject[] bunnies;
        Base3DObject landShark;
        Base3DObject earth;

        Base3DObject bunny;
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
        DeRezedEffect deRezed;

        ColorFilterEffect colorFilter;
        BleachEffect bleach;
        ScanLinesEffect scanLines;

        FXAAEffect fxaa;
        #endregion

        #region UI bits
        protected int line = 8;

        protected const int lineHeight = 16;
        protected const int lineCount = 21;

        protected BasePostProcessingEffect selectedEffect = null;
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

            bunny = new Base3DObject(this, "Models/bunny", "Shaders/RenderObjectNotTangentsOrTexCoords");
            bunny.Position = new Vector3(2, -1.5f, -20);
            bunny.LightPosition = LightPosition;
            Components.Add(bunny);

            cube = new Base3DObject(this, "Models/cube");
            cube.Position = new Vector3(0, 0, -10);
            cube.ColorAsset = "Textures/h2mcpCube";
            cube.BumpAsset = "Textures/h2mcpCubeNormal";
            cube.LightPosition = LightPosition;
            Components.Add(cube);

            bunnies = new Base3DObject[10];
            for (int s = 0; s < 10; s++)
            {
                bunnies[s] = new Base3DObject(this, "Models/Bunny");
                bunnies[s].Position = new Vector3(20 + (s * 12), -1.5f, -10 - (s * 12));
                bunnies[s].ColorAsset = "Textures/WindmillTopColor";
                bunnies[s].BumpAsset = "Textures/WindmillTopNormal";
                bunnies[s].LightPosition = LightPosition;
                Components.Add(bunnies[s]);
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

            //GodRays = new CrepuscularRays(this, LightPosition, "Textures/flare", 1500, .99f, .99f, .5f, .12f, .25f);
            GodRays = new CrepuscularRays(this, new Vector3(10,10,100), "Textures/flare", 100, .99f, .99f, .5f, .12f, .25f);
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

            colorFilter = new ColorFilterEffect(this, Color.White, .5f, .5f, .5f);
            colorFilter.Enabled = false;
            ppManager.AddEffect(colorFilter);

            bleach = new BleachEffect(this, 1);
            bleach.Enabled = false;
            ppManager.AddEffect(bleach);

            scanLines = new ScanLinesEffect(this, .001f, .001f, 128);
            scanLines.Enabled = false;
            ppManager.AddEffect(scanLines);

            deRezed = new DeRezedEffect(this, 128);
            deRezed.Enabled = false;
            ppManager.AddEffect(deRezed);

            fxaa = new FXAAEffect(this,.0312f,.063f,1f, FXAA.Technique.EdgeLumincense);
            fxaa.Enabled = false;
            ppManager.AddEffect(fxaa);
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

            earth.Rotate(Vector3.Up, .01f);

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
                selectedEffect = bloom;

            if (kbm.KeyPress(Keys.F2))
                selectedEffect = GodRays;
            
            if (kbm.KeyPress(Keys.F3))
                selectedEffect = dof;

            if (kbm.KeyPress(Keys.F4))
                selectedEffect = fog;

            if (kbm.KeyPress(Keys.F5))
                selectedEffect = haze;

            if (kbm.KeyPress(Keys.F6))
                selectedEffect = radialBlur;

            if (kbm.KeyPress(Keys.F7))
                selectedEffect = ripple;

            if (kbm.KeyPress(Keys.F8))
                selectedEffect = sun;

            if (kbm.KeyPress(Keys.F9))
                selectedEffect = sepia;

            if (kbm.KeyPress(Keys.F10))
                selectedEffect = greyScale;

            if (kbm.KeyPress(Keys.F11))
                selectedEffect = invert;

            if (kbm.KeyPress(Keys.F12))
                selectedEffect = deRezed;

            if (kbm.KeyPress(Keys.D1))
                selectedEffect = colorFilter;

            if (kbm.KeyPress(Keys.D2))
                selectedEffect = bleach;

            if (kbm.KeyPress(Keys.D3))
                selectedEffect = scanLines;

            if (kbm.KeyPress(Keys.D4))
                selectedEffect = fxaa;


            if (selectedEffect != null)
            {
                if (kbm.KeyPress(Keys.Space))
                    selectedEffect.Enabled = !selectedEffect.Enabled;

                if (selectedEffect == bloom)
                {
                    if (kbm.KeyPress(Keys.G))
                        bloom.Glare = !bloom.Glare;

                    if (kbm.KeyDown(Keys.Y))
                        bloom.BloomThreshold = MathHelper.Min(bloom.BloomThreshold + .001f, 1f);
                    if (kbm.KeyDown(Keys.H))
                        bloom.BloomThreshold = MathHelper.Max(bloom.BloomThreshold - .001f, 0.0f);

                    if (kbm.KeyDown(Keys.U))
                        bloom.BlurAmount = MathHelper.Min(bloom.BlurAmount + .1f, 4);
                    if (kbm.KeyDown(Keys.J))
                        bloom.BlurAmount = MathHelper.Max(bloom.BlurAmount - .1f, 0.1f);
                }
                else if (selectedEffect == GodRays)
                {
                    if (kbm.KeyDown(Keys.R))
                        GodRays.LightSourceSize = MathHelper.Min(GodRays.LightSourceSize + .5f, 10000);
                    if (kbm.KeyDown(Keys.F))
                        GodRays.LightSourceSize = MathHelper.Max(GodRays.LightSourceSize - .5f, .5f);

                    if (kbm.KeyDown(Keys.T))
                        GodRays.Density = MathHelper.Min(GodRays.Density + .01f, 1);
                    if (kbm.KeyDown(Keys.G))
                        GodRays.Density = MathHelper.Max(GodRays.Density - .01f, 0);

                    if (kbm.KeyDown(Keys.Y))
                        GodRays.Decay = MathHelper.Min(GodRays.Decay + .0001f, 1);
                    if (kbm.KeyDown(Keys.H))
                        GodRays.Decay = MathHelper.Max(GodRays.Decay - .0001f, 0);

                    if (kbm.KeyDown(Keys.U))
                        GodRays.Weight = MathHelper.Min(GodRays.Weight + .001f, 1);
                    if (kbm.KeyDown(Keys.J))
                        GodRays.Weight = MathHelper.Max(GodRays.Weight - .001f, 0);

                    if (kbm.KeyDown(Keys.I))
                        GodRays.Exposure = MathHelper.Min(GodRays.Exposure + .001f, 1);
                    if (kbm.KeyDown(Keys.K))
                        GodRays.Exposure = MathHelper.Max(GodRays.Exposure - .001f, 0);

                    if (kbm.KeyDown(Keys.O))
                        GodRays.BrightThreshold = MathHelper.Min(GodRays.BrightThreshold + .01f, .999f);
                    if (kbm.KeyDown(Keys.L))
                        GodRays.BrightThreshold = MathHelper.Max(GodRays.BrightThreshold - .01f, 0);
                }
                else if (selectedEffect == dof)
                {
                    if (kbm.KeyDown(Keys.R))
                        dof.DiscRadius = MathHelper.Min(dof.DiscRadius + .1f, camera.Viewport.MaxDepth);
                    if (kbm.KeyDown(Keys.F))
                        dof.DiscRadius = MathHelper.Max(dof.DiscRadius - .1f, 0);

                    if (kbm.KeyDown(Keys.T))
                        dof.FocalDistance = MathHelper.Min(dof.FocalDistance + .1f, camera.Viewport.MaxDepth);
                    if (kbm.KeyDown(Keys.G))
                        dof.FocalDistance = MathHelper.Max(dof.FocalDistance - .1f, 0);

                    if (kbm.KeyDown(Keys.Y))
                        dof.FocalRange = MathHelper.Min(dof.FocalRange + .1f, camera.Viewport.MaxDepth);
                    if (kbm.KeyDown(Keys.H))
                        dof.FocalRange = MathHelper.Max(dof.FocalRange - .1f, 0);
                }
                else if (selectedEffect == fog)
                {
                    if (kbm.KeyDown(Keys.R))
                        fog.FogDistance = MathHelper.Min(fog.FogDistance + .1f, camera.Viewport.MaxDepth);
                    if (kbm.KeyDown(Keys.F))
                        fog.FogDistance = MathHelper.Max(fog.FogDistance - .1f, 1);

                    if (kbm.KeyDown(Keys.T))
                        fog.FogRange = MathHelper.Min(fog.FogRange + .1f, camera.Viewport.MaxDepth);
                    if (kbm.KeyDown(Keys.G))
                        fog.FogRange = MathHelper.Max(fog.FogRange - .1f, 1);

                    if (kbm.KeyDown(Keys.Y))
                        fog.FogColor = SetColor(fog.FogColor.ToVector4() + new Vector4(.01f, 0, 0, 0));
                    if (kbm.KeyDown(Keys.H))
                        fog.FogColor = SetColor(fog.FogColor.ToVector4() - new Vector4(.01f, 0, 0, 0));

                    if (kbm.KeyDown(Keys.U))
                        fog.FogColor = SetColor(fog.FogColor.ToVector4() + new Vector4(0, .01f, 0, 0));
                    if (kbm.KeyDown(Keys.J))
                        fog.FogColor = SetColor(fog.FogColor.ToVector4() - new Vector4(0, .01f, 0, 0));

                    if (kbm.KeyDown(Keys.I))
                        fog.FogColor = SetColor(fog.FogColor.ToVector4() + new Vector4(0, 0, .01f, 0));
                    if (kbm.KeyDown(Keys.K))
                        fog.FogColor = SetColor(fog.FogColor.ToVector4() - new Vector4(0, 0, .01f, 0));

                    if (kbm.KeyDown(Keys.O))
                        fog.FogColor = SetColor(fog.FogColor.ToVector4() + new Vector4(0, 0, 0, .01f));
                    if (kbm.KeyDown(Keys.L))
                        fog.FogColor = SetColor(fog.FogColor.ToVector4() - new Vector4(0, 0, 0, .01f));
                }
                else if (selectedEffect == haze)
                {
                    if (kbm.KeyPress(Keys.H))
                        haze.High = !haze.High;
                }
                else if (selectedEffect == radialBlur)
                {
                    if (kbm.KeyDown(Keys.R))
                        radialBlur.Scale = MathHelper.Min(radialBlur.Scale + .001f, 1);
                    if (kbm.KeyDown(Keys.F))
                        radialBlur.Scale = MathHelper.Max(radialBlur.Scale - .001f, 0);
                }
                else if (selectedEffect == ripple)
                {
                    if (kbm.KeyDown(Keys.T))
                        ripple.Distortion = MathHelper.Min(ripple.Distortion + .01f, 100);
                    if (kbm.KeyDown(Keys.G))
                        ripple.Distortion = MathHelper.Max(ripple.Distortion - .01f, 0);

                    if (kbm.KeyDown(Keys.Y))
                        ripple.ScreenPosition = SetVector2(ripple.ScreenPosition + new Vector2(.001f, 0));
                    if (kbm.KeyDown(Keys.H))
                        ripple.ScreenPosition = SetVector2(ripple.ScreenPosition - new Vector2(.001f, 0));

                    if (kbm.KeyDown(Keys.U))
                        ripple.ScreenPosition = SetVector2(ripple.ScreenPosition + new Vector2(0, .001f));
                    if (kbm.KeyDown(Keys.J))
                        ripple.ScreenPosition = SetVector2(ripple.ScreenPosition + new Vector2(0, .001f));

                }
                else if (selectedEffect == colorFilter)
                {
                    if (kbm.KeyDown(Keys.R))
                        colorFilter.Bright = MathHelper.Min(colorFilter.Bright + .001f, 1);
                    if (kbm.KeyDown(Keys.F))
                        colorFilter.Bright = MathHelper.Max(colorFilter.Bright - .001f, 0);

                    if (kbm.KeyDown(Keys.T))
                        colorFilter.Saturation = MathHelper.Min(colorFilter.Saturation + .001f, 1);
                    if (kbm.KeyDown(Keys.G))
                        colorFilter.Saturation = MathHelper.Max(colorFilter.Saturation - .001f, 0);

                    if (kbm.KeyDown(Keys.Y))
                        colorFilter.Burn = MathHelper.Min(colorFilter.Burn + .001f, 1);
                    if (kbm.KeyDown(Keys.H))
                        colorFilter.Burn = MathHelper.Max(colorFilter.Burn - .001f, 0);

                    if (kbm.KeyDown(Keys.U))
                        colorFilter.Color = SetColor(colorFilter.Color.ToVector4() + new Vector4(.01f, 0, 0, 0));
                    if (kbm.KeyDown(Keys.J))
                        colorFilter.Color = SetColor(colorFilter.Color.ToVector4() - new Vector4(.01f, 0, 0, 0));

                    if (kbm.KeyDown(Keys.I))
                        colorFilter.Color = SetColor(colorFilter.Color.ToVector4() + new Vector4(0, .01f, 0, 0));
                    if (kbm.KeyDown(Keys.K))
                        colorFilter.Color = SetColor(colorFilter.Color.ToVector4() - new Vector4(0, .01f, 0, 0));

                    if (kbm.KeyDown(Keys.O))
                        colorFilter.Color = SetColor(colorFilter.Color.ToVector4() + new Vector4(0, 0, .01f, 0));
                    if (kbm.KeyDown(Keys.L))
                        colorFilter.Color = SetColor(colorFilter.Color.ToVector4() - new Vector4(0, 0, .01f, 0));
                }
                else if (selectedEffect == bleach)
                {
                    if (kbm.KeyDown(Keys.R))
                        bleach.Opacity = MathHelper.Min(bleach.Opacity + .001f, 1);
                    if (kbm.KeyDown(Keys.F))
                        bleach.Opacity = MathHelper.Max(bleach.Opacity - .001f, 0);
                }
                else if (selectedEffect == scanLines)
                {
                    if (kbm.KeyDown(Keys.R))
                        scanLines.NoiseIntensity = MathHelper.Min(scanLines.NoiseIntensity + .001f, 1);
                    if (kbm.KeyDown(Keys.F))
                        scanLines.NoiseIntensity = MathHelper.Max(scanLines.NoiseIntensity - .001f, 0);

                    if (kbm.KeyDown(Keys.T))
                        scanLines.LineIntensity = MathHelper.Min(scanLines.LineIntensity + .001f, 1);
                    if (kbm.KeyDown(Keys.G))
                        scanLines.LineIntensity = MathHelper.Max(scanLines.LineIntensity - .001f, 0);

                    if (kbm.KeyDown(Keys.Y))
                        scanLines.LineCount = MathHelper.Min(scanLines.LineCount + 1, 20000);
                    if (kbm.KeyDown(Keys.H))
                        scanLines.LineCount = MathHelper.Max(scanLines.LineCount - 1, 0);
                }
                else if (selectedEffect == deRezed)
                {
                    if (kbm.KeyDown(Keys.R))
                        deRezed.NumberOfTiles = MathHelper.Min(deRezed.NumberOfTiles + 1, 1024);
                    if (kbm.KeyDown(Keys.F))
                        deRezed.NumberOfTiles = MathHelper.Max(deRezed.NumberOfTiles - 1, 1);
                }
                else if (selectedEffect == fxaa)
                {
                    if (kbm.KeyPress(Keys.R))
                    {
                        int v = (int)fxaa.TechniqueUsed;
                        v++;
                        if (v >= (int)FXAA.Technique.MAX_DO_NOT_USE)
                            v = 0;

                        fxaa.TechniqueUsed = (FXAA.Technique)v;
                    }

                    if (kbm.KeyPress(Keys.F))
                    {
                        int v = (int)fxaa.TechniqueUsed;
                        v--;
                        if (v < 0)
                            v = (int)FXAA.Technique.MAX_DO_NOT_USE-1;

                        fxaa.TechniqueUsed = (FXAA.Technique)v;
                    }

                    if (kbm.KeyDown(Keys.T))
                        fxaa.ContrastThreshold = MathHelper.Clamp(fxaa.ContrastThreshold + .001f, 0, 1);
                    if (kbm.KeyDown(Keys.G))
                        fxaa.ContrastThreshold = MathHelper.Clamp(fxaa.ContrastThreshold - .001f, 0, 1);

                    if (kbm.KeyDown(Keys.Y))
                        fxaa.RelativeThreshold = MathHelper.Clamp(fxaa.RelativeThreshold + .001f, 0, 1);
                    if (kbm.KeyDown(Keys.H))
                        fxaa.RelativeThreshold = MathHelper.Clamp(fxaa.RelativeThreshold - .001f, 0, 1);

                    if (kbm.KeyDown(Keys.U))
                        fxaa.SubpixelBlending = MathHelper.Clamp(fxaa.SubpixelBlending + .001f, 0, 1);
                    if (kbm.KeyDown(Keys.J))
                        fxaa.SubpixelBlending = MathHelper.Clamp(fxaa.SubpixelBlending - .001f, 0, 1);

                    if (kbm.KeyPress(Keys.I))
                        fxaa.RenderHalfScreen = !fxaa.RenderHalfScreen;

                    if (kbm.KeyDown(Keys.O))
                        fxaa.Vline = MathHelper.Clamp(fxaa.Vline + .001f, 0, 1);
                    if (kbm.KeyDown(Keys.L))
                        fxaa.Vline = MathHelper.Clamp(fxaa.Vline - .001f, 0, 1);
                }
            }
        }

        protected Color SetColor(Vector4 newC)
        {
            return new Color(Vector4.Clamp(newC, Vector4.Zero, Vector4.One));
        }

        protected Vector2 SetVector2(Vector2 v)
        {
            return v;
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

            line = 0;
            int x = 8;

            spriteBatch.Begin();
            spriteBatch.Draw(Content.Load<Texture2D>("Textures/HUD/HUDBackground"), new Rectangle(0, 0, 250, lineHeight * lineCount), Color.White);
            WriteLine("Post Processing", x, Color.Gold);
            WriteLine($"[F1 ] - Bloom On: {bloom.Enabled}",x, bloom.Enabled ? Color.Yellow : Color.Gold);
            WriteLine($"[F2 ] - God Rays On: {GodRays.Enabled}", x, GodRays.Enabled ? Color.Yellow : Color.Gold);
            WriteLine($"[F3 ] - Depth of Field On: {dof.Enabled}", x, dof.Enabled ? Color.Yellow : Color.Gold);
            WriteLine($"[F4 ] - Fog On: {fog.Enabled}", x, fog.Enabled ? Color.Yellow : Color.Gold);
            WriteLine($"[F5 ] - Heat Haze On: {haze.Enabled}", x, haze.Enabled ? Color.Yellow : Color.Gold);
            WriteLine($"[F6 ] - Radial Blur On: {radialBlur.Enabled}", x,radialBlur.Enabled ? Color.Yellow : Color.Gold);
            WriteLine($"[F7 ] - Ripple On: {ripple.Enabled}", x, ripple.Enabled ? Color.Yellow : Color.Gold);
            WriteLine($"[F8 ] - Sun On: {sun.Enabled}", x, sun.Enabled ? Color.Yellow : Color.Gold);
            WriteLine($"[F9 ] - Sepia On: {sepia.Enabled}", x, sepia.Enabled ? Color.Yellow : Color.Gold);
            WriteLine($"[F10] - Grey Scale On: {greyScale.Enabled}", x, greyScale.Enabled ? Color.Yellow : Color.Gold);
            WriteLine($"[F11] - Invert Color On: {invert.Enabled}", x, invert.Enabled ? Color.Yellow : Color.Gold);
            WriteLine($"[F12] - DeRezed On: {deRezed.Enabled}", x, deRezed.Enabled ? Color.Yellow : Color.Gold);
            WriteLine($"[D1 ] - Color Filter ON: {colorFilter.Enabled}", x, colorFilter.Enabled ? Color.Yellow : Color.Gold);
            WriteLine($"[D2 ] - Bleach ON: {bleach.Enabled}", x, bleach.Enabled ? Color.Yellow : Color.Gold);
            WriteLine($"[D3 ] - Scan Lines ON: {scanLines.Enabled}", x, scanLines.Enabled ? Color.Yellow : Color.Gold);
            WriteLine($"[D4 ] - FXAA ON: {fxaa.Enabled}", x, fxaa.Enabled ? Color.Yellow : Color.Gold);

            WriteLine($"", x, Color.Gold);
            WriteLine($"", x, Color.Gold);
            WriteLine($"[ESC] - Exit", x, Color.Gold);

            line = 0;
            x += 255;

            if (selectedEffect != null)
            {
                
                spriteBatch.Draw(Content.Load<Texture2D>("Textures/HUD/HUDBackground"), new Rectangle(255, 0, 250, lineHeight * 9), Color.White);
                WriteLine($"[{selectedEffect.GetType().Name}]", x, Color.Gold);
                WriteLine($"[SPC] - Toggle Enabled", x, selectedEffect.Enabled ? Color.LimeGreen : Color.Green);

                if (selectedEffect.Enabled)
                {
                    if (selectedEffect == bloom)
                    {
                        WriteLine($"[G  ] Galre On: {bloom.Glare}", x, bloom.Glare ? Color.LimeGreen : Color.Green);
                        WriteLine($"[Y/H] Bloom Threshold +- : {bloom.BloomThreshold}", x, Color.LimeGreen);
                        WriteLine($"[U/J] Blur Amount +- : {bloom.BlurAmount}", x, Color.LimeGreen);
                    }

                    if (selectedEffect == GodRays)
                    {
                        WriteLine($"[R/F] Light Source Size +-: {GodRays.LightSourceSize}", x, Color.LimeGreen);
                        WriteLine($"[T/G] Density +-: {GodRays.Density}", x, Color.LimeGreen);
                        WriteLine($"[Y/H] Decay +-: {GodRays.Decay}", x, Color.LimeGreen);
                        WriteLine($"[U/J] Weight +-: {GodRays.Weight}", x, Color.LimeGreen);
                        WriteLine($"[I/K] Exposure +-: {GodRays.Exposure}", x, Color.LimeGreen);
                        WriteLine($"[O/L] Bright Threshold +-: {GodRays.BrightThreshold}", x, Color.LimeGreen);
                    }

                    if (selectedEffect == dof)
                    {
                        WriteLine($"[R/F] Disc Radius +- : {dof.DiscRadius}", x, Color.LimeGreen);
                        WriteLine($"[T/G] Focal Distance +- : {dof.FocalDistance}", x, Color.LimeGreen);
                        WriteLine($"[Y/H] Focal Range +- : {dof.FocalRange}", x, Color.LimeGreen);
                    }

                    if (selectedEffect == fog)
                    {
                        WriteLine($"[R/F] - Fog Distance +- : {fog.FogDistance}", x, Color.LimeGreen);
                        WriteLine($"[T/G] -  Fog Range +- : {fog.FogRange}", x, Color.LimeGreen);
                        WriteLine($"[Y/H] - Fog Color (R) +- : {fog.FogColor.ToVector4().X}", x, Color.LimeGreen);
                        WriteLine($"[U/J] -  Fog Color (G) +- : {fog.FogColor.ToVector4().Y}", x, Color.LimeGreen);
                        WriteLine($"[I/K] -  Fog Color (B) +- : {fog.FogColor.ToVector4().Z}", x, Color.LimeGreen);
                        WriteLine($"[O/L] -  Fog Color (A) +- : {fog.FogColor.ToVector4().W}", x, Color.LimeGreen);
                    }

                    if (selectedEffect == haze)
                    {
                        WriteLine($"[H  ] - High On: {haze.High}", x, haze.High ? Color.LimeGreen : Color.Green);
                    }

                    if (selectedEffect == radialBlur)
                    {
                        WriteLine($"[R/F] - Sale +=: {radialBlur.Scale}", x, Color.LimeGreen);
                    }

                    if (selectedEffect == ripple)
                    {
                        WriteLine($"[T/G] - Distortion +=: {ripple.Distortion}", x, Color.LimeGreen);
                        WriteLine($"[Y/H] - Screen Position X +=: {ripple.ScreenPosition.X}", x, Color.LimeGreen);
                        WriteLine($"[U/J] - Screen Position Y +=: {ripple.ScreenPosition.Y}", x, Color.LimeGreen);
                    }

                    if (selectedEffect == colorFilter)
                    {
                        WriteLine($"[R/F] - Bright +=: {colorFilter.Bright}", x, Color.LimeGreen);
                        WriteLine($"[T/G] - Saturation +=: {colorFilter.Saturation}", x, Color.LimeGreen);
                        WriteLine($"[Y/H] - Burn +=: {colorFilter.Burn}", x, Color.LimeGreen);
                        WriteLine($"[U/J] - Color (R) +- : {colorFilter.Color.ToVector3().X}", x, Color.LimeGreen);
                        WriteLine($"[I/K] - Color (G) +- : {colorFilter.Color.ToVector3().Y}", x, Color.LimeGreen);
                        WriteLine($"[O/L] - Color (B) +- : {colorFilter.Color.ToVector3().Z}", x, Color.LimeGreen);
                    }
                    if (selectedEffect == bleach)
                    {
                        WriteLine($"[R/F] - Opacity +=: {bleach.Opacity}", x, Color.LimeGreen);
                    }

                    if (selectedEffect == scanLines)
                    {
                        WriteLine($"[R/F] - Noise Intensity +=: {scanLines.NoiseIntensity}", x, Color.LimeGreen);
                        WriteLine($"[T/G] - Line Intensity +=: {scanLines.LineIntensity}", x, Color.LimeGreen);
                        WriteLine($"[Y/H] - Line Count +=: {scanLines.LineCount}", x, Color.LimeGreen);
                    }

                    if (selectedEffect == deRezed)
                    {
                        WriteLine($"[R/F] - Number Of Tiles +=: {deRezed.NumberOfTiles}", x, Color.LimeGreen);
                    }

                    if (selectedEffect == fxaa)
                    {
                        WriteLine($"[R/F] - Technique: {fxaa.TechniqueUsed}", x, Color.LimeGreen);
                        WriteLine($"[T/G] - Contrast Threshold +=: {fxaa.ContrastThreshold}", x, Color.LimeGreen);
                        WriteLine($"[Y/H] - Relative Threshold +=: {fxaa.RelativeThreshold}", x, Color.LimeGreen);
                        WriteLine($"[U/J] - Subpixel Blending +=: {fxaa.SubpixelBlending}", x, Color.LimeGreen);
                        WriteLine($"[I] - Split Screen: {fxaa.RenderHalfScreen}", x, Color.LimeGreen);
                        WriteLine($"[O/L] - Split At : {fxaa.Vline}", x, Color.LimeGreen);
                    }
                }
            }

            line = (lineCount + 2) * lineHeight;
            WriteLine("", 8, Color.Gold);
            WriteLine("Camera Controls: [WASD - Translate] [Arrow Keys - Rotate]", 8, Color.Gold);
            spriteBatch.End();
        }

        protected void WriteLine(string str,float x, Color color)
        {
            spriteBatch.DrawString(Content.Load<SpriteFont>("Fonts/hudFont"), str, new Vector2(x, line), color);
            line += lineHeight;
        }

        public void SaveJpg(Texture2D texture, string name)
        {
            FileStream s = new FileStream(name, FileMode.Create);
            texture.SaveAsJpeg(s, texture.Width, texture.Height);
            s.Close();
        }
    }
}
