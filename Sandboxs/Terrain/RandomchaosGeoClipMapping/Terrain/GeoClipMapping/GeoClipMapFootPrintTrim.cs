using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RandomchaosGeoClipMapping.Terrain.GeoClipMapping
{
    public class GeoClipMapFootPrintTrim : GeoClipMapFootPrintBase
    {
        public GeoClipMapFootPrintTrim(Game game, short m, bool lr) : base(game)
        {
            verts = new VertexPositionColor[(m + 1) * 2];

            if (lr)
            {
                width = (short)(m + 1);
                height = 2;
            }
            else
            {
                width = 2;
                height = (short)(m + 1);
            }

            color = Color.Blue;
        }
    }
}
