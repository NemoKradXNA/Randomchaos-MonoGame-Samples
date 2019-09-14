using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using RandomchaosGeoClipMapping.Terrain.GeoClipMapping;
using RandomchaosMGBase.BaseClasses;

namespace RandomchaosGeoClipMapping
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Base3DCamera camera;

        List<GeoClipMap> maps = new List<GeoClipMap>();

        protected int currentMap = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.GraphicsProfile = GraphicsProfile.HiDef;

            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;

            IsMouseVisible = true;

            camera = new Base3DCamera(this, .5f, 20000);
            camera.Position = new Vector3(0, 10, 0);
            Components.Add(camera);
            Services.AddService(typeof(Base3DCamera), camera);


            string heightMap = "Textures/TerrainMaps/BigTest";
            maps.Add(GetMapType(GeoClipMapRenderTypesEnum.Textured, heightMap));
            maps.Add(GetMapType(GeoClipMapRenderTypesEnum.WireFrame, heightMap));
            maps.Add(GetMapType(GeoClipMapRenderTypesEnum.FlatWireFrame, heightMap));

            SetMap(0);

            foreach (GeoClipMap map in maps)
                Components.Add(map);
        }

        void SetMap(int mapIdx)
        {
            maps[currentMap].Enabled = false;
            currentMap = mapIdx;
            maps[currentMap].Enabled = true;
        }

        GeoClipMap GetMapType(GeoClipMapRenderTypesEnum type, string heightMap)
        {
            GeoClipMap map = new GeoClipMap(this, 255, heightMap);
            map.RenderType = type;
            map.Position = new Vector3(0, 0, 0);
            map.Enabled = false;

            return map;
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float speedTran = .1f;
            float speedRot = .01f;

            KeyboardState thisKBState = Keyboard.GetState();

            if (thisKBState.IsKeyDown(Keys.W) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y > 0)
                camera.Translate(Vector3.Forward * speedTran);
            if (thisKBState.IsKeyDown(Keys.S) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y < 0)
                camera.Translate(Vector3.Backward * speedTran);
            if (thisKBState.IsKeyDown(Keys.A) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X < 0)
                camera.Translate(Vector3.Left * speedTran);
            if (thisKBState.IsKeyDown(Keys.D) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X > 0)
                camera.Translate(Vector3.Right * speedTran);

            if (thisKBState.IsKeyDown(Keys.Left) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X < 0)
                camera.Rotate(Vector3.Up, speedRot);
            if (thisKBState.IsKeyDown(Keys.Right) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X > 0)
                camera.Rotate(Vector3.Up, -speedRot);
            if (thisKBState.IsKeyDown(Keys.Up) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y > 0)
                camera.Rotate(Vector3.Right, speedRot);
            if (thisKBState.IsKeyDown(Keys.Down) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y < 0)
                camera.Rotate(Vector3.Right, -speedRot);

            if (thisKBState.IsKeyDown(Keys.F1))
                SetMap(0);
            if (thisKBState.IsKeyDown(Keys.F2))
                SetMap(1);
            if (thisKBState.IsKeyDown(Keys.F3))
                SetMap(2);

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


            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            spriteBatch.DrawString(Content.Load<SpriteFont>("Fonts/hudFont"), "Camera Controls: [WASD - Translate] [Arrow Keys - Rotate]", new Vector2(8, 8), Color.Gold);
            spriteBatch.DrawString(Content.Load<SpriteFont>("Fonts/hudFont"), "[F1] - Textured", new Vector2(8, 24), Color.Gold);
            spriteBatch.DrawString(Content.Load<SpriteFont>("Fonts/hudFont"), "[F2] - Wireframe showing clip map and heights", new Vector2(8, 40), Color.Gold);
            spriteBatch.DrawString(Content.Load<SpriteFont>("Fonts/hudFont"), "[F3] - Wireframe showing clip map flat", new Vector2(8, 56), Color.Gold);
            spriteBatch.DrawString(Content.Load<SpriteFont>("Fonts/hudFont"), "ESC - Exit", new Vector2(8, 72), Color.Gold);
            spriteBatch.End();
        }
    }
}
