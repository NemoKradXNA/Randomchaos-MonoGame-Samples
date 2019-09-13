using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RandomchaosMGBase.BaseClasses
{
    public class BaseSkyBox : Base3DObject
    {
        public string textureAsset { get; set; }

        public BaseSkyBox(Game game, string textureAsset) : base(game, "Models/SkyBox")
        {
            this.textureAsset = textureAsset;

            Rotation = new Quaternion(0, 0, 0, 1);
            Scale = new Vector3(99, 99, 99);            
        }

        public override void Initialize()
        {
            base.Initialize();

            Effect = Game.Content.Load<Effect>("Shaders/SkyBoxShader");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        TextureCube cube = null;

        public override void Draw(GameTime gameTime)
        {
            if (Enabled && Visible)
            {

                Game.GraphicsDevice.BlendState = BlendState.Opaque;

                if (cube == null)
                {
                    cube = Game.Content.Load<TextureCube>(textureAsset);

                    //cube = new TextureCube(Game.GraphicsDevice, 128, false, SurfaceFormat.Rg32);
                }

                Matrix World = Matrix.CreateScale(Scale) *
                                Matrix.CreateFromQuaternion(Rotation) *
                                Matrix.CreateTranslation(camera.Position);

                Effect.Parameters["World"].SetValue(World);
                Effect.Parameters["View"].SetValue(camera.View);
                Effect.Parameters["Projection"].SetValue(camera.Projection);
                Effect.Parameters["surfaceTexture"].SetValue(cube);

                Effect.Parameters["EyePosition"].SetValue(camera.Position);

                for (int pass = 0; pass < Effect.CurrentTechnique.Passes.Count; pass++)
                {
                    for (int msh = 0; msh < mesh.Meshes.Count; msh++)
                    {
                        ModelMesh meshSky = mesh.Meshes[msh];
                        for (int prt = 0; prt < meshSky.MeshParts.Count; prt++)
                            meshSky.MeshParts[prt].Effect = Effect;
                        meshSky.Draw();
                    }
                }
            }
        }
    }
}
