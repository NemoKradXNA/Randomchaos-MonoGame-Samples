using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using RandomchaosMGBase.InputManagers;

namespace CommunityPost13424
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteData sprite1;
        SpriteData sprite2;
        SpriteData sprite3;

        KeyboardStateManager kbm;
        SpriteFont font;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            kbm = new KeyboardStateManager(this);

            sprite1 = new SpriteData(this, new Vector2(74,74), Vector2.One * 64, Color.Red);
            Components.Add(sprite1);

            sprite2 = new SpriteData(this, new Vector2(graphics.PreferredBackBufferWidth - 74, graphics.PreferredBackBufferHeight - 74), Vector2.One * 64, Color.Blue);
            Components.Add(sprite2);

            sprite3 = new SpriteData(this, new Vector2(graphics.PreferredBackBufferWidth /2, graphics.PreferredBackBufferHeight /2), Vector2.One * 128, Color.Gold);
            Components.Add(sprite3);
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float speedTran = 1f;


            if (kbm.KeyDown(Keys.W) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y > 0)
                MoveSprite(sprite1, new Vector2(0, -1) * speedTran);
            if (kbm.KeyDown(Keys.S) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y < 0)
                MoveSprite(sprite1, new Vector2(0, 1) * speedTran);
            if (kbm.KeyDown(Keys.A) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X < 0)
                MoveSprite(sprite1, new Vector2(-1, 0) * speedTran);
            if (kbm.KeyDown(Keys.D) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X > 0)
                MoveSprite(sprite1, new Vector2(1, 0) * speedTran);


            if (kbm.KeyDown(Keys.Up) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y > 0)
                MoveSprite(sprite2, new Vector2(0, -1) * speedTran);
            if (kbm.KeyDown(Keys.Down) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y < 0)
                MoveSprite(sprite2, new Vector2(0, 1) * speedTran);
            if (kbm.KeyDown(Keys.Left) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X < 0)
                MoveSprite(sprite2, new Vector2(-1, 0) * speedTran);
            if (kbm.KeyDown(Keys.Right) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X > 0)
                MoveSprite(sprite2, new Vector2(1, 0) * speedTran);

            base.Update(gameTime);
        }

        void MoveSprite(SpriteData sprite, Vector2 move)
        {
            Rectangle spritesBounds = sprite.GetBounds(move);
            bool canMove = true;

            //// We only want sprites.
            //List<SpriteData> sprites = Components.Where(c => c is SpriteData).Cast<SpriteData>().ToList();

            foreach (GameComponent dc in Components)
            {
                if (dc is SpriteData) // Only comparing sprites...
                {
                    if (dc != sprite) // Don't want to bounds check ourselves..
                    {
                        Rectangle targetBounds = ((SpriteData)dc).GetBounds(Vector2.Zero);
                        if (spritesBounds.Intersects(targetBounds))
                        {
                            canMove = false;
                            break;
                        }
                    }
                }
            }

            if (canMove)
                sprite.Position += move;

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            base.Draw(gameTime);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            spriteBatch.DrawString(font, "ESC - Exit", new Vector2(8, 8), Color.Gold);
            spriteBatch.DrawString(font, "WASD - Translate Red Sprite", new Vector2(8, 8 + font.LineSpacing), Color.Gold);
            spriteBatch.DrawString(font, "Arrow Keys - Translate Blue Sprite", new Vector2(8, 8 + (font.LineSpacing * 2)), Color.Gold);
            spriteBatch.End();
        }
    }
}
