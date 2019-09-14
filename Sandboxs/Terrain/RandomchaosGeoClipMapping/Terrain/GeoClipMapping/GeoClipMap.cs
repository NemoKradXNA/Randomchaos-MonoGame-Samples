
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RandomchaosMGBase.BaseClasses;

namespace RandomchaosGeoClipMapping.Terrain.GeoClipMapping
{
    // http://http.developer.nvidia.com/GPUGems2/gpugems2_chapter02.html

    public class GeoClipMap : DrawableGameComponent
    {
        protected GeoClipMapLayer[] layers = new GeoClipMapLayer[3];
        protected GeoClipMapCentre centre;

        public Vector3 Position = Vector3.Zero;
        public Vector3 Scale = Vector3.One;
        public Vector3 Offset = new Vector3(1, 0, -1);

        protected GeoClipMapRenderTypesEnum _RenderType = GeoClipMapRenderTypesEnum.Textured;
        public GeoClipMapRenderTypesEnum RenderType
        {
            get { return _RenderType; }
            set
            {
                _RenderType = value;
                foreach (GeoClipMapLayer l in layers)
                    l.RenderType = _RenderType;

                centre.RenderType = _RenderType;
            }
        }

        protected Base3DCamera camera
        {
            get { return (Base3DCamera)Game.Services.GetService(typeof(Base3DCamera)); }
        }

        public GeoClipMap(Game game, short n, string heightMap) : base(game)
        {
            for (int l = 0; l < layers.Length; l++)
            {
                layers[l] = new GeoClipMapLayer(game, n, heightMap);
                layers[l].Scale = Scale;

                for (int s = 0; s < l; s++)
                    layers[l].Scale /= 2;

                if (l != 0)
                    layers[l].CorseScale = layers[l].Scale;

                if (l != 0 && ((l + 1) % 2) == 0)
                {
                    //layers[l].Rotate(Vector3.Up, MathHelper.Pi);
                }
            }

            centre = new GeoClipMapCentre(game, n);
            centre.Scale = Scale;

            int lc = layers.Length;
            for (int s = 0; s < lc; s++)
                centre.Scale /= 2;
        }

        public override void Initialize()
        {
            base.Initialize();

            for (int l = 0; l < layers.Length; l++)
                layers[l].Initialize();

            centre.Initialize();

        }
        //Texture2D heightMap;
        protected override void LoadContent()
        {
            
        }

        public override void Update(GameTime gameTime)
        {

            if (!Enabled)
                return;

            Position = new Vector3((int)camera.Position.X, Position.Y, (int)camera.Position.Z);

            for (uint l = 0; l < layers.Length; l++)
            {
                layers[l].Position = Position;

                if (l != 0)
                {
                    if (l == 1)
                        layers[l].Position = Position - new Vector3(.5f, 0, -.5f);
                    if (l == 2)
                        layers[l].Position = Position - new Vector3(.75f, 0, -.75f);

                }

                layers[l].Update(gameTime);
            }

            centre.Position = Position - new Vector3(1f, 0, -.75f);
            centre.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            if (!Enabled)
                return;

            for (int l = 0; l < layers.Length; l++)
                layers[l].Draw(gameTime);

            centre.Draw(gameTime);
        }
    }
}
