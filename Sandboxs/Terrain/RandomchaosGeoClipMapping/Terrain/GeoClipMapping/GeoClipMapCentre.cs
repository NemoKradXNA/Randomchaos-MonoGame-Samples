using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RandomchaosMGBase.BaseClasses;

namespace RandomchaosGeoClipMapping.Terrain.GeoClipMapping
{
    public enum GeoClipMapRenderTypesEnum
    {
        Textured,
        FlatWireFrame,
        WireFrame
    }

    public class GeoClipMapCentre : DrawableGameComponent
    {
        protected GeoClipMapFootPrintBlock block;

        protected Effect effect;

        public Vector3 Position = Vector3.Zero;
        public Vector3 Scale = Vector3.One;
        public Quaternion Orientation = Quaternion.Identity;

        public Matrix World = Matrix.Identity;

        protected GeoClipMapRenderTypesEnum _RenderType = GeoClipMapRenderTypesEnum.Textured;
        protected string effectToLoad = string.Empty;
        public GeoClipMapRenderTypesEnum RenderType
        {
            get { return _RenderType; }
            set
            {
                _RenderType = value;
                switch (_RenderType)
                {
                    case GeoClipMapRenderTypesEnum.Textured:
                        effectToLoad = "Shaders/GeoClipMapLayer";
                        break;
                    case GeoClipMapRenderTypesEnum.FlatWireFrame:
                        effectToLoad = "Shaders/GeoClipMapLayer_FlatWire";
                        break;
                    case GeoClipMapRenderTypesEnum.WireFrame:
                        effectToLoad = "Shaders/GeoClipMapLayer_HeightWire";
                        break;
                }
            }
        }

        protected Base3DCamera camera
        {
            get { return (Base3DCamera)Game.Services.GetService(typeof(Base3DCamera)); }
        }

        public GeoClipMapCentre(Game game, short n) : base(game)
        {
            short m = (short)((n + 1) / 4);
            short edge = (short)((n / 2) - 1);

            block = new GeoClipMapFootPrintBlock(game, n);

            block.Position = new Vector3(-edge, 0, -edge);
            block.color = Color.MintCream;
        }

        public override void Initialize()
        {
            base.Initialize();

            block.Initialize();
        }

        protected override void LoadContent()
        {
            effect = Game.Content.Load<Effect>(effectToLoad);
        }

        public override void Update(GameTime gameTime)
        {
            World = Matrix.CreateScale(Scale) * Matrix.CreateFromQuaternion(Orientation) * Matrix.CreateTranslation(Position);
            base.Update(gameTime);

            block.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            effect.Parameters["world"].SetValue(World);
            effect.Parameters["wvp"].SetValue(World * camera.View * camera.Projection);

            effect.CurrentTechnique.Passes[0].Apply();

            //if (camera.Frustum.Intersects(block.getBounds(World)))
            block.Draw(gameTime);

        }

        public void Rotate(Vector3 axis, float angle)
        {
            axis = Vector3.Transform(axis, Matrix.CreateFromQuaternion(Orientation));
            Orientation = Quaternion.Normalize(Quaternion.CreateFromAxisAngle(axis, angle) * Orientation);
        }
    }
}
