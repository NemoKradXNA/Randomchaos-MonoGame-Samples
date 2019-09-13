using Microsoft.Xna.Framework;

namespace RandomchaosMGBase.BaseClasses.PostProcessing
{
    public class DepthOfFieldEffect : BasePostProcessingEffect
    {
        public PoissonDiscBlur pdb;
        public DepthOfField dof;

        public float DiscRadius { get { return pdb.DiscRadius; } set { pdb.DiscRadius = value; } }

        public DepthOfFieldEffect(Game game, float startFocusDistance, float startFocusRange) : base(game)
        {
            pdb = new PoissonDiscBlur(game);
            dof = new DepthOfField(game);
            dof.FocusDistance = startFocusDistance;
            dof.FocusRange = startFocusRange;

            AddPostProcess(pdb);
            AddPostProcess(dof);
        }

        public float FocalDistance
        {
            get { return dof.FocusDistance; }
            set { dof.FocusDistance = MathHelper.Clamp(value, camera.Viewport.MinDepth, camera.Viewport.MaxDepth); }
        }

        public float FocalRange
        {
            get { return dof.FocusRange; }
            set { dof.FocusRange = MathHelper.Clamp(value, camera.Viewport.MinDepth, camera.Viewport.MaxDepth); }
        }
    }
}
