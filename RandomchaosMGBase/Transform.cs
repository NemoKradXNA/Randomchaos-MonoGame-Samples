using Microsoft.Xna.Framework;

namespace RandomchaosMGBase
{
    public class Transform
    {
        public static Transform EmptyTransform { get { return new Transform(); } }


        protected Vector3 _Scale { get; set; }
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
        public Vector3 Position 
        {
            get
            {
                if (Parent != null)
                    return _position + Parent.Transform.Position;
                else
                    return _position;
            }
            set
            {
                if (Parent != null)
                {
                    _position = value - Parent.Transform.Position;
                }
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
                    return RotationRef / Parent.Transform.Rotation;
                else
                    return RotationRef;
            }
            set
            {
                if (Parent != null)
                    RotationRef = value * Parent.Transform.Rotation;
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
        /// This transforms parent if it has one
        /// </summary>

        IHasTransform parent = null;
        public IHasTransform Parent
        {
            get { return parent; }
            set { parent = value; }
        }

      
        /// <summary>
        /// ctor
        /// </summary>
        public Transform()
        {
            //TestMe = new Pretend();

            _Scale = Vector3.One;
            _position = Vector3.Zero;
            RotationRef = Quaternion.Identity;
            Parent = null;

            Update();
        }

        public void Update()
        {
            if (Parent != null)
                _world = Matrix.CreateScale(LocalScale) * Matrix.CreateFromQuaternion(LocalRotation) * Matrix.CreateTranslation(LocalPosition) * Parent.Transform.World;
            else
                _world = Matrix.CreateScale(LocalScale) * Matrix.CreateFromQuaternion(LocalRotation) * Matrix.CreateTranslation(LocalPosition);
        }       

        
        public static Transform Identity
        {
            get { return new Transform(); }
        }
    }
}
