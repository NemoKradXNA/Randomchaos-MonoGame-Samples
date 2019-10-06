using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using RandomchaosMGBase.BaseClasses;
using RandomchaosMGBase.InputManagers;
using RandomchaosMGDeferredLighting.Lights;
using RandomchaosMGBase.BaseClasses.PostProcessing;
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

        protected Effect shadowMapEffect;

        /// <summary>
        /// Deferred effect.
        /// </summary>
        protected Effect deferredSceneRenderEffect;

        /// <summary>
        /// Light map used for deferred render
        /// </summary>
        protected RenderTarget2D lightMap;

        /// <summary>
        /// Effect used for glow mapping
        /// </summary>
        protected Effect glowMapToLightMapEffect;

        /// <summary>
        /// Deferred directional lighting effects
        /// </summary>
        protected Effect deferredDirectionalLightEffect;

        /// <summary>
        /// Deferred cone light effect.
        /// </summary>
        protected Effect deferredConeLightEffect;

        /// <summary>
        /// Deferred point light effect
        /// </summary>
        protected Effect deferredPointLightEffect;

        /// <summary>
        /// Modified for the deferred shadow mapping
        /// </summary>
        public float deferredDirectionalShadowMapMod = 0.000001f;
        public float deferredConeShadowMapMod = 0.0001f;

        /// <summary>
        /// Model used for point light volumes
        /// </summary>
        protected Model pointLightVolumeModel;

        /// <summary>
        /// Quad used for deferred lighting
        /// </summary>
        protected ScreenQuad sceneQuad;

        protected Vector2[] taps = new Vector2[]{
                    new Vector2(-0.326212f,-0.40581f),new Vector2(-0.840144f,-0.07358f),
                    new Vector2(-0.695914f,0.457137f),new Vector2(-0.203345f,0.620716f),
                    new Vector2(0.96234f,-0.194983f),new Vector2(0.473434f,-0.480026f),
                    new Vector2(0.519456f,0.767022f),new Vector2(0.185461f,-0.893124f),
                    new Vector2(0.507431f,0.064425f),new Vector2(0.89642f,0.412458f),
                    new Vector2(-0.32194f,-0.932615f),new Vector2(-0.791559f,-0.59771f)};

        List<DeferredDirectionalLight> DirectionalLights = new List<DeferredDirectionalLight>();
        List<DeferredPointLight> PointLights = new List<DeferredPointLight>();
        List<DeferredConeLight> ConeLights = new List<DeferredConeLight>();

        List<BaseLight> ShadowLights = new List<BaseLight>();
        List<Base3DObject> ShadowCasters = new List<Base3DObject>();

        bool DebugShadowMaps = false;
        bool DebugLighting = true;

        protected DeferredLightingCamera camera;

        KeyboardStateManager kbm;
        PostProcessingManager PostProcessingManager;

        SpriteFont infoFont;
        SpriteFont debugFont;

        DeferredConeLight coneLight;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.GraphicsProfile = GraphicsProfile.HiDef;

            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;

            IsMouseVisible = true;

            kbm = new KeyboardStateManager(this);

            camera = new DeferredLightingCamera(this, .5f, 20000);
            camera.Position = new Vector3(0, 1, 0);
            Components.Add(camera);
            Services.AddService(typeof(Base3DCamera), camera);

            BaseSkyBox skyBox = new BaseSkyBox(this, "Textures/SkyBox/HMcubemap");
            skyBox.effectAsset = "Shaders/DeferredRender/SkyBoxShader";
            Components.Add(skyBox);

            Plane plane = new Plane(this, "Shaders/DeferredRender/DeferredModelRender");
            plane.ColorAsset = "Textures/brick";
            plane.BumpAsset = "Textures/brickNormal";
            plane.OcclusionAsset = "Textures./brickOcclusion";
            plane.SpecularAsset = "Textures/brickSpecular";
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
            sphere.ColorAsset = "Textures/sand";
            sphere.BumpAsset = "Textures/sandNormal";
            sphere.UVMultiplier = Vector2.One * 2;
            sphere.UVMultiplier = Vector2.One * 2;
            Components.Add(sphere);

            DeferredDirectionalLight directionalLight = new DeferredDirectionalLight(this, new Vector3(10, 10, 10), Color.AliceBlue, 1, true);
            directionalLight.Transform.LookAt(Vector3.Zero, 1, Vector3.Forward);
            Components.Add(directionalLight);
            DirectionalLights.Add(directionalLight);

            DeferredPointLight pointLight = new DeferredPointLight(this, new Vector3(0, 1, -20), Color.Red, 5, 1, false);
            Components.Add(pointLight);
            PointLights.Add(pointLight);

            coneLight = new DeferredConeLight(this, new Vector3(-10, 5, -25), Color.Gold, 1, MathHelper.ToRadians(45), 5, true);
            coneLight.Transform.LookAt(new Vector3(0, 0, -20), 1, Vector3.Forward);
            Components.Add(coneLight);
            ConeLights.Add(coneLight);

            DeferredConeLight coneLight2 = new DeferredConeLight(this, new Vector3(0, 20, -20), Color.Green, 1, MathHelper.ToRadians(25), 50, true);
            coneLight2.Transform.LookAt(new Vector3(0, 0, -20), 1, Vector3.Forward);
            Components.Add(coneLight2);
            ConeLights.Add(coneLight2);

            Cube brickBlock = new Cube(this, "Shaders/DeferredRender/DeferredModelRender");
            brickBlock.ColorAsset = "Textures/brick";
            brickBlock.BumpAsset = "Textures/brickNormal";
            brickBlock.OcclusionAsset = "Textures./brickOcclusion";
            brickBlock.SpecularAsset = "Textures/brickSpecular";
            brickBlock.Position = new Vector3(-3, 1.5f, -20);
            brickBlock.Rotate(Vector3.Up + Vector3.Forward + Vector3.Left, 15);
            
            Components.Add(brickBlock);

            // Shadow casters and receivers..
            AddShadowCasterReceiver(bunny);
            AddShadowCasterReceiver(cube);
            AddShadowCasterReceiver(sphere);
            AddShadowCasterReceiver(brickBlock);

            // Lights that cast shadows.
            ShadowLights.Add(directionalLight);
            ShadowLights.Add(coneLight);
            ShadowLights.Add(coneLight2);
        }

        #region All the deferred lighting functions, might mvoe this to it's onw class
        void AddShadowCasterReceiver(Base3DObject obj)
        {
            ShadowCasters.Add(obj);
        }

        /// <summary>
        /// Do shadow mapping for a light.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="light"></param>
        /// <param name="shadowCasters"></param>
        public void RenderLightShadows(GameTime gameTime, BaseLight light, List<Base3DObject> shadowCasters)
        {
            if (light.ShadowMap == null)
            {
                // the bigger the map the better :)
                try
                {
                    light.ShadowMap = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width * 5, GraphicsDevice.Viewport.Height * 5, false, SurfaceFormat.Single, DepthFormat.Depth24Stencil8);
                }
                catch
                {
                    light.ShadowMap = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width * 2, GraphicsDevice.Viewport.Height * 2, false, SurfaceFormat.Single, DepthFormat.Depth24Stencil8);
                }
            }

            // Clear shadow map..
            GraphicsDevice.SetRenderTarget(light.ShadowMap);
            GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
            GraphicsDevice.Clear(Color.Transparent);

            if (shadowMapEffect == null)
            {
                shadowMapEffect = Content.Load<Effect>("Shaders/ShadowMap");
            }

            shadowMapEffect.Parameters["vp"].SetValue(light.View * light.Projection);

            int cnt = shadowCasters.Count;

            for (int c = 0; c < cnt; c++)
            {
                shadowCasters[c].Draw(gameTime, shadowMapEffect);
            }

            GraphicsDevice.SetRenderTarget(null);
        }

        /// <summary>
        /// Method to do the deferred lighting draw call
        /// </summary>
        /// <param name="camera"></param>
        public void DrawDeferred(DeferredLightingCamera camera)
        {
            GraphicsDevice.Clear(camera.ClearColor);

            if (deferredSceneRenderEffect == null)
                deferredSceneRenderEffect = Content.Load<Effect>("Shaders/DeferredRender/DeferredSceneRender");

            deferredSceneRenderEffect.Parameters["colorMap"].SetValue(camera.RenderTarget);
            deferredSceneRenderEffect.Parameters["lightMap"].SetValue(lightMap);
            //deferredSceneRenderEffect.Parameters["sgrMap"].SetValue(camera.SpecularGlowReflectionMap);

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            deferredSceneRenderEffect.CurrentTechnique.Passes[0].Apply();

            if (sceneQuad == null)
            {
                sceneQuad = new ScreenQuad(this);
                sceneQuad.Initialize();
            }

            sceneQuad.Draw(-Vector2.One, Vector2.One);
        }

        public void DeferredLighting(GameTime gameTime, DeferredLightingCamera camera)
        {
            if (lightMap == null)
                lightMap = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);

            GraphicsDevice.SetRenderTarget(lightMap);

            GraphicsDevice.Clear(Color.Black);
            GraphicsDevice.BlendState = BlendState.Additive;

            //use the same operation on the alpha channel
            GraphicsDevice.DepthStencilState = DepthStencilState.None;
            GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;

            // Apply the glow value to the light map.
            ApplyGlowMap(camera);

            int lc = 0;

            lc = DirectionalLights.Count;
            for (int l = 0; l < lc; l++)
            {
                if (DirectionalLights[l].Intensity > 0 && DirectionalLights[l].Color != Color.Black)
                    RenderDirectionalLight(camera, DirectionalLights[l]);
            }

            lc = ConeLights.Count;
            for (int l = 0; l < lc; l++)
            {
                if (ConeLights[l].Intensity > 0 && ConeLights[l].Color != Color.Black)
                    RenderConeLight(camera, ConeLights[l]);
            }

            lc = PointLights.Count;
            for (int l = 0; l < lc; l++)
            {
                if (PointLights[l].Intensity > 0 && PointLights[l].Color != Color.Black)
                    RenderPointLight(camera, PointLights[l]);
            }

            GraphicsDevice.SetRenderTarget(null);
        }

        /// <summary>
        /// Render a point light
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="pointLight"></param>
        public void RenderPointLight(DeferredLightingCamera camera, DeferredPointLight pointLight)
        {
            if (deferredPointLightEffect == null)
                deferredPointLightEffect = Content.Load<Effect>("Shaders/DeferredRender/DeferredPointLight");

            if (pointLightVolumeModel == null)
                pointLightVolumeModel = Content.Load<Model>("Models/PointLightVolume");

            //set the G-Buffer parameters
            deferredPointLightEffect.Parameters["normalMap"].SetValue(camera.NormalBuffer);
            deferredPointLightEffect.Parameters["depthMap"].SetValue(camera.DepthBuffer);
            deferredPointLightEffect.Parameters["sgrMap"].SetValue(camera.SpecularGlowReflectionMap);

            //compute the light world matrix
            //scale according to light radius, and translate it to light position
            Matrix sphereWorldMatrix = Matrix.CreateScale(pointLight.Radius) * Matrix.CreateTranslation(pointLight.Transform.Position);

            //light position
            deferredPointLightEffect.Parameters["lightPosition"].SetValue(pointLight.Transform.Position);

            //set the color, radius and Intensity
            deferredPointLightEffect.Parameters["Color"].SetValue(pointLight.Color.ToVector3());
            deferredPointLightEffect.Parameters["lightRadius"].SetValue(pointLight.Radius);
            deferredPointLightEffect.Parameters["lightIntensity"].SetValue(pointLight.Intensity);

            //parameters for specular computations
            deferredPointLightEffect.Parameters["cameraPosition"].SetValue(camera.Transform.Position);
            deferredPointLightEffect.Parameters["InvertViewProjection"].SetValue(Matrix.Invert(camera.View * camera.Projection));

            Matrix[] transforms = new Matrix[pointLightVolumeModel.Bones.Count];
            pointLightVolumeModel.CopyAbsoluteBoneTransformsTo(transforms);
            Matrix meshWorld;

            int mmpc;
            for (int m = 0; m < pointLightVolumeModel.Meshes.Count; m++)
            {
                if (pointLightVolumeModel.Meshes[m].ParentBone != null)
                    meshWorld = transforms[pointLightVolumeModel.Meshes[m].ParentBone.Index] * sphereWorldMatrix;
                else
                    meshWorld = transforms[0] * sphereWorldMatrix;

                mmpc = pointLightVolumeModel.Meshes[m].MeshParts.Count;
                for (int mp = 0; mp < mmpc; mp++)
                {
                    deferredPointLightEffect.Parameters["wvp"].SetValue(meshWorld * camera.View * camera.Projection);

                    int pCnt = deferredPointLightEffect.CurrentTechnique.Passes.Count;
                    for (int p = 0; p < pCnt; p++)
                    {
                        deferredPointLightEffect.CurrentTechnique.Passes[p].Apply();

                        GraphicsDevice.SetVertexBuffer(pointLightVolumeModel.Meshes[m].MeshParts[mp].VertexBuffer);
                        GraphicsDevice.Indices = pointLightVolumeModel.Meshes[m].MeshParts[mp].IndexBuffer;
                        GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, pointLightVolumeModel.Meshes[m].MeshParts[mp].VertexOffset, 0, pointLightVolumeModel.Meshes[m].MeshParts[mp].PrimitiveCount);
                    }
                }
            }

        }

        /// <summary>
        /// Render a cone light
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="coneLight"></param>
        public void RenderConeLight(DeferredLightingCamera camera, DeferredConeLight coneLight)
        {
            if (deferredConeLightEffect == null)
                deferredConeLightEffect = Content.Load<Effect>("Shaders/DeferredRender/DeferredConeLight");

            // Load Light parameters
            deferredConeLightEffect.Parameters["lightDirection"].SetValue(coneLight.Direction);
            deferredConeLightEffect.Parameters["LightPosition"].SetValue(coneLight.Transform.Position);
            deferredConeLightEffect.Parameters["Color"].SetValue(coneLight.Color.ToVector3());

            deferredConeLightEffect.Parameters["ViewProjectionInv"].SetValue(Matrix.Invert(camera.View * camera.Projection));
            deferredConeLightEffect.Parameters["LightViewProjection"].SetValue(coneLight.View * coneLight.Projection);

            deferredConeLightEffect.Parameters["CameraPosition"].SetValue(camera.Transform.Position);
            deferredConeLightEffect.Parameters["sgrMap"].SetValue(camera.SpecularGlowReflectionMap);
            deferredConeLightEffect.Parameters["normalMap"].SetValue(camera.NormalBuffer);
            deferredConeLightEffect.Parameters["depthMap"].SetValue(camera.DepthBuffer);
            deferredConeLightEffect.Parameters["power"].SetValue(coneLight.Intensity);

            deferredConeLightEffect.Parameters["ConeAngle"].SetValue(coneLight.Angle);
            deferredConeLightEffect.Parameters["ConeDecay"].SetValue(coneLight.Decay);

            deferredConeLightEffect.Parameters["CastShadow"].SetValue(coneLight.CastShadow);
            if (coneLight.CastShadow)
            {
                deferredConeLightEffect.Parameters["shadowMap"].SetValue(coneLight.ShadowMap);
                deferredConeLightEffect.Parameters["mod"].SetValue(deferredConeShadowMapMod);
                deferredConeLightEffect.Parameters["DiscRadius"].SetValue(1.5f);
                deferredConeLightEffect.Parameters["hardShadows"].SetValue(coneLight.HardShadows);

                deferredConeLightEffect.Parameters["Taps"].SetValue(taps);
            }

            deferredConeLightEffect.CurrentTechnique.Passes[0].Apply();
            // Set sampler state to Point as the Surface type requires it in XNA 4.0
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            if (sceneQuad == null)
            {
                sceneQuad = new ScreenQuad(this);
                sceneQuad.Initialize();
            }

            sceneQuad.Draw(-Vector2.One, Vector2.One);
        }

        /// <summary>
        /// Render a directional light.
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="directionalLight"></param>
        public void RenderDirectionalLight(DeferredLightingCamera camera, DeferredDirectionalLight directionalLight)
        {
            if (deferredDirectionalLightEffect == null)
                deferredDirectionalLightEffect = Content.Load<Effect>("Shaders/DeferredRender/DeferredDirectionalLight");

            // Call lighting methods.
            deferredDirectionalLightEffect.Parameters["lightDirection"].SetValue(directionalLight.Direction);
            deferredDirectionalLightEffect.Parameters["Color"].SetValue(directionalLight.Color.ToVector3());
            deferredDirectionalLightEffect.Parameters["power"].SetValue(directionalLight.Intensity);

            deferredDirectionalLightEffect.Parameters["CameraPosition"].SetValue(camera.Transform.Position);
            deferredDirectionalLightEffect.Parameters["sgrMap"].SetValue(camera.SpecularGlowReflectionMap);
            deferredDirectionalLightEffect.Parameters["normalMap"].SetValue(camera.NormalBuffer);
            deferredDirectionalLightEffect.Parameters["depthMap"].SetValue(camera.DepthBuffer);

            deferredDirectionalLightEffect.Parameters["CastShadow"].SetValue(directionalLight.CastShadow);
            if (directionalLight.CastShadow)
            {
                deferredDirectionalLightEffect.Parameters["shadowMap"].SetValue(directionalLight.ShadowMap);
                deferredDirectionalLightEffect.Parameters["mod"].SetValue(deferredDirectionalShadowMapMod);
                deferredDirectionalLightEffect.Parameters["hardShadows"].SetValue(directionalLight.HardShadows);

                deferredDirectionalLightEffect.Parameters["DiscRadius"].SetValue(1f);

                deferredDirectionalLightEffect.Parameters["Taps"].SetValue(taps);
            }
            deferredDirectionalLightEffect.Parameters["viewProjectionInv"].SetValue(Matrix.Invert(camera.View * camera.Projection));
            deferredDirectionalLightEffect.Parameters["lightViewProjection"].SetValue(directionalLight.View * directionalLight.Projection);

            


            deferredDirectionalLightEffect.Techniques[0].Passes[0].Apply();

            if (sceneQuad == null)
            {
                sceneQuad = new ScreenQuad(this);
                sceneQuad.Initialize();
            }

            sceneQuad.Draw(-Vector2.One, Vector2.One);
        }

        /// <summary>
        /// Apply the glow map
        /// </summary>
        /// <param name="camera"></param>
        public void ApplyGlowMap(DeferredLightingCamera camera)
        {
            if (glowMapToLightMapEffect == null)
                glowMapToLightMapEffect = Content.Load<Effect>("Shaders/DeferredRender/GlowMapToLightMapShader");

            glowMapToLightMapEffect.Parameters["sgrMap"].SetValue(camera.SpecularGlowReflectionMap);

            glowMapToLightMapEffect.Techniques[0].Passes[0].Apply();

            if (sceneQuad == null)
            {
                sceneQuad = new ScreenQuad(this);
                sceneQuad.Initialize();
            }

            sceneQuad.Draw(-Vector2.One, Vector2.One);
        }

        #endregion

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

            //coneLight.Rotate(Vector3.Up, .01f);

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
                deferredDirectionalShadowMapMod += .000001f;
            if (kbm.KeyDown(Keys.F))
                deferredDirectionalShadowMapMod -= .000001f;

            if (kbm.KeyDown(Keys.T))
                deferredConeShadowMapMod += .000001f;
            if (kbm.KeyDown(Keys.G))
                deferredConeShadowMapMod -= .000001f;

            if (kbm.KeyPress(Keys.Space))
            {
                foreach (BaseLight light in ShadowLights)
                    light.HardShadows = !light.HardShadows;
            }

            if (kbm.KeyPress(Keys.F1))
            {
                foreach (BaseLight light in ShadowLights)
                    light.CastShadow = !light.CastShadow;
            }

            deferredDirectionalShadowMapMod = MathHelper.Clamp(deferredDirectionalShadowMapMod, 0, 1);
            deferredConeShadowMapMod = MathHelper.Clamp(deferredConeShadowMapMod, 0, 1);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            #region Pre Render Set Up
            #region Shadow Map Set Up
            // Set up shadow maps
            if (ShadowLights != null && ShadowLights.Count > 0 && ShadowCasters != null && ShadowCasters.Count > 0 )
            {
                // We have lights to cast shadows, objects that will block light, and obects to receive the shadows...
                foreach (BaseLight light in ShadowLights)
                    RenderLightShadows(gameTime, light, ShadowCasters);
            }
            #endregion

            
            // Setup the required render targets
            GraphicsDevice.SetRenderTargets(camera.RenderTarget, camera.SpecularGlowReflectionMap, camera.NormalBuffer, camera.DepthBuffer);
            GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
            GraphicsDevice.Clear(camera.ClearColor);
            #endregion

            // Draw all the items in the scene, they need to use the deferred Model Render shader.
            base.Draw(gameTime);

            #region Bring all the RT together
            GraphicsDevice.SetRenderTarget(null);

            #region Create Light Maps
            // Create the Light map
            DeferredLighting(gameTime, camera);
            #endregion

            #region Combine all the maps and create final scene
            GraphicsDevice.SetRenderTarget(camera.DeferredRenderTarget);
            GraphicsDevice.Clear(Color.Black);
            DrawDeferred(camera);

            GraphicsDevice.SetRenderTarget(null);

            // Combine to Final output.
            camera.FinalRenderTexture = camera.DeferredRenderTarget;
            #endregion

            #region Optional Post Processing (see RandomchaosMGPostProcessingSandBox
            // Plug in Post processing here..
            if (PostProcessingManager != null && PostProcessingManager.Enabled)
            {
                GraphicsDevice.Clear(Color.Magenta);
                PostProcessingManager.Draw(gameTime, camera.FinalRenderTexture, camera.DepthBuffer);
            }
            #endregion

            #region Render the final scene
            // Render final out put to the screen.
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            spriteBatch.Draw(camera.FinalRenderTexture, new Rectangle(0,0,graphics.PreferredBackBufferWidth,graphics.PreferredBackBufferHeight), Color.White);
            spriteBatch.End();
            #endregion
            #endregion

            #region Debug & HUD
            // Debug, show shadow map...
            if (DebugShadowMaps)
            {
                if (ShadowLights != null && ShadowLights.Count > 0 && ShadowCasters != null && ShadowCasters.Count > 0)
                {
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
                    // We have lights to cast shadows, objects that will block light, and objects to receive the shadows...
                    int s = 512;
                    int m = 0;
                    foreach (BaseLight light in ShadowLights)
                    {
                        spriteBatch.Draw(light.ShadowMap, new Rectangle(s * m++, 0, s, s), Color.White);
                    }
                    spriteBatch.End();
                }
            }

            // Deferred Render debug
            if (DebugLighting) // && GameComponentHelper.InEditor)
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
                spriteBatch.Draw(lightMap, new Rectangle((w * 4) + 7, 1, w, h), Color.White);

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

            if (DebugLighting || DebugShadowMaps)
                top += (GraphicsDevice.Viewport.Height / 5) + debugFont.LineSpacing;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            spriteBatch.DrawString(infoFont, $"[WASD] - Translate Camera", new Vector2(8, top ), Color.Gold);
            spriteBatch.DrawString(infoFont, $"[Arrow Keys] - Rotate Camera", new Vector2(8, top + lineHeight), Color.Gold);
            spriteBatch.DrawString(infoFont, $"[R/F] - Directional Shadow Offset +- {deferredDirectionalShadowMapMod} ", new Vector2(8, top + lineHeight * 2), Color.Gold);
            spriteBatch.DrawString(infoFont, $"[T/G] - Cone Shadow Offset +- {deferredConeShadowMapMod} ", new Vector2(8, top + lineHeight * 3), Color.Gold);
            spriteBatch.DrawString(infoFont, $"[SPC] - Toggle Hord/Soft Shadows", new Vector2(8, top + lineHeight * 4), Color.Gold);
            spriteBatch.DrawString(infoFont, $"[F1] - Toggle Shadows", new Vector2(8, top + lineHeight * 5), Color.Gold);
            spriteBatch.End();
            #endregion
        }        
    }
}
