using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RandomchaosGeoClipMapping.Terrain.GeoClipMapping
{
    public class GeoClipMapFootPrintBase : DrawableGameComponent
    {
        public VertexPositionColor[] verts;
        protected Vector3[] positions;
        protected short[] index;
        public short height;
        public short width;

        public Vector3 Position = Vector3.Zero;
        public Vector3 Scale = Vector3.One;
        public Quaternion Orientation = Quaternion.Identity;
        public Matrix World = Matrix.Identity;

        public Color color = Color.Black;

        public BoundingBox Bounds
        {
            get { return new BoundingBox(verts[verts.Length - 1].Position, verts[0].Position); }
        }

        public BoundingBox getBounds(Matrix world)
        {
            return new BoundingBox(Vector3.Transform(verts[verts.Length - 1].Position, world), Vector3.Transform(verts[0].Position, world));
        }
        public GeoClipMapFootPrintBase(Game game) : base(game)
        {
        }

        protected override void LoadContent()
        {
            index = new short[(width - 1) * (height - 1) * 6];
            for (short x = 0; x < width - 1; x++)
            {
                for (short y = 0; y < height - 1; y++)
                {
                    index[(x + y * (width - 1)) * 6] = (short)((x + 1) + (y + 1) * width);
                    index[(x + y * (width - 1)) * 6 + 1] = (short)((x + 1) + y * width);
                    index[(x + y * (width - 1)) * 6 + 2] = (short)(x + y * width);

                    index[(x + y * (width - 1)) * 6 + 3] = (short)((x + 1) + (y + 1) * width);
                    index[(x + y * (width - 1)) * 6 + 4] = (short)(x + y * width);
                    index[(x + y * (width - 1)) * 6 + 5] = (short)(x + (y + 1) * width);
                }
            }

            positions = new Vector3[verts.Length];

            // position data.
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    positions[x + y * width] = new Vector3(y, 0, x);
                    verts[x + y * width].Color = color;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            World = Matrix.CreateScale(Scale) * Matrix.CreateFromQuaternion(Orientation) * Matrix.CreateTranslation(Position);

            for (int p = 0; p < positions.Length; p++)
            {
                verts[p].Position = Vector3.Transform(positions[p], World);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Game.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, verts, 0, verts.Length, index, 0, (width - 1) * (height - 1) * 2);
        }

        public void Rotate(Vector3 axis, float angle)
        {
            axis = Vector3.Transform(axis, Matrix.CreateFromQuaternion(Orientation));
            Orientation = Quaternion.Normalize(Quaternion.CreateFromAxisAngle(axis, angle) * Orientation);
        }
    }
}
