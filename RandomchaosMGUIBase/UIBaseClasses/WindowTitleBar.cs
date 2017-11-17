using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RandomchaosMGBase;
using RandomchaosMGBase.InputManagers;

namespace RandomchaosMGUIBase.UIBaseClasses
{
    public class WindowTitleBar : ControlBase
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

        public Vector2 TextShadowOffset { get; set; }
        public Color TextShadowColor { get; set; }

        string fontAsset { get; set; }
        Vector2 textSize { get; set; }

        SpriteFont font;

        public WindowTitleBar(Game game, Rectangle sizeRect, string text, string fontAsset, string backgroundAsset = null) : base(game, sizeRect, backgroundAsset)
        {
            this.fontAsset = fontAsset;
            Text = text;

            TextColor = Color.White;
            TextShadowColor = Color.Black;

            OnMouseButtonDownEvent += MouseDown;
        }

        protected virtual void MouseDown(object sender, bool leftButton, Vector2 pos)
        {
            if (leftButton)
            {
                SetFocus(this);
            }
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            font = Game.Content.Load<SpriteFont>(fontAsset);
            textSize = font.MeasureString(text);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            StartDraw(gameTime);
            Vector2 textPosition = new Vector2((RenderSize.Width / 2) - textSize.X/2, (RenderSize.Height/2) - textSize.Y/2);

            spriteBatch.DrawString(font, text, textPosition, TextShadowColor);
            spriteBatch.DrawString(font, text, textPosition - TextShadowOffset, TextColor);
            EndDraw(gameTime);
        }
    }
}
