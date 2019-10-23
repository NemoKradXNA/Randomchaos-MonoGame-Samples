using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RandomchaosMGBase;
using RandomchaosMGBase.BaseClasses;

namespace RandomchaosMGBase
{
    public static class GameComponentHelper
    {
        public static Ray BuildRay(Point screenPixel, Base3DCamera camera)
        {
            Viewport vp = new Viewport(0, 0, camera.Game.GraphicsDevice.PresentationParameters.BackBufferWidth, camera.Game.GraphicsDevice.PresentationParameters.BackBufferHeight);
            vp.MinDepth = camera.Viewport.MinDepth;
            vp.MaxDepth = camera.Viewport.MaxDepth;

            Vector3 nearSource = vp.Unproject(new Vector3(screenPixel.X, screenPixel.Y, camera.Viewport.MinDepth), camera.Projection, camera.View, Matrix.Identity);
            Vector3 farSource = vp.Unproject(new Vector3(screenPixel.X, screenPixel.Y, camera.Viewport.MaxDepth), camera.Projection, camera.View, Matrix.Identity);
            Vector3 direction = farSource - nearSource;

            direction.Normalize();

            return new Ray(nearSource, direction);
        }

        public static Vector2 CameraScreenPoint(Point screenPixel, Base3DCamera camera)
        {
            Vector2 retVal = new Vector2(((((float)screenPixel.X)) / camera.Game.GraphicsDevice.PresentationParameters.BackBufferWidth) * camera.Viewport.Width, ((((float)screenPixel.Y)) / camera.Game.GraphicsDevice.PresentationParameters.BackBufferHeight) * camera.Viewport.Height);

            return retVal;
        }

        public static float RayPicking(Point screenPixel, BoundingBox volume, Base3DCamera camera)
        {
            float? retVal = float.MaxValue;

            BuildRay(screenPixel, camera).Intersects(ref volume, out retVal);

            if (retVal != null)
                return retVal.Value;
            else
                return float.MaxValue;
        }               

        public static short[] BoxIndexMap = new short[] {
                0, 1, 0,
                2, 1, 3,
                2, 3, 4,
                5, 4, 6,
                5, 7, 6,
                7, 0, 4,
                1, 5, 2,
                6, 3, 7
                };

        public static void BuildBoxCorners(List<BoundingBox> boxs, List<Matrix> worlds, Color color, out VertexPositionColor[] points, out short[] index)
        {

            points = new VertexPositionColor[boxs.Count * 8];
            short[] inds = new short[points.Length * 3];

            for (int b = 0; b < boxs.Count; b++)
            {
                Vector3[] thisCorners = boxs[b].GetCorners();

                points[(b * 8) + 0] = new VertexPositionColor(thisCorners[1], color);
                points[(b * 8) + 1] = new VertexPositionColor(thisCorners[0], color);
                points[(b * 8) + 2] = new VertexPositionColor(thisCorners[2], color);
                points[(b * 8) + 3] = new VertexPositionColor(thisCorners[3], color);
                points[(b * 8) + 4] = new VertexPositionColor(thisCorners[5], color);
                points[(b * 8) + 5] = new VertexPositionColor(thisCorners[4], color);
                points[(b * 8) + 6] = new VertexPositionColor(thisCorners[6], color);
                points[(b * 8) + 7] = new VertexPositionColor(thisCorners[7], color);

                for (int i = 0; i < 24; i++)
                {
                    inds[(b * 24) + i] = (short)(BoxIndexMap[i] + (b * 8));
                }
            }

            index = inds;
        }

        public static BoundingBox TransformedBoundingBoxAA(BoundingBox boxToTRansfork, Vector3 Position, Vector3 Scale)
        {
            Matrix AAWorld = Matrix.CreateScale(Scale) * Matrix.CreateTranslation(Position);

            Vector3 min = Vector3.Transform(boxToTRansfork.Min, AAWorld);
            Vector3 max = Vector3.Transform(boxToTRansfork.Max, AAWorld);

            return new BoundingBox(min, max);
        }

        /// <summary>
        /// Method to turn a 3D object to face a position in world space.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="speed"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public static void LookAt(Vector3 target, float speed, Vector3 position, Quaternion rotation, Vector3 fwd)
        {
            LookAt(target, speed, position, ref rotation, fwd);
        }
        public static void LookAt(Vector3 target, float speed, Vector3 position, ref Quaternion rotation, Vector3 fwd)
        {
            if (fwd == Vector3.Zero)
                fwd = Vector3.Forward;

            Vector3 tminusp = target - position;
            Vector3 ominusp = fwd;

            if (tminusp == Vector3.Zero)
                return;

            tminusp.Normalize();

            float theta = (float)System.Math.Acos(Vector3.Dot(tminusp, ominusp));
            Vector3 cross = Vector3.Cross(ominusp, tminusp);

            if (cross == Vector3.Zero)
                return;

            cross.Normalize();

            Quaternion targetQ = Quaternion.CreateFromAxisAngle(cross, theta);
            rotation = Quaternion.Slerp(rotation, targetQ, speed);
        }

        public static void LookAtLockRotation(Vector3 target, float speed, Vector3 position, ref Quaternion rotation, Vector3 fwd, Vector3 lockedRots)
        {
            LookAt(target, speed, position, ref rotation, fwd);

            LockRotation(ref rotation, lockedRots);

        }

        public static void LockRotation(ref Quaternion rotation, Vector3 lockedRots)
        {
            lockedRots -= -Vector3.One;
            Vector3 rots = GameComponentHelper.QuaternionToEulerAngleVector3(rotation) * lockedRots;
            rotation = Quaternion.CreateFromRotationMatrix(Matrix.CreateRotationX(rots.X) * Matrix.CreateRotationY(rots.Y) * Matrix.CreateRotationX(rots.Z));
        }
        // Converts a Quaternion to Euler angles (X = Yaw, Y = Pitch, Z = Roll)
        public static Vector3 QuaternionToEulerAngleVector3(Quaternion rotation)
        {
            Vector3 rotationaxes = new Vector3();
            Vector3 forward = Vector3.Transform(Vector3.Forward, rotation);
            Vector3 up = Vector3.Transform(Vector3.Up, rotation);

            rotationaxes = AngleTo(new Vector3(), forward);

            if (rotationaxes.X == MathHelper.PiOver2)
            {
                rotationaxes.Y = (float)Math.Atan2((double)up.X, (double)up.Z);
                rotationaxes.Z = 0;
            }
            else if (rotationaxes.X == -MathHelper.PiOver2)
            {
                rotationaxes.Y = (float)Math.Atan2((double)-up.X, (double)-up.Z);
                rotationaxes.Z = 0;
            }
            else
            {
                up = Vector3.Transform(up, Matrix.CreateRotationY(-rotationaxes.Y));
                up = Vector3.Transform(up, Matrix.CreateRotationX(-rotationaxes.X));

                //rotationaxes.Z = (float)Math.Atan2((double)-up.Z, (double)up.Y);
                rotationaxes.Z = (float)Math.Atan2((double)-up.X, (double)up.Y);
            }

            return rotationaxes;
        }

        public static Quaternion EulerAngleVector3ToQuaterion(Vector3 euler)
        {
            return Quaternion.CreateFromRotationMatrix(Matrix.CreateFromYawPitchRoll(euler.Y, euler.X, euler.Z));
        }

        // Returns Euler angles that point from one point to another
        public static Vector3 AngleTo(Vector3 from, Vector3 location)
        {
            Vector3 angle = new Vector3();
            Vector3 v3 = Vector3.Normalize(location - from);

            angle.X = (float)Math.Asin(v3.Y);
            angle.Y = (float)Math.Atan2((double)-v3.X, (double)-v3.Z);

            return angle;
        }
        /// <summary>
        /// Method to rotate an object.
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        /// <param name="rotation"></param>
        public static void Rotate(Vector3 axis, float angle, ref Quaternion rotation)
        {
            axis = Vector3.Transform(axis, Matrix.CreateFromQuaternion(rotation));
            rotation = Quaternion.Normalize(Quaternion.CreateFromAxisAngle(axis, angle) * rotation);
        }

        public static void RotateAA(Vector3 axis, float angle, ref Quaternion rotation)
        {
            //axis = Vector3.Transform(axis, Matrix.CreateFromQuaternion(rotation));
            rotation = Quaternion.Normalize(Quaternion.CreateFromAxisAngle(axis, angle) * rotation);
        }
        /// <summary>
        /// Method to translate a 3D object based on it's rotation
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static Vector3 Translate3D(Vector3 distance, Quaternion rotation)
        {
            return Vector3.Transform(distance, Matrix.CreateFromQuaternion(rotation));
        }
        /// <summary>
        /// Method to translate a 3D object
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static Vector3 Translate3D(Vector3 distance)
        {
            return Vector3.Transform(distance, Matrix.CreateFromQuaternion(Quaternion.Identity));
        }

        static Dictionary<Type, int> instanceCounts = new Dictionary<Type, int>();
        public static Dictionary<Type, int> InstanceCounts
        {
            get
            {
                return instanceCounts;
            }
        }
        public static string GenerateObjectName(Type type)
        {
            if (!instanceCounts.ContainsKey(type))
                instanceCounts.Add(type, 0);

            string name = $"{type.Name}{instanceCounts[type]}";
            instanceCounts[type]++;

            return name;
        }
    }
}
