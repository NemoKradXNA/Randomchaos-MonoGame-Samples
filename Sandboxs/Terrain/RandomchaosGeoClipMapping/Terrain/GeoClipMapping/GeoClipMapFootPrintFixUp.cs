using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RandomchaosGeoClipMapping.Terrain.GeoClipMapping
{
    public enum FixUpTypesEnum
    {
        Top,
        Left,
        Right,
        Bottom
    }
    public class GeoClipMapFootPrintFixUp : GeoClipMapFootPrintBase
    {
        public FixUpTypesEnum FixUpType = FixUpTypesEnum.Top;
        public GeoClipMapFootPrintFixUp(Game game, short m, bool lr) : base(game)
        {
            verts = new VertexPositionColor[m * 3];

            if (!lr)
            {
                width = m;
                height = 3;
            }
            else
            {
                width = 3;
                height = m;
            }

            color = Color.Green;
        }

        public override void Initialize()
        {
            base.Initialize();

            // Make the edge red.
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    switch (FixUpType)
                    {
                        case FixUpTypesEnum.Top:
                            if (x == 0)
                                verts[x + y * width].Color = new Color(1f, 0, 0, 1f);
                            break;
                        case FixUpTypesEnum.Left:
                            if (y == 0)
                                verts[x + y * width].Color = new Color(1f, 1f, 0, 1f);
                            break;
                        case FixUpTypesEnum.Right:
                            if (y == height - 1)
                                verts[x + y * width].Color = new Color(1f, 1f, 1f, 1f);
                            break;
                        case FixUpTypesEnum.Bottom:
                            if (x == width - 1)
                                verts[x + y * width].Color = new Color(1f, 1f, 1f, 0);
                            break;
                    }
                }
            }
        }
    }
}
