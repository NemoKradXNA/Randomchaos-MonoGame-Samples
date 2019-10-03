using Microsoft.Xna.Framework;

namespace RandomchaosMGDeferredLighting.Lights
{
    public class DeferredPointLight : BaseLight
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="game"></param>
        public DeferredPointLight(Game game) : base(game) { }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="game"></param>
        /// <param name="position"></param>
        /// <param name="color"></param>
        /// <param name="radius"></param>
        /// <param name="intensity"></param>
        /// <param name="castShadow"></param>
        public DeferredPointLight(Game game, Vector3 position, Color color, float radius, float intensity, bool castShadow) : base(game, position, color, intensity, castShadow)
        {
            Radius = radius;
        }

        /// <summary>
        /// Radius of the point light
        /// </summary>
        public float Radius { get; set; }
    }
}
