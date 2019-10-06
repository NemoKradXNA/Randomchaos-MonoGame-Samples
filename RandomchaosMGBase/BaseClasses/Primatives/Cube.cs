using Microsoft.Xna.Framework;

namespace RandomchaosMGBase.BaseClasses.Primatives
{
    public class Cube : PrimativeBase
    {
        public Cube(Game game) : base(game) { }
        public Cube(Game game, string effectAsset) : base(game, effectAsset)
        {

        }

        protected override void BuildData()
        {
            VertexList.AddRange(new Vector3[]
            {
                new Vector3(.5f, -.5f, .5f), new Vector3(-.5f, -.5f, .5f), new Vector3(-.5f, .5f, .5f),new Vector3(.5f, .5f, .5f),
                new Vector3(.5f, -.5f, -.5f), new Vector3(-.5f, -.5f, -.5f), new Vector3(-.5f, .5f, -.5f), new Vector3(.5f, .5f, -.5f),
                new Vector3(.5f, .5f, -.5f), new Vector3(-.5f, .5f, -.5f), new Vector3(-.5f, .5f, .5f),new Vector3(.5f, .5f, .5f),
                new Vector3(.5f, -.5f, -.5f),new Vector3(-.5f, -.5f, -.5f),new Vector3(-.5f, -.5f, .5f),new Vector3(.5f, -.5f, .5f),
                new Vector3(-.5f, -.5f, .5f),new Vector3(-.5f, -.5f, -.5f),new Vector3(-.5f, .5f, -.5f),new Vector3(-.5f, .5f, .5f),
                new Vector3(.5f, -.5f, .5f),new Vector3(.5f, -.5f, -.5f),new Vector3(.5f, .5f, -.5f),new Vector3(.5f, .5f, .5f)
            });

            NormalList.AddRange(new Vector3[]
            {
                Vector3.Backward,Vector3.Backward,Vector3.Backward,Vector3.Backward,
                Vector3.Forward,Vector3.Forward,Vector3.Forward,Vector3.Forward,
                Vector3.Up,Vector3.Up,Vector3.Up,Vector3.Up,
                Vector3.Down,Vector3.Down,Vector3.Down,Vector3.Down,
                Vector3.Left,Vector3.Left,Vector3.Left,Vector3.Left,
                Vector3.Right,Vector3.Right,Vector3.Right,Vector3.Right,
            });

            //TangentList.AddRange(new Vector3[]
            //{
            //    Vector3.Cross(VertexList[0], -NormalList[0]),Vector3.Cross(VertexList[1], -NormalList[1]),Vector3.Cross(VertexList[2], -NormalList[2]),Vector3.Cross(VertexList[3], -NormalList[3]),
            //    Vector3.Cross(VertexList[4], -NormalList[4]),Vector3.Cross(VertexList[5], -NormalList[5]),Vector3.Cross(VertexList[6], -NormalList[6]),Vector3.Cross(VertexList[7], -NormalList[7]),
            //    Vector3.Cross(VertexList[8], -NormalList[8]),Vector3.Cross(VertexList[9], -NormalList[9]),Vector3.Cross(VertexList[10], -NormalList[10]),Vector3.Cross(VertexList[11], -NormalList[11]),
            //    Vector3.Cross(VertexList[12], -NormalList[12]),Vector3.Cross(VertexList[13], -NormalList[13]),Vector3.Cross(VertexList[14], -NormalList[14]),Vector3.Cross(VertexList[15], -NormalList[15]),
            //    Vector3.Cross(VertexList[16], -NormalList[16]),Vector3.Cross(VertexList[17], -NormalList[17]),Vector3.Cross(VertexList[18], -NormalList[18]),Vector3.Cross(VertexList[19], -NormalList[19]),
            //    Vector3.Cross(VertexList[20], -NormalList[20]),Vector3.Cross(VertexList[21], -NormalList[21]),Vector3.Cross(VertexList[22], -NormalList[22]),Vector3.Cross(VertexList[23], -NormalList[23]),

            //});
            TangentList.AddRange(new Vector3[]
            {
                Vector3.Right,Vector3.Right,Vector3.Right,Vector3.Right,
                Vector3.Left,Vector3.Left,Vector3.Left,Vector3.Left,
                Vector3.Left,Vector3.Left,Vector3.Left,Vector3.Left,
                Vector3.Right,Vector3.Right,Vector3.Right,Vector3.Right,
                Vector3.Up,Vector3.Up,Vector3.Up,Vector3.Up,
                Vector3.Down,Vector3.Down,Vector3.Down,Vector3.Down

            });

            TexCoordList.AddRange(new Vector2[]
            {
                new Vector2(1, 1),new Vector2(0, 1),new Vector2(0,0),new Vector2(1, 0),
                new Vector2(0, 1),new Vector2(1, 1),new Vector2(1,0),new Vector2(0, 0),
                new Vector2(0, 1),new Vector2(1, 1),new Vector2(1,0),new Vector2(0, 0),
                new Vector2(1, 1),new Vector2(0, 1),new Vector2(0,0),new Vector2(1, 0),
                new Vector2(1, 1),new Vector2(0, 1),new Vector2(0,0),new Vector2(1, 0),
                new Vector2(1, 1),new Vector2(0, 1),new Vector2(0,0),new Vector2(1, 0),
                new Vector2(0, 1),new Vector2(1, 1),new Vector2(1,0),new Vector2(0, 0),
            });

            ColorList.AddRange(new Color[]
            {
                Color.White,Color.White,Color.White,Color.White,
                Color.White,Color.White,Color.White,Color.White,
                Color.White,Color.White,Color.White,Color.White,
                Color.White,Color.White,Color.White,Color.White,
                Color.White,Color.White,Color.White,Color.White,
                Color.White,Color.White,Color.White,Color.White,
            });


            IndexList.AddRange(new int[]
            {
                0, 1, 2, 2, 3, 0, // Front
                4, 7, 6, 6, 5, 4, // Back
                8, 11, 10, 10, 9, 8, // Top
                12, 13, 14, 14, 15, 12, // Bottom
                16, 17, 18, 18, 19, 16, // Left
                20, 23, 22, 22, 21, 20, // Right
            });
        }
    }
}
