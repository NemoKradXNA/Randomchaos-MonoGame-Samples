using Microsoft.Xna.Framework;

namespace RandomchaosMGBase.Noise
{
    /// <summary>
    /// Abstract class for generating noise.
    /// </summary>
	public class NoiseBase : INoise
    {

        /// <summary>
        /// The frequency of the fractal.
        /// </summary>
        public float Frequency { get; set; }

        /// <summary>
        /// The amplitude of the fractal.
        /// </summary>
        public float Amplitude { get; set; }

        /// <summary>
        /// The offset applied to each dimension.
        /// </summary>
        public Vector3 Offset { get; set; }

        /// <summary>
        /// Create a noise object.
        /// </summary>
		public NoiseBase() { }

        /// <summary>
        /// Sample the noise in 1 dimension.
        /// </summary>
		public virtual float Sample1D(float x) { return 0; }

        /// <summary>
        /// Sample the noise in 2 dimensions.
        /// </summary>
		public virtual float Sample2D(float x, float y) { return 0; }

        /// <summary>
        /// Sample the noise in 3 dimensions.
        /// </summary>
		public virtual float Sample3D(float x, float y, float z) { return 0; }

        /// <summary>
        /// Update the seed.
        /// </summary>
        public virtual void UpdateSeed(int seed) { }

    }
}
