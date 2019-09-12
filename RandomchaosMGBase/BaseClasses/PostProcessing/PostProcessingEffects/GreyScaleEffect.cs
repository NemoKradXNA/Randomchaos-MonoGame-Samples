using Microsoft.Xna.Framework;

namespace RandomchaosMGBase.BaseClasses.PostProcessing
{
    public class GreyScaleEffect : BasePostProcessingEffect
    {
        GreyScale gs;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="game"></param>
        public GreyScaleEffect(Game game) : base(game)
        {
            gs = new GreyScale(game);

            AddPostProcess(gs);
        }
    }
}
