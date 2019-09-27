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
        SpriteBatch spriteBatch;

        KeyboardStateManager kbm;
        Base3DCamera camera;

        RCLayeredFire Fire3D;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.GraphicsProfile = GraphicsProfile.HiDef;

            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;

            IsMouseVisible = true;

            kbm = new KeyboardStateManager(this);

            camera = new Base3DCamera(this, .5f, 20000);
            camera.Position = new Vector3(0, 0, 10);
            Components.Add(camera);
            Services.AddService(typeof(Base3DCamera), camera);

            Fire3D = new RCLayeredFire(this, 6);
            Components.Add(Fire3D);
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
        }
    }
}
//public class RCLayeredFire
//{
//    public bool Colapsed;
//    private RCModel[] flame;
//    private RCModel[] flame2;
//    private string flameModelAsset;
//    private string flameTextureAsset;
//    private int flameLayers;
//    private float animSpeed;
//    private float tick;
//    private float flameOffset;
//    private Vector3 camerasLastPosition = new Vector3(0, 0, 0);
//    private Texture2D flameTexture;
//    public float AnimationSpeed { get { return animSpeed; } set { animSpeed = value; } }
//    public float FlameOffSet { get { return flameOffset; } set { flameOffset = value; } }

//    public RCLayeredFire(string modelAsset, string textureAsset, int layers, string Name) : base(Name)
//    {
//        flameModelAsset = modelAsset;
//        flameTextureAsset = textureAsset;
//        flameLayers = layers;
//        flame = new RCModel[flameLayers];
//        flame2 = new RCModel[flameLayers];

//        base.Rotate(new Vector3(1, 0, 0), MathHelper.PiOver2);

//        for (int f = 0; f < flameLayers; f++)
//        {
//            flame[f] = new RCModel(flameModelAsset, "flame" + f.ToString());
//            flame[f].Rotation = myRotation;
//            flame2[f] = new RCModel(flameModelAsset, "flame" + f.ToString());
//            flame2[f].Rotation = myRotation;
//            flame2[f].Rotate(new Vector3(0, 0, 1), MathHelper.Pi);
//        }
//    }

//    public void LoadGraphicsContent(GraphicsDevice myDevice, ContentManager myLoader)
//    {
//        flameTexture = myLoader.Load<Texture2D>(flameTextureAsset);

//        for (int f = 0; f < flameLayers; f++)
//        {
//            flame[f].Rotation = myRotation;
//            // Now collapsed to a sigle position to ease rotation.
//            flame[f].Position = new Vector3(myPosition.X, myPosition.Y, myPosition.Z + (((float)f) / 10f));
//            //flame[f].Position = myPosition; 
//            flame[f].SetShader(myShader);
//            flame[f].LoadGraphicsContent(myDevice, myLoader);
//            flame[f].UseBasicRender = false;
//            flame2[f].Rotation = myRotation;
//            flame2[f].Rotate(new Vector3(0, 0, 1), MathHelper.Pi);
//            flame2[f].Position = new Vector3(myPosition.X, myPosition.Y, myPosition.Z - (((float)f) / 10f)); flame2[f].SetShader(myShader);
//            flame2[f].LoadGraphicsContent(myDevice, myLoader);
//        }
//    }

//    public void RenderChildren(GraphicsDevice myDevice)
//    {
//        CullMode cull = myDevice.RenderState.CullMode;
//        bool depthBuffer = myDevice.RenderState.DepthBufferEnable;
//        bool Zwrite = myDevice.RenderState.DepthBufferWriteEnable;
//        bool AlphaEnable = myDevice.RenderState.AlphaBlendEnable;
//        BlendFunction blendOp = myDevice.RenderState.BlendFunction;
//        Blend srcBlend = myDevice.RenderState.SourceBlend;
//        Blend destblend = myDevice.RenderState.DestinationBlend;
//        //if (cull != CullMode.None) 
//        // myDevice.RenderState.CullMode = CullMode.None; 
//        if (depthBuffer != true)
//            myDevice.RenderState.DepthBufferEnable = true;
//        if (Zwrite != true)
//            myDevice.RenderState.DepthBufferWriteEnable = true;
//        if (AlphaEnable != true)
//            myDevice.RenderState.AlphaBlendEnable = true;
//        if (blendOp != BlendFunction.Add)
//            myDevice.RenderState.BlendFunction = BlendFunction.Add;
//        if (srcBlend != Blend.One)
//            myDevice.RenderState.SourceBlend = Blend.One;
//        if (destblend != Blend.One)
//            myDevice.RenderState.DestinationBlend = Blend.One;

//        if (RCCameraManager.ActiveCamera.Position != camerasLastPosition)
//        {
//            Vector3 tminusp = myPosition - RCCameraManager.ActiveCamera.Position;
//            tminusp.Normalize();
//            float angle = (float)Math.Acos(Vector3.Dot(tminusp, Vector3.Forward));
//            //this.Rotate(new Vector3(0, 0, 1), angle); 
//            //this.Revolve(myPosition, new Vector3(0, 0, 1), angle);
//            camerasLastPosition = RCCameraManager.ActiveCamera.Position;
//        }

//        Effect effect = RCShaderManager.GetShader(myShader).Effect;

//        if (effect.Parameters["flameTexture"] != null)
//            effect.Parameters["flameTexture"].SetValue(flameTexture);
//        if (effect.Parameters["ticks"] != null)
//            effect.Parameters["ticks"].SetValue(tick += animSpeed);

//        for (int f = 0; f < flameLayers; f++)
//        {
//            flame[f].Rotation = myRotation;
//            flame[f].Scaling = myScaling;
//            //flame[f].AnimationSpeed = animSpeed; 
//            flame2[f].Rotation = myRotation;
//            flame2[f].Rotate(new Vector3(0, 0, 1), MathHelper.Pi);
//            flame2[f].Scaling = myScaling;
//            flame2[f].AnimationSpeed = animSpeed;

//            if (effect.Parameters["Index"] != null)
//                effect.Parameters["Index"].SetValue(flameOffset + (float)Convert.ToDouble(f) / 10);

//            flame[f].Draw(myDevice);
//            //flame2[f].Draw(myDevice);
//        }

//        if (cull != myDevice.RenderState.CullMode)
//            myDevice.RenderState.CullMode = cull;
//        if (depthBuffer != myDevice.RenderState.DepthBufferEnable)
//            myDevice.RenderState.DepthBufferEnable = depthBuffer;
//        if (Zwrite != myDevice.RenderState.DepthBufferWriteEnable)
//            myDevice.RenderState.DepthBufferWriteEnable = Zwrite;
//        if (AlphaEnable != myDevice.RenderState.AlphaBlendEnable)
//            myDevice.RenderState.AlphaBlendEnable = AlphaEnable;
//        if (blendOp != myDevice.RenderState.BlendFunction)
//            myDevice.RenderState.BlendFunction = blendOp;
//        if (srcBlend != myDevice.RenderState.SourceBlend)
//            myDevice.RenderState.SourceBlend = srcBlend;
//        if (destblend != myDevice.RenderState.DestinationBlend)
//            myDevice.RenderState.DestinationBlend = destblend;
//    }
//}