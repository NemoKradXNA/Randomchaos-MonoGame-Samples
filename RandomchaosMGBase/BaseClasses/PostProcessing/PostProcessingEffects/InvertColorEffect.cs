using Microsoft.Xna.Framework;

namespace RandomchaosMGBase.BaseClasses.PostProcessing
{
    public class InvertColorEffect : BasePostProcessingEffect
    {
        InvertColor ivc;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="game"></param>
        public InvertColorEffect(Game game) : base(game)
        {
            ivc = new InvertColor(game);

            AddPostProcess(ivc);
        }
    }
}
