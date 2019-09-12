using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RandomchaosMGBase.BaseClasses
{
    public class Base3DObject : DrawableGameComponent
    {
        string modelName;

        public Base3DCamera camera
        {
            get { return (Base3DCamera)Game.Services.GetService(typeof(Base3DCamera)); }
        }

        /// <summary>
        /// Position
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// Scale
        /// </summary>
        public Vector3 Scale;

        /// <summary>
        /// Rotation
        /// </summary>
        public Quaternion Rotation;

        protected Model mesh;

        /// <summary>
        /// World
        /// </summary>
        public Matrix World;

        public Effect Effect;

        Matrix[] transforms;
        Matrix meshWorld;
        Matrix meshWVP;

        public Vector3 LightPosition = new Vector3(50, 50, 100);

        public string ColorAsset;
        public string BumpAsset;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="game"></param>
        /// <param name="modelAssetName"></param>
        /// <param name="shaderAssetName"></param>
        public Base3DObject(Game game, string modelAssetName) : base(game)
        {
            Position = Vector3.Zero;
            Scale = Vector3.One;
            Rotation = Quaternion.Identity;
            modelName = modelAssetName;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            World = Matrix.CreateScale(Scale) *
                      Matrix.CreateFromQuaternion(Rotation) *
                      Matrix.CreateTranslation(Position);

            if (mesh == null && modelName != string.Empty)
            {
                mesh = Game.Content.Load<Model>(modelName);

                transforms = new Matrix[mesh.Bones.Count];
                mesh.CopyAbsoluteBoneTransformsTo(transforms);
            }

            if (Effect == null)
                Effect = Game.Content.Load<Effect>("Shaders/RenderObject");
        }


        public void Translate(Vector3 distance)
        {
            Position += Vector3.Transform(distance, Matrix.CreateFromQuaternion(Rotation));
        }

        public void Rotate(Vector3 axis, float angle)
        {
            axis = Vector3.Transform(axis, Matrix.CreateFromQuaternion(Rotation));
            Rotation = Quaternion.Normalize(Quaternion.CreateFromAxisAngle(axis, angle) * Rotation);
        }


        public virtual void Draw(GameTime gameTime, Effect effect)
        {
            Game.GraphicsDevice.BlendState = BlendState.Opaque;
            Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Game.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            Game.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;

            foreach (ModelMesh meshM in mesh.Meshes)
            {
                // Do the world stuff. 
                // Scale * transform * pos * rotation
                meshWorld = transforms[meshM.ParentBone.Index] * World;
                meshWVP = meshWorld * camera.View * camera.Projection;

                effect.Parameters["world"].SetValue(meshWorld);
                effect.Parameters["wvp"].SetValue(meshWorld * camera.View * camera.Projection);
                effect.Parameters["textureMat"].SetValue(Game.Content.Load<Texture2D>(ColorAsset));
                effect.Parameters["BumpMap"].SetValue(Game.Content.Load<Texture2D>(BumpAsset));
                effect.Parameters["lightDirection"].SetValue(Position - LightPosition);

                effect.CurrentTechnique.Passes[0].Apply();

                foreach (ModelMeshPart meshPart in meshM.MeshParts)
                {
                    Game.GraphicsDevice.SetVertexBuffer(meshPart.VertexBuffer);
                    Game.GraphicsDevice.Indices = meshPart.IndexBuffer;

                    Game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, meshPart.StartIndex, meshPart.PrimitiveCount);
                }
            }

        }
        /// <summary>
        /// Draw method
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            Draw(gameTime, Effect);
            //DrawBox(bounds);
        }
    }
}
