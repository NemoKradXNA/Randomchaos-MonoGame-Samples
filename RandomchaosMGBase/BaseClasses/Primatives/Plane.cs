using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RandomchaosMGBase.BaseClasses.Primatives
{
    public class Plane : PrimativeBase
    {        
        public Plane(Game game) : base(game) { }
        public Plane(Game game, string effectAsset = "") : base(game, effectAsset)
        {
        }

        protected override void BuildData()
        {
            int SquareSize = 10;

            int[] index = new int[(SquareSize - 1) * (SquareSize - 1) * 6];
            Vector2 uv = Vector2.Zero;
            Vector3 center = new Vector3(SquareSize, 0, SquareSize) * .5f;

            for (int x = 0; x < SquareSize; x++)
            {
                for (int y = 0; y < SquareSize; y++)
                {
                    uv = new Vector2(x, y) / (float)(SquareSize - 1);
                    VertexList.Add(new Vector3(x, 0, y) - center);
                    NormalList.Add(Vector3.Up);
                    TexCoordList.Add(uv);
                    TangentList.Add(new Vector3(-1, 0, 1));
                    ColorList.Add(Color.White);
                }
            }

            for (int x = 0; x < SquareSize - 1; x++)
            {
                for (int y = 0; y < SquareSize - 1; y++)
                {
                    index[(x + y * (SquareSize - 1)) * 6] = ((x + 1) + (y + 1) * SquareSize);
                    index[(x + y * (SquareSize - 1)) * 6 + 1] = ((x + 1) + y * SquareSize);
                    index[(x + y * (SquareSize - 1)) * 6 + 2] = (x + y * SquareSize);

                    index[(x + y * (SquareSize - 1)) * 6 + 3] = ((x + 1) + (y + 1) * SquareSize);
                    index[(x + y * (SquareSize - 1)) * 6 + 4] = (x + y * SquareSize);
                    index[(x + y * (SquareSize - 1)) * 6 + 5] = (x + (y + 1) * SquareSize);
                }
            }

            IndexList.AddRange(index);
        }        
    }
}
