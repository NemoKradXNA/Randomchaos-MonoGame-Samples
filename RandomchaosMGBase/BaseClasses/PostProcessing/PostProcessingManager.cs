
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RandomchaosMGBase.BaseClasses.PostProcessing
{
    public class PostProcessingManager
    {
        protected Game Game;
        public Texture2D Scene;
        public Texture2D DepthBuffer;

        public RenderTarget2D newScene;

        public bool Enabled { get; set; }

        protected List<BasePostProcessingEffect> postProcessingEffects = new List<BasePostProcessingEffect>();

        public SpriteBatch spriteBatch
        {
            get { return (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch)); }
        }

        public PostProcessingManager(Game game)
        {
            Game = game;
            Enabled = true;
        }

        public void AddEffect(BasePostProcessingEffect ppEfect)
        {
            postProcessingEffects.Add(ppEfect);
        }

        public virtual void Draw(GameTime gameTime, Texture2D scene, Texture2D depth)
        {
            int maxEffect = postProcessingEffects.Count;

            Scene = scene;

            for (int e = 0; e < maxEffect; e++)
            {
                if (postProcessingEffects[e].Enabled)
                {
                    postProcessingEffects[e].orgScene = scene;
                    postProcessingEffects[e].Draw(gameTime, Scene, depth);
                    Scene = postProcessingEffects[e].lastScene;
                }
            }

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            spriteBatch.Draw(Scene, new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height), Color.White);
            spriteBatch.End();
        }
    }
}
