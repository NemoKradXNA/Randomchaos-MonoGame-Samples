using Microsoft.Xna.Framework;

namespace RandomchaosMGBase.BaseClasses.PostProcessing
{
    public class SepiaEffect : BasePostProcessingEffect
    {
        Sepia s;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="game"></param>
        public SepiaEffect(Game game) : base(game)
        {
            s = new Sepia(game);

            AddPostProcess(s);
        }
    }
}
