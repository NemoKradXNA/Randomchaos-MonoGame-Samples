using Microsoft.Xna.Framework;

namespace RandomchaosMGBase.BaseClasses.Primatives
{
    public class Quad : PrimativeBase
    {
        public Quad(Game game) : base(game) { }
        public Quad(Game game, string effectAsset = "") : base(game, effectAsset)
        {

        }

        protected override void BuildData()
        {
            VertexList.AddRange(new Vector3[]
            {
                new Vector3(.5f, -.5f, 0), new Vector3(-.5f, -.5f, 0), new Vector3(-.5f, .5f, 0),new Vector3(.5f, .5f, 0),
            });

            NormalList.AddRange(new Vector3[]
            {
                Vector3.Backward,Vector3.Backward,Vector3.Backward,Vector3.Backward,
            });

            TexCoordList.AddRange(new Vector2[]
            {
                new Vector2(1, 1),new Vector2(0, 1),new Vector2(0,0),new Vector2(1, 0),
            });

            ColorList.AddRange(new Color[]
            {
                Color.White,Color.White,Color.White,Color.White,
            });


            IndexList.AddRange(new int[]
            {
                0, 1, 2, 2, 3, 0, // Front
            });

            CalculateTangents();
        }
    }
}
