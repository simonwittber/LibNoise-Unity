using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibNoise.Operator
{
	/// <summary>
	/// Provides a noise module that moves the coordinates of the input value before
	/// returning the output value from a source module. [OPERATOR]
	/// </summary>
	public class Translate : ModuleBase
	{
		#region Fields

		private float m_x = 1.0f;
		private float m_y = 1.0f;
		private float m_z = 1.0f;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of Translate.
		/// </summary>
		public Translate()
			: base(1)
		{
		}

		/// <summary>
		/// Initializes a new instance of Translate.
		/// </summary>
		/// <param name="x">The translation on the x-axis.</param>
		/// <param name="y">The translation on the y-axis.</param>
		/// <param name="z">The translation on the z-axis.</param>
		/// <param name="input">The input module.</param>
		public Translate(float x, float y, float z, ModuleBase input)
			: base(1)
		{
			this.m_modules[0] = input;
			this.X = x;
			this.Y = y;
			this.Z = z;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the translation on the x-axis.
		/// </summary>
		public float X
		{
			get { return this.m_x; }
			set { this.m_x = value; }
		}

		/// <summary>
		/// Gets or sets the translation on the y-axis.
		/// </summary>
		public float Y
		{
			get { return this.m_y; }
			set { this.m_y = value; }
		}

		/// <summary>
		/// Gets or sets the translation on the z-axis.
		/// </summary>
		public float Z
		{
			get { return this.m_z; }
			set { this.m_z = value; }
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
			return this.m_modules[0].GetValue(x + this.m_x, y + this.m_y, z + this.m_z);
		}

		#endregion
	}
}