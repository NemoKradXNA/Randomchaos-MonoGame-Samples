using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using RandomchaosMGBase.InputManagers;

using RandomchaosMGUIBase.UIBaseClasses;

/// <summary>
/// Wiki : https://github.com/NemoKradXNA/Randomchaos-MonoGame-Samples/wiki
/// </summary>
namespace RandomchaosMGUIBaseSandBox
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        MouseStateManager msm;
        KeyboardStateManager kbm;
        MouseRenderer mr;
        
        ButtonBase btnShowWindow;
        ButtonBase btnShowFileBrowser;

        ScreenCanvasBase screenCanvas; // Container for controll used directly on the screen...

        Texture2D t;

        FileWindowsBase fileBrowser;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = 1208;
            graphics.ApplyChanges();

            Content.RootDirectory = "Content";

            msm = new MouseStateManager(this);
            kbm = new KeyboardStateManager(this);

            screenCanvas = new ScreenCanvasBase(this, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight));
            Components.Add(screenCanvas);

            btnShowWindow = new ButtonBase(this, new Rectangle(graphics.PreferredBackBufferWidth - 108, graphics.PreferredBackBufferHeight - 40, 100, 32), "Show Window", "Fonts/Airel12");
            btnShowWindow.TextColor = Color.Black;
            btnShowWindow.ShadowOffset = Vector2.Zero;
            btnShowWindow.OnMouseClickEvent += ShowWindow;
            screenCanvas.AddChild(btnShowWindow);

            btnShowFileBrowser = new ButtonBase(this, new Rectangle(graphics.PreferredBackBufferWidth - 266, graphics.PreferredBackBufferHeight - 40, 150, 32), "Show File Browser", "Fonts/Airel12");
            btnShowFileBrowser.TextColor = Color.Black;
            btnShowFileBrowser.ShadowOffset = Vector2.Zero;
            btnShowFileBrowser.OnMouseClickEvent += ShowWindow;
            screenCanvas.AddChild(btnShowFileBrowser);
            
            // Add this last of all.
            mr = new MouseRenderer(this, "Textures/Icons/Mouse");
            Components.Add(mr);
        }

        Point windowSp = new Point(100, 100);
        Point newWOffset = new Point(50, 50);
        int n = 0;

        WindowBase lastWindow = null;

        void ShowWindow(object sender, bool leftButton, Vector2 position)
        {
            WindowBase win = null;
            if (sender == btnShowWindow)
                win = ShowWindow();
            if (sender == btnShowFileBrowser)
                win = ShowFileBrowser();

            if (windowSp.X + newWOffset.X + 400 > graphics.PreferredBackBufferWidth)
                windowSp.X = 0;
            else
                windowSp.X += newWOffset.X;

            if (windowSp.Y + newWOffset.Y + 200 > graphics.PreferredBackBufferHeight)
                windowSp.Y = 0;
            else
                windowSp.Y += newWOffset.Y;

            if (win != null)
            {
                lastWindow = win;
                Components.Add(win);
            }
        }

        WindowBase ShowWindow()
        {
            WindowBase win = new WindowBase(this, new Rectangle(windowSp.X, windowSp.Y, 400, 200), "Fonts/Airel12", "Test Window" + n.ToString());
            win.OnCloseWindowEvent += CloseWindow;
            win.Initialize();
            win.Visible = false;

            
            n++;

            return win;
        }

        FileWindowsBase ShowFileBrowser()
        {
            if (fileBrowser == null)
            {
                fileBrowser = new FileWindowsBase(this, new Rectangle(windowSp.X, windowSp.Y, 600, 500), "Fonts/Airel12", "File Browser");
                fileBrowser.CurrentFolder = "C:\\";//@"C:\Development\MonoGame\Randomchaos_MonoGame_Samples\RandomchaosMG\Sandboxs\RandomchaosMGUIBaseSandBox\Content\";
                fileBrowser.OnCloseWindowEvent += CloseWindow;
                fileBrowser.Initialize();
                fileBrowser.Visible = false;
                return fileBrowser;
            }

            return null;
        }

        void CloseWindow(object sender)
        {
            Components.Remove((WindowBase)sender);
            if (sender == fileBrowser)
                fileBrowser = null;
            sender = null;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            t = new Texture2D(GraphicsDevice, 1, 1);
            t.SetData<Color>(new Color[] { Color.White });
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            msm.PreUpdate(gameTime);
            kbm.PreUpdate(gameTime);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //Window.Title = ControlBase.FocusControl != null ? ControlBase.FocusControl.ToString() : "";

            // TODO: Add your update logic here

            base.Update(gameTime);

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);


            //spriteBatch.Begin();
            //spriteBatch.Draw(t, window.ScissorRectangle, new Color(0, 100, 0, 128));
            //spriteBatch.Draw(t, window.txtTest.ScissorRectangle, new Color(0,100,0,128));
            //spriteBatch.End();

            if (lastWindow != null)
            {
                lastWindow.Visible = true;
                lastWindow = null;
            }
        }
    }
}
