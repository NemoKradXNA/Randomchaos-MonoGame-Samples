using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace RandomchaosMGBase
{
    public interface ITransform : IDisposable, IHasName
    {
        string ParentGUID { get; set; }
        string GUID { get; }

        Vector3 Scale
        { get; set; }

        Vector2 Scale2D
        { get; set; }


        Vector3 Position
        { get; set; }

        Vector2 Position2D
        { get; set; }

        Vector2 LocalPosition2D
        { get; set; }

        Quaternion Rotation
        { get; set; }

        Vector3 LocalPosition { get; set; }
        Vector3 LocalScale { get; set; }
        Quaternion LocalRotation { get; set; }

        Matrix World
        { get; }

        IHasTransform Parent
        { get; set; }

        List<IHasTransform> Children { get; set; }

        IHasTransform Owner { get; set; }


        bool SetParent(IHasTransform transform);

        void LookAt(Vector3 target, float speed, Vector3 fwd);
        void LookAtLockRotation(Vector3 target, float speed, Vector3 fwd, Vector3 lockedRots);

        Vector3 GetIntPosition();

        void Translate(Vector3 distance);

        void TranslateAA(Vector3 distance);

        void Rotate(Vector3 axis, float angle);
        void RotateAA(Vector3 axis, float angle);

        void Update();
    }
}
