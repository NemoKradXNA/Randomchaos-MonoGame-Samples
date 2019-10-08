using System.Linq;
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

        public void CalculateTangents()
        {
            int triangleCount = IndexList.Count;
            int vertexCount = VertexList.Count;

            Vector3[] tan1 = new Vector3[vertexCount];
            Vector3[] tan2 = new Vector3[vertexCount];
            Vector4[] tangents = new Vector4[vertexCount];
            for (int a = 0; a < triangleCount; a += 3)
            {
                int i1 = IndexList[a + 0];
                int i2 = IndexList[a + 1];
                int i3 = IndexList[a + 2];
                Vector3 v1 = VertexList[i1];
                Vector3 v2 = VertexList[i2];
                Vector3 v3 = VertexList[i3];
                Vector2 w1 = TexCoordList[i1];
                Vector2 w2 = TexCoordList[i2];
                Vector2 w3 = TexCoordList[i3];
                float x1 = v2.X - v1.X;
                float x2 = v3.X - v1.X;
                float y1 = v2.Y - v1.Y;
                float y2 = v3.Y - v1.Y;
                float z1 = v2.Z - v1.Z;
                float z2 = v3.Z - v1.Z;
                float s1 = w2.X - w1.X;
                float s2 = w3.X - w1.X;
                float t1 = w2.Y - w1.Y;
                float t2 = w3.Y - w1.Y;
                float r = 1.0f / (s1 * t2 - s2 * t1);
                Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
                Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);
                tan1[i1] += sdir;
                tan1[i2] += sdir;
                tan1[i3] += sdir;
                tan2[i1] += tdir;
                tan2[i2] += tdir;
                tan2[i3] += tdir;
            }

            for (int a = 0; a < vertexCount; ++a)
            {
                Vector3 n = NormalList[a];
                Vector3 t = tan1[a];
                Vector3 tmp = (t - n * Vector3.Dot(n, t));
                tmp.Normalize();
                TangentList.Add(tmp);
            }
        }
    }
}
