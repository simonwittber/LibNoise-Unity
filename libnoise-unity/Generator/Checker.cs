using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace LibNoise.Generator
{
	/// <summary>
	/// Provides a noise module that outputs a checkerboard pattern. [GENERATOR]
	/// </summary>
	public class Checker : ModuleBase
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of Checker.
		/// </summary>
		public Checker()
			: base(0)
		{
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
			int ix = (int)(Mathf.Floor(Utils.MakeInt32Range(x)));
			int iy = (int)(Mathf.Floor(Utils.MakeInt32Range(y)));
			int iz = (int)(Mathf.Floor(Utils.MakeInt32Range(z)));
			return (ix & 1 ^ iy & 1 ^ iz & 1) != 0 ? -1.0f : 1.0f;
		}

		#endregion
	}
}