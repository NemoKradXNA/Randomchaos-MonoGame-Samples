using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RandomchaosMGBase.VertexTypes;

namespace RandomchaosMGBase.BaseClasses
{
    public class Base3DTerrain : Base3DObject
    {
        protected List<Vector3> VertexList = new List<Vector3>();
        protected List<Vector3> NormalList = new List<Vector3>();
        protected List<Vector3> TangentList = new List<Vector3>();
        protected List<Vector2> TexCoordList = new List<Vector2>();
        protected List<Color> ColorList = new List<Color>();
        protected List<int> IndexList = new List<int>();

        /// <summary>
        /// Height map string reference to be used to create the terrain.
        /// </summary>
        public string HeightMap { get; set; }
        /// <summary>
        /// Loaded height map texture
        /// </summary>
        public Texture2D HeightMapTexture { get; set; }

        public Texture2D SplatMapTexture { get; set; }

        /// <summary>
        /// Width of patch (X)
        /// </summary>
        public int Width { get { return HeightMapTexture.Width; } }
        /// <summary>
        /// Height (Z) of patch
        /// </summary>
        public int Height { get { return HeightMapTexture.Height; } }

        /// <summary>
        /// Max height (Y)
        /// </summary>
        public float HeightMax { get; set; }
        /// <summary>
        /// Min height (Y)
        /// </summary>
        public float HeightMin { get; set; }

        protected Vector3 center;

        public List<Vector2> uvChannels = new List<Vector2>();
        public List<string> diffuseChannels = new List<string>();
        public List<string> bumpChannels = new List<string>();

        /// <summary>
        /// Seed used for randomzation
        /// </summary>
        public int seed { get; set; }

        protected Random rnd;


        public Base3DTerrain(Game game, string heightMapAsset) : base(game, string.Empty)
        {
            HeightMax = 200;
            HeightMin = 0;

            seed = 2019;
            rnd = new Random(seed);

            HeightMap = heightMapAsset;
        }

        protected float[] heightData;

        protected virtual void ClearData()
        {
            VertexList = new List<Vector3>();
            NormalList = new List<Vector3>();
            TangentList = new List<Vector3>();
            TexCoordList = new List<Vector2>();
            ColorList = new List<Color>();
            IndexList = new List<int>();
        }

        protected virtual void BuildData()
        {
            ClearData();

            if (string.IsNullOrEmpty(HeightMap))
            {
                HeightMapTexture = new Texture2D(Game.GraphicsDevice, 255, 255);
                List<Color> blankMap = new List<Color>();
                for (int c = 0; c < 255 * 255; c++)
                {
                    blankMap.Add(Color.TransparentBlack);
                }

                HeightMapTexture.SetData(blankMap.ToArray());
            }
            else
                HeightMapTexture = Game.Content.Load<Texture2D>(HeightMap);

            if (Width != Height)
                throw new Exception("Terrain height Map MUST be square and power of two...");

            Color[] mapHeightData = new Color[Width * Height];

            HeightMapTexture.GetData(mapHeightData);

            // Find height map range
            float min, max;
            min = float.MaxValue;
            max = float.MinValue;
            heightData = new float[Width * Height];

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {
                    heightData[y + x * Width] = mapHeightData[y + x * Width].ToVector3().Y;

                    if (heightData[y + x * Width] < min)
                        min = heightData[y + x * Width];
                    if (heightData[y + x * Width] > max)
                        max = heightData[y + x * Width];
                }

            // Average
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {
                    heightData[y + x * Width] = (heightData[y + x * Width]) / ((max - min) + 1);
                }

            int[] index = new int[(Width - 1) * (Height - 1) * 6];
            Vector2 uv = Vector2.Zero;
            center = new Vector3(Width, (HeightMax - HeightMin) * .5f, Height) * .5f;

            float hmod = 1f / Width;

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    float h = MathHelper.Lerp(HeightMin, HeightMax, heightData[y + x * Width]);// + MathHelper.Lerp(-hmod, hmod, (float)rnd.NextDouble());
                    heightData[y + x * Width] = h;
                    //h = data[y + x * Width];

                    uv = new Vector2(x, y) / (float)(Height - 1);
                    VertexList.Add(new Vector3(x, h, y) - center);
                    NormalList.Add(Vector3.Zero);
                    TexCoordList.Add(uv);
                    TangentList.Add(Vector3.Zero);
                    ColorList.Add(Color.White);
                }
            }

            for (int x = 0; x < Width - 1; x++)
            {
                for (int y = 0; y < Height - 1; y++)
                {
                    index[(x + y * (Width - 1)) * 6] = ((x + 1) + (y + 1) * Height);
                    index[(x + y * (Width - 1)) * 6 + 1] = ((x + 1) + y * Height);
                    index[(x + y * (Width - 1)) * 6 + 2] = (x + y * Height);

                    index[(x + y * (Width - 1)) * 6 + 3] = ((x + 1) + (y + 1) * Height);
                    index[(x + y * (Width - 1)) * 6 + 4] = (x + y * Height);
                    index[(x + y * (Width - 1)) * 6 + 5] = (x + (y + 1) * Height);
                }
            }

            IndexList.AddRange(index);

            // Calculate tangents
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (x != 0 && x < Width - 1)
                        TangentList[x + y * Width] = VertexList[x - 1 + y * Width] - VertexList[x + 1 + y * Width];
                    else
                        if (x == 0)
                        TangentList[x + y * Width] = VertexList[x + y * Width] - VertexList[x + 1 + y * Width];
                    else
                        TangentList[x + y * Width] = VertexList[x - 1 + y * Width] - VertexList[x + y * Width];
                }
            }

            // Calculate Normals
            for (int i = 0; i < NormalList.Count; i++)
                NormalList[i] = new Vector3(0, 0, 0);

            for (int i = 0; i < index.Length / 3; i++)
            {
                int index1 = index[i * 3];
                int index2 = index[i * 3 + 1];
                int index3 = index[i * 3 + 2];

                Vector3 side1 = VertexList[index1] - VertexList[index3];
                Vector3 side2 = VertexList[index1] - VertexList[index2];
                Vector3 normal = Vector3.Cross(side1, side2);

                NormalList[index1] += normal;
                NormalList[index2] += normal;
                NormalList[index3] += normal;
            }

            for (int i = 0; i < NormalList.Count; i++)
                NormalList[i] = Vector3.Normalize(NormalList[i]);

            BuildSplatMap();
        }

        public override void Initialize()
        {
            base.Initialize();

            BuildData();

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

            // Generate box's
            BoundingBox = new BoundingBox(min, max);

            
        }

        protected virtual void BuildSplatMap()
        {
            SplatMapTexture = new Texture2D(Game.GraphicsDevice, Width, Height, false, SurfaceFormat.Color);

            Color[] pixels = new Color[Width * Height];

            float min = heightData.Min();
            float max = heightData.Max();
            float avg = heightData.Average();

            //float meanHeight = max - min;// (HeightMax + HeightMin) * .5f;
            float[] bands = new float[4] { avg / 2, avg, avg + (max / 6), max };
            float r, g, b, a;

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Vector3 n = NormalList[y + x * Width];
                    float thisHeight = heightData[y + x * Width];

                    //if (x == 0 || y == 0 || x == Width - 1 || y == Height - 1)
                    //    thisHeight = 0;

                    r = g = b = a = 0;

                    float dp = MathHelper.ToDegrees((float)Math.Acos(Vector3.Dot(Vector3.Up, n)));

                    float bm = .5f;
                    float bandMod = MathHelper.Lerp(-bm, bm, (float)rnd.NextDouble());

                    if (thisHeight <= bands[0] + bandMod) // Dirst
                    {
                        r = 1;
                        if (dp >= 50) // Stone
                        {
                            r = g = a = 0;
                            b = 1;
                        }
                    }
                    else if (thisHeight <= bands[1] + bandMod) // Grass 
                    {
                        g = 1;
                        if (dp > 45) // Stone
                        {
                            if (rnd.NextDouble() > .25f)
                            {
                                r = g = a = 0;
                                b = 1;
                            }
                        }
                        else if (dp >= 25) // Dirt
                        {
                            if (rnd.NextDouble() > .25f)
                            {
                                g = b = a = 0;
                                r = 1;
                            }
                        }
                    }
                    else if (thisHeight <= bands[2] + bandMod) // Stone
                    {
                        b = 1;
                        if (dp == 0) // Grass
                        {
                            if (rnd.NextDouble() > .25f)
                            {
                                r = b = a = 0;
                                g = 1;
                            }
                        }
                        else if (dp >= 20) // Dirt
                        {
                            g = b = a = 0;
                            r = 1;
                        }
                    }
                    else if (thisHeight <= bands[3]) // Snow
                    {
                        a = 1;

                        //if (dp == 0) // Grass
                        //{
                        //    r = b = a = 0;
                        //    g = 1;
                        //}
                    }

                    pixels[x + y * Width] = new Color(r, g, b, a);
                }
            }

            SplatMapTexture.SetData(pixels);
        }


        public override void Update(GameTime gameTime)
        {
            World = Matrix.CreateScale(Scale) *
                      Matrix.CreateFromQuaternion(Rotation) *
                      Matrix.CreateTranslation(Position);

            if (Effect == null)
            {
                Effect = Game.Content.Load<Effect>("Shaders/BasicTerrainShader");


                Effect.Parameters["lightDirection"].SetValue(LightPosition - Position);
                Effect.Parameters["splatMap"].SetValue(SplatMapTexture);

                int cc = diffuseChannels.Count;
                for (int c = 0; c < cc; c++)
                {
                    Effect.Parameters[$"UVChannel{c}"].SetValue(uvChannels[c]);
                    Effect.Parameters[$"textureChannel{c}"].SetValue(Game.Content.Load<Texture2D>(diffuseChannels[c]));
                    Effect.Parameters[$"normalChannel{c}"].SetValue(Game.Content.Load<Texture2D>(bumpChannels[c]));
                }
            }
        }

    }
}
