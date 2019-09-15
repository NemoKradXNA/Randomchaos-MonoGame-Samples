using Microsoft.Xna.Framework;

namespace RandomchaosMGBase.BaseClasses.PostProcessing
{
    public class ScanLinesEffect : BasePostProcessingEffect
    {
        ScanLines scanLines;
        public float NoiseIntensity { get { return scanLines.NoiseIntensity; } set { scanLines.NoiseIntensity = value; } }
        public float LineIntensity { get { return scanLines.LineIntensity; } set { scanLines.LineIntensity = value; } }
        public float LineCount { get { return scanLines.LineCount; } set { scanLines.LineCount = value; } }

        public ScanLinesEffect(Game game, float noiseIntensity, float lineIntensity, float lineCount) : base(game)
        {
            scanLines = new ScanLines(game, noiseIntensity, lineCount, lineCount);

            AddPostProcess(scanLines);
        }
    }
}
