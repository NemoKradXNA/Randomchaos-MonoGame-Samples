using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RandomchaosMGBase;
using RandomchaosMGBase.InputManagers;

namespace RandomchaosMGUIBase.UIBaseClasses
{
    public class ScrollBarBase : ControlBase
    {
        public ScrollBarTypesEnum ScrollBarType { get; set; }

        ButtonBase slider;

        public float Value = 0;

        public ScrollRectangleBase ScrollRect;

        public bool IsSliding;

        public ScrollBarBase(Game game, Rectangle sizeRect, string sliderAsset = null, string backgroundAsset = null) : base(game, sizeRect, backgroundAsset)
        {
            slider = new ButtonBase(game, sizeRect, "", "", null, null, sliderAsset);
            slider.Transform.LocalPosition2D = Vector2.Zero;

            slider.OnMouseButtonDownEvent += Sliding;
            slider.OnMouseClickEvent += StopSliding;
            slider.OnMouseLeaveEvent += SlidingStops;

            AddChild(slider);
        }

        protected virtual void Sliding(object sender, bool leftButton, Vector2 position)
        {
            if (MouseManager.InitialLeftButtonDown)
                IsSliding = leftButton;
        }

        protected virtual void StopSliding(object sender, bool leftButton, Vector2 position)
        {
            if (leftButton)
                IsSliding = false;
        }

        protected virtual void SlidingStops(object sender)
        {
            IsSliding = false;
        }


        public override void Update(GameTime gameTime)
        {
            if (ScrollRect != null)
            {
                float r;
                // Size the scroller to the amout to scroll..
                switch (ScrollBarType)
                {
                    case ScrollBarTypesEnum.Horizontal:
                        float absw = Math.Abs(ScrollRect.minScrollValue.X);
                        r = (RenderSize.Width / absw);
                        int width = (int)(RenderSize.Width * r);
                        slider.RenderSize = new Rectangle(slider.RenderSize.X, slider.RenderSize.Y, width, slider.RenderSize.Height);
                        float x = Math.Max(0, MathHelper.Lerp(0, -(RenderSize.Width - width), ScrollRect.TotalScrollValue.X / absw));
                        slider.Transform.LocalPosition2D = new Vector2(x, slider.Transform.LocalPosition2D.Y);

                        if (IsSliding)
                        {
                            float delta = -MouseManager.PositionDelta.X * 4;
                            ScrollRect.ScrollValue.X = delta;
                        }
                        break;
                    case ScrollBarTypesEnum.Vertical:
                        float absh = Math.Abs(ScrollRect.minScrollValue.Y);
                        r = (RenderSize.Height / absh);
                        int height = (int)(RenderSize.Height *r);
                        slider.RenderSize = new Rectangle(slider.RenderSize.X, slider.RenderSize.Y, slider.RenderSize.Width, height);
                        float y = Math.Max(0, MathHelper.Lerp(0, -(RenderSize.Height - height), ScrollRect.TotalScrollValue.Y / absh));
                        slider.Transform.LocalPosition2D = new Vector2(slider.Transform.LocalPosition2D.X, y);

                        if (IsSliding)
                        {
                            float delta = -MouseManager.PositionDelta.Y * 4;
                            ScrollRect.ScrollValue.Y = delta;
                        }
                        break;
                }
            }

            base.Update(gameTime);
        }
    }
}
