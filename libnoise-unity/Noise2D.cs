using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LibNoise
{
	/// <summary>
	/// Provides a two-dimensional noise map.
	/// </summary>
	public class Noise2D : IDisposable
	{
		#region Constants

		public const float South = -90.0f;
		public const float North = 90.0f;
		public const float West = -180.0f;
		public const float East = 180.0f;
		public const float AngleMin = -180.0f;
		public const float AngleMax = 180.0f;
		public const float Left = -1.0f;
		public const float Right = 1.0f;
		public const float Top = -1.0f;
		public const float Bottom = 1.0f;

		#endregion

		#region Fields

		private int m_width = 0;
		private int m_height = 0;
		private float m_borderValue = float.NaN;
		private float[,] m_data = null;
		private ModuleBase m_generator = null;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of Noise2D.
		/// </summary>
		protected Noise2D()
		{
		}

		/// <summary>
		/// Initializes a new instance of Noise2D.
		/// </summary>
		/// <param name="size">The width and height of the noise map.</param>
		public Noise2D(int size)
			: this(size, size, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of Noise2D.
		/// </summary>
		/// <param name="size">The width and height of the noise map.</param>
		/// <param name="generator">The generator module.</param>
		public Noise2D(int size, ModuleBase generator)
			: this(size, size, generator)
		{
		}

		/// <summary>
		/// Initializes a new instance of Noise2D.
		/// </summary>
		/// <param name="width">The width of the noise map.</param>
		/// <param name="height">The height of the noise map.</param>
		public Noise2D(int width, int height)
			: this(width, height, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of Noise2D.
		/// </summary>
		/// <param name="width">The width of the noise map.</param>
		/// <param name="height">The height of the noise map.</param>
		/// <param name="generator">The generator module.</param>
		public Noise2D(int width, int height, ModuleBase generator)
		{
			this.m_generator = generator;
			this.m_width = width;
			this.m_height = height;
			this.m_data = new float[width, height];
		}

		#endregion

		#region Indexers

		/// <summary>
		/// Gets or sets a value in the noise map by its position.
		/// </summary>
		/// <param name="x">The position on the x-axis.</param>
		/// <param name="y">The position on the y-axis.</param>
		/// <returns>The corresponding value.</returns>
		public float this[int x, int y]
		{
			get
			{
				if (x < 0 && x >= this.m_width)
				{
					throw new ArgumentOutOfRangeException();
				}
				if (y < 0 && y >= this.m_height)
				{
					throw new ArgumentOutOfRangeException();
				}
				return this.m_data[x, y];
			}
			set
			{
				if (x < 0 && x >= this.m_width)
				{
					throw new ArgumentOutOfRangeException();
				}
				if (y < 0 && y >= this.m_height)
				{
					throw new ArgumentOutOfRangeException();
				}
				this.m_data[x, y] = value;
			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the constant value at the noise maps borders.
		/// </summary>
		public float Border
		{
			get { return this.m_borderValue; }
			set { this.m_borderValue = value; }
		}

		/// <summary>
		/// Gets or sets the generator module.
		/// </summary>
		public ModuleBase Generator
		{
			get { return this.m_generator; }
			set { this.m_generator = value; }
		}

		/// <summary>
		/// Gets the height of the noise map.
		/// </summary>
		public int Height
		{
			get { return this.m_height; }
		}

		/// <summary>
		/// Gets the width of the noise map.
		/// </summary>
		public int Width
		{
			get { return this.m_width; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Clears the noise map.
		/// </summary>
		public void Clear()
		{
			this.Clear(0.0f);
		}

		/// <summary>
		/// Clears the noise map.
		/// </summary>
		/// <param name="value">The constant value to clear the noise map with.</param>
		public void Clear(float value)
		{
			for (int x = 0; x < this.m_width; x++)
			{
				for (int y = 0; y < this.m_height; y++)
				{
					this.m_data[x, y] = value;
				}
			}
		}

		/// <summary>
		/// Generates a cylindrical projection of a point in the noise map.
		/// </summary>
		/// <param name="angle">The angle of the point.</param>
		/// <param name="height">The height of the point.</param>
		/// <returns>The corresponding noise map value.</returns>
		private float GenerateCylindrical(float angle, float height)
		{
			float x = Mathf.Cos(angle * Utils.DegToRad);
			float y = height;
			float z = Mathf.Sin(angle * Utils.DegToRad);
			return this.m_generator.GetValue(x, y, z);
		}

		/// <summary>
		/// Generates a cylindrical projection of the noise map.
		/// </summary>
		/// <param name="angleMin">The maximum angle of the clip region.</param>
		/// <param name="angleMax">The minimum angle of the clip region.</param>
		/// <param name="heightMin">The minimum height of the clip region.</param>
		/// <param name="heightMax">The maximum height of the clip region.</param>
		public void GenerateCylindrical(float angleMin, float angleMax, float heightMin, float heightMax)
		{
			if (angleMax <= angleMin || heightMax <= heightMin || this.m_generator == null)
			{
				throw new ArgumentException();
			}
			float ae = angleMax - angleMin;
			float he = heightMax - heightMin;
			float xd = ae / (float)this.m_width;
			float yd = he / (float)this.m_height;
			float ca = angleMin;
			float ch = heightMin;
			for (int x = 0; x < this.m_width; x++)
			{
				ch = heightMin;
				for (int y = 0; y < this.m_height; y++)
				{
					this.m_data[x, y] = (float)this.GenerateCylindrical(ca, ch);
					ch += yd;
				}
				ca += xd;
			}
		}

		/// <summary>
		/// Generates a planar projection of a point in the noise map.
		/// </summary>
		/// <param name="x">The position on the x-axis.</param>
		/// <param name="y">The position on the y-axis.</param>
		/// <returns>The corresponding noise map value.</returns>
		private float GeneratePlanar(float x, float y)
		{
			return this.m_generator.GetValue(x, 0.0f, y);
		}

		/// <summary>
		/// Generates a planar projection of the noise map.
		/// </summary>
		/// <param name="left">The clip region to the left.</param>
		/// <param name="right">The clip region to the right.</param>
		/// <param name="top">The clip region to the top.</param>
		/// <param name="bottom">The clip region to the bottom.</param>
		public void GeneratePlanar(float left, float right, float top, float bottom)
		{
			this.GeneratePlanar(left, right, top, bottom, false);
		}

		/// <summary>
		/// Generates a non-seamless planar projection of the noise map.
		/// </summary>
		/// <param name="left">The clip region to the left.</param>
		/// <param name="right">The clip region to the right.</param>
		/// <param name="top">The clip region to the top.</param>
		/// <param name="bottom">The clip region to the bottom.</param>
		/// <param name="seamless">Indicates whether the resulting noise map should be seamless.</param>
		public void GeneratePlanar(float left, float right, float top, float bottom, bool seamless)
		{
			if (right <= left || bottom <= top || this.m_generator == null)
			{
				throw new ArgumentException();
			}
			float xe = right - left;
			float ze = bottom - top;
			float xd = xe / (float)this.m_width;
			float zd = ze / (float)this.m_height;
			float xc = left;
			float zc = top;
			float fv = 0.0f;
			for (int x = 0; x < this.m_width; x++)
			{
				zc = top;
				for (int z = 0; z < this.m_height; z++)
				{
					if (!seamless) { fv = (float)this.GeneratePlanar(xc, zc); }
					else
					{
						float swv = this.GeneratePlanar(xc, zc);
						float sev = this.GeneratePlanar(xc + xe, zc);
						float nwv = this.GeneratePlanar(xc, zc + ze);
						float nev = this.GeneratePlanar(xc + xe, zc + ze);
						float xb = 1.0f - ((xc - left) / xe);
						float zb = 1.0f - ((zc - top) / ze);
						float z0 = Utils.InterpolateLinear(swv, sev, xb);
						float z1 = Utils.InterpolateLinear(nwv, nev, xb);
						fv = (float)Utils.InterpolateLinear(z0, z1, zb);
					}
					this.m_data[x, z] = fv;
					zc += zd;
				}
				xc += xd;
			}
		}

		/// <summary>
		/// Generates a spherical projection of a point in the noise map.
		/// </summary>
		/// <param name="lat">The latitude of the point.</param>
		/// <param name="lon">The longitude of the point.</param>
		/// <returns>The corresponding noise map value.</returns>
		private float GenerateSpherical(float lat, float lon)
		{
			float r = Mathf.Cos(Utils.DegToRad * lat);
			return this.m_generator.GetValue(r * Mathf.Cos(Utils.DegToRad * lon), Mathf.Sin(Utils.DegToRad * lat),
				r * Mathf.Sin(Utils.DegToRad * lon));
		}

		/// <summary>
		/// Generates a spherical projection of the noise map.
		/// </summary>
		/// <param name="south">The clip region to the south.</param>
		/// <param name="north">The clip region to the north.</param>
		/// <param name="west">The clip region to the west.</param>
		/// <param name="east">The clip region to the east.</param>
		public void GenerateSpherical(float south, float north, float west, float east)
		{
			if (east <= west || north <= south || this.m_generator == null)
			{
				throw new ArgumentException();
			}
			float loe = east - west;
			float lae = north - south;
			float xd = loe / (float)this.m_width;
			float yd = lae / (float)this.m_height;
			float clo = west;
			float cla = south;
			for (int x = 0; x < this.m_width; x++)
			{
				cla = south;
				for (int y = 0; y < this.m_height; y++)
				{
					this.m_data[x, y] = (float)this.GenerateSpherical(cla, clo);
					cla += yd;
				}
				clo += xd;
			}
		}

		/// <summary>
		/// Creates a normal map for the current content of the noise map.
		/// </summary>
		/// <param name="device">The graphics device to use.</param>
		/// <param name="scale">The scaling of the normal map values.</param>
		/// <returns>The created normal map.</returns>
		public Texture2D GetNormalMap(float scale)
		{
			Texture2D result = new Texture2D(this.m_width, this.m_height);
			Color[] data = new Color[this.m_width * this.m_height];
			for (int y = 0; y < this.m_height; y++)
			{
				for (int x = 0; x < this.m_width; x++)
				{
					Vector3 normX = Vector3.zero;
					Vector3 normY = Vector3.zero;
					Vector3 normalVector = new Vector3();
					if (x > 0 && y > 0 && x < this.m_width - 1 && y < this.m_height - 1)
					{
						normX = new Vector3((this.m_data[x - 1, y] - this.m_data[x + 1, y]) / 2 * scale, 0, 1);
						normY = new Vector3(0, (this.m_data[x, y - 1] - this.m_data[x, y + 1]) / 2 * scale, 1);
						normalVector = normX + normY;
						normalVector.Normalize();
						Vector3 texVector = Vector3.zero;
						texVector.x = (normalVector.x + 1) / 2f;
						texVector.y = (normalVector.y + 1) / 2f;
						texVector.z = (normalVector.z + 1) / 2f;
						data[x + y * this.m_height] = new Color(texVector.x, texVector.y, texVector.z);
					}
					else
					{
						normX = new Vector3(0, 0, 1);
						normY = new Vector3(0, 0, 1);
						normalVector = normX + normY;
						normalVector.Normalize();
						Vector3 texVector = Vector3.zero;
						texVector.x = (normalVector.x + 1) / 2f;
						texVector.y = (normalVector.y + 1) / 2f;
						texVector.z = (normalVector.x + 1) / 2f;
						data[x + y * this.m_height] = new Color(texVector.x, texVector.y, texVector.z);
					}
				}
			}
			result.SetPixels(data);
			result.Apply();
			return result;
		}

		/// <summary>
		/// Creates a grayscale texture map for the current content of the noise map.
		/// </summary>
		/// <param name="device">The graphics device to use.</param>
		/// <returns>The created texture map.</returns>
		public Texture2D GetTexture()
		{
			return this.GetTexture(Gradient.Grayscale);
		}

		/// <summary>
		/// Creates a texture map for the current content of the noise map.
		/// </summary>
		/// <param name="device">The graphics device to use.</param>
		/// <param name="gradient">The gradient to color the texture map with.</param>
		/// <returns>The created texture map.</returns>
		public Texture2D GetTexture(Gradient gradient)
		{
			return this.GetTexture(ref gradient);
		}

		/// <summary>
		/// Creates a texture map for the current content of the noise map.
		/// </summary>
		/// <param name="device">The graphics device to use.</param>
		/// <param name="gradient">The gradient to color the texture map with.</param>
		/// <returns>The created texture map.</returns>
		public Texture2D GetTexture(ref Gradient gradient)
		{
			Texture2D result = new Texture2D(this.m_width, this.m_height);
			Color[] data = new Color[this.m_width * this.m_height];
			int id = 0;
			for (int y = 0; y < this.m_height; y++)
			{
				for (int x = 0; x < this.m_width; x++, id++)
				{
					float d = 0.0f;
					if (!float.IsNaN(this.m_borderValue) && (x == 0 || x == this.m_width - 1 || y == 0 || y == this.m_height - 1))
					{
						d = this.m_borderValue;
					}
					else
					{
						d = this.m_data[x, y];
					}
					data[id] = gradient[d];
				}
			}
			result.SetPixels(data);
			result.Apply();
			return result;
		}

		#endregion

		#region IDisposable Members


		[NonSerialized]
		private bool m_disposed = false;

		/// <summary>
		/// Gets a value whether the object is disposed.
		/// </summary>
		public bool IsDisposed
		{
			get { return this.m_disposed; }
		}

		/// <summary>
		/// Immediately releases the unmanaged resources used by this object.
		/// </summary>
		public void Dispose()
		{
			if (!this.m_disposed) { this.m_disposed = this.Disposing(); }
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Immediately releases the unmanaged resources used by this object.
		/// </summary>
		/// <returns>True if the object is completely disposed.</returns>
		protected virtual bool Disposing()
		{
			if (this.m_data != null) { this.m_data = null; }
			this.m_width = 0;
			this.m_height = 0;
			return true;
		}

		#endregion
	}
}