using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RandomchaosMGBase.BaseClasses;

namespace RandomchaosMGDeferredLighting
{
    public class DeferredLightingCamera : Base3DCamera
    {
        public Color ClearColor { get; set; }

        protected RenderTarget2D renderTarget;
        public virtual RenderTarget2D RenderTarget
        {
            get
            {
                if (renderTarget == null)
                {
                    renderTarget = new RenderTarget2D(Game.GraphicsDevice, Game.GraphicsDevice.PresentationParameters.BackBufferWidth,
                        Game.GraphicsDevice.PresentationParameters.BackBufferHeight, false, Game.GraphicsDevice.PresentationParameters.BackBufferFormat,
                        DepthFormat.Depth24Stencil8);
                }

                return renderTarget;
            }
            set { renderTarget = value; }
        }

        protected RenderTarget2D specularGlowReflectionMap;
        public virtual RenderTarget2D SpecularGlowReflectionMap
        {
            get
            {
                if (specularGlowReflectionMap == null)
                {
                    specularGlowReflectionMap = new RenderTarget2D(Game.GraphicsDevice, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
                }

                return specularGlowReflectionMap;
            }
            set
            {
                specularGlowReflectionMap = value;
            }
        }

        protected RenderTarget2D depthBuffer;
        public virtual RenderTarget2D DepthBuffer
        {
            get
            {
                if (depthBuffer == null)
                    depthBuffer = new RenderTarget2D(Game.GraphicsDevice, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height, false, SurfaceFormat.Single, DepthFormat.Depth24Stencil8);

                return depthBuffer;
            }

            set
            {
                depthBuffer = value;
            }
        }

        protected RenderTarget2D normalBuffer;
        public virtual RenderTarget2D NormalBuffer
        {
            get
            {
                if (normalBuffer == null)
                    normalBuffer = new RenderTarget2D(Game.GraphicsDevice, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height, false, SurfaceFormat.Rgba1010102, DepthFormat.Depth24Stencil8);

                return normalBuffer;
            }
            set
            {
                normalBuffer = value;
            }
        }

        protected RenderTarget2D deferredRenderTarget;
        public virtual RenderTarget2D DeferredRenderTarget
        {
            get
            {
                if (deferredRenderTarget == null)
                {
                    deferredRenderTarget = new RenderTarget2D(Game.GraphicsDevice, Game.GraphicsDevice.PresentationParameters.BackBufferWidth,
                       Game.GraphicsDevice.PresentationParameters.BackBufferHeight, false, Game.GraphicsDevice.PresentationParameters.BackBufferFormat,
                       DepthFormat.Depth24Stencil8);
                }

                return deferredRenderTarget;
            }
            set { deferredRenderTarget = value; }
        }

        public virtual Texture2D FinalRenderTexture { get; set; }

        public DeferredLightingCamera(Game game, float minDepth, float maxDepth) : base(game, minDepth, maxDepth)
        {
            ClearColor = Color.CornflowerBlue;
        }
    }
}
