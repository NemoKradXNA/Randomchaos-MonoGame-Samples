using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RandomchaosMGBase.VertexTypes;

namespace RandomchaosMGBase.BaseClasses.Primatives
{
    public class PrimativeBase : Base3DObject
    {
        protected List<Vector3> VertexList = new List<Vector3>();
        protected List<Vector3> NormalList = new List<Vector3>();
        protected List<Vector3> TangentList = new List<Vector3>();
        protected List<Vector2> TexCoordList = new List<Vector2>();
        protected List<Color> ColorList = new List<Color>();
        protected List<int> IndexList = new List<int>();

        public PrimativeBase(Game game) : base(game) { BuildData(); }
        public PrimativeBase(Game game, string effectAsset = "") : base(game,"", effectAsset) { BuildData(); }

        protected virtual void BuildData() { }

        protected override void LoadContent()
        {
            List<ModelBone> bones = new List<ModelBone>();
            List<ModelMesh> meshes = new List<ModelMesh>();
            List<ModelMeshPart> parts = new List<ModelMeshPart>();

            Vector3 max = Vector3.One * float.MinValue;
            Vector3 min = Vector3.One * float.MaxValue;

            bones.Add(new ModelBone()
            {
                Index = 0,
                ModelTransform = Matrix.Identity,
                Name = this.Name,
                Parent = new ModelBone(),
                Transform = Matrix.Identity,
            });

            List<VertexPositionColorNormalTextureTangent> verts = new List<VertexPositionColorNormalTextureTangent>();

            for (int v = 0; v < VertexList.Count; v++)
            {
                verts.Add(new VertexPositionColorNormalTextureTangent(VertexList[v], NormalList[v], TangentList[v], TexCoordList[v], ColorList[v]));

                if (VertexList[v].X > max.X)
                    max.X = VertexList[v].X;
                if (VertexList[v].Y > max.Y)
                    max.Y = VertexList[v].Y;
                if (VertexList[v].Z > max.Z)
                    max.Z = VertexList[v].Z;

                if (VertexList[v].X < min.X)
                    min.X = VertexList[v].X;
                if (VertexList[v].Y < min.Y)
                    min.Y = VertexList[v].Y;
                if (VertexList[v].Z < min.Z)
                    min.Z = VertexList[v].Z;
            }


            IndexBuffer indexBuffer = new IndexBuffer(GraphicsDevice, IndexElementSize.ThirtyTwoBits, IndexList.Count, BufferUsage.WriteOnly);
            indexBuffer.SetData(IndexList.ToArray());

            VertexBuffer vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColorNormalTextureTangent), verts.Count, BufferUsage.WriteOnly);
            vertexBuffer.SetData(verts.ToArray());

            parts.Add(new ModelMeshPart()
            {
                IndexBuffer = indexBuffer,
                NumVertices = verts.Count,
                PrimitiveCount = IndexList.Count / 3,
                StartIndex = 0,
                VertexBuffer = vertexBuffer,
                VertexOffset = 0,
            });

            meshes.Add(new ModelMesh(Game.GraphicsDevice, parts));

            mesh = new Model(Game.GraphicsDevice, bones, meshes);

            transforms = new Matrix[] { Matrix.Identity };
        }

        public virtual void ClearData()
        {
            VertexList.Clear();
            NormalList.Clear();
            TangentList.Clear();
            TexCoordList.Clear();
            ColorList.Clear();
            IndexList.Clear();
        }
    }
}
