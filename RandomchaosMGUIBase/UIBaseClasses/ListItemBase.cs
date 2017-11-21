using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RandomchaosMGBase;
using RandomchaosMGBase.InputManagers;

namespace RandomchaosMGUIBase.UIBaseClasses
{
    public class ListItemBase : ButtonBase
    {
        public object Tag { get; set; }

        public ListItemBase(Game game, Rectangle sizeRect, string text, string fontAsset, string iconAsset = null, Rectangle? iconRect = null, string backgroundAsset = null) : base(game, sizeRect, text, fontAsset, iconAsset, iconRect, backgroundAsset)
        {

        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
