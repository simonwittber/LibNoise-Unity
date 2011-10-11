using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace LibNoise.Operator
{
	/// <summary>
	/// Provides a noise module that maps the output value from a source module onto an
	/// arbitrary function curve. [OPERATOR]
	/// </summary>
	public class Curve : ModuleBase
	{
		#region Fields

		private List<KeyValuePair<float, float>> m_data = new List<KeyValuePair<float,float>>();

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of Curve.
		/// </summary>
		public Curve()
			: base(1)
		{
		}

		/// <summary>
		/// Initializes a new instance of Curve.
		/// </summary>
		/// <param name="input">The input module.</param>
		public Curve(ModuleBase input)
			: base(1)
		{
			this.m_modules[0] = input;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the number of control points.
		/// </summary>
		public int ControlPointCount
		{
			get { return this.m_data.Count; }
		}

		/// <summary>
		/// Gets the list of control points.
		/// </summary>
		public List<KeyValuePair<float, float>> ControlPoints
		{
			get { return this.m_data; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Adds a control point to the curve.
		/// </summary>
		/// <param name="input">The curves input value.</param>
		/// <param name="output">The curves output value.</param>
		public void Add(float input, float output)
		{
			KeyValuePair<float, float> kvp = new KeyValuePair<float, float>(input, output);
			if (!this.m_data.Contains(kvp))
			{
				this.m_data.Add(kvp);
			}
			this.m_data.Sort(delegate(KeyValuePair<float, float> lhs, KeyValuePair<float, float> rhs)
				{ return lhs.Key.CompareTo(rhs.Key); });
		}

		/// <summary>
		/// Clears the control points.
		/// </summary>
		public void Clear()
		{
			this.m_data.Clear();
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
			System.Diagnostics.Debug.Assert(this.ControlPointCount >= 4);
			float smv = this.m_modules[0].GetValue(x, y, z);
			int ip;
			for (ip = 0; ip < this.m_data.Count; ip++)
			{
				if (smv < this.m_data[ip].Key)
				{
					break;
				}
			}
			int i0 = (int)Mathf.Clamp(ip - 2, 0, this.m_data.Count - 1);
			int i1 = (int)Mathf.Clamp(ip - 1, 0, this.m_data.Count - 1);
			int i2 = (int)Mathf.Clamp(ip, 0, this.m_data.Count - 1);
			int i3 = (int)Mathf.Clamp(ip + 1, 0, this.m_data.Count - 1);
			if (i1 == i2)
			{
				return this.m_data[i1].Value;
			}
			float ip0 = this.m_data[i1].Key;
			float ip1 = this.m_data[i2].Key;
			float a = (smv - ip0) / (ip1 - ip0);
			return Utils.InterpolateCubic(this.m_data[i0].Value, this.m_data[i1].Value, this.m_data[i2].Value, this.m_data[i3].Value, a);
		}

		#endregion
	}
}