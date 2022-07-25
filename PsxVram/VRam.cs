using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PSX
{
	public class VRam
	{
		public List<Vector2> GenericCluts
		{
			get
			{
				return this._genericCluts;
			}
		}

		public List<Vector2> MapCluts
		{
			get
			{
				return this._mapCluts;
			}
		}

		public Texture2D RamTexture { get; private set; }

		public Texture2D ClutTexture { get; private set; }

		public Texture2D CollisionTexture { get; private set; }

		public void LoadContent(ContentManager content)
		{
			this.CollisionTexture = content.Load<Texture2D>("collision");
		}

		public Rectangle[] GetDrawingRectsFor(int x, int y, int width, int height)
		{
			List<Rectangle> list = new List<Rectangle>();
			if (x < this.RamTexture.Width)
			{
				if (x + width < this.RamTexture.Width)
				{
					list.Add(new Rectangle(x, y, width, height));
				}
				else
				{
					list.Add(new Rectangle(x, y, this.RamTexture.Width - x, height));
					list.Add(new Rectangle(x - this.RamTexture.Width, y + 512, width - this.RamTexture.Width - x, height));
				}
			}
			else
			{
				list.Add(new Rectangle(x - this.RamTexture.Width, y + 512, width, height));
			}
			return list.ToArray();
		}

		public void ImportImageData(VRamRect toRect, byte[] data)
		{
			int num = data.Length * 2;
			byte[] array = new byte[num];
			for (int i = 0; i < data.Length; i++)
			{
				array[i * 2] = (byte)(data[i] & 15);
				array[i * 2 + 1] = (byte)(data[i] >> 4);
			}
			array = array.SelectMany((byte b) => new byte[]
			{
				b,
				b,
				b,
				b
			}).ToArray<byte>();
			if (toRect.TopLeft.X >= this.RamTexture.Width)
			{
				this.RamTexture.SetData<byte>(0, new Rectangle?(new Rectangle(toRect.TopLeft.X - this.RamTexture.Width, toRect.TopLeft.Y + 512, toRect.Dimensions.X, toRect.Dimensions.Y)), array, 0, array.Length);
				return;
			}
			if (toRect.TopLeft.X + toRect.Dimensions.X <= this.RamTexture.Width)
			{
				this.RamTexture.SetData<byte>(0, new Rectangle?(new Rectangle(toRect.TopLeft.X, toRect.TopLeft.Y, toRect.Dimensions.X, toRect.Dimensions.Y)), array, 0, array.Length);
				return;
			}
			Rectangle value = new Rectangle(toRect.TopLeft.X, toRect.TopLeft.Y, this.RamTexture.Width - toRect.TopLeft.X, toRect.Dimensions.Y);
			byte[] array2 = new byte[value.Width * value.Height * 4];
			for (int j = 0; j < value.Height; j++)
			{
				Array.Copy(array, j * value.Width, array2, j * value.Width, value.Width);
			}
			this.RamTexture.SetData<byte>(0, new Rectangle?(value), array2, 0, array2.Length);
			Rectangle value2 = new Rectangle(toRect.TopLeft.X - this.RamTexture.Width, toRect.TopLeft.Y + 512, toRect.Dimensions.X - this.RamTexture.Width - toRect.TopLeft.X, toRect.Dimensions.Y);
			byte[] array3 = new byte[value2.Width * value2.Height * 4];
			for (int k = 0; k < value2.Height; k++)
			{
				Array.Copy(array, value.Width + k * value2.Width, array3, k * value2.Width, value2.Width);
			}
			this.RamTexture.SetData<byte>(0, new Rectangle?(value2), array3, 0, array3.Length);
		}

		public void Clear()
		{
			this.CreateTextures();
			this._mapCluts.Clear();
		}

		public void GenerateGenericCluts()
		{
			this._genericCluts.Clear();
			for (int i = 240; i < 256; i++)
			{
				for (int j = 0; j < 1024; j += 64)
				{
					this.ExportClutFromRamToClutTexture(j, i);
					this._genericCluts.Add(new Vector2((float)j / 4f, (float)(i - 240)));
				}
			}
		}

		public void GenerateMapCluts()
		{
			this._mapCluts.Clear();
			for (int i = 240; i < 256; i++)
			{
				for (int j = 2048; j < 3072; j += 64)
				{
					this.ExportClutFromRamToClutTexture(j, i);
					this._mapCluts.Add(new Vector2((float)((j - 2048) / 4), (float)(i - 240 + 16)));
				}
			}
		}

		private void ExportClutFromRamToClutTexture(int x, int y)
		{
			int x2 = (x < this.RamTexture.Width) ? x : (x - this.RamTexture.Width);
			int y2 = (x < this.RamTexture.Width) ? y : (y + 512);
			byte[] data = new byte[256];
			this.RamTexture.GetData<byte>(0, new Rectangle?(new Rectangle(x2, y2, 64, 1)), data, 0, 256);
			byte[] array = this.CreateColorsFromBytes(data, 16);
			int num = (x < 2048) ? x : (x - 2048);
			int num2 = (x < 2048) ? y : (y + 16);
			this.ClutTexture.SetData<byte>(0, new Rectangle?(new Rectangle(num / 4, num2 - 240, 16, 1)), array, 0, array.Length);
		}

		private byte[] CreateColorsFromBytes(byte[] data, int colorCount)
		{
			byte[] array = new byte[64];
			for (int i = 0; i < colorCount; i++)
			{
				ushort num = (ushort)((int)data[i * 16] | (int)data[i * 16 + 4] << 4 | (int)data[i * 16 + 8] << 8 | (int)data[i * 16 + 12] << 12);
				array[i * 4] = (byte)((num & 31) * 8);
				array[i * 4 + 1] = (byte)((num >> 5 & 31) * 8);
				array[i * 4 + 2] = (byte)((num >> 10 & 31) * 8);
				array[i * 4 + 3] = (byte)(((byte)(num >> 15) == 0) ? 0 : byte.MaxValue);
			}
			return array;
		}

		public static void InitInstance(GraphicsDevice device)
		{
			VRam.Instance = new VRam(device);
		}

		public static VRam Instance { get; private set; }

		private VRam(GraphicsDevice device)
		{
			this._graphicsDevice = device;
			this.CreateTextures();
		}

		private void CreateTextures()
		{
			this.RamTexture = new Texture2D(this._graphicsDevice, 2048, 1024);
			this.ClutTexture = new Texture2D(this._graphicsDevice, 256, 32, false, SurfaceFormat.Color);
		}

		public const int RamTextureHeightBeforeLoop = 512;

		private const int TileWidthPixels = 16;

		private const int TileHeightPixels = 16;

		private const double TilePsxBytesPerPixel = 0.5;

		private const double TilePsxSizeBytes = 128.0;

		private const int ClutWidthPixels = 16;

		private const int ClutHeightPixels = 1;

		private const int ClutPsxBytesPerPixel = 2;

		private const int ClutPsxSizeBytes = 32;

		private const int ClutPcBytesPerPixel = 4;

		private const int ClutPcSizeBytes = 64;

		private readonly List<Vector2> _genericCluts = new List<Vector2>();

		private readonly List<Vector2> _mapCluts = new List<Vector2>();

		private readonly GraphicsDevice _graphicsDevice;
	}
}
