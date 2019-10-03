using Microsoft.Xna.Framework;

namespace RandomchaosMGDeferredLighting.Lights
{
    public class DeferredDirectionalLight : BaseLight
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="game"></param>
        public DeferredDirectionalLight(Game game) : base(game) { }

        /// <summary>
        /// Shadow offset distance
        /// </summary>
        public float ShadowOffsetDistance { get; set; }

        public bool HardShadows { get; set; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="game"></param>
        /// <param name="position"></param>
        /// <param name="color"></param>
        /// <param name="intensity"></param>
        /// <param name="castShadow"></param>
        public DeferredDirectionalLight(Game game, Vector3 position, Color color, float intensity, bool castShadow)
            : base(game, position, color, intensity, castShadow)
        {
            ShadowOffsetDistance = 250;
        }

        /// <summary>
        /// View matrix property
        /// </summary>
        public override Matrix View
        {
            get
            {
                if (camera != null)
                {
                    Vector3 cp = camera.Transform.Position;
                    return Matrix.CreateLookAt(cp + (Direction * -ShadowOffsetDistance), cp + (Direction * ShadowOffsetDistance), Vector3.Up);
                }
                else
                    return Matrix.Identity;
            }
        }

        /// <summary>
        /// Projection matrix property
        /// </summary>
        public override Matrix Projection
        {
            get
            {
                if (camera != null && camera.Viewport.MinDepth > 0)
                    return Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4*.25f, Game.GraphicsDevice.Viewport.AspectRatio, 1, 2000);
                else
                    return Matrix.Identity;
            }
        }

        /// <summary>
        /// Direction of light
        /// </summary>
        public Vector3 Direction
        {
            get
            {
                return Transform.World.Forward;
            }
        }
        
    }
}
