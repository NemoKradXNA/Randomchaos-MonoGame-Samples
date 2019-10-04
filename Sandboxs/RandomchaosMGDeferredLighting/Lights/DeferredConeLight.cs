using System;
using Microsoft.Xna.Framework;

namespace RandomchaosMGDeferredLighting.Lights
{
    public class DeferredConeLight : BaseLight
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="game"></param>
        public DeferredConeLight(Game game) : base(game)
        {
            ShadowOffsetDistance = 250;
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="game"></param>
        /// <param name="position"></param>
        /// <param name="color"></param>
        /// <param name="intensity"></param>
        /// <param name="angle"></param>
        /// <param name="decay"></param>
        /// <param name="castShadow"></param>
        public DeferredConeLight(Game game, Vector3 position, Color color, float intensity, float angle, float decay, bool castShadow)
            : base(game, position, color, intensity, castShadow)
        {
            Angle = angle;
            Decay = decay;
        }

        /// <summary>
        /// Cone's rotation matrix
        /// </summary>
        protected Matrix RotationMatrix
        {
            get { return Matrix.CreateFromQuaternion(Transform.Rotation); }
        }

        /// <summary>
        /// Cones translation matrix
        /// </summary>
        protected Matrix TraslationMatrix
        {
            get { return Matrix.CreateTranslation(Transform.Position); }
        }

        /// <summary>
        /// Cone's world matrix
        /// </summary>
        protected Matrix World
        {
            get { return RotationMatrix * TraslationMatrix; }
        }

        /// <summary>
        /// Direction the cone is pointing
        /// </summary>
        public Vector3 Direction
        {
            get
            {
                return Transform.World.Forward;
            }
        }

        /// <summary>
        /// Method to rotate the cone
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        public void Rotate(Vector3 axis, float angle)
        {
            Transform.Rotate(axis, angle);
        }
                
        /// <summary>
        /// Cone view matrix property
        /// </summary>
        public override Matrix View
        {
            get
            {
                if (camera != null)
                {
                    return Matrix.CreateLookAt(Transform.Position, Transform.Position + (Direction * 10), Vector3.Transform(Vector3.Forward, Matrix.Invert(World)));
                }
                else
                    return Matrix.Identity;
            }
        }

        /// <summary>
        /// Cone projection matrix property
        /// </summary>
        public override Matrix Projection
        {
            get
            {
                if (camera != null && camera.Viewport.MinDepth > 0)
                {
                    float fov = (float)Math.Acos(MathHelper.Clamp(Angle, float.Epsilon, .9999999f)) * 2f;
                    //fov = MathHelper.PiOver4;

                    return Matrix.CreatePerspectiveFieldOfView(fov, camera.Viewport.AspectRatio, camera.Viewport.MinDepth, camera.Viewport.MaxDepth);
                }
                else
                    return Matrix.Identity;
            }
        }


        /// <summary>
        /// Angle of the cone
        /// </summary>
        public float Angle { get; set; }

        /// <summary>
        /// Lighting decay value
        /// </summary>
        public float Decay { get; set; }
    }
}
