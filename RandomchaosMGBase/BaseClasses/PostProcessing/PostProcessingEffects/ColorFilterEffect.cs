using Microsoft.Xna.Framework;

namespace RandomchaosMGBase.BaseClasses.PostProcessing
{
    public class ColorFilterEffect : BasePostProcessingEffect
    {
        protected ColorFilter colorFilter;

        public float Burn { get { return colorFilter.Burn; } set { colorFilter.Burn = value; } }
        public float Saturation { get { return colorFilter.Saturation; } set { colorFilter.Saturation = value; } }
        public float Bright { get { return colorFilter.Bright; } set { colorFilter.Bright = value; } }
        public Color Color { get { return colorFilter.Color; } set { colorFilter.Color = value; } }

        public ColorFilterEffect(Game game, Color color, float burn, float saturation, float bright) : base(game)
        {
            colorFilter = new ColorFilter(game, color, burn, saturation, bright);

            AddPostProcess(colorFilter);
        }
    }
}
