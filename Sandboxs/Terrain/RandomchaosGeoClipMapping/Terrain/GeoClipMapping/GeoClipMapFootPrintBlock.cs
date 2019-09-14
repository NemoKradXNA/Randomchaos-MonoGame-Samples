using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RandomchaosGeoClipMapping.Terrain.GeoClipMapping
{
    public enum BlockTypesEnum
    {
        Centre,
        TopLeft,
        Top,
        TopRight,
        Right,
        BottomRight,
        Bottom,
        BottomLeft,
        Left
    }

    public class GeoClipMapFootPrintBlock : GeoClipMapFootPrintBase
    {
        public BlockTypesEnum BlockType = BlockTypesEnum.Centre;
        public GeoClipMapFootPrintBlock(Game game, short m) : base(game)
        {
            verts = new VertexPositionColor[m * m];

            width = m;
            height = m;

            color = Color.DarkGray;
        }

        public override void Initialize()
        {
            base.Initialize();

            // Make the edge red.
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    switch (BlockType)
                    {
                        case BlockTypesEnum.Centre:
                            if (x == 0)
                                verts[x + y * width].Color = new Color(1f, 0, 0, 1f);
                            if (y == 0)
                                verts[x + y * width].Color = new Color(1f, 1f, 0, 1f);
                            if (y == height - 1)
                                verts[x + y * width].Color = new Color(1f, 1f, 1f, 1f);
                            if (x == width - 1)
                                verts[x + y * width].Color = new Color(1f, 1f, 1f, 0);
                            break;
                        case BlockTypesEnum.TopLeft:
                            if (x == 0)
                                verts[x + y * width].Color = new Color(1f, 0, 0, 1f);
                            if (y == 0)
                                verts[x + y * width].Color = new Color(1f, 1f, 0, 1f);
                            break;
                        case BlockTypesEnum.Top:
                            if (x == 0)
                                verts[x + y * width].Color = new Color(1f, 0, 0, 1f);
                            break;
                        case BlockTypesEnum.TopRight:
                            if (x == 0)
                                verts[x + y * width].Color = new Color(1f, 0, 0, 1f);
                            if (y == height - 1)
                                verts[x + y * width].Color = new Color(1f, 1f, 1f, 1f);
                            break;
                        case BlockTypesEnum.Left:
                            if (y == 0)
                                verts[x + y * width].Color = new Color(1f, 1f, 0, 1f);
                            break;
                        case BlockTypesEnum.Right:
                            if (y == height - 1)
                                verts[x + y * width].Color = new Color(1f, 1f, 1f, 1f);
                            break;
                        case BlockTypesEnum.BottomLeft:
                            if (x == width - 1)
                                verts[x + y * width].Color = new Color(1f, 1f, 1f, 0);
                            if (y == 0)
                                verts[x + y * width].Color = new Color(1f, 1f, 0, 1f);
                            break;
                        case BlockTypesEnum.Bottom:
                            if (x == width - 1)
                                verts[x + y * width].Color = new Color(1f, 1f, 1f, 0);
                            break;
                        case BlockTypesEnum.BottomRight:
                            if (x == width - 1)
                                verts[x + y * width].Color = new Color(1f, 1f, 1f, 0);
                            if (y == height - 1)
                                verts[x + y * width].Color = new Color(1f, 1f, 1f, 1f);
                            break;
                    }
                }
            }
        }

    }
}
