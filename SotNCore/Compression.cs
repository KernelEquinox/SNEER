using System;
using System.IO;

namespace SotN
{
	public static class Compression
	{
		public static void DecompressData(uint startIndex, BinaryReader reader, bool closeReaderWhenDone, BinaryWriter writer, bool closeWriterWhenDone)
		{
			long position = reader.BaseStream.Position;
			reader.BaseStream.Seek((long)((ulong)startIndex), SeekOrigin.Begin);
			byte[] array = reader.ReadBytes(8);
			Compression._readLeft = true;
			Compression._writeLeft = false;
			bool flag = false;
			while (!flag)
			{
				byte b = Compression.ReadNybble(reader);
				switch (b)
				{
				case 0:
				{
					byte b2 = Compression.ReadNybble(reader);
					byte b3 = Compression.ReadNybble(reader);
					int num = ((int)b2 << 4) + (int)b3 + 19;
					for (int i = 0; i < num; i++)
					{
						Compression.WriteNybble(writer, 0);
					}
					break;
				}
				case 1:
				{
					byte value = Compression.ReadNybble(reader);
					Compression.WriteNybble(writer, value);
					break;
				}
				case 2:
				{
					byte value2 = Compression.ReadNybble(reader);
					Compression.WriteNybble(writer, value2);
					Compression.WriteNybble(writer, value2);
					break;
				}
				case 3:
				{
					byte value3 = Compression.ReadNybble(reader);
					Compression.WriteNybble(writer, value3);
					value3 = Compression.ReadNybble(reader);
					Compression.WriteNybble(writer, value3);
					break;
				}
				case 4:
				{
					byte value4 = Compression.ReadNybble(reader);
					Compression.WriteNybble(writer, value4);
					value4 = Compression.ReadNybble(reader);
					Compression.WriteNybble(writer, value4);
					value4 = Compression.ReadNybble(reader);
					Compression.WriteNybble(writer, value4);
					break;
				}
				case 5:
				{
					byte value5 = Compression.ReadNybble(reader);
					byte b4 = (byte)(Compression.ReadNybble(reader) + 3);
					for (int j = 0; j < (int)b4; j++)
					{
						Compression.WriteNybble(writer, value5);
					}
					break;
				}
				case 6:
				{
					byte b5 = (byte)(Compression.ReadNybble(reader) + 3);
					for (int k = 0; k < (int)b5; k++)
					{
						Compression.WriteNybble(writer, 0);
					}
					break;
				}
				case 7:
				case 8:
				case 9:
				case 10:
				case 11:
				case 12:
				case 13:
				case 14:
				{
					byte b6 = (byte)(array[(int)(b - 7)] & byte.MaxValue);
					byte b7 = (byte)(b6 >> 4);
					byte b8 = (byte)(b6 & 15);
					if (b7 == 6)
					{
						for (int l = 0; l < (int)(b8 + 3); l++)
						{
							Compression.WriteNybble(writer, 0);
						}
					}
					else if (b7 == 2)
					{
						for (int m = 0; m < 2; m++)
						{
							Compression.WriteNybble(writer, b8);
						}
					}
					else if (b7 == 1)
					{
						Compression.WriteNybble(writer, b8);
					}
					break;
				}
				case 15:
					flag = true;
					break;
				}
			}
			if (closeReaderWhenDone)
			{
				reader.Close();
			}
			else
			{
				reader.BaseStream.Position = position;
			}
			if (closeWriterWhenDone)
			{
				writer.Close();
			}
		}

		private static byte ReadNybble(BinaryReader reader)
		{
			byte result;
			if (Compression._readLeft)
			{
				result = (byte)(reader.ReadByte() >> 4 & 15);
				reader.BaseStream.Seek(-1L, SeekOrigin.Current);
			}
			else
			{
				result = (byte)(reader.ReadByte() & 15);
			}
			Compression._readLeft = !Compression._readLeft;
			return result;
		}

		private static void WriteNybble(BinaryWriter writer, byte value)
		{
			if (Compression._writeLeft)
			{
				int num = writer.BaseStream.ReadByte();
				writer.BaseStream.Seek(-1L, SeekOrigin.Current);
				writer.Write((byte)(num + ((int)value << 4)));
			}
			else
			{
				writer.Write(value);
				writer.BaseStream.Seek(-1L, SeekOrigin.Current);
			}
			Compression._writeLeft = !Compression._writeLeft;
		}

		private static bool _readLeft;

		private static bool _writeLeft;
	}
}
