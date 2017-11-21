using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RandomchaosMGBase;
using RandomchaosMGBase.InputManagers;

namespace RandomchaosMGUIBase.UIBaseClasses
{
    public class LabelBase : ControlBase
    {
        public Color TextColor { get; set; }
        string text { get; set; }
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                if (font != null)
                {
                    textSize = font.MeasureString(text);
                }
            }
        }

        string fontAsset { get; set; }

        Vector2 textSize { get; set; }
        SpriteFont font;

        public LabelBase(Game game, string fontAsset, string labelText) : base(game, Rectangle.Empty, null)
        {
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
            TextColor = Color.White;

            this.fontAsset = fontAsset;
            Text = labelText;
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            font = Game.Content.Load<SpriteFont>(fontAsset);
            textSize = font.MeasureString(text);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            RenderSize = new Rectangle(0, 0, (int)textSize.X, (int)textSize.Y);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            StartDraw(gameTime);

            spriteBatch.DrawString(font, text, Vector2.Zero, TextColor);

            EndDraw(gameTime);
        }
    }
}
