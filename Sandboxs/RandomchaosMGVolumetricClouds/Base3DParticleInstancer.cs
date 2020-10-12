using System.Collections.Generic;
using System.Linq;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RandomchaosMGBase.BaseClasses;

namespace RandomchaosMGVolumetricClouds
{
    public class Base3DParticleInstancer : DrawableGameComponent
    {
        public Vector3 Position = Vector3.Zero;
        public Vector3 Scale = Vector3.One;
        public Quaternion Rotation = Quaternion.Identity;
        public Vector2 Size = Vector2.One * .5f;
        public Matrix World = Matrix.Identity;

        public BlendState thisBlendState = BlendState.Additive;
        public DepthStencilState thisDepthStencilState = DepthStencilState.None;

        public string TextureAsset { get; set; }

        public Base3DCamera Camera
        {
            get { return ((Base3DCamera)Game.Services.GetService(typeof(Base3DCamera))); }
        }

        protected DynamicVertexBuffer instanceVertexBuffer;

        //public BaseDeferredObject Object;
        public Dictionary<Base3DParticleInstance, Matrix> instanceTransformMatrices = new Dictionary<Base3DParticleInstance, Matrix>();
        public VertexBuffer modelVertexBuffer;
        public int vertCount = 0;
        public IndexBuffer indexBuffer;

        public Dictionary<int, Base3DParticleInstance> Instances = new Dictionary<int, Base3DParticleInstance>();

        protected Effect effect;

        protected static VertexDeclaration instanceVertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0),
            new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 1),
            new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 2),
            new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 3)
        );

        string effectAsset;
        public Color Color = Color.White;

        public Base3DParticleInstancer(Game game, string effectAsset)
            : base(game)
        {
            this.effectAsset = effectAsset;
            TextureAsset = string.Empty;
        }


        protected override void LoadContent()
        {
            base.LoadContent();

            effect = Game.Content.Load<Effect>(effectAsset);
            effect.CurrentTechnique = effect.Techniques["BasicTextureH"];

            List<Vector2> t = new List<Vector2>();

            // Build Vert Elemets
            List<VertexPositionTexture> verts = new List<VertexPositionTexture>();
            List<int> indx = new List<int>();
            Vector2 texCoord = Vector2.Zero;

            indx.Add(0);
            indx.Add(1);
            indx.Add(2);
            indx.Add(2);
            indx.Add(3);
            indx.Add(0);

            for (int d = 0; d < 4; d++)
            {
                switch (d)
                {
                    case 0:
                        texCoord = new Vector2(1, 1);
                        break;
                    case 1:
                        texCoord = new Vector2(0, 1);
                        break;
                    case 2:
                        texCoord = new Vector2(0, 0);
                        break;
                    case 3:
                        texCoord = new Vector2(1, 0);
                        break;
                }
                verts.Add(new VertexPositionTexture(Vector3.Zero, texCoord));
            }

            vertCount = verts.Count;
            modelVertexBuffer = new VertexBuffer(Game.GraphicsDevice, typeof(VertexPositionTexture), vertCount, BufferUsage.WriteOnly);
            modelVertexBuffer.SetData(verts.ToArray());
            indexBuffer = new IndexBuffer(Game.GraphicsDevice, IndexElementSize.ThirtyTwoBits, indx.Count, BufferUsage.WriteOnly);
            indexBuffer.SetData(indx.ToArray());
        }

        public override void Draw(GameTime gameTime)
        {
            Draw(gameTime, effect);
        }

        Vector2[] imageUVMap = new Vector2[] {new Vector2(0,0),new Vector2(.25f,0),new Vector2(.50f,0),new Vector2(.75f,0),
        new Vector2(0,.25f),new Vector2(.25f,.25f),new Vector2(.50f,.25f),new Vector2(.75f,.25f),
        new Vector2(0,.50f),new Vector2(.25f,.50f),new Vector2(.50f,.50f),new Vector2(.75f,.50f),
        new Vector2(0,.75f),new Vector2(.25f,.75f),new Vector2(.50f,.75f),new Vector2(.75f,.75f) };

        public virtual void Draw(GameTime gameTime, Effect thisEffect)
        {
            if (!Visible || !Enabled)
                return;

            World = Matrix.CreateScale(Scale) * Matrix.CreateFromQuaternion(Rotation) * Matrix.CreateTranslation(Position);

            if (instanceTransformMatrices.Count == 0)
                return;

            // If we have more instances than room in our vertex buffer, grow it to the neccessary size.
            if ((instanceVertexBuffer == null) || (instanceTransformMatrices.Count != instanceVertexBuffer.VertexCount))
                CalcVertexBuffer();


            thisEffect.Parameters["lightColor"].SetValue(Color.ToVector4());

            //////////////////////////////////////////////////////////////////
            // REMEMBER THE CloudSpriteSheetOrg texture is property of MS   //
            // Do not comercialy use this texture!                          //
            //////////////////////////////////////////////////////////////////
            thisEffect.Parameters["partTexture"].SetValue(Game.Content.Load<Texture2D>(TextureAsset));

            thisEffect.Parameters["EyePosition"].SetValue(Camera.Position);

            thisEffect.Parameters["vp"].SetValue(Camera.View * Camera.Projection);
            thisEffect.Parameters["World"].SetValue(World);

            thisEffect.Parameters["imgageUV"].SetValue(imageUVMap);

            GraphicsDevice.SetVertexBuffers(
                        new VertexBufferBinding(modelVertexBuffer, 0, 0),
                        new VertexBufferBinding(instanceVertexBuffer, 0, 1)
                    );


            Game.GraphicsDevice.BlendState = thisBlendState;
            Game.GraphicsDevice.DepthStencilState = thisDepthStencilState;

            thisEffect.CurrentTechnique.Passes[0].Apply();
            GraphicsDevice.Indices = indexBuffer;

            Game.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0,
                modelVertexBuffer.VertexCount, instanceTransformMatrices.Count);

            //Game.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0,
            //                                       modelVertexBuffer.VertexCount, 0,
            //                                       2,
            //                                       instanceTransformMatrices.Count);

        }

        public void CalcVertexBuffer()
        {
            if (instanceVertexBuffer != null)
                instanceVertexBuffer.Dispose();

            instanceVertexBuffer = new DynamicVertexBuffer(GraphicsDevice, instanceVertexDeclaration, instanceTransformMatrices.Count, BufferUsage.WriteOnly);

            // Transfer the latest instance transform matrices into the instanceVertexBuffer.
            instanceVertexBuffer.SetData(instanceTransformMatrices.Values.ToArray(), 0, instanceTransformMatrices.Count, SetDataOptions.Discard);
        }

        /// <summary>
        /// Method to translate object
        /// </summary>
        /// <param name="distance"></param>
        public void TranslateOO(Vector3 distance)
        {
            Position += Vector3.Transform(distance, Rotation);
        }
        public void TranslateAA(Vector3 distance)
        {
            Position += Vector3.Transform(distance, Quaternion.Identity);
        }
        /// <summary>
        /// Method to rotate object
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        public void Rotate(Vector3 axis, float angle)
        {
            axis = Vector3.Transform(axis, Rotation);
            Rotation = Quaternion.Normalize(Quaternion.CreateFromAxisAngle(axis, angle) * Rotation);
        }
    }
}
