using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace RandomchaosMGBase.BaseClasses.Primatives
{
    public class ScreenQuad
    {
        VertexPositionTexture[] corners;
        VertexBuffer vb;
        short[] ib;
        //VertexDeclaration vertDec;

        Game Game;

        public ScreenQuad(Game game)
        {
            Game = game;
            corners = new VertexPositionTexture[4];
            corners[0].Position = new Vector3(0, 0, 0);
            corners[0].TextureCoordinate = Vector2.Zero;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public virtual void Initialize()
        {
            //vertDec = VertexPositionTexture.VertexDeclaration;

            corners = new VertexPositionTexture[]
                    {
                        new VertexPositionTexture(
                            new Vector3(0,0,0),
                            new Vector2(1,1)),
                        new VertexPositionTexture(
                            new Vector3(0,0,0),
                            new Vector2(0,1)),
                        new VertexPositionTexture(
                            new Vector3(0,0,0),
                            new Vector2(0,0)),
                        new VertexPositionTexture(
                            new Vector3(0,0,0),
                            new Vector2(1,0))
                    };

            ib = new short[] { 0, 1, 2, 2, 3, 0 };
            vb = new VertexBuffer(Game.GraphicsDevice, typeof(VertexPositionTexture), corners.Length, BufferUsage.None); 
        }

        public virtual void Draw(Vector2 v1, Vector2 v2)
        {
            //Game.GraphicsDevice.VertexDeclaration = vertDec;
            corners[0].Position.X = v2.X; // 1
            corners[0].Position.Y = v1.Y; // -1

            corners[1].Position.X = v1.X; // -1
            corners[1].Position.Y = v1.Y; // -1

            corners[2].Position.X = v1.X; // -1
            corners[2].Position.Y = v2.Y; // 1

            corners[3].Position.X = v2.X; // 1
            corners[3].Position.Y = v2.Y; // 1

            vb.SetData(corners);
            Game.GraphicsDevice.SetVertexBuffer(vb);

            Game.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, corners, 0, 4, ib, 0, 2);
        }
    }
}
