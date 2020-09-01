using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using RandomchaosMGBase.BaseClasses;
using RandomchaosMGBase.InputManagers;

namespace RandomchaosMGFire
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        SpriteFont font;

        KeyboardStateManager kbm;
        Base3DCamera camera;

        CPUOldSchoolFire Fire;
        GPUOldSchoolFire Fire2;
        GPUOldSchoolFireBox Fire3;
        RCLayeredFire Fire3D;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.GraphicsProfile = GraphicsProfile.HiDef;

            //graphics.PreferredBackBufferWidth = 1920;
            //graphics.PreferredBackBufferHeight = 1080;

            IsMouseVisible = true;

            kbm = new KeyboardStateManager(this);

            camera = new Base3DCamera(this, .5f, 20000);
            camera.Position = new Vector3(0, 0, 10);
            Components.Add(camera);
            Services.AddService(typeof(Base3DCamera), camera);

            Fire3D = new RCLayeredFire(this, 8);
            Fire3D.Scale = new Vector3(2, 6, 8);
            Fire3D.AnimationSpeed = 5;// .01f;
            Fire3D.FlameOffSet = .5f;

            Components.Add(Fire3D);

            Fire = new CPUOldSchoolFire(this, new Point(400, 240));
            Components.Add(Fire);

            Fire2 = new GPUOldSchoolFire(this, new Point(400,240));
            Components.Add(Fire2);

            Fire3 = new GPUOldSchoolFireBox(this);
            Components.Add(Fire3);

            Fire3D.Enabled = false;
            Fire.Enabled = false;
            Fire2.Enabled = false;
            Fire3.Enabled = false;
            Fire3D.Enabled = true;
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

            font = Content.Load<SpriteFont>("Fonts/hudFont");
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
            kbm.PreUpdate(gameTime);

            base.Update(gameTime);

            if (kbm.KeyPress(Keys.Space))
            { }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || kbm.KeyDown(Keys.Escape))
                Exit();

            // Camera controls..
            float speedTran = .1f;
            float speedRot = .01f;

            if (kbm.KeyDown(Keys.W) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y > 0)
                camera.Translate(Vector3.Forward * speedTran);
            if (kbm.KeyDown(Keys.S) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y < 0)
                camera.Translate(Vector3.Backward * speedTran);
            if (kbm.KeyDown(Keys.A) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X < 0)
                camera.Translate(Vector3.Left * speedTran);
            if (kbm.KeyDown(Keys.D) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X > 0)
                camera.Translate(Vector3.Right * speedTran);

            if (kbm.KeyDown(Keys.Left) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X < 0)
                camera.Rotate(Vector3.Up, speedRot);
            if (kbm.KeyDown(Keys.Right) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X > 0)
                camera.Rotate(Vector3.Up, -speedRot);
            if (kbm.KeyDown(Keys.Up) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y > 0)
                camera.Rotate(Vector3.Right, speedRot);
            if (kbm.KeyDown(Keys.Down) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y < 0)
                camera.Rotate(Vector3.Right, -speedRot);

            if (kbm.KeyPress(Keys.F1))
            {
                Fire3D.Enabled = true;
                Fire.Enabled = !Fire3D.Enabled;
                Fire2.Enabled = !Fire3D.Enabled;
                Fire3.Enabled = !Fire3D.Enabled;
            }

            if (kbm.KeyPress(Keys.F2))
            {
                Fire.Enabled = true;
                Fire2.Enabled = !Fire.Enabled;
                Fire3.Enabled = !Fire.Enabled;
                Fire3D.Enabled = !Fire.Enabled;
            }

            if (kbm.KeyPress(Keys.F3))
            {
                Fire2.Enabled = true;
                Fire.Enabled = !Fire2.Enabled;
                Fire3.Enabled = !Fire2.Enabled;
                Fire3D.Enabled = !Fire2.Enabled;
            }

            if (kbm.KeyPress(Keys.F4))
            {
                Fire3.Enabled = true;
                Fire.Enabled = !Fire3.Enabled;
                Fire2.Enabled = !Fire3.Enabled;
                Fire3D.Enabled = !Fire3.Enabled;
            }

            if (Fire.Enabled || Fire2.Enabled || Fire3.Enabled)
            {
                if (kbm.KeyDown(Keys.R))
                {
                    Fire.Oxygen += .001f;
                    Fire2.Oxygen += .001f;
                    Fire3.Oxygen += .001f;
                }
                if (kbm.KeyDown(Keys.F))
                {
                    Fire.Oxygen -= .001f;
                    Fire2.Oxygen -= .001f;
                    Fire3.Oxygen -= .001f;
                }

                if (kbm.KeyDown(Keys.T))
                {
                    Fire.MinFuel += 1;
                    Fire2.MinFuel += 1;
                    Fire3.MinFuel += 1;
                }
                if (kbm.KeyDown(Keys.G))
                {
                    Fire.MinFuel -= 1;
                    Fire2.MinFuel -= 1;
                    Fire3.MinFuel -= 1;
                }

                if (kbm.KeyDown(Keys.Y))
                {
                    Fire.MaxFuel += 1;
                    Fire2.MaxFuel += 1;
                    Fire3.MaxFuel += 1;
                }
                if (kbm.KeyDown(Keys.H))
                {
                    Fire.MaxFuel -= 1;
                    Fire2.MaxFuel -= 1;
                    Fire3.MaxFuel -= 1;
                }

                if (kbm.KeyPress(Keys.Space))
                {
                    Fire.ShowDebug = !Fire.ShowDebug;
                    Fire2.ShowDebug = Fire.ShowDebug;
                    Fire3.ShowDebug = Fire.ShowDebug;
                }
            }

            if (Fire3D.Enabled)
            {
                if (kbm.KeyDown(Keys.R))
                    Fire3D.NoiseFrequency += .001f;
                if (kbm.KeyDown(Keys.F))
                    Fire3D.NoiseFrequency -= .001f;

                if (kbm.KeyDown(Keys.T))
                    Fire3D.NoiseStrength += .01f;
                if (kbm.KeyDown(Keys.G))
                    Fire3D.NoiseStrength -= .01f;

                if (kbm.KeyDown(Keys.Y))
                    Fire3D.FlameOffSet += .001f;
                if (kbm.KeyDown(Keys.H))
                    Fire3D.FlameOffSet -= .001f;

                if (kbm.KeyDown(Keys.U))
                    Fire3D.AnimationSpeed += .01f;
                if (kbm.KeyDown(Keys.J))
                    Fire3D.AnimationSpeed -= .01f;

                if (kbm.KeyDown(Keys.I))
                    Fire3D.Color = SetColor(Fire3D.Color.ToVector4() + new Vector4(0.01f, 0, 0, 1));
                if (kbm.KeyDown(Keys.K))
                    Fire3D.Color = SetColor(Fire3D.Color.ToVector4() - new Vector4(0.01f, 0, 0, 1));

                if (kbm.KeyDown(Keys.O))
                    Fire3D.Color = SetColor(Fire3D.Color.ToVector4() + new Vector4(0, 0.01f, 0, 1));
                if (kbm.KeyDown(Keys.L))
                    Fire3D.Color = SetColor(Fire3D.Color.ToVector4() - new Vector4(0, 0.01f, 0, 1));

                if (kbm.KeyDown(Keys.P))
                    Fire3D.Color = SetColor(Fire3D.Color.ToVector4() + new Vector4(0, 0, 0.01f, 1));
                if (kbm.KeyDown(Keys.OemSemicolon))
                    Fire3D.Color = SetColor(Fire3D.Color.ToVector4() - new Vector4(0, 0, 0.01f, 1));

                Fire3D.NoiseFrequency = MathHelper.Clamp(Fire3D.NoiseFrequency, 0, 1);
                Fire3D.NoiseStrength = MathHelper.Clamp(Fire3D.NoiseStrength, 0, 10);
                Fire3D.FlameOffSet = MathHelper.Clamp(Fire3D.FlameOffSet, 0, 1);
                Fire3D.AnimationSpeed = MathHelper.Clamp(Fire3D.AnimationSpeed, 0, 10);
            }
        }

        protected Color SetColor(Vector4 newC)
        {
            return new Color(Vector4.Clamp(newC, Vector4.Zero, Vector4.One));
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            base.Draw(gameTime);

            line = 0;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            WriteLine("Camera Controls: [WASD - Translate] [Arrow Keys - Rotate]", Color.Gold);
            WriteLine($"[F1] - Toggle NVIDIA FIre {Fire3D.Enabled}", Color.Gold);
            WriteLine($"[F2] - Toggle Old School CPU Fire {Fire.Enabled}", Color.Gold);
            WriteLine($"[F3] - Toggle Old School GPU Fire {Fire2.Enabled}", Color.Gold);
            WriteLine($"[F4] - Toggle Old School GPU Fire Box {Fire2.Enabled}", Color.Gold);

            WriteLine($"-----------------------------------------------", Color.Gold);

            if (Fire3D.Enabled)
            {
                WriteLine($"[R/F] - Flame Noise Frequency +- {Fire3D.NoiseFrequency}", Color.Gold);
                WriteLine($"[T/G] - Flame Noise Strength +- {Fire3D.NoiseStrength}", Color.Gold);
                WriteLine($"[Y/H] - Flame Offset +- {Fire3D.FlameOffSet}", Color.Gold);
                WriteLine($"[U/J] - Flame Animation Speed +- {Fire3D.AnimationSpeed}", Color.Gold);
                WriteLine($"[I/K] - Flame Color (R) +- : {Fire3D.Color.ToVector4().X}", Color.LimeGreen);
                WriteLine($"[O/L] - Flame Color (G) +- : {Fire3D.Color.ToVector4().Y}", Color.LimeGreen);
                WriteLine($"[P/;] - Flame Color (B) +- : {Fire3D.Color.ToVector4().Z}", Color.LimeGreen);                
            }
            if (Fire.Enabled || Fire2.Enabled || Fire3.Enabled)
            {
                WriteLine($"Fire Type: " + ((Fire2.Enabled || false) ? "GPU" : "CPU"), Color.Gold);
                WriteLine($"[R/F] Toggle Oxygen: {Fire.Oxygen}",  Color.Gold);
                WriteLine($"[T/G] Toggle Min Fuel: {Fire.MinFuel}",  Color.Gold);
                WriteLine($"[Y/H] Toggle Max Fuel: {Fire.MaxFuel}", Color.Gold);
                WriteLine($"Show Debug: {Fire.ShowDebug}", Color.Gold);
            }
            spriteBatch.End();
        }

        int line = 0;
        protected void WriteLine(string msg, Color color)
        {
            spriteBatch.DrawString(font, msg, new Vector2(8, 8 + (font.LineSpacing * line)), Color.Gold);
            line++;
        }
    }
}
