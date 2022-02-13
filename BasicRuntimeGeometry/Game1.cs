using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RandomchaosMGBase.BaseClasses;
using RandomchaosMGBase.InputManagers;
using System.Collections.Generic;

namespace BasicRuntimeGeometry
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        KeyboardStateManager kbm;
        Base3DCamera camera;

        Cube cube;
        Cube cubeMapped;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            kbm = new KeyboardStateManager(this);

            camera = new Base3DCamera(this, .5f, 20000);
            camera.Position = new Vector3(0, 1, 10);
            Components.Add(camera);
            Services.AddService(typeof(Base3DCamera), camera);

            cube = new Cube(this, "Textures/monoGameLogo", "Shaders/BasicShader");
            cube.Transform.Position = Vector3.Left;
            Components.Add(cube);

            // Set the uv values based on the texture we want from our texture atlas.
            List<Vector2> uvMap = new List<Vector2>()
            {
                new Vector2(0, 0),new Vector2(.25f, 0),new Vector2(.25f, .25f),new Vector2(0, .25f), // Front Face: Top left texture (white)
                new Vector2(.5f, 0),new Vector2(.25f, 0),new Vector2(.25f, .25f),new Vector2(.5f, .25f), // Back Face: black
                new Vector2(.5f, 0),new Vector2(.75f, .25f),new Vector2(.75f, 0),new Vector2(.5f, 0), // Top Face: pink
                new Vector2(.75f, .25f),new Vector2(1, .25f),new Vector2(1, .5f),new Vector2(.75f, .5f), // Bottom Face :   Yellow
                new Vector2(0, .5f),new Vector2(.25f, .5f),new Vector2(.25f, .75f),new Vector2(0, .75f), // LEft Face : Green
                new Vector2(1, .75f),new Vector2(.75f, .75f),new Vector2(.75f, 1),new Vector2(1, 1), // Right Face : Dark Red
            };

            cubeMapped = new Cube(this, "Textures/atlas", "Shaders/BasicShader", uvMap);
            cubeMapped.Transform.Position = Vector3.Right;
            Components.Add(cubeMapped);
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
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

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
