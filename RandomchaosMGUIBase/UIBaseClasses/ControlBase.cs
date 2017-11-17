using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RandomchaosMGBase;
using RandomchaosMGBase.InputManagers;

namespace RandomchaosMGUIBase.UIBaseClasses
{
    public class ControlBase : DrawableGameComponent, IHasTransform
    {
        #region Statics
        /// <summary>
        /// Denotes the current control that  has focus.
        /// </summary>
        public static ControlBase FocusControl = null;
        #endregion

        #region Event Delegates
        /// <summary>
        /// Caleed when the mouse pointer is over the control
        /// </summary>
        public event MouseOver OnMouseOverEvent;

        /// <summary>
        /// Fired when the mouse leaves the control
        /// </summary>
        public event MouseEnter OnMouseEnterEvent;

        /// <summary>
        /// Is called when the mouse pointer leaves the bounds of the control
        /// </summary>
        public event MouseLeave OnMouseLeaveEvent;

        /// <summary>
        /// Fired when over conrole and mouse button is down
        /// </summary>
        public event MouseButtonDown OnMouseButtonDownEvent;

        /// <summary>
        /// Is called when the mouse pointer is over the control, and the mouse is cliecked 
        /// </summary>
        public event MouseClick OnMouseClickEvent;

        /// <summary>
        /// Is called when the cotnrol has focus
        /// </summary>
        public event GotFocus OnGotFocusEvent;

        /// <summary>
        /// Is called when the control loses focus.
        /// </summary>
        public event LostFocus OnLostFocusEvent;
        #endregion

        #region Properties
        /// <summary>
        /// Does this control take mouse events?
        /// </summary>
        public bool IsClickable { get; set; }

        /// <summary>
        /// Access to the mouse manager.
        /// </summary>
        protected MouseStateManager MouseManager { get { return Game.Services.GetService<MouseStateManager>(); } }

        /// <summary>
        /// Used to load the texture to be used for the background.
        /// </summary>
        public string BackgroundAsset { get; set; }

        /// <summary>
        /// The size of the control eb be rendered.
        /// </summary>
        public Rectangle RenderSize { get; set; }

        /// <summary>
        /// This has be be the screen position of the control as it is used when detecing mouse intersects.
        /// </summary>
        public Rectangle BoundsRectangle
        {
            get
            {
                Vector2 p = Transform.Position2D;
                return new Rectangle((int)p.X, (int)p.Y, RenderSize.Width, RenderSize.Height);
            }
        }

        /// <summary>
        /// Scissor rectangle to stop control overflow.
        /// </summary>
        public Rectangle ScissorRectangle { get; set; }

        /// <summary>
        /// Transform used for position, scale and rotation..
        /// </summary>
        public Transform Transform { get; set; }

        /// <summary>
        /// Color to render the bounds in when required..
        /// </summary>
        public Color BoundsColor { get; set; }

        /// <summary>
        /// Color to render the background in
        /// </summary>
        public Color BackgroundColor { get; set; }

        /// <summary>
        /// Used to state if we want to render the bouds rectangle or not.
        /// </summary>
        public bool RenderBoundsRectnagle { get; set; }

        /// <summary>
        /// The thickness of the border we want to render, defaults to 1,1
        /// </summary>
        public Vector2 BorderThickness { get; set; }

        /// <summary>
        /// Color of the border to be rendered.
        /// </summary>
        public Color BorderColor { get; set; }

        /// <summary>
        /// Indicates if this control has focus...
        /// </summary>
        public bool HasFocus { get; set; }

        /// <summary>
        /// Indicates if the mouse is over the current comtrol
        /// </summary>
        public bool IsMouseOver { get; set; }

        protected bool HasInitialized { get; set; }
        #endregion

        #region Fields

        /// <summary>
        /// Texture used to render the bounds of the control, useful for debugging :)
        /// </summary>
        Texture2D BoundsTexture;

        /// <summary>
        /// Used by parent control to render its self and it's children
        /// </summary>
        protected SpriteBatch spriteBatch;        

        /// <summary>
        /// The actual texture used to render as the background.
        /// </summary>
        Texture2D BackgroundTexture;

        /// <summary>
        /// List of the children to be rendered (if any)
        /// </summary>
        public List<ControlBase> Children = new List<ControlBase>();        

        /// <summary>
        /// Used to switch the scissor test on
        /// </summary>
        RasterizerState rasterizerState = new RasterizerState() { ScissorTestEnable = true };

        /// <summary>
        /// Used to put the origonal scissor rect back
        /// </summary>
        Rectangle lastScissorRect;

        /// <summary>
        /// Size of the border to be rendered.
        /// </summary>
        Rectangle borderRect;
        #endregion

        public ControlBase(Game game, Rectangle sizeRect, string backgroundAsset = null) : base(game)
        {
            BoundsColor = new Color(255, 0, 0, 16);
            Transform = new Transform();

            RenderSize = sizeRect;
            ScissorRectangle = RenderSize;

            BackgroundAsset = backgroundAsset;

            Transform.LocalPosition2D = new Vector2(sizeRect.X, sizeRect.Y);

            BackgroundColor = Color.DarkGray;
            BorderColor = Color.Gray;

            BorderThickness = Vector2.One;

            OnGotFocusEvent += SetFocus;

            IsClickable = true;
        }

        #region Override Methods
        public override void Initialize()
        {
            base.Initialize();

            HasInitialized = true;

            int childCount = Children.Count;

            for (int child = 0; child < childCount; child++)
            {
                Children[child].Initialize();
            }
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            BoundsTexture = new Texture2D(Game.GraphicsDevice, 1, 1);
            BoundsTexture.SetData<Color>(new Color[] { Color.White });

            if (BackgroundAsset != null)
                BackgroundTexture = Game.Content.Load<Texture2D>(BackgroundAsset);
            else
            {
                BackgroundTexture = new Texture2D(Game.GraphicsDevice, 1, 1);
                BackgroundTexture   .SetData<Color>(new Color[] { Color.White });
            }

        }

        public override void Update(GameTime gameTime)
        {
            HasFocus = ControlBase.FocusControl == this;

            Transform.Update();
            base.Update(gameTime);

            // Ensure the render rectangle is in line with the current position.
            Vector2 p = Vector2.Zero;

            // If we are the root control, our position is already taken into account by our Transform.
            if (Transform.Parent != null)
                p = Transform.LocalPosition2D;

            p = Vector2.Zero;

            RenderSize = new Rectangle((int)p.X, (int)p.Y, RenderSize.Width, RenderSize.Height);
            borderRect = new Rectangle((int)(p.X - BorderThickness.X), (int)(p.Y - BorderThickness.Y), RenderSize.Width + (int)(BorderThickness.X * 2), RenderSize.Height + (int)(BorderThickness.Y * 2));

            // Scissor Rect is not start x/y and height width, but screen coords
            ScissorRectangle = new Rectangle((int)(Transform.Position2D.X - BorderThickness.X), (int)(Transform.Position2D.Y - BorderThickness.Y), RenderSize.Right + (int)(BorderThickness.X * 2), RenderSize.Bottom + (int)(BorderThickness.Y * 2));

            // If we have a parent and it's a ControlBase too, then we need to make sure our scissor rectangle fits in it.
            if (Transform.Parent != null && Transform.Parent is ControlBase)
            {
                ControlBase thisParent = (ControlBase)Transform.Parent;
                ScissorRectangle = new Rectangle(MathHelper.Max(thisParent.ScissorRectangle.X + (int)thisParent.BorderThickness.X, ScissorRectangle.X), MathHelper.Max(thisParent.ScissorRectangle.Y + (int)thisParent.BorderThickness.Y, ScissorRectangle.Y), MathHelper.Min((thisParent.ScissorRectangle.Right - (int)thisParent.BorderThickness.X) - ScissorRectangle.X, ScissorRectangle.Width), MathHelper.Min((thisParent.ScissorRectangle.Bottom - (int)thisParent.BorderThickness.Y) - ScissorRectangle.Y, ScissorRectangle.Height));
            }

            // Check for mouse over events.
            if (BoundsRectangle.Intersects(MouseManager.PositionRect))
            {
                if (!IsMouseOver)
                {
                    if (OnMouseEnterEvent != null)
                        OnMouseEnterEvent(this);
                }

                IsMouseOver = true;
                if (OnMouseOverEvent != null)
                    OnMouseOverEvent(this, MouseManager.Position);

                if (MouseManager.LeftButtonDown || MouseManager.RightButtonDown)
                {
                    if (OnMouseButtonDownEvent != null)
                        OnMouseButtonDownEvent(this, MouseManager.LeftButtonDown, MouseManager.Position);
                }

                if (IsClickable)
                {
                    // Was the left button clicked?
                    if (MouseManager.LeftClicked)
                    {
                        if (OnGotFocusEvent != null)
                            OnGotFocusEvent(this);

                        if (OnMouseClickEvent != null)
                            OnMouseClickEvent(this, true, MouseManager.Position);
                    }

                    // Was the right button clicked?
                    if (MouseManager.RightClicked && OnMouseClickEvent != null)
                        OnMouseClickEvent(this, false, MouseManager.Position);
                }
            }
            else // Mouse no longet over the control
            {
                // Has the mose been clicked?
                if (MouseManager.LeftClicked || MouseManager.RightClicked)
                {
                    if (OnLostFocusEvent != null)
                        OnLostFocusEvent(this);

                    if (HasFocus)
                    {
                        ControlBase.FocusControl = null;
                    }
                }

                // Has the mouse been over this control?
                if (IsMouseOver)
                {
                    IsMouseOver = false;

                    if (OnMouseLeaveEvent != null)
                        OnMouseLeaveEvent(this);
                }

            }

            int childCount = Children.Count;

            for (int child = 0; child < childCount; child++)
            {
                if (Children[child].Visible && Children[child].Enabled)
                    Children[child].Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            StartDraw(gameTime);

            // Do my draw call, my background....
            spriteBatch.Draw(BoundsTexture, borderRect, BorderColor);
            spriteBatch.Draw(BackgroundTexture, RenderSize, BackgroundTexture.Bounds, BackgroundColor, Transform.LocalRotation.Z, Vector2.Zero, SpriteEffects.None, 0);
            EndDraw(gameTime);

            int childCount = Children.Count;

            for (int child = 0; child < childCount; child++)
            {
                // Ensure the child has my sprite batch..
                if (Children[child].spriteBatch != spriteBatch)
                    Children[child].spriteBatch = spriteBatch;

                if (Children[child].Visible)
                    Children[child].Draw(gameTime);
            }
        }
        #endregion

        #region Virtual Methods

        protected virtual void SetFocus(object sender)
        {
            if (sender == this)
            {
               // if (ControlBase.FocusControl == null || (ControlBase.FocusControl.DrawOrder >= ((ControlBase)sender).DrawOrder))
                {
                    ControlBase.FocusControl = this;

                    // Move this control to the end of the render que...
                    BringToFront();
                }
            }
        }

        protected virtual void BringToFront()
        {
            ControlBase TopControl = this;

            while (TopControl.Transform.Parent != null)
            {
                TopControl = (ControlBase)TopControl.Transform.Parent;
            }

            // Make sure we are before the mouse renderer...
            MouseRenderer mr = (MouseRenderer)Game.Components.SingleOrDefault(m => m is MouseRenderer);
            int idx = Game.Components.Max(c => c is DrawableGameComponent ? ((DrawableGameComponent)c).DrawOrder : 0);

            TopControl.DrawOrder = TopControl .UpdateOrder = idx+1;
            mr.DrawOrder = mr.UpdateOrder = TopControl.DrawOrder + 1;
        }

                
        public virtual void StartDraw(GameTime gameTime)
        {
//            rasterizerState = null;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, rasterizerState, null, Transform.World);
            lastScissorRect = spriteBatch.GraphicsDevice.ScissorRectangle;
            spriteBatch.GraphicsDevice.ScissorRectangle = ScissorRectangle;
        }

        public virtual void EndDraw(GameTime gameTime)
        {
            spriteBatch.GraphicsDevice.ScissorRectangle = lastScissorRect;

            if (RenderBoundsRectnagle)
                DrawBoundsRectangle();

            spriteBatch.End();
        }        

        protected virtual void DrawBoundsRectangle()
        {
            spriteBatch.Draw(BoundsTexture, RenderSize, BoundsColor);
        }

        public virtual void AddChild(ControlBase control)
        {
            control.Transform.Parent = this;
            control.spriteBatch = spriteBatch;

            if (this.HasInitialized)
                control.Initialize();

            Children.Add(control);
        }

        public virtual void RemoveChild(ControlBase control)
        {
            if (Children.Contains(control))
                Children.Remove(control);
        }
        
        #endregion
        
    }
}
