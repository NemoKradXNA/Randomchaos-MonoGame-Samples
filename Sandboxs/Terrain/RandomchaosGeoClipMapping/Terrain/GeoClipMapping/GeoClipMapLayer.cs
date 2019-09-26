using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RandomchaosMGBase.BaseClasses;


namespace RandomchaosGeoClipMapping.Terrain.GeoClipMapping
{
    public class GeoClipMapLayer : DrawableGameComponent
    {
        protected GeoClipMapFootPrintBlock[] blocks = new GeoClipMapFootPrintBlock[12];
        protected GeoClipMapFootPrintFixUp[] fixup = new GeoClipMapFootPrintFixUp[4];
        protected GeoClipMapFootPrintTrim[] trim = new GeoClipMapFootPrintTrim[4];

        protected Effect effect;

        public Vector3 Position = Vector3.Zero;
        public Vector3 Scale = Vector3.One;
        public Quaternion Orientation = Quaternion.Identity;

        public Matrix World = Matrix.Identity;

        public Vector3 CorseScale = Vector3.One;

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

        string heightMapAsset = "Textures/TerrainMaps/MapTest";

        public float maxHeight = 30;

        public GeoClipMapLayer(Game game, short n, string heightMap) : base(game)
        {
            heightMapAsset = heightMap;

            short m = (short)((n + 1) / 4);
            short edge = (short)((m * 2) - 1);

            for (int b = 0; b < blocks.Length; b++)
            {
                blocks[b] = new GeoClipMapFootPrintBlock(game, m);

                switch (b)
                {
                    case 0:
                        blocks[b].Position = new Vector3(-edge, 0, -edge);
                        blocks[b].BlockType = BlockTypesEnum.TopLeft;
                        break;
                    case 1:
                        blocks[b].Position = new Vector3(-m, 0, -edge);
                        blocks[b].BlockType = BlockTypesEnum.Top;
                        break;
                    case 2:
                        blocks[b].Position = new Vector3(1, 0, -edge);
                        blocks[b].BlockType = BlockTypesEnum.Top;
                        break;
                    case 3:
                        blocks[b].Position = new Vector3(m, 0, -edge);
                        blocks[b].BlockType = BlockTypesEnum.TopRight;
                        break;
                    case 4:
                        blocks[b].Position = new Vector3(-edge, 0, -m);
                        blocks[b].BlockType = BlockTypesEnum.Left;
                        break;
                    case 5:
                        blocks[b].Position = new Vector3(m, 0, -m);
                        blocks[b].BlockType = BlockTypesEnum.Right;
                        break;
                    case 6:
                        blocks[b].Position = new Vector3(-edge, 0, 1);
                        blocks[b].BlockType = BlockTypesEnum.Left;
                        break;
                    case 7:
                        blocks[b].Position = new Vector3(m, 0, 1);
                        blocks[b].BlockType = BlockTypesEnum.Right;
                        break;
                    case 8:
                        blocks[b].Position = new Vector3(-edge, 0, m);
                        blocks[b].BlockType = BlockTypesEnum.BottomLeft;
                        break;
                    case 9:
                        blocks[b].Position = new Vector3(-m, 0, m);
                        blocks[b].BlockType = BlockTypesEnum.Bottom;
                        break;
                    case 10:
                        blocks[b].Position = new Vector3(1, 0, m);
                        blocks[b].BlockType = BlockTypesEnum.Bottom;
                        break;
                    case 11:
                        blocks[b].Position = new Vector3(m, 0, m);
                        blocks[b].BlockType = BlockTypesEnum.BottomRight;
                        break;
                }
                blocks[b].Orientation *= Orientation;
            }

            for (int b = 0; b < fixup.Length; b++)
            {
                switch (b)
                {
                    case 0:
                        fixup[b] = new GeoClipMapFootPrintFixUp(game, m, false);
                        fixup[b].Position = new Vector3(-1, 0, -edge);
                        fixup[b].FixUpType = FixUpTypesEnum.Top;
                        break;
                    case 1:
                        fixup[b] = new GeoClipMapFootPrintFixUp(game, m, false);
                        fixup[b].Position = new Vector3(-1, 0, m);
                        fixup[b].FixUpType = FixUpTypesEnum.Bottom;
                        break;
                    case 2:
                        fixup[b] = new GeoClipMapFootPrintFixUp(game, m, true);
                        fixup[b].Position = new Vector3(-edge, 0, -1);
                        fixup[b].FixUpType = FixUpTypesEnum.Left;
                        break;
                    case 3:
                        fixup[b] = new GeoClipMapFootPrintFixUp(game, m, true);
                        fixup[b].Position = new Vector3(m, 0, -1);
                        fixup[b].FixUpType = FixUpTypesEnum.Right;
                        break;
                }
                fixup[b].Orientation *= Orientation;
            }

            for (int b = 0; b < trim.Length; b++)
            {
                switch (b)
                {
                    case 0:
                        trim[b] = new GeoClipMapFootPrintTrim(game, m, false);
                        trim[b].Position = new Vector3(-m, 0, -m);
                        break;
                    case 1:
                        trim[b] = new GeoClipMapFootPrintTrim(game, m, false);
                        trim[b].Position = new Vector3(0, 0, -m);
                        break;
                    case 2:
                        trim[b] = new GeoClipMapFootPrintTrim(game, m, true);
                        trim[b].Position = new Vector3(m - 1, 0, -m);
                        break;
                    case 3:
                        trim[b] = new GeoClipMapFootPrintTrim(game, m, true);
                        trim[b].Position = new Vector3(m - 1, 0, 0);
                        break;
                }
                trim[b].Orientation *= Orientation;
            }
        }


        public override void Initialize()
        {
            base.Initialize();

            for (int b = 0; b < blocks.Length; b++)
                blocks[b].Initialize();

            for (int b = 0; b < fixup.Length; b++)
                fixup[b].Initialize();

            for (int b = 0; b < trim.Length; b++)
                trim[b].Initialize();

        }
        Texture2D heightMap;
        protected override void LoadContent()
        {
            effect = Game.Content.Load<Effect>(effectToLoad);
            RenderType = _RenderType;

            heightMap = Game.Content.Load<Texture2D>(heightMapAsset);

            Color[] data = new Color[heightMap.Width * heightMap.Height];
            heightMap.GetData<Color>(data);

            heightMap = new Texture2D(Game.GraphicsDevice, heightMap.Width, heightMap.Height, true, SurfaceFormat.Vector4);
            Vector4[] data2 = new Vector4[data.Length];
            for (int x = 0; x < data.Length; x++)
                data2[x] = new Vector4(data[x].R / 255f, data[x].G / 255f, data[x].B / 255f, data[x].A / 255f);
            heightMap.SetData<Vector4>(data2);

            //FileStream f = new FileStream("height.jpg", FileMode.Create);
            //heightMap.SaveAsJpeg(f, heightMap.Width, heightMap.Height);
            //f.Close();

            if (effect.Parameters["heightMap"] != null)
            {
                effect.Parameters["heightMap"].SetValue(heightMap);

                effect.Parameters["scale"].SetValue(Scale);
                if (effect.Parameters["scale2"]!=null)
                    effect.Parameters["scale2"].SetValue(CorseScale);

                effect.Parameters["sqrt"].SetValue((new Vector2(heightMap.Width, heightMap.Height) / 2));

                effect.Parameters["hp"].SetValue(new Vector2(heightMap.Width, heightMap.Height) / 2);
                effect.Parameters["OneOverWidth"].SetValue(1f / heightMap.Width);
            }

            if (effect.Parameters["LayerMap0"] != null)
            {
                effect.Parameters["LayerMap0"].SetValue(Game.Content.Load<Texture2D>("Textures/Terrain/dirt"));
                effect.Parameters["BumpMap0"].SetValue(Game.Content.Load<Texture2D>("Textures/Terrain/dirtNormal"));

                effect.Parameters["LayerMap1"].SetValue(Game.Content.Load<Texture2D>("Textures/Terrain/grass"));
                effect.Parameters["BumpMap1"].SetValue(Game.Content.Load<Texture2D>("Textures/Terrain/grassNormal"));

                effect.Parameters["LayerMap2"].SetValue(Game.Content.Load<Texture2D>("Textures/Terrain/stone"));
                effect.Parameters["BumpMap2"].SetValue(Game.Content.Load<Texture2D>("Textures/Terrain/stoneNormal"));

                effect.Parameters["LayerMap3"].SetValue(Game.Content.Load<Texture2D>("Textures/Terrain/snow2"));
                effect.Parameters["BumpMap3"].SetValue(Game.Content.Load<Texture2D>("Textures/Terrain/snow2Normal"));

                effect.Parameters["mod"].SetValue((heightMap.Width + heightMap.Height) / 2f);

            }
        }

        public override void Update(GameTime gameTime)
        {
            World = Matrix.CreateScale(Scale) * Matrix.CreateFromQuaternion(Orientation) * Matrix.CreateTranslation(Position);
            base.Update(gameTime);

            for (int b = 0; b < blocks.Length; b++)
                blocks[b].Update(gameTime);

            for (int b = 0; b < fixup.Length; b++)
                fixup[b].Update(gameTime);

            for (int b = 0; b < trim.Length; b++)
                trim[b].Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            effect.Parameters["world"].SetValue(World);
            effect.Parameters["wvp"].SetValue(World * camera.View * camera.Projection);

            if (effect.Parameters["vp"] != null)
                effect.Parameters["vp"].SetValue(camera.View * camera.Projection);

            if (effect.Parameters["EyePosition"] != null)
                effect.Parameters["EyePosition"].SetValue(camera.Position);

            if (effect.Parameters["maxHeight"] != null)
                effect.Parameters["maxHeight"].SetValue(maxHeight);



            effect.CurrentTechnique.Passes[0].Apply();

            for (int b = 0; b < blocks.Length; b++)
            {
                //if (camera.Frustum.Intersects(blocks[b].getBounds(World)))
                blocks[b].Draw(gameTime);
            }

            for (int b = 0; b < fixup.Length; b++)
                fixup[b].Draw(gameTime);

            for (int b = 0; b < trim.Length; b++)
                trim[b].Draw(gameTime);
        }

        public void Rotate(Vector3 axis, float angle)
        {
            axis = Vector3.Transform(axis, Matrix.CreateFromQuaternion(Orientation));
            Orientation = Quaternion.Normalize(Quaternion.CreateFromAxisAngle(axis, angle) * Orientation);
        }
    }
}
