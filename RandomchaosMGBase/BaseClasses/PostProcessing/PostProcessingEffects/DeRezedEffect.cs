using Microsoft.Xna.Framework;

namespace RandomchaosMGBase.BaseClasses.PostProcessing
{
    public class DeRezedEffect : BasePostProcessingEffect
    {
        DeRezed deRezed;
        public int NumberOfTiles { get { return deRezed.NumberOfTiles; } set { deRezed.NumberOfTiles = value; } }

        public DeRezedEffect(Game game, int numberOfTiles) : base(game)
        {
            deRezed = new DeRezed(game, numberOfTiles);

            AddPostProcess(deRezed);
        }
    }
}
