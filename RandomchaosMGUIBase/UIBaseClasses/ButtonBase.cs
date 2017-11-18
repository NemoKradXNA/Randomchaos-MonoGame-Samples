using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RandomchaosMGBase;
using RandomchaosMGBase.InputManagers;

namespace RandomchaosMGUIBase.UIBaseClasses
{
    public class ButtonBase : ControlBase
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

        public Vector2 ShadowOffset { get; set; }
        public Color ShadowColor { get; set; }

        string fontAsset { get; set; }
        Vector2 textSize { get; set; }

        SpriteFont font;

        public Vector2? IconOffset { get; set; }
        string iconAsset { get; set; }
        Texture2D Icon { get; set; }

        public Vector2? TextOffset { get; set; }
        Rectangle? IconRectangle;

        public Color HoverColor = Color.Silver;
        public Color ButtonDownColor = Color.White;

        public bool IsSelceted { get; set; }

        Color orgBackgroundColor;

        public Color IconColor = new Color(255, 255, 255, 32);

        public ButtonBase(Game game, Rectangle sizeRect, string text, string fontAsset, string iconAsset = null, Rectangle? iconRect = null, string backgroundAsset = null) : base(game, sizeRect, backgroundAsset)
        {
            this.fontAsset = fontAsset;
            Text = text;

            TextColor = Color.White;
            ShadowColor = Color.Black;

            this.iconAsset = iconAsset;
            IconRectangle = iconRect;

            OnMouseEnterEvent += setHover;
            OnMouseLeaveEvent += unsetHover;
            OnMouseButtonDownEvent += selecting;
            OnMouseClickEvent += clicked;
        }

        protected void setHover(object sender)
        {
            orgBackgroundColor = BackgroundColor;
            BackgroundColor = HoverColor;
        }

        protected void selecting(object sender, bool leftButton, Vector2 position)
        {

            IsSelceted = true;
            BackgroundColor = ButtonDownColor;
        }

        protected void unsetHover(object sender)
        {
            IsSelceted = false;
            BackgroundColor = orgBackgroundColor;
        }

        protected void clicked(object sender, bool leftButton, Vector2 position)
        {
            BackgroundColor = orgBackgroundColor;
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            font = Game.Content.Load<SpriteFont>(fontAsset);
            textSize = font.MeasureString(text);

            if (!string.IsNullOrWhiteSpace(iconAsset))
                Icon = Game.Content.Load<Texture2D>(iconAsset);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            StartDraw(gameTime);

            Vector2 pos = Vector2.Zero;

            if (Icon != null && IconRectangle != null)
            {
                if (IconOffset == null)
                    pos += new Vector2((RenderSize.Width / 2) - IconRectangle.Value.Width / 2, (RenderSize.Height / 2) - IconRectangle.Value.Height / 2);
                else
                    pos += IconOffset.Value;

                IconRectangle = new Rectangle((int)pos.X, (int)pos.Y, IconRectangle.Value.Width, IconRectangle.Value.Height);
                spriteBatch.Draw(Icon, IconRectangle.Value, IconColor);
            }

            if (!string.IsNullOrEmpty(text))
            {
                
                if (TextOffset == null)
                    pos +=  new Vector2((RenderSize.Width / 2) - textSize.X / 2, (RenderSize.Height / 2) - textSize.Y / 2);
                else
                    pos += TextOffset.Value;

                spriteBatch.DrawString(font, text, pos, ShadowColor);
                spriteBatch.DrawString(font, text, pos - ShadowOffset, TextColor);
            }
            EndDraw(gameTime);
        }
    }
}
