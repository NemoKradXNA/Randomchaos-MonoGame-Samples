using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RandomchaosMGBase.BaseClasses.PostProcessing
{
    public class BasePostProcess
    {
        public Vector2 HalfPixel;

        public Base3DCamera camera
        {
            get { return (Base3DCamera)Game.Services.GetService(typeof(Base3DCamera)); }
        }

        public SpriteBatch spriteBatch
        {
            get { return (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch)); }
        }

        public Texture2D DepthBuffer;

        public Texture2D BackBuffer;
        public Texture2D orgBuffer;

        public bool Enabled = true;

        Texture2D b;

        public SpriteSortMode SortMode = SpriteSortMode.Immediate;
        public BlendState Blend = BlendState.Opaque;
        public SamplerState Sampler = SamplerState.AnisotropicClamp;


        protected Effect effect;

        protected Game Game;
        public RenderTarget2D newScene;

        public bool UsesVertexShader = false;

        public BasePostProcess(Game game)
        {
            Game = game;
        }


        public virtual void Draw(GameTime gameTime)
        {
            if (Enabled)
            {
                spriteBatch.Begin(SortMode, Blend, Sampler, DepthStencilState.None, RasterizerState.CullCounterClockwise);
                effect.CurrentTechnique.Passes[0].Apply();
                spriteBatch.Draw(BackBuffer, new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height), Color.White);
                spriteBatch.End();
            }
        }
    }
}
