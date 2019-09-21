using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using RandomchaosMGBase.BaseClasses;
using RandomchaosMGBase.InputManagers;

namespace RandomchaosMGLightAndShade
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;

        KeyboardStateManager kbm;
        Base3DCamera camera;

        Color AmbientColor = Color.CornflowerBlue;
        float AmbientIntensity = .5f;

        #region Light & Shade I
        LightAndShadeModel skullAmbient;
        LightAndShadeModel sphereAmbient;
        LightAndShadeModel teapotAmbient;

        LightAndShadeModel skullDiffuse;
        LightAndShadeModel sphereDiffuse;
        LightAndShadeModel teapotDiffuse;

        LightAndShadeModel skullAmbientDiffuse;
        LightAndShadeModel sphereAmbientDiffuse;
        LightAndShadeModel teapotAmbientDiffuse;

        LightModel light;
        #endregion

        #region Light & Shade II

        LightAndShadeModel skullAmbientDiffuseColorLight;
        LightAndShadeModel sphereAmbientDiffuseColorLight;
        LightAndShadeModel teapotAmbientDiffuseColorLight;

        LightAndShadeModel skullAmbientDiffuseColorMultiLight;
        LightAndShadeModel sphereAmbientDiffuseColorMultiLight;
        LightAndShadeModel teapotAmbientDiffuseColorMultiLight;

        LightAndShadeModel skullAmbientDiffuseColorBlinnPhongLight;
        LightAndShadeModel sphereAmbientDiffuseColorBlinnPhongLight;
        LightAndShadeModel teapotAmbientDiffuseColorBlinnPhongLight;

        List<LightModel> lights;

        int currentLightEdit = 0;
        #endregion

        #region Light & Shade III
        LightAndShadeModel colorPlanet;
        LightAndShadeModel colorGlowPlanet;
        LightAndShadeModel colorGlowBumpPlanet;
        LightAndShadeModel colorGlowBumpReflectPlanet;

        LightModel lightIII;
        #endregion

        #region Light & Shade V
        BaseSkyBox skyBox;

        LightAndShadeModel skullGlass;
        LightAndShadeModel sphereGlass;
        LightAndShadeModel teapotGlass;

        LightAndShadeModel skullGlassFrez;
        LightAndShadeModel sphereGlassFrez;
        LightAndShadeModel teapotGlassFrez;

        LightAndShadeModel skullGlassFrezFree;
        LightAndShadeModel sphereGlassFrezFree;
        LightAndShadeModel teapotGlassFrezFree;
        #endregion

        #region Light & Shade VI
        LightAndShadeModel skullReflect;
        LightAndShadeModel sphereReflect;
        LightAndShadeModel teapotReflect;

        LightAndShadeModel skullReflectRefract;
        LightAndShadeModel sphereReflectRefract;
        LightAndShadeModel teapotReflectRefract;

        LightAndShadeModel skullRefract;
        LightAndShadeModel sphereRefract;
        LightAndShadeModel teapotRefract;

        LightAndShadeModel skullRC3DGlass;
        LightAndShadeModel sphereRC3DGlass;
        LightAndShadeModel teapotRC3DGlass;
        #endregion

        #region Light & Shade VIII
        LightModel lightVIII;

        LightAndShadeModel earth;
        #endregion


        List<bool> ShowLightAndShader = new List<bool>() { false, false, false, false, false, false, false, false };

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.GraphicsProfile = GraphicsProfile.HiDef;

            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;

            IsMouseVisible = true;


            float dist = -25f;

            kbm = new KeyboardStateManager(this);

            camera = new Base3DCamera(this, .5f, 20000);
            camera.Position = new Vector3(0, 0, 0);
            Components.Add(camera);
            Services.AddService(typeof(Base3DCamera), camera);


            #region Light And Shade I
            light = new LightModel(this, Color.White);
            light.Position = new Vector3(7, 7, dist);
            Components.Add(light);

            ShowLightAndShader[0] = true;

            // Ambient Only
            skullAmbient = new LightAndShadeModel(this, "Models/skullocc", "Shaders/AmbientLight");
            skullAmbient.Scale = Vector3.One * .5f;
            skullAmbient.AmbientColor = AmbientColor;
            skullAmbient.AmbientIntensity = AmbientIntensity;
            skullAmbient.Position = new Vector3(0, 5, dist);
            Components.Add(skullAmbient);

            sphereAmbient = new LightAndShadeModel(this, "Models/sphere", "Shaders/AmbientLight");
            sphereAmbient.AmbientColor = AmbientColor;
            sphereAmbient.AmbientIntensity = AmbientIntensity;
            sphereAmbient.Position = new Vector3(5, 5, dist);
            Components.Add(sphereAmbient);

            teapotAmbient = new LightAndShadeModel(this, "Models/teapot", "Shaders/AmbientLight");
            teapotAmbient.AmbientColor = AmbientColor;
            teapotAmbient.AmbientIntensity = AmbientIntensity;
            teapotAmbient.Position = new Vector3(-5, 5, dist);
            Components.Add(teapotAmbient);

            // Diffuse Only
            skullDiffuse = new LightAndShadeModel(this, "Models/skullocc", "Shaders/DiffuseLight");
            skullDiffuse.Scale = Vector3.One * .5f;
            skullDiffuse.SetLightParams(0, light);
            skullDiffuse.Position = new Vector3(0, 0, dist);
            Components.Add(skullDiffuse);

            sphereDiffuse = new LightAndShadeModel(this, "Models/sphere", "Shaders/DiffuseLight");
            sphereDiffuse.SetLightParams(0, light);
            sphereDiffuse.Position = new Vector3(5, 0, dist);
            Components.Add(sphereDiffuse);

            teapotDiffuse = new LightAndShadeModel(this, "Models/teapot", "Shaders/DiffuseLight");
            teapotDiffuse.SetLightParams(0, light);
            teapotDiffuse.Position = new Vector3(-5, 0, dist);
            Components.Add(teapotDiffuse);

            // Ambitne & Diffuse
            skullAmbientDiffuse = new LightAndShadeModel(this, "Models/skullocc", "Shaders/AmbientDiffuseLight");
            skullAmbientDiffuse.Scale = Vector3.One * .5f;
            skullAmbientDiffuse.SetLightParams(0, light);
            skullAmbientDiffuse.AmbientColor = AmbientColor;
            skullAmbientDiffuse.AmbientIntensity = AmbientIntensity;
            skullAmbientDiffuse.Position = new Vector3(0, -5, dist);
            Components.Add(skullAmbientDiffuse);

            sphereAmbientDiffuse = new LightAndShadeModel(this, "Models/sphere", "Shaders/AmbientDiffuseLight");
            sphereAmbientDiffuse.AmbientColor = AmbientColor;
            sphereAmbientDiffuse.SetLightParams(0, light);
            sphereAmbientDiffuse.AmbientIntensity = AmbientIntensity;
            sphereAmbientDiffuse.Position = new Vector3(5, -5, dist);
            Components.Add(sphereAmbientDiffuse);

            teapotAmbientDiffuse = new LightAndShadeModel(this, "Models/teapot", "Shaders/AmbientDiffuseLight");
            teapotAmbientDiffuse.AmbientColor = AmbientColor;
            teapotAmbientDiffuse.SetLightParams(0, light);
            teapotAmbientDiffuse.AmbientIntensity = AmbientIntensity;
            teapotAmbientDiffuse.Position = new Vector3(-5, -5, dist);
            Components.Add(teapotAmbientDiffuse);
            #endregion

            #region Light & Shade II

            ShowLightAndShader[1] = false;
            // Colored Lights
            lights = new List<LightModel>();

            for (int l = 0; l < 3; l++)
            {
                LightModel lm = null;
                switch (l)
                {
                    case 0:
                        lm = new LightModel(this, Color.Gold, Color.White);
                        lm.Position = new Vector3(7, 7, dist);
                        break;
                    case 1:
                        lm = new LightModel(this, Color.Red, Color.White);
                        lm.Position = new Vector3(-7, 7, dist);
                        break;
                    case 2:
                        lm = new LightModel(this, Color.Blue, Color.White);
                        lm.Position = new Vector3(-7, -7, dist);
                        break;
                }

                lights.Add(lm);
                Components.Add(lm);
            }

            // Diffuse Colored
            skullAmbientDiffuseColorLight = new LightAndShadeModel(this, "Models/skullocc", "Shaders/AmbientDiffuseColoredLight");
            skullAmbientDiffuseColorLight.Scale = Vector3.One * .5f;
            skullAmbientDiffuseColorLight.AmbientColor = AmbientColor;
            skullAmbientDiffuseColorLight.AmbientIntensity = AmbientIntensity;
            skullAmbientDiffuseColorLight.Position = new Vector3(0, 5, dist);
            skullAmbientDiffuseColorLight.SetLightParams(0, lights[0]);
            Components.Add(skullAmbientDiffuseColorLight);

            sphereAmbientDiffuseColorLight = new LightAndShadeModel(this, "Models/sphere", "Shaders/AmbientDiffuseColoredLight");
            sphereAmbientDiffuseColorLight.AmbientColor = AmbientColor;
            sphereAmbientDiffuseColorLight.AmbientIntensity = AmbientIntensity;
            sphereAmbientDiffuseColorLight.Position = new Vector3(5, 5, dist);
            sphereAmbientDiffuseColorLight.SetLightParams(0, lights[0]);
            Components.Add(sphereAmbientDiffuseColorLight);

            teapotAmbientDiffuseColorLight = new LightAndShadeModel(this, "Models/teapot", "Shaders/AmbientDiffuseColoredLight");
            teapotAmbientDiffuseColorLight.AmbientColor = AmbientColor;
            teapotAmbientDiffuseColorLight.AmbientIntensity = AmbientIntensity;
            teapotAmbientDiffuseColorLight.Position = new Vector3(-5, 5, dist);
            teapotAmbientDiffuseColorLight.SetLightParams(0, lights[0]);
            Components.Add(teapotAmbientDiffuseColorLight);

            // MultiLight
            skullAmbientDiffuseColorMultiLight = new LightAndShadeModel(this, "Models/skullocc", "Shaders/ADCMultiLight");
            skullAmbientDiffuseColorMultiLight.MultiLight = true;
            skullAmbientDiffuseColorMultiLight.Scale = Vector3.One * .5f;
            skullAmbientDiffuseColorMultiLight.AmbientColor = AmbientColor;
            skullAmbientDiffuseColorMultiLight.AmbientIntensity = AmbientIntensity;
            skullAmbientDiffuseColorMultiLight.Position = new Vector3(0, 0, dist);
            Components.Add(skullAmbientDiffuseColorMultiLight);

            sphereAmbientDiffuseColorMultiLight = new LightAndShadeModel(this, "Models/sphere", "Shaders/ADCMultiLight");
            sphereAmbientDiffuseColorMultiLight.MultiLight = true;
            sphereAmbientDiffuseColorMultiLight.AmbientColor = AmbientColor;
            sphereAmbientDiffuseColorMultiLight.AmbientIntensity = AmbientIntensity;
            sphereAmbientDiffuseColorMultiLight.Position = new Vector3(5, 0, dist);
            Components.Add(sphereAmbientDiffuseColorMultiLight);

            teapotAmbientDiffuseColorMultiLight = new LightAndShadeModel(this, "Models/teapot", "Shaders/ADCMultiLight");
            teapotAmbientDiffuseColorMultiLight.MultiLight = true;
            teapotAmbientDiffuseColorMultiLight.AmbientColor = AmbientColor;
            teapotAmbientDiffuseColorMultiLight.AmbientIntensity = AmbientIntensity;
            teapotAmbientDiffuseColorMultiLight.Position = new Vector3(-5, 0, dist);
            Components.Add(teapotAmbientDiffuseColorMultiLight);

            // Light with specualr Blinn-Phong
            skullAmbientDiffuseColorBlinnPhongLight = new LightAndShadeModel(this, "Models/skullocc", "Shaders/BlinnPhong");
            skullAmbientDiffuseColorBlinnPhongLight.MultiLight = false;
            skullAmbientDiffuseColorBlinnPhongLight.Scale = Vector3.One * .5f;
            skullAmbientDiffuseColorBlinnPhongLight.AmbientColor = AmbientColor;
            skullAmbientDiffuseColorBlinnPhongLight.AmbientIntensity = AmbientIntensity;
            skullAmbientDiffuseColorBlinnPhongLight.Position = new Vector3(0, -5, dist);
            Components.Add(skullAmbientDiffuseColorBlinnPhongLight);

            sphereAmbientDiffuseColorBlinnPhongLight = new LightAndShadeModel(this, "Models/sphere", "Shaders/BlinnPhong");
            sphereAmbientDiffuseColorBlinnPhongLight.MultiLight = false;
            sphereAmbientDiffuseColorBlinnPhongLight.AmbientColor = AmbientColor;
            sphereAmbientDiffuseColorBlinnPhongLight.AmbientIntensity = AmbientIntensity;
            sphereAmbientDiffuseColorBlinnPhongLight.Position = new Vector3(5, -5, dist);
            Components.Add(sphereAmbientDiffuseColorBlinnPhongLight);

            teapotAmbientDiffuseColorBlinnPhongLight = new LightAndShadeModel(this, "Models/teapot", "Shaders/BlinnPhong");
            teapotAmbientDiffuseColorBlinnPhongLight.MultiLight = false;
            teapotAmbientDiffuseColorBlinnPhongLight.AmbientColor = AmbientColor;
            teapotAmbientDiffuseColorBlinnPhongLight.AmbientIntensity = AmbientIntensity;
            teapotAmbientDiffuseColorBlinnPhongLight.Position = new Vector3(-5, -5, dist);
            Components.Add(teapotAmbientDiffuseColorBlinnPhongLight);

            skullAmbientDiffuseColorBlinnPhongLight.SetLightParams(0, lights[0]);
            sphereAmbientDiffuseColorBlinnPhongLight.SetLightParams(0, lights[0]);
            teapotAmbientDiffuseColorBlinnPhongLight.SetLightParams(0, lights[0]);

            for (int l = 0; l < lights.Count; l++)
            {
                skullAmbientDiffuseColorMultiLight.SetLightParams(l, lights[l]);
                sphereAmbientDiffuseColorMultiLight.SetLightParams(l, lights[l]);
                teapotAmbientDiffuseColorMultiLight.SetLightParams(l, lights[l]);

                
            }
            #endregion

            #region Light & Shade III
            lightIII = new LightModel(this, Color.Gold);
            lightIII.Position = new Vector3(10, 0, 0);
            Components.Add(lightIII);

            ShowLightAndShader[2] = false;

            colorPlanet = new LightAndShadeModel(this, "Models/sphere", "Shaders/ColorMap", "Textures/Earth_Diffuse");
            colorPlanet.Position = new Vector3(-2, 0, -5);
            colorPlanet.SetLightParams(0, lightIII);
            Components.Add(colorPlanet);

            colorGlowPlanet = new LightAndShadeModel(this, "Models/sphere", "Shaders/ColorGlowMap", "Textures/Earth_Diffuse", "Textures/Earth_Night");
            colorGlowPlanet.Position = new Vector3(0, 0, -7);
            colorGlowPlanet.SetLightParams(0, lightIII);
            Components.Add(colorGlowPlanet);

            colorGlowBumpPlanet = new LightAndShadeModel(this, "Models/sphere", "Shaders/ColorGlowBumpMap", "Textures/Earth_Diffuse", "Textures/Earth_Night", "Textures/Earth_NormalMap");
            colorGlowBumpPlanet.Position = new Vector3(2, 0, -9);
            colorGlowBumpPlanet.SetLightParams(0, lightIII);
            Components.Add(colorGlowBumpPlanet);

            colorGlowBumpReflectPlanet = new LightAndShadeModel(this, "Models/sphere", "Shaders/ColorGlowBumpReflect", "Textures/Earth_Diffuse", "Textures/Earth_Night", "Textures/Earth_NormalMap", "Textures/Earth_ReflectionMask");
            colorGlowBumpReflectPlanet.Position = new Vector3(4, 0, -11);
            colorGlowBumpReflectPlanet.SetLightParams(0, lightIII);
            Components.Add(colorGlowBumpReflectPlanet);

            #endregion


            #region Light & Shade V
            ShowLightAndShader[3] = false;
            skyBox = new BaseSkyBox(this, "Textures/cubemap");
            Components.Add(skyBox);

            skullGlass = new LightAndShadeModel(this, "Models/skullocc", "Shaders/glass1", "Textures/cubemap");
            skullGlass.Scale = Vector3.One * .5f;
            skullGlass.Tint = Color.White;
            skullGlass.Position = new Vector3(0, 5, dist);
            Components.Add(skullGlass);

            sphereGlass = new LightAndShadeModel(this, "Models/sphere", "shaders/glass1", "Textures/cubemap");
            sphereGlass.Tint = Color.White;
            sphereGlass.Position = new Vector3(-5, 5, dist);
            Components.Add(sphereGlass);

            teapotGlass = new LightAndShadeModel(this, "Models/teapot", "shaders/glass1", "Textures/cubemap");
            teapotGlass.Position = new Vector3(5, 5, dist);
            teapotGlass.Tint = Color.White;
            Components.Add(teapotGlass);

            skullGlassFrez = new LightAndShadeModel(this, "Models/skullocc", "shaders/glass2", "Textures/cubemap", "Textures/Fresnel");
            skullGlassFrez.Scale = Vector3.One * .5f;
            skullGlassFrez.Tint = Color.White;
            skullGlassFrez.Position = new Vector3(0, 0, dist);
            Components.Add(skullGlassFrez);

            sphereGlassFrez = new LightAndShadeModel(this, "Models/sphere", "shaders/glass2", "Textures/cubemap", "Textures/Fresnel");
            sphereGlassFrez.Position = new Vector3(-5, 0, dist);
            sphereGlassFrez.Tint = Color.White;
            Components.Add(sphereGlassFrez);

            teapotGlassFrez = new LightAndShadeModel(this, "Models/teapot", "shaders/glass2", "Textures/cubemap", "Textures/Fresnel");
            teapotGlassFrez.Position = new Vector3(5, 0, dist);
            teapotGlassFrez.Tint = Color.White;
            Components.Add(teapotGlassFrez);

            skullGlassFrezFree = new LightAndShadeModel(this, "Models/skullocc", "shaders/glass3", "Textures/cubemap");
            skullGlassFrezFree.Scale = Vector3.One * .5f;
            skullGlassFrezFree.Tint = Color.White;
            skullGlassFrezFree.Position = new Vector3(0, -5, dist);
            Components.Add(skullGlassFrezFree);

            sphereGlassFrezFree = new LightAndShadeModel(this, "Models/sphere", "shaders/glass3", "Textures/cubemap");
            sphereGlassFrezFree.Position = new Vector3(-5, -5, dist);
            sphereGlassFrezFree.Tint = Color.White;
            Components.Add(sphereGlassFrezFree);

            teapotGlassFrezFree = new LightAndShadeModel(this, "Models/teapot", "shaders/glass3", "Textures/cubemap");
            teapotGlassFrezFree.Position = new Vector3(5, -5, dist);
            teapotGlassFrezFree.Tint = Color.White;
            Components.Add(teapotGlassFrezFree);

            #endregion

            #region Light & Shade VI
            ShowLightAndShader[4] = false;
            skullRefract = new LightAndShadeModel(this, "Models/skullocc", "Shaders/Refraction", "Textures/cubemap");
            skullRefract.Scale = Vector3.One * .5f;
            skullRefract.Position = new Vector3(0, 5, dist);
            Components.Add(skullRefract);

            sphereRefract = new LightAndShadeModel(this, "Models/sphere", "Shaders/Refraction", "Textures/cubemap");
            sphereRefract.Position = new Vector3(-5, 5, dist);
            Components.Add(sphereRefract);

            teapotRefract = new LightAndShadeModel(this, "Models/teapot", "Shaders/Refraction", "Textures/cubemap");
            teapotRefract.Position = new Vector3(5, 5, dist);
            Components.Add(teapotRefract);

            skullReflect = new LightAndShadeModel(this, "Models/skullocc", "Shaders/Reflection", "Textures/cubemap");
            skullReflect.Scale = Vector3.One * .5f;
            skullReflect.Position = new Vector3(0, 0, dist);
            Components.Add(skullReflect);

            sphereReflect = new LightAndShadeModel(this, "Models/sphere", "Shaders/Reflection", "Textures/cubemap");
            sphereReflect.Position = new Vector3(-5, 0, dist);
            Components.Add(sphereReflect);

            teapotReflect = new LightAndShadeModel(this, "Models/teapot", "Shaders/Reflection", "Textures/cubemap");
            teapotReflect.Position = new Vector3(5, 0, dist);
            Components.Add(teapotReflect);

            skullReflectRefract = new LightAndShadeModel(this, "Models/skullocc", "Shaders/ReflectionAndRefraction", "Textures/cubemap");
            skullReflectRefract.Scale = Vector3.One * .5f;
            skullReflectRefract.Position = new Vector3(0, -5, dist);
            Components.Add(skullReflectRefract);

            sphereReflectRefract = new LightAndShadeModel(this, "Models/sphere", "Shaders/ReflectionAndRefraction", "Textures/cubemap");
            sphereReflectRefract.Position = new Vector3(-5, -5, dist);
            Components.Add(sphereReflectRefract);

            teapotReflectRefract = new LightAndShadeModel(this, "Models/teapot", "Shaders/ReflectionAndRefraction", "Textures/cubemap");
            teapotReflectRefract.Position = new Vector3(5, -5, dist);
            Components.Add(teapotReflectRefract);

            skullRC3DGlass = new LightAndShadeModel(this, "Models/skullocc", "Shaders/RC3DGlass", "Textures/cubemap");
            skullRC3DGlass.Scale = Vector3.One * .5f;
            skullRC3DGlass.Position = new Vector3(0, -10, dist);
            Components.Add(skullRC3DGlass);

            sphereRC3DGlass = new LightAndShadeModel(this, "Models/sphere", "Shaders/RC3DGlass", "Textures/cubemap");
            sphereRC3DGlass.Position = new Vector3(-5, -10, dist);
            Components.Add(sphereRC3DGlass);

            teapotRC3DGlass = new LightAndShadeModel(this, "Models/teapot", "Shaders/RC3DGlass", "Textures/cubemap");
            teapotRC3DGlass.Position = new Vector3(5, -10, dist);
            Components.Add(teapotRC3DGlass);
            #endregion

            #region Light & Shade VIII
            ShowLightAndShader[5] = false;

            lightVIII = new LightModel(this, Color.White);
            lightVIII.Position = Vector3.Right * 10;
            Components.Add(lightVIII);

            earth = new LightAndShadeModel(this, "Models/sphere", "Shaders/PlanetShader", "Textures/Earth_Diffuse", "Textures/Earth_Night", "Textures/Earth_NormalMap", "Textures/Earth_ReflectionMask", "Textures/Earth_Cloud", "Textures/WaterRipples", "Textures/Earth_Atmos");
            earth.Position = new Vector3(0, 0, -50);
            earth.SetLightParams(0, lightVIII);
            earth.Scale = Vector3.One * 10f;
            Components.Add(earth);
            #endregion
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

            font = Content.Load<SpriteFont>("Fonts/font");
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

            #region Light & Shade I
            if (ShowLightAndShader[0])
            {
                // Light controls
                if (kbm.KeyDown(Keys.I) || GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed)
                    light.Position += Vector3.Forward;
                if (kbm.KeyDown(Keys.K) || GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed)
                    light.Position += Vector3.Backward;
                if (kbm.KeyDown(Keys.J) || GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed)
                    light.Position += Vector3.Left;
                if (kbm.KeyDown(Keys.L) || GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed)
                    light.Position += Vector3.Right;
                if (kbm.KeyDown(Keys.U) || GamePad.GetState(PlayerIndex.One).Buttons.RightShoulder == ButtonState.Pressed)
                    light.Position += Vector3.Up;
                if (kbm.KeyDown(Keys.O) || GamePad.GetState(PlayerIndex.One).Buttons.LeftShoulder == ButtonState.Pressed)
                    light.Position += Vector3.Down;
            }
            #endregion
            #region Light & Shade II
            else if (ShowLightAndShader[1])
            {
                if (kbm.KeyDown(Keys.I) || GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed)
                    lights[currentLightEdit].Position += Vector3.Forward;
                if (kbm.KeyDown(Keys.K) || GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed)
                    lights[currentLightEdit].Position += Vector3.Backward;
                if (kbm.KeyDown(Keys.J) || GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed)
                    lights[currentLightEdit].Position += Vector3.Left;
                if (kbm.KeyDown(Keys.L) || GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed)
                    lights[currentLightEdit].Position += Vector3.Right;
                if (kbm.KeyDown(Keys.U) || GamePad.GetState(PlayerIndex.One).Buttons.RightShoulder == ButtonState.Pressed)
                    lights[currentLightEdit].Position += Vector3.Up;
                if (kbm.KeyDown(Keys.O) || GamePad.GetState(PlayerIndex.One).Buttons.LeftShoulder == ButtonState.Pressed)
                    lights[currentLightEdit].Position += Vector3.Down;

                if (kbm.KeyDown(Keys.F3) || (GamePad.GetState(PlayerIndex.One).Triggers.Left > 0 && GamePad.GetState(PlayerIndex.One).Buttons.LeftStick == ButtonState.Pressed))
                    lights[currentLightEdit].LightIntensity -= .01f;
                if (kbm.KeyDown(Keys.F4) || (GamePad.GetState(PlayerIndex.One).Triggers.Right > 0 && GamePad.GetState(PlayerIndex.One).Buttons.LeftStick == ButtonState.Pressed))
                    lights[currentLightEdit].LightIntensity += .01f;

                if (kbm.KeyDown(Keys.F5) || (GamePad.GetState(PlayerIndex.One).Triggers.Left > 0 && GamePad.GetState(PlayerIndex.One).Buttons.LeftStick == ButtonState.Pressed))
                    lights[currentLightEdit].SpecularIntensity -= .01f;
                if (kbm.KeyDown(Keys.F6) || (GamePad.GetState(PlayerIndex.One).Triggers.Right > 0 && GamePad.GetState(PlayerIndex.One).Buttons.LeftStick == ButtonState.Pressed))
                    lights[currentLightEdit].SpecularIntensity += .01f;

                lights[currentLightEdit].LightIntensity = MathHelper.Clamp(lights[currentLightEdit].LightIntensity, 0, 1);
                lights[currentLightEdit].SpecularIntensity = MathHelper.Clamp(lights[currentLightEdit].SpecularIntensity, 0, 1);

                if (kbm.KeyDown(Keys.Space))
                {
                    currentLightEdit++;

                    if (currentLightEdit > 2)
                        currentLightEdit = 0;
                }
            }
            #endregion
            #region Light & Shade III
            else if (ShowLightAndShader[5])
            {
                if (kbm.KeyDown(Keys.I) || GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed)
                    lightVIII.Position += Vector3.Forward;
                if (kbm.KeyDown(Keys.K) || GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed)
                    lightVIII.Position += Vector3.Backward;
                if (kbm.KeyDown(Keys.J) || GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed)
                    lightVIII.Position += Vector3.Left;
                if (kbm.KeyDown(Keys.L) || GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed)
                    lightVIII.Position += Vector3.Right;
                if (kbm.KeyDown(Keys.U) || GamePad.GetState(PlayerIndex.One).Buttons.RightShoulder == ButtonState.Pressed)
                    lightVIII.Position += Vector3.Up;
                if (kbm.KeyDown(Keys.O) || GamePad.GetState(PlayerIndex.One).Buttons.LeftShoulder == ButtonState.Pressed)
                    lightVIII.Position += Vector3.Down;
            }
            #endregion

            if (kbm.KeyPress(Keys.D1))
                SetLightAndShade(0);

            if (kbm.KeyPress(Keys.D2))
                SetLightAndShade(1);

            if (kbm.KeyPress(Keys.D3))
                SetLightAndShade(2);

            if (kbm.KeyPress(Keys.D4))
                SetLightAndShade(3);

            if (kbm.KeyPress(Keys.D5))
                SetLightAndShade(4);

            if (kbm.KeyPress(Keys.D6))
                SetLightAndShade(5);


            // Ambient Levels
            if (kbm.KeyDown(Keys.F1) || GamePad.GetState(PlayerIndex.One).Triggers.Left > 0)
                AmbientIntensity -= .01f;
            if (kbm.KeyDown(Keys.F2) || GamePad.GetState(PlayerIndex.One).Triggers.Right > 0)
                AmbientIntensity += .01f;

            AmbientIntensity = MathHelper.Clamp(AmbientIntensity, 0, 1);

            skullAmbient.AmbientIntensity = AmbientIntensity;
            sphereAmbient.AmbientIntensity = AmbientIntensity;
            teapotAmbient.AmbientIntensity = AmbientIntensity;

            skullAmbientDiffuse.AmbientIntensity = AmbientIntensity;
            sphereAmbientDiffuse.AmbientIntensity = AmbientIntensity;
            teapotAmbientDiffuse.AmbientIntensity = AmbientIntensity;

            skullAmbientDiffuse.LightPosition = light.Position;
            sphereAmbientDiffuse.LightPosition = light.Position;
            teapotAmbientDiffuse.LightPosition = light.Position;

            skullDiffuse.LightPosition = light.Position;
            sphereDiffuse.LightPosition = light.Position;
            teapotDiffuse.LightPosition = light.Position;

            skullAmbient.Enabled = ShowLightAndShader[0];
            sphereAmbient.Enabled = ShowLightAndShader[0];
            teapotAmbient.Enabled = ShowLightAndShader[0];
            skullDiffuse.Enabled = ShowLightAndShader[0];
            sphereDiffuse.Enabled = ShowLightAndShader[0];
            teapotDiffuse.Enabled = ShowLightAndShader[0];
            skullAmbientDiffuse.Enabled = ShowLightAndShader[0];
            sphereAmbientDiffuse.Enabled = ShowLightAndShader[0];
            teapotAmbientDiffuse.Enabled = ShowLightAndShader[0];

            light.Enabled = ShowLightAndShader[0];

            skullAmbientDiffuseColorLight.AmbientColor = AmbientColor;
            skullAmbientDiffuseColorLight.AmbientIntensity = AmbientIntensity;
            skullAmbientDiffuseColorLight.Enabled = ShowLightAndShader[1];

            sphereAmbientDiffuseColorLight.AmbientColor = AmbientColor;
            sphereAmbientDiffuseColorLight.AmbientIntensity = AmbientIntensity;
            sphereAmbientDiffuseColorLight.Enabled = ShowLightAndShader[1];

            teapotAmbientDiffuseColorLight.AmbientColor = AmbientColor;
            teapotAmbientDiffuseColorLight.AmbientIntensity = AmbientIntensity;
            teapotAmbientDiffuseColorLight.Enabled = ShowLightAndShader[1];
            
            skullAmbientDiffuseColorMultiLight.AmbientColor = AmbientColor;
            skullAmbientDiffuseColorMultiLight.AmbientIntensity = AmbientIntensity;
            skullAmbientDiffuseColorMultiLight.Enabled = ShowLightAndShader[1];

            sphereAmbientDiffuseColorMultiLight.AmbientColor = AmbientColor;
            sphereAmbientDiffuseColorMultiLight.AmbientIntensity = AmbientIntensity;
            sphereAmbientDiffuseColorMultiLight.Enabled = ShowLightAndShader[1];

            teapotAmbientDiffuseColorMultiLight.AmbientColor = AmbientColor;
            teapotAmbientDiffuseColorMultiLight.AmbientIntensity = AmbientIntensity;
            teapotAmbientDiffuseColorMultiLight.Enabled = ShowLightAndShader[1];

            skullAmbientDiffuseColorBlinnPhongLight.AmbientColor = AmbientColor;
            skullAmbientDiffuseColorBlinnPhongLight.AmbientIntensity = AmbientIntensity;
            skullAmbientDiffuseColorBlinnPhongLight.Enabled = ShowLightAndShader[1];

            sphereAmbientDiffuseColorBlinnPhongLight.AmbientColor = AmbientColor;
            sphereAmbientDiffuseColorBlinnPhongLight.AmbientIntensity = AmbientIntensity;
            sphereAmbientDiffuseColorBlinnPhongLight.Enabled = ShowLightAndShader[1];

            teapotAmbientDiffuseColorBlinnPhongLight.AmbientColor = AmbientColor;
            teapotAmbientDiffuseColorBlinnPhongLight.AmbientIntensity = AmbientIntensity;
            teapotAmbientDiffuseColorBlinnPhongLight.Enabled = ShowLightAndShader[1];

            for (int l = 0; l < lights.Count; l++)
            {
                lights[l].Enabled = ShowLightAndShader[1];
            }

            float modelRotSpeed = .001f;
            colorPlanet.Rotate(Vector3.Down, modelRotSpeed);
            colorGlowPlanet.Rotate(Vector3.Down, modelRotSpeed);
            colorGlowBumpPlanet.Rotate(Vector3.Down, modelRotSpeed);
            colorGlowBumpReflectPlanet.Rotate(Vector3.Down, modelRotSpeed);
            earth.Rotate(Vector3.Down, modelRotSpeed);

            colorPlanet.Enabled = ShowLightAndShader[2];
            colorGlowPlanet.Enabled = ShowLightAndShader[2];
            colorGlowBumpPlanet.Enabled = ShowLightAndShader[2];
            colorGlowBumpReflectPlanet.Enabled = ShowLightAndShader[2];

            lightIII.Enabled = ShowLightAndShader[2];

            skyBox.Enabled = ShowLightAndShader[3] || ShowLightAndShader[4];

            skullGlass.Enabled = ShowLightAndShader[3];
            sphereGlass.Enabled = ShowLightAndShader[3];
            teapotGlass.Enabled = ShowLightAndShader[3];

            skullGlassFrez.Enabled = ShowLightAndShader[3];
            sphereGlassFrez.Enabled = ShowLightAndShader[3];
            teapotGlassFrez.Enabled = ShowLightAndShader[3];

            skullGlassFrezFree.Enabled = ShowLightAndShader[3];
            sphereGlassFrezFree.Enabled = ShowLightAndShader[3];
            teapotGlassFrezFree.Enabled = ShowLightAndShader[3];

            skullRefract.Enabled = ShowLightAndShader[4];
            sphereRefract.Enabled = ShowLightAndShader[4];
            teapotRefract.Enabled = ShowLightAndShader[4];

            skullReflect.Enabled = ShowLightAndShader[4];
            sphereReflect.Enabled = ShowLightAndShader[4];
            teapotReflect.Enabled = ShowLightAndShader[4];

            skullReflectRefract.Enabled = ShowLightAndShader[4];
            sphereReflectRefract.Enabled = ShowLightAndShader[4];
            teapotReflectRefract.Enabled = ShowLightAndShader[4];

            skullRC3DGlass.Enabled = ShowLightAndShader[4];
            sphereRC3DGlass.Enabled = ShowLightAndShader[4];
            teapotRC3DGlass.Enabled = ShowLightAndShader[4];

            earth.Enabled = ShowLightAndShader[5];

        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if(ShowLightAndShader[2] || ShowLightAndShader[5])
                GraphicsDevice.Clear(Color.Black);
            else
                GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);

            lines = 0;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            WriteLine("WASD - Translate Camera", Color.Gold);
            WriteLine("Arrow Keys - Rotate Camera", Color.Gold);
            if (ShowLightAndShader[0])
            {
                WriteLine("IJKLUO - Translate Light", Color.Gold);
                WriteLine("F1/F2 - Alter Ambient Intensity", Color.Gold);
                WriteLine("Esc - Exit", Color.Gold);
                WriteLine($"Light Position: {light.Position}", Color.Gold);
                WriteLine($"Lignt Intensity: {AmbientIntensity}", Color.Gold);
            }
            else if (ShowLightAndShader[1])
            {
                WriteLine("IJKLUO - Translate Light", Color.Gold);
                WriteLine("F1/F2 - Alter Ambient Intensity", Color.Gold);
                WriteLine("F3/F4 - Alter Diffuse Intensity", Color.Gold);
                WriteLine("F5/F6 - Alter Specular Intensity", Color.Gold);
                WriteLine("Space - Alter Current Light", Color.Gold);
                WriteLine("Esc - Exit", Color.Gold);

                WriteLine($"Light Position: {lights[currentLightEdit].Position}", Color.Gold);
                WriteLine($"Ambient Light Intensity: {AmbientIntensity}", Color.Gold);
                WriteLine($"Diffuse Light Intensity: {lights[currentLightEdit].LightIntensity}", Color.Gold);
                WriteLine($"Specular Intensity: {lights[currentLightEdit].SpecularIntensity}", Color.Gold);
                WriteLine($"Current Light: {(currentLightEdit + 1)}", Color.Gold);
            }
            else if (ShowLightAndShader[5])
            {
                WriteLine("Translate Light: I,K,J,L,U,O", Color.Gold);
                WriteLine("Translate Camera: W,A,S,D", Color.Gold);
            }
            WriteLine("---------------------------------------", Color.Gold);
            WriteLine($"D1 - Toggle Light & Shade I {ShowLightAndShader[0]}", Color.Gold);
            WriteLine($"D2 - Toggle Light & Shade II {ShowLightAndShader[1]}", Color.Gold);
            WriteLine($"D3 - Toggle Light & Shade III {ShowLightAndShader[2]}", Color.Gold);
            WriteLine($"D4 - Toggle Light & Shade V {ShowLightAndShader[3]}", Color.Gold);
            WriteLine($"D5 - Toggle Light & Shade VI {ShowLightAndShader[4]}", Color.Gold);
            WriteLine($"D6 - Toggle Light & Shade VIII {ShowLightAndShader[5]}", Color.Gold);
            spriteBatch.End();
        }

        int lines = 0;
        void WriteLine(string msg, Color color)
        {
            spriteBatch.DrawString(font, msg, new Vector2(5, font.LineSpacing * lines), color);
            lines++;
        }

        void SetLightAndShade(int idx)
        {
            for (int l = 0; l < ShowLightAndShader.Count; l++)
            {
                ShowLightAndShader[l] = l == idx ? ShowLightAndShader[l] = !ShowLightAndShader[l] : false;
            }
        }
    }
}
