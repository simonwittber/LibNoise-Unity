using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LibNoise.Operator
{
	/// <summary>
	/// Provides a noise module that maps the output value from a source module onto an
	/// exponential curve. [OPERATOR]
	/// </summary>
	public class Exponent : ModuleBase
	{
		#region Fields

		private float m_exponent = 1.0f;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of Exponent.
		/// </summary>
		public Exponent()
			: base(1)
		{
		}

		/// <summary>
		/// Initializes a new instance of Exponent.
		/// </summary>
		/// <param name="exponent">The exponent to use.</param>
		/// <param name="input">The input module.</param>
		public Exponent(float exponent, ModuleBase input)
			: base(1)
		{
			this.m_modules[0] = input;
			this.Value = exponent;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the exponent.
		/// </summary>
		public float Value
		{
			get { return this.m_exponent; }
			set { this.m_exponent = value; }
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
			System.Diagnostics.Debug.Assert(this.m_modules[0] != null);
			float v = this.m_modules[0].GetValue(x, y, z);
			return (Mathf.Pow(Mathf.Abs((v + 1.0f) / 2.0f), this.m_exponent) * 2.0f - 1.0f);
		}

		#endregion
	}
}