using Microsoft.Xna.Framework;

namespace RandomchaosMGBase.BaseClasses.PostProcessing
{
    public class BleachEffect  : BasePostProcessingEffect 
    {
        BleachByPass bleachByPass;

        public float Opacity { get { return bleachByPass.Opacity; } set { bleachByPass.Opacity = value; } }

        public BleachEffect(Game game, float opacity) : base(game)
        {
            bleachByPass = new BleachByPass(game, opacity);

            AddPostProcess(bleachByPass);
        }
    }
}
