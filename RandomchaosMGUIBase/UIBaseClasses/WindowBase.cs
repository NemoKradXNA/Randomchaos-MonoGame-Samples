using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RandomchaosMGBase;
using RandomchaosMGBase.InputManagers;

namespace RandomchaosMGUIBase.UIBaseClasses
{
    public class WindowBase : ControlBase
    {
        public event ClosingWindow OnClosingWindowEvent;
        public event CloseWindow OnCloseWindowEvent;

        WindowTitleBar titleBar;

        ButtonBase btnClose;

        ButtonBase btnScaleWindow;

        public bool IsSizable
        {
            get { return btnScaleWindow.Visible; }
            set
            {
                btnScaleWindow.Visible = value;
            }
        }

        protected bool IsMoving { get; set; }
        protected bool IsReSizing { get; set; }

        public Point MinDimensions { get; set; }

        public WindowBase(Game game, Rectangle sizeRect, string fontAsset, string titleText, string backgroundAsset = null) : base(game, sizeRect, backgroundAsset)
        {
            MinDimensions = new Point(200, 150);

            titleBar = new WindowTitleBar(game, new Rectangle(0, 0, sizeRect.Width, 32), titleText, fontAsset);
            titleBar.BackgroundColor = Color.DarkRed;
            titleBar.TextColor = Color.White;
            titleBar.TextShadowOffset = new Vector2(-2, 2);

            titleBar.OnMouseButtonDownEvent += Dragging;
            titleBar.OnMouseClickEvent += StopDragging;

            AddChild(titleBar);

            btnClose = new ButtonBase(game, new Rectangle(sizeRect.Width - 30, 1, 30, 30), "", fontAsset, "Textures/Icons/CloseIcon", new Rectangle(0, 0, 16, 16));
            btnClose.HoverColor = Color.Red;
            btnClose.BackgroundColor = Color.DarkRed;
            btnClose.BorderColor = Color.Black;
            btnClose.OnMouseClickEvent += CloseThisWindow;
            btnClose.ButtonDownColor = new Color(200, 0, 0, 255);
            btnClose.BorderThickness = Vector2.Zero;
            AddChild(btnClose);

            btnScaleWindow = new ButtonBase(game, new Rectangle(sizeRect.Width - 30, sizeRect.Height - 30, 30, 30), "", fontAsset, "Textures/Icons/ScaleWindowIcon", new Rectangle(0, 0, 24, 24));
            btnScaleWindow.IconColor = Color.White;

            btnScaleWindow.BorderThickness = Vector2.Zero;
            btnScaleWindow.ButtonDownColor = Color.Gray;
            btnScaleWindow.HoverColor = Color.LightGray;

            btnScaleWindow.OnMouseButtonDownEvent += Sizing;
            btnScaleWindow.OnMouseClickEvent += StopSizing;
            btnScaleWindow.OnMouseLeaveEvent += SizingStop;

            AddChild(btnScaleWindow);
            
            Transform.LocalPosition2D = new Vector2(sizeRect.X, sizeRect.Y);
            Transform.Update();
        }

        public override void Update(GameTime gameTime)
        {
            if (btnClose.IsSelceted)
                IsMoving = false;

            
            if (IsReSizing)
                ReSizing();

            // Ensure the content is lined up..
            titleBar.RenderSize = new Rectangle(0, 0, RenderSize.Width, 32);
            btnClose.Transform.LocalPosition2D = new Vector2(RenderSize.Width - 32, 1);
            btnScaleWindow.Transform.LocalPosition2D = new Vector2(RenderSize.Width - 32, RenderSize.Height - 32);

            base.Update(gameTime);

            if (IsMoving)
            {
                if (ControlBase.FocusControl == this.titleBar)
                    Moving();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        protected virtual void CloseThisWindow(object sender, bool leftButton, Vector2 pos)
        {
            if (leftButton)
            {
                if (ClosingWindow())
                {
                    if (OnCloseWindowEvent != null)
                        OnCloseWindowEvent(this);
                }
            }
        }

        protected virtual bool ClosingWindow()
        {
            if (OnClosingWindowEvent != null)
                OnClosingWindowEvent(this);

            return true;
        }

        protected virtual void Dragging(object sender, bool leftButton, Vector2 position)
        {
            if (MouseManager.InitialLeftButtonDown)
                IsMoving = leftButton;
        }

        protected virtual void StopDragging(object sender, bool leftButton, Vector2 position)
        {
            if (leftButton)
                IsMoving = false;
        }

        protected virtual void Sizing(object sender, bool leftButton, Vector2 position)
        {
            if (MouseManager.InitialLeftButtonDown)
                IsReSizing = leftButton;
        }

        protected virtual void StopSizing(object sender, bool leftButton, Vector2 position)
        {
            if (leftButton)
                IsReSizing = false;
        }

        protected virtual void SizingStop(object sender)
        {
            IsReSizing = false;
        }

        protected virtual void Moving()
        {
            Vector2 change = MouseManager.PositionDelta;
            Transform.LocalPosition2D -= change;
        }

        protected virtual void ReSizing()
        {
            Vector2 change = MouseManager.PositionDelta;
            Point newDim = new Point(MathHelper.Max(MinDimensions.X, RenderSize.Width - (int)change.X), MathHelper.Max(MinDimensions.Y, RenderSize.Height - (int)change.Y));
            RenderSize = new Rectangle(RenderSize.X, RenderSize.Y, newDim.X, newDim.Y);
        }

        
    }
}
