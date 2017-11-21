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

        public ListBoxBase(Game game, Rectangle sizeRect, string fontAsset) : base(game, sizeRect, null)
        {
            this.fontAsset = fontAsset;
            Items = new List<ListItemBase>();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Children = Children.Where(d => d.Visible).ToList(); // Nuke dead items.


            int sq = (int)Math.Sqrt(Items.Count);

            Vector2 s = new Vector2(RenderSize.Width, RenderSize.Height) / ((Vector2.One * (IconSize + padding)));

            Vector2 padd = Vector2.One * padding;
            Vector2 offset = (new Vector2(RenderSize.Width, RenderSize.Height) * .25f) + new Vector2(8, 8);// + new Vector2(IconSize, IconSize);

            s = new Vector2(Math.Max(s.X, sq), sq + 1);

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
        }

        public void AddItem(string text, string imgAsset = "", object tagData = null)
        {
            ListItemBase newItem = new ListItemBase(Game, new Rectangle(0, 0,(int)(IconSize.X + padding.X),(int)(IconSize.Y + padding.Y)), text, fontAsset, imgAsset, new Rectangle(0, 0, (int)IconSize.X, (int)IconSize.Y));
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
            AddChild(newItem);
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
