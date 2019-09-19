using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RandomchaosMGBase.BaseClasses;

namespace RandomchaosMGBasicWater
{
    public class RCWater : Base3DObject
    {
        private VertexBuffer vb;
        private IndexBuffer ib;
        List<VertexPositionNormalTexture> myVertices;
        protected int myHeight = 128;
        protected int myWidth = 128;
        
        /// <summary>
        /// Default 128
        /// </summary>
        public int Height
        {
            get { return myHeight; }
            set { myHeight = value; }
        }
        /// <summary>
        /// Default 128
        /// </summary>
        public int Width
        {
            get { return myWidth; }
            set { myWidth = value; }
        }

        public string myEnvironmentAsset;

        protected Color myDeepWater = Color.Black;
        protected Color myShallowWater = Color.Black;
        protected Color myReflection = Color.DarkGray;

        protected float myBumpHeight = 0.10f;
        protected float myHDRMult = 3.0f;
        protected float myReflectionAmt = .25f;
        protected float myWaterColorAmount = 1.0f;
        protected float myWaveAmplitude = 1.0f;
        protected float myWaveFrequency = 0.1f;

        /// <summary>
        /// Min 0, Max 2.0
        /// </summary>
        public float BumpMapHeight
        {
            get { return myBumpHeight; }
            set { myBumpHeight = value; }
        }
        /// <summary>
        /// Min 0, Max 100
        /// </summary>
        public float HDRMultiplier
        {
            get { return myHDRMult; }
            set { myHDRMult = value; }
        }
        /// <summary>
        /// Min 0, Max 2.0
        /// </summary>
        public float ReflectionAmount
        {
            get { return myReflectionAmt; }
            set { myReflectionAmt = value; }
        }
        /// <summary>
        /// Min 0, Max 2.0
        /// </summary>
        public float WaterColorAmount
        {
            get { return myWaterColorAmount; }
            set { myWaterColorAmount = value; }
        }
        /// <summary>
        /// Min 0, Max 10.0
        /// </summary>
        public float WaveAmplitude
        {
            get { return myWaveAmplitude; }
            set { myWaveAmplitude = value; }
        }
        /// <summary>
        /// Min 0, Max 1.0
        /// </summary>
        public float WaveFrequency
        {
            get { return myWaveFrequency; }
            set { myWaveFrequency = value; }
        }

        protected float tick = 0.0f;
        protected float animSpeed = 1.1f;

        protected bool alphaCheck;
        protected Color myAlphaBlendColor;

        /// <summary>
        /// Set to get transparent water.
        /// </summary>
        public Color AlphaBlendColor
        {
            get { return myAlphaBlendColor; }
            set
            {
                myAlphaBlendColor = value;
                if (value == Color.Black)
                    alphaCheck = false;
                else
                    alphaCheck = true;
            }
        }

        /// <summary>
        /// Min 0, Max 0.1
        /// </summary>
        public float AnimationSpeed
        {
            get { return animSpeed; }
            set { animSpeed = value; }
        }

        /// <summary>
        /// Color of deep water
        /// </summary>
        public Color DeepWaterColor
        {
            get { return myDeepWater; }
            set { myDeepWater = value; }
        }
        /// <summary>
        /// Color of shallow water
        /// </summary>
        public Color ShallowWaterColor
        {
            get { return myShallowWater; }
            set { myShallowWater = value; }
        }
        /// <summary>
        /// Color of reflection
        /// </summary>
        public Color ReflectionColor
        {
            get { return myReflection; }
            set { myReflection = value; }
        }

        public RCWater(Game game) : base(game, string.Empty)
        {
            myWidth = 128;
            myHeight = 128;
        }

        public override void Update(GameTime gameTime)
        {
            Transform.Update();

            if (Effect == null)
            {
                Effect = Game.Content.Load<Effect>("Shaders/Ocean");
            }

            Effect.Parameters["viewI"].SetValue(Matrix.Invert(camera.View));
            Effect.Parameters["normalMap"].SetValue(Game.Content.Load<Texture2D>(BumpAsset));

            Effect.Parameters["cubeMap"].SetValue(Game.Content.Load<TextureCube>(myEnvironmentAsset));

            Effect.Parameters["deepColor"].SetValue(myDeepWater.ToVector4());
            Effect.Parameters["shallowColor"].SetValue(myShallowWater.ToVector4());
            Effect.Parameters["reflectionColor"].SetValue(myReflection.ToVector4());
            Effect.Parameters["time"].SetValue((float)gameTime.TotalGameTime.TotalSeconds);

            Effect.Parameters["bumpHeight"].SetValue(myBumpHeight);
            Effect.Parameters["hdrMultiplier"].SetValue(myHDRMult);
            Effect.Parameters["reflectionAmount"].SetValue(myReflectionAmt);
            Effect.Parameters["waterAmount"].SetValue(myWaterColorAmount);
            Effect.Parameters["waveAmp"].SetValue(myWaveAmplitude);
            Effect.Parameters["waveFreq"].SetValue(myWaveFrequency);

            Effect.Parameters["waveFrequency"].SetValue(new float[] { 1f, 2f });
            Effect.Parameters["waveAmplitude"].SetValue(new float[] { 1f, .5f });
            Effect.Parameters["wavePhase"].SetValue(new float[] { .5f, 1.3f });
            Effect.Parameters["waveDirection"].SetValue(new Vector2[] { new Vector2(-.1f, .1f), new Vector2(-.7f , .7f) });
        }

        public override void Initialize()
        {            
            Vector3 center = new Vector3((myWidth / 2), 0, (myHeight / 2));

            Vector3 max = Vector3.One * float.MinValue;
            Vector3 min = Vector3.One * float.MaxValue;

            // Vertices's
            myVertices = new List<VertexPositionNormalTexture>();

            for (int x = 0; x < myWidth; x++)
            {
                for (int y = 0; y < myHeight; y++)
                {
                    Vector3 v = new Vector3(x, 0, y) - center;
                    min = Vector3.Min(min, v);
                    max = Vector3.Max(max, v);
                    Vector2 uv = new Vector2((float)x / myWidth, (float)y / myHeight) * 1;
                    

                    myVertices.Add(new VertexPositionNormalTexture(v, Vector3.Up, uv));
                }
            }

            //Index
            int[] index = new int[(myWidth - 1) * (myHeight - 1) * 6];
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

            ib = new IndexBuffer(GraphicsDevice, IndexElementSize.ThirtyTwoBits, index.Length, BufferUsage.WriteOnly);
            ib.SetData(index);

            vb = new VertexBuffer(GraphicsDevice, typeof(VertexPositionNormalTexture), myVertices.Count, BufferUsage.WriteOnly);
            vb.SetData(myVertices.ToArray());
            
            List<ModelBone> bones = new List<ModelBone>();
            List<ModelMesh> meshes = new List<ModelMesh>();
            List<ModelMeshPart> parts = new List<ModelMeshPart>();

            bones.Add(new ModelBone()
            {
                Index = 0,
                ModelTransform = Matrix.Identity,
                Name = this.Name,
                Parent = new ModelBone(),
                Transform = Matrix.Identity,
            });

            parts.Add(new ModelMeshPart()
            {
                IndexBuffer = ib,
                NumVertices = myVertices.Count,
                PrimitiveCount = index.Length / 3,
                StartIndex = 0,
                VertexBuffer = vb,
                VertexOffset = 0,
            });

            meshes.Add(new ModelMesh(Game.GraphicsDevice, parts));

            mesh = new Model(Game.GraphicsDevice, bones, meshes);

            transforms = new Matrix[] { Matrix.Identity };

            BoundingBox = new BoundingBox(min - Vector3.Up, max + Vector3.Up);
        }

        public override void Draw(GameTime gameTime, Effect effect)
        {
            Game.GraphicsDevice.BlendState = BlendState.Opaque;
            if (alphaCheck)
            {
                Game.GraphicsDevice.BlendState = new BlendState()
                {
                    ColorSourceBlend = Blend.BlendFactor,
                    ColorDestinationBlend = Blend.One,
                };
                Game.GraphicsDevice.BlendFactor = myAlphaBlendColor;
            }

            Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Game.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            foreach (ModelMesh meshM in mesh.Meshes)
            {
                // Do the world stuff. 
                // Scale * transform * pos * rotation
                if (meshM.ParentBone != null)
                    meshWorld = transforms[meshM.ParentBone.Index] * World;
                else
                    meshWorld = transforms[0] * World;

                meshWVP = meshWorld * camera.View * camera.Projection;

                effect.Parameters["world"].SetValue(meshWorld);
                effect.Parameters["wvp"].SetValue(meshWVP);

                if (effect.Parameters["color"] != null)
                    effect.Parameters["color"].SetValue(Color.ToVector4());

                if (effect.Parameters["textureMat"] != null)
                {
                    if (!string.IsNullOrEmpty(ColorAsset))
                        effect.Parameters["textureMat"].SetValue(Game.Content.Load<Texture2D>(ColorAsset));
                    else
                        effect.Parameters["textureMat"].SetValue(defaultTexture);
                }

                if (effect.Parameters["BumpMap"] != null)
                {
                    if (!string.IsNullOrEmpty(BumpAsset))
                        effect.Parameters["BumpMap"].SetValue(Game.Content.Load<Texture2D>(BumpAsset));
                    else
                        effect.Parameters["BumpMap"].SetValue(defaultBump);
                }

                if (effect.Parameters["lightDirection"] != null)
                    effect.Parameters["lightDirection"].SetValue(Position - LightPosition);

                effect.CurrentTechnique.Passes[0].Apply();

                foreach (ModelMeshPart meshPart in meshM.MeshParts)
                {
                    Game.GraphicsDevice.SetVertexBuffer(meshPart.VertexBuffer);
                    Game.GraphicsDevice.Indices = meshPart.IndexBuffer;

                    Game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, meshPart.StartIndex, meshPart.PrimitiveCount);
                }
            }

            if (DrawBoxBounds)
                DrawBoundsBoxs(gameTime);

        }

    }
}
