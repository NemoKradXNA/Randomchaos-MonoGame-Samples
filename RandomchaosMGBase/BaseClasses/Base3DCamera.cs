using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RandomchaosMGBase.BaseClasses
{
    public class Base3DCamera : GameComponent, IHasTransform
    {
        public Transform Transform { get; set; }

        /// <summary>
        /// World
        /// </summary>
        public Matrix World
        {
            get { return Transform.World; }
        }


        public Vector3 Scale
        {
            get { return Transform.Scale; }
            set { Transform.Scale = value; }
        }


        public Vector3 Position
        {
            get { return Transform.Position; }
            set { Transform.Position = value; }
        }

        public Quaternion Rotation
        {
            get { return Transform.Rotation; }
            set { Transform.Rotation = value; }
        }

        /// <summary>
        /// View
        /// </summary>
        protected Matrix view;
        /// <summary>
        /// View
        /// </summary>
        public Matrix View
        {
            get { return view; }
            set { view = value; }
        }
        /// <summary>
        /// Projection
        /// </summary>
        protected Matrix projection;
        /// <summary>
        /// Projection
        /// </summary>
        public Matrix Projection
        {
            get { return projection; }
        }
        /// <summary>
        /// View port
        /// </summary>
        public Viewport Viewport;

        /// <summary>
        /// Frustum
        /// </summary>
        public BoundingFrustum Frustum
        {
            get
            {
                return new BoundingFrustum(Matrix.Multiply(View, Projection));
            }
        }

        /// <summary>
        /// View ports min depth
        /// </summary>
        protected float minDepth;
        /// <summary>
        /// Viewports max depth.
        /// </summary>
        protected float maxDepth;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="game"></param>
        /// <param name="minDepth"></param>
        /// <param name="maxDepth"></param>
        public Base3DCamera(Game game, float minDepth, float maxDepth)
            : base(game)
        {
            Transform = new Transform();
            this.minDepth = minDepth;
            this.maxDepth = maxDepth;
        }
        /// <summary>
        /// Initialization
        /// </summary>
        public override void Initialize()
        {
            Viewport = Game.GraphicsDevice.Viewport;
            Viewport.MinDepth = minDepth;
            Viewport.MaxDepth = maxDepth;
        }

        /// <summary>
        /// Method to update.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            Transform.Update();
            view = Matrix.Invert(World);

            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.Pi / 4, (float)Viewport.Width / (float)Viewport.Height, Viewport.MinDepth, Viewport.MaxDepth);
            base.Update(gameTime);
        }
        public void Translate(Vector3 distance)
        {
            Transform.Translate(distance);
        }

        public void Rotate(Vector3 axis, float angle)
        {
            Transform.Rotate(axis, angle);
        }

    }
}
