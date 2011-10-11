using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LibNoise.Generator
{
	/// <summary>
	/// Provides a noise module that outputs concentric spheres. [GENERATOR]
	/// </summary>
	public class Spheres : ModuleBase
	{
		#region Fields

		private float m_frequency = 1.0f;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of Spheres.
		/// </summary>
		public Spheres()
			: base(0)
		{
		}

		/// <summary>
		/// Initializes a new instance of Spheres.
		/// </summary>
		/// <param name="frequency">The frequency of the concentric spheres.</param>
		public Spheres(float frequency)
			: base(0)
		{
			this.Frequency = frequency;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the frequency of the concentric spheres.
		/// </summary>
		public float Frequency
		{
			get { return this.m_frequency; }
			set { this.m_frequency = value; }
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
			float dfc = Mathf.Sqrt(x * x + y * y + z * z);
			float dfss = dfc - Mathf.Floor(dfc);
			float dfls = 1.0f - dfss;
			float nd = Mathf.Min(dfss, dfls);
			return 1.0f - (nd * 4.0f);
		}

		#endregion
	}
}