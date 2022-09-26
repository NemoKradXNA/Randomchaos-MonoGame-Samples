using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CommunityPost18067Core
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        SpriteFont font;

        Texture2D texture;

        bool scissorRectOn { get; set; }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            font = Content.Load<SpriteFont>("Fonts/font");

            base.Initialize();

            texture = new Texture2D(GraphicsDevice, 1, 1);
            texture.SetData(new Color[] { new Color(1f, 1f, 1f, .5f) });
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here


            if (Keyboard.GetState().IsKeyDown(Keys.F1))
                scissorRectOn = true;

            if (Keyboard.GetState().IsKeyDown(Keys.F2))
                scissorRectOn = false;


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp);

            _spriteBatch.DrawString(font, "F1 to toggle Scissor Test On", new Vector2(7, 9), Color.Black);
            _spriteBatch.DrawString(font,"F1 to toggle Scissor Test On", new Vector2(8,8), Color.Gold);

            _spriteBatch.DrawString(font, "F2 to toggle Scissor Test Off", new Vector2(7, 9 + font.LineSpacing), Color.Black);
            _spriteBatch.DrawString(font, "F2 to toggle Scissor Test Off", new Vector2(8, 8 + font.LineSpacing), Color.Gold);

            _spriteBatch.DrawString(font, $"Scissor Test On [{scissorRectOn}]", new Vector2(7, 9 + font.LineSpacing * 2), Color.Black);
            _spriteBatch.DrawString(font, $"Scissor Test On [{scissorRectOn}]", new Vector2(8, 8 + font.LineSpacing * 2), Color.Gold);

            _spriteBatch.End();


            Rectangle orgScissorRec = _spriteBatch.GraphicsDevice.ScissorRectangle;

            RasterizerState rasterizerState = new RasterizerState() { ScissorTestEnable = scissorRectOn };

            // Area I want to not be able to draw outside of
            Rectangle targetRect = new Rectangle(GraphicsDevice.Viewport.Width / 4, GraphicsDevice.Viewport.Height / 4, GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

            // set it in the sprite batch
            _spriteBatch.GraphicsDevice.ScissorRectangle = targetRect;

            // Begin
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, rasterizerState);

            // Draw a red Box to show the target rectangle
            _spriteBatch.Draw(texture, targetRect, Color.Red);

            // Draw a box in the target rect..
            _spriteBatch.Draw(texture, new Rectangle(targetRect.X + 8, targetRect.Y + 8, 100,100), Color.Blue);

            // Draw a yellow recangle that over hangs the target rectangle, see how any part of id rawn out side of the target rect is culled.
            _spriteBatch.Draw(texture, new Rectangle(targetRect.Center.X, targetRect.Center.Y, (int)(targetRect.Width/1.5f), (int)(targetRect.Height / 1.5f)), Color.Yellow);

            _spriteBatch.End();

            

            base.Draw(gameTime);
        }
    }
}