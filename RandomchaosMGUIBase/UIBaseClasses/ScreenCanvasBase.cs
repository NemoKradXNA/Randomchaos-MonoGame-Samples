using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RandomchaosMGBase;
using RandomchaosMGBase.InputManagers;


namespace RandomchaosMGUIBase.UIBaseClasses
{
    public class ScreenCanvasBase : ControlBase
    {

        public ScreenCanvasBase(Game game, Rectangle sizeRect, string backgroundAsset = null) : base(game, sizeRect, backgroundAsset)
        {
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
            BorderThickness = Vector2.Zero;
            IsClickable = false;
        }
    }
}
