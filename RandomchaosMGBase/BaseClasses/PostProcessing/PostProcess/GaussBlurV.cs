using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RandomchaosMGBase.BaseClasses.PostProcessing
{
    public class GaussBlurV : BasePostProcess
    {
        private Vector4[] sampleOffsetsVert;
        private float[] sampleWeightsVert;

        private const int Sample_Count = 11;

        protected float blurAmount;
        public float BlurAmount
        {
            get { return blurAmount; }
            set
            {
                blurAmount = value;
                if (sampleOffsetsVert != null)
                    SetBlurEffectParameters(0, 1.0f / (float)(this.Game.GraphicsDevice.Viewport.Height / 2f), ref sampleOffsetsVert, ref sampleWeightsVert);
            }
        }

        public GaussBlurV(Game game, float amount) : base(game)
        {
            blurAmount = amount;
        }

        public override void Draw(GameTime gameTime)
        {
            if (effect == null)
            {
                effect = Game.Content.Load<Effect>("Shaders/PostProcessing/GaussianBlur");

                sampleOffsetsVert = new Vector4[Sample_Count];

                sampleWeightsVert = new float[Sample_Count];

                SetBlurEffectParameters(0, 1.0f / (float)(this.Game.GraphicsDevice.Viewport.Height / 2f), ref sampleOffsetsVert, ref sampleWeightsVert);

            }

            effect.Parameters["SampleOffsets"].SetValue(sampleOffsetsVert);
            effect.Parameters["SampleWeights"].SetValue(sampleWeightsVert);

            // Set Params.
            base.Draw(gameTime);
        }
        /// <summary>
        /// Computes sample weightings and texture coordinate offsets
        /// for one pass of a separable gaussian blur filter.
        /// </summary>
        private void SetBlurEffectParameters(float dx, float dy, ref Vector4[] offsets, ref float[] weights)
        {
            // The first sample always has a zero offset.
            weights[0] = ComputeGaussian(0);
            offsets[0] = new Vector4();

            // Maintain a sum of all the weighting values.
            float totalWeights = weights[0];

            // Add pairs of additional sample taps, positioned
            // along a line in both directions from the center.
            for (int i = 0; i < Sample_Count / 2; i++)
            {
                // Store weights for the positive and negative taps.
                float weight = ComputeGaussian(i + 1);

                weights[i * 2 + 1] = weight;
                weights[i * 2 + 2] = weight;

                totalWeights += weight * 2;

                // To get the maximum amount of blurring from a limited number of
                // pixel shader samples, we take advantage of the bilinear filtering
                // hardware inside the texture fetch unit. If we position our texture
                // coordinates exactly halfway between two texels, the filtering unit
                // will average them for us, giving two samples for the price of one.
                // This allows us to step in units of two texels per sample, rather
                // than just one at a time. The 1.5 offset kicks things off by
                // positioning us nicely in between two texels.
                float sampleOffset = i * 2 + 1.5f;

                Vector4 delta = new Vector4(dx, dy, 1.0f, 1.0f) * sampleOffset;

                // Store texture coordinate offsets for the positive and negative taps.
                offsets[i * 2 + 1] = delta;
                offsets[i * 2 + 2] = -delta;
            }

            // Normalize the list of sample weightings, so they will always sum to one.
            for (int i = 0; i < Sample_Count; i++)
            {
                weights[i] /= totalWeights;
            }
        }

        /// <summary>
        /// Evaluates a single point on the gaussian falloff curve.
        /// Used for setting up the blur filter weightings.
        /// </summary>
        private float ComputeGaussian(float n)
        {
            float theta = blurAmount;

            return (float)((1.0 / Math.Sqrt(2 * Math.PI * theta)) *
                           Math.Exp(-(n * n) / (2 * theta * theta)));
        }
    }
}
