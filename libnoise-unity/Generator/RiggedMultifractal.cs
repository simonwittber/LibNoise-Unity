using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LibNoise.Generator
{
	/// <summary>
	/// Provides a noise module that outputs 3-dimensional ridged-multifractal noise. [GENERATOR]
	/// </summary>
	public class RiggedMultifractal : ModuleBase
	{
		#region Fields

		private float m_frequency = 1.0f;
		private float m_lacunarity = 2.0f;
		private QualityMode m_quality = QualityMode.Medium;
		private int m_octaveCount = 6;
		private int m_seed = 0;
		private float[] m_weights = new float[Utils.OctavesMaximum];

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of RiggedMultifractal.
		/// </summary>
		public RiggedMultifractal()
			: base(0)
		{
			this.UpdateWeights();
		}

		/// <summary>
		/// Initializes a new instance of RiggedMultifractal.
		/// </summary>
		/// <param name="frequency">The frequency of the first octave.</param>
		/// <param name="lacunarity">The lacunarity of the ridged-multifractal noise.</param>
		/// <param name="octaves">The number of octaves of the ridged-multifractal noise.</param>
		/// <param name="seed">The seed of the ridged-multifractal noise.</param>
		/// <param name="quality">The quality of the ridged-multifractal noise.</param>
		public RiggedMultifractal(float frequency, float lacunarity, int octaves, int seed, QualityMode quality)
			: base(0)
		{
			this.Frequency = frequency;
			this.Lacunarity = lacunarity;
			this.OctaveCount = octaves;
			this.Seed = seed;
			this.Quality = quality;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the frequency of the first octave.
		/// </summary>
		public float Frequency
		{
			get { return this.m_frequency; }
			set { this.m_frequency = value; }
		}

		/// <summary>
		/// Gets or sets the lacunarity of the ridged-multifractal noise.
		/// </summary>
		public float Lacunarity
		{
			get { return this.m_lacunarity; }
			set
			{
				this.m_lacunarity = value;
				this.UpdateWeights();
			}
		}

		/// <summary>
		/// Gets or sets the quality of the ridged-multifractal noise.
		/// </summary>
		public QualityMode Quality
		{
			get { return this.m_quality; }
			set { this.m_quality = value; }
		}

		/// <summary>
		/// Gets or sets the number of octaves of the ridged-multifractal noise.
		/// </summary>
		public int OctaveCount
		{
			get { return this.m_octaveCount; }
			set { this.m_octaveCount = (int)Mathf.Clamp(value, 1, Utils.OctavesMaximum); }
		}

		/// <summary>
		/// Gets or sets the seed of the ridged-multifractal noise.
		/// </summary>
		public int Seed
		{
			get { return this.m_seed; }
			set { this.m_seed = value; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Updates the weights of the ridged-multifractal noise.
		/// </summary>
		private void UpdateWeights()
		{
			float f = 1.0f;
			for (int i = 0; i < Utils.OctavesMaximum; i++)
			{
				this.m_weights[i] = Mathf.Pow(f, -1.0f);
				f *= this.m_lacunarity;
			}
		}

		#endregion

		#region ModuleBase Members

		/// <summary>
		/// Returns the output value for the given input coordinates.
		/// </summary>
		/// <param name="x">The input coordinate on the x-axis.</param>
		/// <param name="y">The input coordinate on the y-axis.</param>
		/// <param name="z">The input coordinate on the z-axis.</param>
		/// <returns>The resulting output value.</returns>
		public override float GetValue(float x, float y, float z)
		{
			x *= this.m_frequency;
			y *= this.m_frequency;
			z *= this.m_frequency;
			float signal = 0.0f;
			float value = 0.0f;
			float weight = 1.0f;
			float offset = 1.0f;
			float gain = 2.0f;
			for (int i = 0; i < this.m_octaveCount; i++)
			{
				float nx = Utils.MakeInt32Range(x);
				float ny = Utils.MakeInt32Range(y);
				float nz = Utils.MakeInt32Range(z);
				long seed = (this.m_seed + i) & 0x7fffffff;
				signal = Utils.GradientCoherentNoise3D(nx, ny, nz, seed, this.m_quality);
				signal = Mathf.Abs(signal);
				signal = offset - signal;
				signal *= signal;
				signal *= weight;
				weight = signal * gain;
				if (weight > 1.0f) { weight = 1.0f; }
				if (weight < 0.0f) { weight = 0.0f; }
				value += (signal * this.m_weights[i]);
				x *= this.m_lacunarity;
				y *= this.m_lacunarity;
				z *= this.m_lacunarity;
			}
			return (value * 1.25f) - 1.0f;
		}

		#endregion
	}
}