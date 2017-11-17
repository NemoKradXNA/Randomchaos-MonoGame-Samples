using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace RandomchaosMGBase.InputManagers
{
    public class MouseRenderer: DrawableGameComponent, IHasTransform
    {
        MouseStateManager MouseManager { get { return Game.Services.GetService<MouseStateManager>(); } }

        SpriteBatch spriteBatch { get; set; }

        string mouseAsset { get; set; }

        Texture2D mouseTexture { get; set; }

        public Transform Transform { get; set; }

        public Color Color = Color.White;
        public Point Size = new Point(16, 24);

        public MouseRenderer(Game game, string iconAsset) : base(game)
        {
            mouseAsset = iconAsset;
            Transform = new Transform();
        }


        protected override void LoadContent()
        {
            base.LoadContent();

            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            mouseTexture = Game.Content.Load<Texture2D>(mouseAsset);
        }

        public override void Update(GameTime gameTime)
        {
            Transform.Position2D = MouseManager.Position;
            Transform.Update();
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null,null,null,null,Transform.World);

            spriteBatch.Draw(mouseTexture, new Rectangle(0,0, Size.X, Size.Y), Color);

            spriteBatch.End();
        }
    }
}
