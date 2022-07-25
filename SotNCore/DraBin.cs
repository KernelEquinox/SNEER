using System;
using System.Collections.Generic;
using System.IO;
using PSX;

namespace SotN
{
	public class DraBin
	{
		public bool IsValid
		{
			get
			{
				return this._rawData != null;
			}
		}

		public void Init(string filename)
		{
			FileStream input;
			try
			{
				input = new FileStream(filename, FileMode.Open);
			}
			catch
			{
				return;
			}
			BinaryReader binaryReader = new BinaryReader(input);
			this._rawData = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
			int num = 0;
			for (;;)
			{
				binaryReader.BaseStream.Seek(DraBin.CompressedTilesOffset + (long)(num * 4), SeekOrigin.Begin);
				uint num2 = binaryReader.ReadUInt32();
				if (num2 == 0U)
				{
					break;
				}
				uint num3 = num2.FromPsxPtrToOffset(SotnFileType.DraBin);
				binaryReader.BaseStream.Seek((long)((ulong)num3), SeekOrigin.Begin);
				int num4 = binaryReader.ReadInt32();
				if (num4 == 4)
				{
					binaryReader.BaseStream.Seek(12L, SeekOrigin.Current);
					short num5 = binaryReader.ReadInt16();
					short num6 = binaryReader.ReadInt16();
					short num7 = binaryReader.ReadInt16();
					short num8 = binaryReader.ReadInt16();
					uint startIndex = binaryReader.ReadUInt32().FromPsxPtrToOffset(SotnFileType.DraBin);
					if (num6 % 2 == 0 && num5 % 2 == 0)
					{
						num6 *= 4;
						byte[] array = new byte[(int)(num8 / 2 * num7)];
						Compression.DecompressData(startIndex, DraBin.Instance.GetReader(), true, new BinaryWriter(new MemoryStream(array)), true);
						this.DynamicTiles.Add(num, new CompressedTileInfo(array, new VRamPoint((int)num6, (int)num5), new VRamPoint((int)num8, (int)num7)));
					}
				}
				num++;
			}
			binaryReader.Close();
		}

		public void Deinit()
		{
			this._rawData = null;
			this.DynamicTiles.Clear();
		}

		public BinaryReader GetReader()
		{
			return new BinaryReader(new MemoryStream(this._rawData));
		}

		public static DraBin Instance
		{
			get
			{
				if (DraBin._instance == null)
				{
					DraBin._instance = new DraBin();
				}
				return DraBin._instance;
			}
		}

		public readonly Dictionary<int, CompressedTileInfo> DynamicTiles = new Dictionary<int, CompressedTileInfo>();

		private byte[] _rawData;

		private static readonly long CompressedTilesOffset = 15196L;

		private static DraBin _instance;
	}
}
