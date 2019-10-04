using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RandomchaosMGBase;
using RandomchaosMGBase.BaseClasses;

namespace RandomchaosMGDeferredLighting.Lights
{
    public class BaseLight : GameComponent
    {
        /// <summary>
        /// Scene camera
        /// </summary>
        protected DeferredLightingCamera camera
        {
            get { return ((DeferredLightingCamera)Game.Services.GetService(typeof(Base3DCamera))); }
        }

        /// <summary>
        /// Shadow map created by this light
        /// </summary>
        public RenderTarget2D ShadowMap { get; set; }

        /// <summary>
        /// ITransform
        /// </summary>
        public Transform Transform { get; set; }

        /// <summary>
        /// Light color
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// How bright the light is 0-1
        /// </summary>
        public float Intensity { get; set; }

        /// <summary>
        /// Does this light cast a shadow
        /// </summary>
        public bool CastShadow { get; set; }

        /// <summary>
        /// Lights view matrix
        /// </summary>
        public virtual Matrix View
        {
            get { return Matrix.Identity; }
        }

        /// <summary>
        /// Lights projection matrix
        /// </summary>
        public virtual Matrix Projection
        {
            get { return Matrix.Identity; }
        }


        public virtual bool HardShadows { get; set; }
        /// <summary>
        /// Shadow offset distance
        /// </summary>
        public virtual float ShadowOffsetDistance { get; set; }


        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="game"></param>
        public BaseLight(Game game) : base(game) { }
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="game"></param>
        /// <param name="position"></param>
        /// <param name="color"></param>
        /// <param name="intensity"></param>
        /// <param name="castShadow"></param>
        public BaseLight(Game game, Vector3 position, Color color, float intensity, bool castShadow) : this(game)
        {
            Transform = new Transform();
            Transform.Position = position;
            Color = color;
            Intensity = intensity;
            CastShadow = castShadow;

            
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            Transform.Update();
        }
    }
}
