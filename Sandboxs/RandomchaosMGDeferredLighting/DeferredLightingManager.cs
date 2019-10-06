using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RandomchaosMGBase.BaseClasses;
using RandomchaosMGBase.BaseClasses.PostProcessing;
using RandomchaosMGBase.BaseClasses.Primatives;
using RandomchaosMGDeferredLighting.Lights;

namespace RandomchaosMGDeferredLighting
{
    public class DeferredLightingManager : DrawableGameComponent
    {
        protected DeferredLightingCamera camera { get { return (DeferredLightingCamera)Game.Services.GetService<Base3DCamera>(); } }
        protected SpriteBatch spriteBatch { get { return (SpriteBatch)Game.Services.GetService<SpriteBatch>(); } }
        protected Effect shadowMapEffect;

        /// <summary>
        /// Deferred effect.
        /// </summary>
        protected Effect deferredSceneRenderEffect;

        /// <summary>
        /// Light map used for deferred render
        /// </summary>
        public RenderTarget2D lightMap;

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

        public List<DeferredDirectionalLight> DirectionalLights = new List<DeferredDirectionalLight>();
        public List<DeferredPointLight> PointLights = new List<DeferredPointLight>();
        public List<DeferredConeLight> ConeLights = new List<DeferredConeLight>();

        public List<BaseLight> ShadowLights = new List<BaseLight>();
        public List<Base3DObject> ShadowCasters = new List<Base3DObject>();

        public bool DebugShadowMaps = false;
        public bool DebugLighting = true;

        public PostProcessingManager PostProcessingManager;

        public DeferredLightingManager(Game game) : base(game)
        {
            
        }

        public void AddLight(BaseLight light)
        {
            if (light is DeferredDirectionalLight)
                DirectionalLights.Add((DeferredDirectionalLight)light);
            else if (light is DeferredConeLight)
                ConeLights.Add((DeferredConeLight)light);
            else if (light is DeferredPointLight)
                PointLights.Add((DeferredPointLight)light);

            if(light.CastShadow)
                ShadowLights.Add(light);
        }

        public void AddShadowCaster(Base3DObject obj)
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
                shadowMapEffect = Game.Content.Load<Effect>("Shaders/ShadowMap");
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
                deferredSceneRenderEffect = Game.Content.Load<Effect>("Shaders/DeferredRender/DeferredSceneRender");

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
                sceneQuad = new ScreenQuad(Game);
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
                deferredPointLightEffect = Game.Content.Load<Effect>("Shaders/DeferredRender/DeferredPointLight");

            if (pointLightVolumeModel == null)
                pointLightVolumeModel = Game.Content.Load<Model>("Models/PointLightVolume");

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
                deferredConeLightEffect = Game.Content.Load<Effect>("Shaders/DeferredRender/DeferredConeLight");

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
                sceneQuad = new ScreenQuad(Game);
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
                deferredDirectionalLightEffect = Game.Content.Load<Effect>("Shaders/DeferredRender/DeferredDirectionalLight");

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
                sceneQuad = new ScreenQuad(Game);
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
                glowMapToLightMapEffect = Game.Content.Load<Effect>("Shaders/DeferredRender/GlowMapToLightMapShader");

            glowMapToLightMapEffect.Parameters["sgrMap"].SetValue(camera.SpecularGlowReflectionMap);

            glowMapToLightMapEffect.Techniques[0].Passes[0].Apply();

            if (sceneQuad == null)
            {
                sceneQuad = new ScreenQuad(Game);
                sceneQuad.Initialize();
            }

            sceneQuad.Draw(-Vector2.One, Vector2.One);
        }

        public void PreSceneDrawSetUp(GameTime gameTime)
        {
            #region Pre Render Set Up
            #region Shadow Map Set Up
            // Set up shadow maps
            if (ShadowLights != null && ShadowLights.Count > 0 && ShadowCasters != null && ShadowCasters.Count > 0)
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
        }

        public void PostSceneDraw(GameTime gameTime)
        {
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
            spriteBatch.Draw(camera.FinalRenderTexture, new Rectangle(0, 0, Game.GraphicsDevice.PresentationParameters.BackBufferWidth, Game.GraphicsDevice.PresentationParameters.BackBufferHeight), Color.White);
            spriteBatch.End();
            #endregion
            #endregion
        }
    }
}
