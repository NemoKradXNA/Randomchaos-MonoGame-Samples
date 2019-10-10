using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CommunityPost11896
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Effect effect;

        Vector2 DisplacementScroll;
        Vector2 scrollVelocity = new Vector2(0, .001f);

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.GraphicsProfile = GraphicsProfile.HiDef;

            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;

            IsMouseVisible = true;
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

            effect = Content.Load<Effect>("Shaders/shader");
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

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            int s = (int)(GraphicsDevice.PresentationParameters.BackBufferWidth / 3f);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            spriteBatch.Draw(Content.Load<Texture2D>("Textures/xnauguk"), new Rectangle(0, 0, s, s), Color.White);
            spriteBatch.Draw(Content.Load<Texture2D>("Textures/bumpmap"), new Rectangle(s, 0, s, s), Color.White);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            effect.CurrentTechnique.Passes[0].Apply();
            effect.Parameters["DisplacementScroll"].SetValue(DisplacementScroll += scrollVelocity);
            effect.Parameters["Displacement"].SetValue(Content.Load<Texture2D>("Textures /bumpmap"));
            spriteBatch.Draw(Content.Load<Texture2D>("Textures/xnauguk"), new Rectangle(s*2, 0, s, s), Color.White);
            spriteBatch.End();
        }
    }
}
