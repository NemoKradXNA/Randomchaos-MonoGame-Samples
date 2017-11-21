using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RandomchaosMGBase;
using RandomchaosMGBase.InputManagers;

namespace RandomchaosMGUIBase.UIBaseClasses
{
    public class ScrollRectangleBase : ControlBase
    {
        public Vector2 ScrollValue = Vector2.Zero;
        protected Vector2 maxScrollValue;
        public Vector2 minScrollValue;

        Vector2 lastScrollValue;
        public Vector2 scrollDelta;

        public Vector2 TotalScrollValue;

        public ScrollRectangleBase(Game game, Rectangle sizeRect, string backgroundAsset = null) : base(game, sizeRect, backgroundAsset)
        {

        }

        public override void Update(GameTime gameTime)
        {
            CalcMaxSize();

            int childCount = Children.Count;

            scrollDelta += lastScrollValue - ScrollValue;

            TotalScrollValue = Vector2.Max(Vector2.Min(TotalScrollValue + scrollDelta, Vector2.Zero), minScrollValue);

            for (int child = 0; child < childCount; child++)
            {
                if (Children[child].Visible && Children[child].Enabled)
                {
                    Children[child].Transform.LocalPosition2D = Vector2.Clamp(Children[child].Transform.LocalPosition2D + scrollDelta, minScrollValue, maxScrollValue);
                }
            }

            lastScrollValue = ScrollValue;
            ScrollValue = Vector2.Zero;
            base.Update(gameTime);
        }

        void CalcMaxSize()
        {
            Point max = Point.Zero;

            foreach (ControlBase child in Children)
            {
                if (child.RenderSize.Width > max.X)
                    max.X = child.RenderSize.Width;

                if (child.RenderSize.Height > max.Y)
                    max.Y = child.RenderSize.Height;
            }

            minScrollValue = new Vector2(max.X - RenderSize.Width, max.Y - RenderSize.Height) * -1;
            maxScrollValue = Vector2.Zero;
        }
    }
}
