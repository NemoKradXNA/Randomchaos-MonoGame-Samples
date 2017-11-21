using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RandomchaosMGBase;
using RandomchaosMGBase.InputManagers;

namespace RandomchaosMGUIBase.UIBaseClasses
{
    public class ListBoxBase : ControlBase
    {
        public List<ListItemBase> Items { get; set; }

        public Vector2 IconSize = new Vector2(64, 64);
        public Vector2 padding = new Vector2(8, 24);

        public OnItemSelected OnItemSelectedEvent;
        public OnItemSelected OnItemDoubleClickedEvent;

        string fontAsset;

        ControlBase content;
        ScrollRectangleBase scrollRect;
        ScrollBarBase vScrollBar;
        ScrollBarBase hScrollBar;

        public ListBoxBase(Game game, Rectangle sizeRect, string fontAsset) : base(game, sizeRect, null)
        {
            this.fontAsset = fontAsset;
            Items = new List<ListItemBase>();

            scrollRect = new ScrollRectangleBase(game, sizeRect);
            scrollRect.BackgroundColor = Color.Transparent;
            scrollRect.BorderColor = Color.Transparent;
            AddChild(scrollRect);

            content = new ControlBase(game, sizeRect);
            content.BackgroundColor = Color.Transparent;
            content.BorderColor = Color.Transparent;
            scrollRect.AddChild(content);

            vScrollBar = new ScrollBarBase(game, new Rectangle(0, 0, 16, sizeRect.Height-16));
            vScrollBar.ScrollBarType = ScrollBarTypesEnum.Vertical;
            vScrollBar.Transform.LocalPosition2D = new Vector2(sizeRect.Right -16, 0);
            vScrollBar.ScrollRect = scrollRect;
            vScrollBar.BackgroundColor = new Color(100, 100, 100, 255);
            vScrollBar.SliderColor = new Color(150, 150, 150, 255);
            AddChild(vScrollBar);

            hScrollBar = new ScrollBarBase(game, new Rectangle(0, 0, sizeRect.Width - 16, 16));
            hScrollBar.ScrollBarType = ScrollBarTypesEnum.Horizontal;
            hScrollBar.Transform.LocalPosition2D = new Vector2(0, sizeRect.Height-16);
            hScrollBar.ScrollRect = scrollRect;
            hScrollBar.BackgroundColor = new Color(100, 100, 100, 255);
            hScrollBar.SliderColor = new Color(150, 150, 150, 255);
            AddChild(hScrollBar);
        }

        public override void Update(GameTime gameTime)
        {
            scrollRect.RenderSize = new Rectangle(0, 0, RenderSize.Width, RenderSize.Height - 2);
            content.Children = content.Children.Where(d => d.Visible).ToList(); // Nuke dead items.

            vScrollBar.RenderSize = new Rectangle(0, 0, 16, RenderSize.Height - 16);
            vScrollBar.Transform.LocalPosition2D = new Vector2(RenderSize.Right - 16 , 0);

            hScrollBar.RenderSize = new Rectangle(0, 0, RenderSize.Width - 16, 16);
            hScrollBar.Transform.LocalPosition2D = new Vector2(0, RenderSize.Height - 16);

            int sq = (int)Math.Sqrt(Items.Count);

            Vector2 s = new Vector2(RenderSize.Width, RenderSize.Height) / ((Vector2.One * (IconSize + padding)));

            Vector2 padd = Vector2.One * padding;
            Vector2 offset = (new Vector2(RenderSize.Width, RenderSize.Height) * .25f) + new Vector2(8, 8);// + new Vector2(IconSize, IconSize);

            s = new Vector2(Math.Max(s.X, sq), sq + 1);

            Point contentSize = new Point((int)(s.X * (IconSize.X + padding.X)), (int)(s.Y * (IconSize.Y + padding.Y)));

            for (int y = 0; y < (int)s.Y; y++)
            {
                for (int x = 0; x < (int)s.X; x++)
                {
                    int idx = x + (y * (int)s.X);
                    Vector2 o = new Vector2(x, y);
                    Vector2 p = o * IconSize + (padd * o) + new Vector2(0, 8);
                    if (idx < Items.Count)
                    {
                        Items[idx].Transform.LocalPosition2D = new Vector2(x, y) * (IconSize + padding);
                    }
                }
            }

            content.RenderSize = new Rectangle(0, 0, contentSize.X, contentSize.Y);

            if (content.RenderSize.Width <= RenderSize.Width)
            {
                hScrollBar.Visible = false;
                vScrollBar.RenderSize = new Rectangle(0, 0, 16, RenderSize.Height);
            }
            else
                hScrollBar.Visible = true;

            if (content.RenderSize.Height <= RenderSize.Height)
            {
                vScrollBar.Visible = false;
                hScrollBar.RenderSize = new Rectangle(0, 0, RenderSize.Width, 16);
            }
            else
                vScrollBar.Visible = true;

            base.Update(gameTime);
        }

        public void AddItem(string text, string imgAsset = "", object tagData = null)
        {
            ListItemBase newItem = new ListItemBase(Game, new Rectangle(0, 0, (int)(IconSize.X + padding.X), (int)(IconSize.Y + padding.Y)), text, fontAsset, imgAsset, new Rectangle(0, 0, (int)IconSize.X, (int)IconSize.Y));
            newItem.Transform.Parent = this;
            newItem.Tag = tagData;
            newItem.TextColor = Color.Black;
            newItem.BorderThickness = Vector2.Zero;
            newItem.ButtonDownColor = new Color(255, 255, 255, 100);
            newItem.BackgroundColor = Color.Transparent;
            newItem.BorderColor = Color.Transparent;
            newItem.IconColor = Color.White;
            newItem.TextOffset = new Vector2(0, IconSize.Y);
            newItem.IconOffset = new Vector2(4, -2);
            newItem.OnMouseClickEvent += ItemSelected;
            newItem.OnMouseDoubleClickEvent += UseItem;

            newItem.Initialize();
            Items.Add(newItem);
            content.AddChild(newItem);
        }

        public void RemoveItem(ListItemBase item)
        {
            item.OnMouseClickEvent -= ItemSelected;

            Items.Remove(item);
            item.Visible = false;
        }

        public void ClearList()
        {
            int cnt = Items.Count;

            for (int i = cnt - 1; i >= 0; i--)
            {
                RemoveItem(Items[i]);
            }

            
            Items = new List<ListItemBase>();
        }

        void ItemSelected(object sender, bool leftBtn, Vector2 pos)
        {
            if (OnItemSelectedEvent != null)
                OnItemSelectedEvent(this, sender);
        }

        void UseItem(object sender, bool leftBtn, Vector2 pos)
        {
            if (OnItemDoubleClickedEvent != null)
                OnItemDoubleClickedEvent(this, sender);
        }
    }
}
