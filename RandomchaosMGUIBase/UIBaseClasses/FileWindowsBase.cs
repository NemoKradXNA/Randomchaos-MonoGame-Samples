using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RandomchaosMGBase;
using RandomchaosMGBase.InputManagers;

namespace RandomchaosMGUIBase.UIBaseClasses
{
    public class FileWindowsBase : WindowBase
    {
        LabelBase lblFolder;
        TextBoxBase txtFolder;
        ButtonBase btnBack;

        LabelBase lblFile;
        TextBoxBase txtFile;

        ListBoxBase lstContent;

        ButtonBase btnOK;
        ButtonBase btnCancel;

        public WindowCloseStatesEnum CloseState = WindowCloseStatesEnum.Cancel;

        public ListItemBase SelectedItem;
        public string SelectedFile = null;

        public string CurrentFolder { get; set; }

        public FileWindowsBase(Game game, Rectangle sizeRect, string fontAsset, string titleText, string backgroundAsset = null) : base(game, sizeRect, fontAsset, titleText, backgroundAsset)
        {
            MinDimensions = new Point(450, 450);

            lblFolder = new LabelBase(game, fontAsset, "Folder:");
            lblFolder.TextColor = Color.Black;
            lblFolder.Transform.LocalPosition2D = new Vector2(50, 40);
            AddChild(lblFolder);

            txtFolder = new TextBoxBase(game, new Rectangle(0, 0, sizeRect.Width - 150, 24), fontAsset);
            txtFolder.Transform.LocalPosition2D = new Vector2(110, 40);
            txtFolder.TextOffset = new Vector2(4, 4);
            txtFolder.BackgroundColor = Color.White;
            txtFolder.OnLostFocusEvent += FolderUpdate;
            AddChild(txtFolder);

            btnBack = new ButtonBase(game, new Rectangle(0, 0, 24, 24), "", fontAsset, "Textures/Icons/Back", new Rectangle(0, 0, 24, 24));
            btnBack.Transform.LocalPosition2D = new Vector2(sizeRect.Width - 32, 40);
            btnBack.IconColor = Color.White;
            btnBack.OnMouseClickEvent += FolderUp;
            AddChild(btnBack);

            lblFile = new LabelBase(game, fontAsset, "Selected File:");
            lblFile.TextColor = Color.Black;
            lblFile.Transform.LocalPosition2D = new Vector2(8, 80);
            AddChild(lblFile);

            txtFile = new TextBoxBase(game, new Rectangle(0, 0, sizeRect.Width - 150, 24), fontAsset);
            txtFile.Transform.LocalPosition2D = new Vector2(110, 80);
            txtFile.TextOffset = new Vector2(4, 4);
            txtFile.BackgroundColor = Color.White;
            AddChild(txtFile);

            lstContent = new ListBoxBase(game, new Rectangle(0, 0, sizeRect.Width - 64, sizeRect.Height - 200), fontAsset);
            lstContent.Transform.LocalPosition2D = new Vector2(32, 120);
            AddChild(lstContent);

            btnOK = new ButtonBase(game, new Rectangle(0, 0, 100, 32), "OK", fontAsset);
            btnOK.TextColor = Color.Black;
            btnOK.Transform.LocalPosition2D = new Vector2(100, sizeRect.Height - 64);
            btnOK.OnMouseClickEvent += CloseFileWindow;
            AddChild(btnOK);

            btnCancel = new ButtonBase(game, new Rectangle(0, 0, 100, 32), "Cancel", fontAsset);
            btnCancel.TextColor = Color.Black;
            btnCancel.Transform.LocalPosition2D = new Vector2(sizeRect.Width-200, sizeRect.Height - 64);
            AddChild(btnCancel);
        }

        public override void Initialize()
        {
            base.Initialize();

            txtFolder.Text = CurrentFolder;
            lstContent.OnItemSelectedEvent += ItemSelected;
            lstContent.OnItemDoubleClickedEvent += ItemDoubleCicked;

            UpdList();
        }

        void FolderUp(object sender,bool leftBtn, Vector2 point)
        {
            if (leftBtn)
            {
                if (txtFolder.Text.Contains("\\"))
                {
                    string upFolder = txtFolder.Text.Substring(0, txtFolder.Text.LastIndexOf("\\"));

                    if (Directory.Exists(upFolder))
                    {
                        if (upFolder.Contains("\\"))
                            txtFolder.Text = upFolder;
                        else
                            txtFolder.Text = upFolder + "\\";

                        UpdList();
                    }
                }
            }
        }

        void FolderUpdate(object sender)
        {
            UpdList();
        }

        public override void Update(GameTime gameTime)
        {
            if (IsReSizing)
            {
                // Re set item positions.
                txtFolder.RenderSize = new Rectangle(txtFolder.RenderSize.X, txtFolder.RenderSize.Y, RenderSize.Width-150, txtFolder.RenderSize.Height);
                txtFile.RenderSize = new Rectangle(txtFolder.RenderSize.X, txtFolder.RenderSize.Y, RenderSize.Width - 150, txtFolder.RenderSize.Height);
                btnBack.Transform.LocalPosition2D = new Vector2(RenderSize.Width - 32, 40);

                //lstContent.Transform.LocalPosition2D = new Vector2(100, RenderSize.Height - 64);
                lstContent.RenderSize = new Rectangle(lstContent.RenderSize.X, lstContent.RenderSize.Y, RenderSize.Width - 64, RenderSize.Height - 200);

                btnOK.Transform.LocalPosition2D = new Vector2(100, RenderSize.Height - 64);
                btnCancel.Transform.LocalPosition2D = new Vector2(RenderSize.Width - 200, RenderSize.Height - 64);
            }

            base.Update(gameTime);
        }

        void ItemSelected(object sender, object item)
        {
            SelectedItem = (ListItemBase)item;
            txtFile.Text = SelectedItem.Text.Replace("\n", "");
        }

        void ItemDoubleCicked(object sender, object item)
        {
            string folder = ((ListItemBase)item).Tag.ToString();

            if (Directory.Exists(folder))
            {
                txtFolder.Text = folder;
                UpdList();
            }
            else
            {
                SelectedItem = (ListItemBase)item;

                // it's a file to open.
                CloseState = WindowCloseStatesEnum.Positive;
                CloseFileWindow(this, true, Vector2.Zero);
            }
        }

        void CloseFileWindow(object sender, bool leftBtn, Vector2 point)
        {
            if (leftBtn)
            {
                if (sender == btnOK)
                {
                    CloseState = WindowCloseStatesEnum.Positive;
                }

                if (sender == btnCancel)
                {
                    SelectedFile = null;
                    CloseState = WindowCloseStatesEnum.Cancel;
                }

                if (SelectedItem != null)
                    SelectedFile = SelectedItem.Tag.ToString();
            }

            base.CloseThisWindow(sender, leftBtn, point);
        }

        void FolderUp(object sender, Vector2 point)
        {
            if (txtFolder.Text.Contains("\\"))
            {
                string upFolder = txtFolder.Text.Substring(0, txtFolder.Text.LastIndexOf("\\"));

                if (Directory.Exists(upFolder))
                {
                    if (upFolder.Contains("\\"))
                        txtFolder.Text = upFolder;
                    else
                        txtFolder.Text = upFolder + "\\";

                    UpdList();
                }
            }
        }

        void UpdList()
        {
            if (Directory.Exists(txtFolder.Text))
            {
                // Clear list and re build from content.
                lstContent.ClearList();
                string[] folders = Directory.GetDirectories(txtFolder.Text);

                string[] files = Directory.GetFiles(txtFolder.Text);

                foreach (string folder in folders)
                {
                    string folderName = folder.Substring(folder.LastIndexOf("\\") + 1);
                    lstContent.AddItem(folderName, "Textures/Icons/folder", folder);
                }

                foreach (string file in files)
                {
                    string icon = "Textures/Icons/file";
                    string fileName = file.Substring(file.LastIndexOf("\\") + 1);
                    string fileExt = fileName.Substring(fileName.LastIndexOf('.') + 1);
                    
                    // You would use different icons for different extensions.

                    lstContent.AddItem(fileName, icon, file);
                }
            }
        }
    }
}
