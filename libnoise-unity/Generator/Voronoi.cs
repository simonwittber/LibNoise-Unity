using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LibNoise.Generator
{
	/// <summary>
	/// Provides a noise module that outputs Voronoi cells. [GENERATOR]
	/// </summary>
	public class Voronoi : ModuleBase
	{
		#region Fields

		private float m_displacement = 1.0f;
		private float m_frequency = 1.0f;
		private int m_seed = 0;
		private bool m_distance = false;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of Voronoi.
		/// </summary>
		public Voronoi()
			: base(0)
		{
		}

		/// <summary>
		/// Initializes a new instance of Voronoi.
		/// </summary>
		/// <param name="frequency">The frequency of the first octave.</param>
		/// <param name="displacement">The displacement of the ridged-multifractal noise.</param>
		/// <param name="persistence">The persistence of the ridged-multifractal noise.</param>
		/// <param name="seed">The seed of the ridged-multifractal noise.</param>
		/// <param name="distance">Indicates whether the distance from the nearest seed point is applied to the output value.</param>
		public Voronoi(float frequency, float displacement, int seed, bool distance)
			: base(0)
		{
			this.Frequency = frequency;
			this.Displacement = displacement;
			this.Seed = seed;
			this.UseDistance = distance;
			this.Seed = seed;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the displacement value of the Voronoi cells.
		/// </summary>
		public float Displacement
		{
			get { return this.m_displacement; }
			set { this.m_displacement = value; }
		}

		/// <summary>
		/// Gets or sets the frequency of the seed points.
		/// </summary>
		public float Frequency
		{
			get { return this.m_frequency; }
			set { this.m_frequency = value; }
		}

		/// <summary>
		/// Gets or sets the seed value used by the Voronoi cells.
		/// </summary>
		public int Seed
		{
			get { return this.m_seed; }
			set { this.m_seed = value; }
		}

		/// <summary>
		/// Gets or sets a value whether the distance from the nearest seed point is applied to the output value.
		/// </summary>
		public bool UseDistance
		{
			get { return this.m_distance; }
			set { this.m_distance = value; }
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
			int xi = (x > 0.0f ? (int)x : (int)x - 1);
			int iy = (y > 0.0f ? (int)y : (int)y - 1);
			int iz = (z > 0.0f ? (int)z : (int)z - 1);
			float md = 2147483647.0f;
			float xc = 0;
			float yc = 0;
			float zc = 0;
			for (int zcu = iz - 2; zcu <= iz + 2; zcu++)
			{
				for (int ycu = iy - 2; ycu <= iy + 2; ycu++)
				{
					for (int xcu = xi - 2; xcu <= xi + 2; xcu++)
					{
						float xp = xcu + Utils.ValueNoise3D(xcu, ycu, zcu, this.m_seed);
						float yp = ycu + Utils.ValueNoise3D(xcu, ycu, zcu, this.m_seed + 1);
						float zp = zcu + Utils.ValueNoise3D(xcu, ycu, zcu, this.m_seed + 2);
						float xd = xp - x;
						float yd = yp - y;
						float zd = zp - z;
						float d = xd * xd + yd * yd + zd * zd;
						if (d < md)
						{
							md = d;
							xc = xp;
							yc = yp;
							zc = zp;
						}
					}
				}
			}
			float v;
			if (this.m_distance)
			{
				float xd = xc - x;
				float yd = yc - y;
				float zd = zc - z;
				v = (Mathf.Sqrt(xd * xd + yd * yd + zd * zd)) * Utils.Sqrt3 - 1.0f;
			}
			else
			{
				v = 0.0f;
			}
			return v + (this.m_displacement * (float)Utils.ValueNoise3D((int)(Mathf.Floor(xc)), (int)(Mathf.Floor(yc)),
			  (int)(Mathf.Floor(zc)), 0));
		}

		#endregion
	}
}