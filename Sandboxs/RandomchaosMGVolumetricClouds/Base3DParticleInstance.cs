using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RandomchaosMGVolumetricClouds
{
    public class Base3DParticleInstance : GameComponent
    {
        static int ID;
        private int id;
        public int myID
        {
            get { return id; }
        }
        public Vector3 Position = Vector3.Zero;
        public Vector3 Scale = Vector3.One * .5f;
        public Quaternion Orientation = Quaternion.Identity;

        public Matrix World;
        public Matrix AAWorld;

        public Base3DParticleInstancer Instancer;


        public Base3DParticleInstance(Game game)
            : base(game)
        {
            ID++;
            id = ID;
        }

        Random rnd;

        Vector3 pMods { get; set; }

        public Base3DParticleInstance(Game game, Vector3 position, Vector3 scale, Base3DParticleInstancer instancer, Quaternion orientation)
            : this(game, position, scale, instancer)
        {
            Orientation = orientation;
            this.Update(null);
        }
        public Base3DParticleInstance(Game game, Vector3 position, Vector3 scale, Vector3 mods, Base3DParticleInstancer instancer)
            : this(game, position, scale, instancer)
        {
            pMods = mods;
            this.Update(null);
        }
        public Base3DParticleInstance(Game game, Vector3 position, Vector3 scale, Base3DParticleInstancer instancer)
            : this(game)
        {

            rnd = new Random(DateTime.Now.Millisecond);
            Position = position;
            Scale = scale;

            Instancer = instancer;
            Instancer.instanceTransformMatrices.Add(this, World);
            Instancer.Instances.Add(myID, this);

            this.Update(null);
        }

        public override void Update(GameTime gameTime)
        {
            World = Matrix.CreateScale(Scale) * Matrix.CreateFromQuaternion(Orientation) * Matrix.CreateTranslation(Position);

            // Set the scale
            World.M13 = Scale.X;
            World.M24 = Scale.Y;

            // Set the image, alpha and color mod
            World.M12 = pMods.X;
            World.M23 = pMods.Y;
            World.M34 = pMods.Z;

            Instancer.instanceTransformMatrices[this] = World;
        }


        public void TranslateAA(Vector3 distance)
        {
            Position += Vector3.Transform(distance, Matrix.Identity);
        }

        protected bool Moved(Vector3 distance)
        {
            return distance != Vector3.Zero;
        }
    }
}
