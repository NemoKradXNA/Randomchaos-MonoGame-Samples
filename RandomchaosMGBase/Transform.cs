using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;



namespace RandomchaosMGBase
{
    public class Transform : ITransform, IDisposable
    {
        public static Transform EmptyTransform { get { return new Transform(null); } }

        public IHasTransform Owner { get; set; }

        
        string guid { get; set; }

        public string GUID { get { return guid; } }

        /// <summary>
        /// The scale of the transform (how big it is)
        /// </summary>
        public Vector3 Scale
        {
            get
            {
                if (Parent != null)
                    return _Scale * Parent.Transform.Scale;
                else
                    return _Scale;
            }
            set
            {
                if (Parent != null)
                {
                    _Scale = value / Parent.Transform.Scale;
                }
                else
                    _Scale = value;
            }
        }

        protected Vector3 _Scale { get; set; }

        public Vector2 Scale2D
        {
            get
            {
                return new Vector2(Scale.X, Scale.Y);
            }
            set
            {
                Scale = new Vector3(value.X, value.Y, Scale.Z);
            }
        }

        public Vector3 LocalScale
        {
            get
            {
                return _Scale;
            }
            set { _Scale = value; }
        }

        Vector3 _position;

        /// <summary>
        /// The position of the transform in world space (relative to it's parent)
        /// </summary>
        public Vector3 Position //{ get; set; }
        {
            get
            {
                if (Parent != null)
                    return Vector3.Transform(_position, Parent.Transform.World);
                else
                    return _position;
            }
            set
            {
                if (Parent != null)
                    _position = Vector3.Transform(value, Matrix.Invert(Parent.Transform.World));
                else
                    _position = value;
            }
        }

        public Vector2 Position2D
        {
            get
            {
                return new Vector2(Position.X, Position.Y);
            }
            set
            {
                Position = new Vector3(value.X, value.Y, Position.Z);
            }
        }

        public Vector2 LocalPosition2D
        {
            get
            {
                return new Vector2(LocalPosition.X, LocalPosition.Y);
            }
            set
            {
                LocalPosition = new Vector3(value.X, value.Y, LocalPosition.Z);
            }
        }

        public Vector3 LocalPosition
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }


        public Quaternion LocalRotation
        {
            get
            {
                return RotationRef;
            }
            set
            {
                RotationRef = value;
            }
        }

        /// <summary>
        /// Used to pass the rotation as a ref.
        /// </summary>

        public Quaternion RotationRef;

        /// <summary>
        /// The rotation of the transform in world space (relative to it's parent)
        /// </summary>
        public Quaternion Rotation
        {
            get
            {
                if (Parent != null)
                {
                    Parent.Transform.Update();
                    return RotationRef / Parent.Transform.Rotation;
                }
                else
                    return RotationRef;
            }
            set
            {
                if (Parent != null)
                {
                    Parent.Transform.Update();
                    RotationRef = value * Parent.Transform.Rotation;
                }
                else
                    RotationRef = value;
            }
        }


        protected Matrix _world { get; set; }
        /// <summary>
        /// World transform.
        /// </summary>
        public Matrix World
        {
            get { return _world; }
        }

        /// <summary>
        /// All the child transforms associated with this trandform
        /// </summary>
        public List<IHasTransform> Children { get; set; }

        /// <summary>
        /// This transforms parent if it has one
        /// </summary>

        IHasTransform parent = null;
        public IHasTransform Parent
        {
            get { return parent; }
            set
            {
                if (value == null)
                {
                    if (Parent != null)
                    {
                        if (parent.Transform.Children != null && parent.Transform.Children.SingleOrDefault(t => t == Owner) != null)
                            parent.Transform.Children.Remove(Owner);
                    }
                    parent = value;
                }
                else
                {
                    parent = value;


                    if (parent.Transform.Children == null)
                        parent.Transform.Children = new List<IHasTransform>();

                    if (!parent.Transform.Children.Contains(Owner))
                        parent.Transform.Children.Add(Owner);
                }
            }
        }

        string parentGUI = null;
        public string ParentGUID
        {
            get
            {
                if (parent != null)
                {
                    parentGUI = parent.Transform.GUID;
                }

                return parentGUI;
            }
            set
            {
                parentGUI = value;
            }
        }

        public string Name { get; set; }

        public Transform()
        {
            Scale = Vector3.One;
            Position = Vector3.Zero;
            RotationRef = Quaternion.Identity;

            Children = new List<IHasTransform>();
            Parent = null;

            GenerateDefaultName();

            // Need base world gen;
            _world = Matrix.CreateScale(LocalScale) * Matrix.CreateFromQuaternion(LocalRotation) * Matrix.CreateTranslation(LocalPosition);
        }
        /// <summary>
        /// ctor
        /// </summary>
        public Transform(IHasTransform owner) : this()
        {
            Owner = owner;
        }

        public void Update()
        {
            if (Parent != null)
                _world = Matrix.CreateScale(LocalScale) * Matrix.CreateFromQuaternion(LocalRotation) * Matrix.CreateTranslation(LocalPosition) * Parent.Transform.World;
            else
                _world = Matrix.CreateScale(LocalScale) * Matrix.CreateFromQuaternion(LocalRotation) * Matrix.CreateTranslation(LocalPosition);
        }


        public virtual bool SetParent(IHasTransform transform)
        {
            bool done = true;

            if (transform == null)
            {
                IHasTransform obj = null;

                if (Parent != null)
                    obj = ((Transform)Parent.Transform).Children.SingleOrDefault(t => t.Transform == this);

                if (obj != null)
                    ((Transform)Parent.Transform).Children.Remove(obj);

                Parent = null;
            }
            else
            {
                if (Parent != null && !((Transform)Parent.Transform).Children.Contains(transform))
                    ((Transform)Parent.Transform).Children.Add(transform);
                Parent = transform;

            }

            return done;
        }

        /// <summary>
        /// Method to look at a target.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="speed"></param>
        public void LookAt(Vector3 target, float speed, Vector3 fwd)
        {
            GameComponentHelper.LookAt(target, speed, Position, ref RotationRef, fwd);
        }

        /// <summary>
        /// Method to look at target and lock rotation axis
        /// </summary>
        /// <param name="target"></param>
        /// <param name="speed"></param>
        /// <param name="fwd"></param>
        /// <param name="lockedRots">Axis to be lcoked (multiplied by 0)</param>
        public void LookAtLockRotation(Vector3 target, float speed, Vector3 fwd, Vector3 lockedRots)
        {
            GameComponentHelper.LookAtLockRotation(target, speed, Position, ref RotationRef, fwd, lockedRots);
        }

        public Vector3 GetIntPosition()
        {
            return new Vector3((int)(Position.X + .5f), (int)(Position.Y + .5f), (int)(Position.Z + .5f));
        }

        /// <summary>
        /// Method to translate object
        /// </summary>
        /// <param name="distance"></param>
        public void Translate(Vector3 distance)
        {
            Position += GameComponentHelper.Translate3D(distance, Rotation);
        }

        public void TranslateAA(Vector3 distance)
        {
            Position += GameComponentHelper.Translate3D(distance);
        }

        /// <summary>
        /// Method to rotate object
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        public void Rotate(Vector3 axis, float angle)
        {
            GameComponentHelper.Rotate(axis, angle, ref RotationRef);
        }

        public void RotateAA(Vector3 axis, float angle)
        {
            GameComponentHelper.RotateAA(axis, angle, ref RotationRef);
        }

        public virtual void Dispose()
        {

        }



        public void GenerateDefaultName()
        {
            guid = Guid.NewGuid().ToString();
            Name = GameComponentHelper.GenerateObjectName(this.GetType());
        }



        public static Transform Identity
        {
            get { return new Transform(null); }
        }
    }
}
