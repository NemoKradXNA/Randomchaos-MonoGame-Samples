using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CommunityPost13424
{
    public class SpriteData : DrawableGameComponent
    {
        /// <summary>
        /// Screen position.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Size of sprite
        /// </summary>
        public Vector2 Dimensions { get; set; }

        // Color of sprite.
        public Color Color { get; set; }


        protected Rectangle render { get; set; }

        protected SpriteBatch spriteBatch { get { return (SpriteBatch)Game.Services.GetService<SpriteBatch>(); } }

        public SpriteData(Game game, Vector2 position, Vector2 dims, Color color) : base(game)
        {
            Position = position;
            Dimensions = dims;
            Color = color;

        }

        public override void Update(GameTime gameTime)
        {
            render = new Rectangle((int)Position.X, (int)Position.Y, (int)Dimensions.X, (int)Dimensions.Y);

        }

        public Rectangle GetBounds(Vector2 offset)
        {
            Rectangle aaBounds;

            Vector2 pos = Position + offset;
            Vector2 hdim = Dimensions * .5f;

            // we are rendering the square in the center, so we need to offset our bounds to take that into account.
            aaBounds = new Rectangle((int)(pos.X - hdim.X), (int)(pos.Y - hdim.Y), (int)Dimensions.X, (int)Dimensions.Y);

            return aaBounds;
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Draw(Game.Content.Load<Texture2D>("Textures/WhitePixel"), render, null, Color, 0, Vector2.One * .5f, SpriteEffects.None, 0);
        }

        public void DrawBounds(GameTime gameTime)
        {
            spriteBatch.Draw(Game.Content.Load<Texture2D>("Textures/WhitePixel"), GetBounds(Vector2.Zero), null, new Color(0, 0, 0, .5f), 0, Vector2.One * .5f, SpriteEffects.None, 0);
        }
    }
}
