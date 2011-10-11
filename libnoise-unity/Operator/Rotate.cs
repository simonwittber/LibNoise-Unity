using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

	
namespace LibNoise.Operator
{
	/// <summary>
	/// Provides a noise module that rotates the input value around the origin before
	/// returning the output value from a source module. [OPERATOR]
	/// </summary>
	public class Rotate : ModuleBase
	{
		#region Fields

		private float m_x = 0.0f;
		private float m_x1Matrix = 0.0f;
		private float m_x2Matrix = 0.0f;
		private float m_x3Matrix = 0.0f;
		private float m_y = 0.0f;
		private float m_y1Matrix = 0.0f;
		private float m_y2Matrix = 0.0f;
		private float m_y3Matrix = 0.0f;
		private float m_z = 0.0f;
		private float m_z1Matrix = 0.0f;
		private float m_z2Matrix = 0.0f;
		private float m_z3Matrix = 0.0f;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of Rotate.
		/// </summary>
		public Rotate()
			: base(1)
		{
			this.SetAngles(0.0f, 0.0f, 0.0f);
		}

		/// <summary>
		/// Initializes a new instance of Rotate.
		/// </summary>
		/// <param name="x">The rotation around the x-axis.</param>
		/// <param name="y">The rotation around the y-axis.</param>
		/// <param name="z">The rotation around the z-axis.</param>
		/// <param name="input">The input module.</param>
		public Rotate(float x, float y, float z, ModuleBase input)
			: base(1)
		{
			this.m_modules[0] = input;
			this.SetAngles(x, y, z);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the rotation around the x-axis in degree.
		/// </summary>
		public float X
		{
			get { return this.m_x; }
			set { this.SetAngles(value, this.m_y, this.m_z); }
		}

		/// <summary>
		/// Gets or sets the rotation around the y-axis in degree.
		/// </summary>
		public float Y
		{
			get { return this.m_y; }
			set { this.SetAngles(this.m_x, value, this.m_z); }
		}

		/// <summary>
		/// Gets or sets the rotation around the z-axis in degree.
		/// </summary>
		public float Z
		{
			get { return this.m_x; }
			set { this.SetAngles(this.m_x, this.m_y, value); }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Sets the rotation angles.
		/// </summary>
		/// <param name="x">The rotation around the x-axis.</param>
		/// <param name="y">The rotation around the y-axis.</param>
		/// <param name="z">The rotation around the z-axis.</param>
		private void SetAngles(float x, float y, float z)
		{
			float xc = Mathf.Cos(x * Utils.DegToRad);
			float yc = Mathf.Cos(y * Utils.DegToRad);
			float zc = Mathf.Cos(z * Utils.DegToRad);
			float xs = Mathf.Sin(x * Utils.DegToRad);
			float ys = Mathf.Sin(y * Utils.DegToRad);
			float zs = Mathf.Sin(z * Utils.DegToRad);
			this.m_x1Matrix = ys * xs * zs + yc * zc;
			this.m_y1Matrix = xc * zs;
			this.m_z1Matrix = ys * zc - yc * xs * zs;
			this.m_x2Matrix = ys * xs * zc - yc * zs;
			this.m_y2Matrix = xc * zc;
			this.m_z2Matrix = -yc * xs * zc - ys * zs;
			this.m_x3Matrix = -ys * xc;
			this.m_y3Matrix = xs;
			this.m_z3Matrix = yc * xc;
			this.m_x = x;
			this.m_y = y;
			this.m_z = z;
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
			float nx = (this.m_x1Matrix * x) + (this.m_y1Matrix * y) + (this.m_z1Matrix * z);
			float ny = (this.m_x2Matrix * x) + (this.m_y2Matrix * y) + (this.m_z2Matrix * z);
			float nz = (this.m_x3Matrix * x) + (this.m_y3Matrix * y) + (this.m_z3Matrix * z);
			return this.m_modules[0].GetValue(nx, ny, nz);
		}

		#endregion
	}
}