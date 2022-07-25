using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using PSX;
using SotNMap;

namespace SotN
{
	public static class LayoutReader
	{
		public static MapZone LoadMapLayout(string filename)
		{
			Stream stream;
			BinaryReader binaryReader;
			try
			{
				stream = new FileStream(filename, FileMode.Open);
				binaryReader = new BinaryReader(stream);
			}
			catch
			{
				return null;
			}
			Dictionary<uint, MapTileLayer> dictionary = new Dictionary<uint, MapTileLayer>();
			Dictionary<uint, MapTile> cachedTiles = new Dictionary<uint, MapTile>();
			stream.Seek(12L, SeekOrigin.Begin);
			uint num = binaryReader.ReadUInt32().FromPsxPtrToOffset(SotnFileType.MapBin);
			uint num2 = binaryReader.ReadUInt32();
			uint num3 = num2.FromPsxPtrToOffset(SotnFileType.MapBin);
			uint value = binaryReader.ReadUInt32();
			value.FromPsxPtrToOffset(SotnFileType.MapBin);
			stream.Seek(32L, SeekOrigin.Begin);
			uint num4 = binaryReader.ReadUInt32().FromPsxPtrToOffset(SotnFileType.MapBin);
			uint num5 = binaryReader.ReadUInt32().FromPsxPtrToOffset(SotnFileType.MapBin);
			List<MapRoom> list = new List<MapRoom>();
			int num6 = 0;
			int minX = int.MaxValue;
			int minY = int.MaxValue;
			int maxX = int.MaxValue;
			int maxY = int.MaxValue;
			int width = int.MaxValue;
			int height = int.MaxValue;
			if (num2 != 0U)
			{
				stream.Seek((long)((ulong)(num + 28U)), SeekOrigin.Begin);
				ushort num9 = binaryReader.ReadUInt16();
				stream.Seek((long)((ulong)(num + 40U)), SeekOrigin.Begin);
				binaryReader.ReadUInt16();
				for (;;)
				{
					stream.Seek((long)((ulong)num3 + (ulong)((long)(num6 * 8))), SeekOrigin.Begin);
					if ((byte)binaryReader.PeekChar() == 64)
					{
						break;
					}
					byte b = binaryReader.ReadByte();
					byte b2 = binaryReader.ReadByte();
					byte b3 = binaryReader.ReadByte();
					byte b4 = binaryReader.ReadByte();
					MapRoom mapRoom = new MapRoom(new MapPoint
					{
						X = (int)b,
						Y = (int)b2
					}, new MapPoint
					{
						X = (int)(b3 - b + 1),
						Y = (int)(b4 - b2 + 1)
					});
					byte b5 = binaryReader.ReadByte();
					byte b6 = binaryReader.ReadByte();
					byte b7 = binaryReader.ReadByte();
					byte b8 = binaryReader.ReadByte();
					int num10 = (int)(b7 - ((b7 == 0) ? 0 : 1));
					stream.Seek((long)((ulong)num5 + (ulong)((long)(num10 * 4))), SeekOrigin.Begin);
					uint num11 = binaryReader.ReadUInt32().FromPsxPtrToOffset(SotnFileType.MapBin);
					stream.Seek((long)((ulong)num11), SeekOrigin.Begin);
					uint num12 = binaryReader.ReadUInt32();
					while (num12 != 4294967295U)
					{
						ushort y = binaryReader.ReadUInt16();
						ushort num13 = binaryReader.ReadUInt16();
						ushort num14 = binaryReader.ReadUInt16();
						ushort num15 = binaryReader.ReadUInt16();
						uint num16 = binaryReader.ReadUInt32();
						uint startIndex = num16.FromPsxPtrToOffset(SotnFileType.MapBin);
						VRamPoint topLeft = new VRamPoint((int)(num13 * 4), (int)y);
						VRamPoint vramPoint = new VRamPoint((int)num15, (int)num14);
						if (num16 != 0U && num15 != 0 && num14 != 0)
						{
							byte[] array = new byte[vramPoint.X / 2 * vramPoint.Y];
							using (BinaryWriter binaryWriter = new BinaryWriter(new MemoryStream(array)))
							{
								Compression.DecompressData(startIndex, binaryReader, false, binaryWriter, true);
							}
							VRam.Instance.ImportImageData(new VRamRect(topLeft, vramPoint), array);
						}
						num12 = binaryReader.ReadUInt32();
						binaryReader.BaseStream.Seek(-4L, SeekOrigin.Current);
					}
					binaryReader.BaseStream.Seek((long)(num9 + (ushort)(b8 * 4)), SeekOrigin.Begin);
					uint num17 = binaryReader.ReadUInt32().FromPsxPtrToOffset(SotnFileType.MapBin);
					binaryReader.BaseStream.Seek((long)((ulong)num17), SeekOrigin.Begin);
					short num18 = (short)binaryReader.ReadUInt16();
					short num19 = (short)binaryReader.ReadUInt16();
					long position = binaryReader.BaseStream.Position;
					while (num18 != -1 && num19 != -1)
					{
						binaryReader.BaseStream.Seek(position, SeekOrigin.Begin);
						ushort num20 = binaryReader.ReadUInt16();
						short id = (short)(num20 & 1023);
						byte unknown = (byte)(num20 >> 10 & 7);
						byte unknown2 = (byte)(num20 >> 13 & 7);
						binaryReader.ReadByte();
						byte unknown3 = binaryReader.ReadByte();
						ushort initialState = binaryReader.ReadUInt16();
						MapEntity item = new MapEntity
						{
							RoomPosition = new MapPoint
							{
								X = (int)num18,
								Y = (int)num19
							},
							Id = id,
							Unknown1 = unknown,
							Unknown2 = unknown2,
							Unknown3 = unknown3,
							InitialState = initialState
						};
						mapRoom.Entities.Add(item);
						num18 = (short)binaryReader.ReadUInt16();
						num19 = (short)binaryReader.ReadUInt16();
						position = binaryReader.BaseStream.Position;
					}
					if (b6 != 255)
					{
						stream.Seek((long)((ulong)num4 + (ulong)((long)(b5 * 8))), SeekOrigin.Begin);
						uint num21 = binaryReader.ReadUInt32();
						uint num22 = binaryReader.ReadUInt32();
						if (num21 == 0U && num22 == 0U)
						{
							break;
						}
						if (!dictionary.ContainsKey(num21))
						{
							uint layerAddress = num21.FromPsxPtrToOffset(SotnFileType.MapBin);
							mapRoom.ForegroundLayer = LayoutReader.ReadLayer(binaryReader, layerAddress, cachedTiles);
							dictionary.Add(num21, mapRoom.ForegroundLayer);
						}
						if (!dictionary.ContainsKey(num22))
						{
							uint layerAddress2 = num22.FromPsxPtrToOffset(SotnFileType.MapBin);
							mapRoom.BackgroundLayer = LayoutReader.ReadLayer(binaryReader, layerAddress2, cachedTiles);
							dictionary.Add(num22, mapRoom.BackgroundLayer);
						}
					}
					num6++;
					list.Add(mapRoom);
				}
				// Get the minimum X/Y coordinates
				minX = list.Min((MapRoom room) => room.RoomPosition.X);
				minY = list.Min((MapRoom room) => room.RoomPosition.Y);

				// Get the maximum X/Y coordinates
				maxX = list.Max((MapRoom room) => room.RoomPosition.X + room.Dimensions.X);
				maxY = list.Max((MapRoom room) => room.RoomPosition.Y + room.Dimensions.Y);

				// Calculate width and height based on map coordinates
				width = maxX - minX;
				height = maxY - minY;
				foreach (MapRoom mapRoom2 in list)
				{
					mapRoom2.Init(minX, minY);
				}
				binaryReader.Close();
			}
			MapZone ret = new MapZone(new Rectangle(minX, minY, width, height));
			list.ForEach(delegate(MapRoom r)
			{
				ret.Rooms.Add(r);
			});
			return ret;
		}

		private static MapTileLayer ReadLayer(BinaryReader reader, uint layerAddress, Dictionary<uint, MapTile> cachedTiles)
		{
			long position = reader.BaseStream.Position;
			reader.BaseStream.Seek((long)((ulong)layerAddress), SeekOrigin.Begin);
			uint num = reader.ReadUInt32();
			if (num == 0U)
			{
				return null;
			}
			uint num2 = num.FromPsxPtrToOffset(SotnFileType.MapBin);
			uint num3 = reader.ReadUInt32().FromPsxPtrToOffset(SotnFileType.MapBin);
			Rectangle rectangle = LayoutReader.DecodeLayerSize(reader.ReadBytes(3));
			byte b = reader.ReadByte();
			byte layerLBA = reader.ReadByte();
			byte topmost = reader.ReadByte();
			ushort num4 = reader.ReadUInt16();
			bool flag = (num4 & 512) != 0;
			reader.BaseStream.Seek((long)((ulong)num3), SeekOrigin.Begin);
			uint num5 = reader.ReadUInt32().FromPsxPtrToOffset(SotnFileType.MapBin);
			uint num6 = reader.ReadUInt32().FromPsxPtrToOffset(SotnFileType.MapBin);
			uint num7 = reader.ReadUInt32().FromPsxPtrToOffset(SotnFileType.MapBin);
			uint num8 = reader.ReadUInt32().FromPsxPtrToOffset(SotnFileType.MapBin);
			MapTileLayer mapTileLayer = new MapTileLayer(new MapPoint
			{
				X = rectangle.Width,
				Y = rectangle.Height
			})
			{
				Flags = b,
				LayerLBA = layerLBA,
				Topmost = topmost
			};
			if (DraBin.Instance.IsValid)
			{
				if ((b & 32) != 0)
				{
					mapTileLayer.DynamicTiles.Add(DraBin.Instance.DynamicTiles[3]);
				}
				if ((b & 64) != 0)
				{
					mapTileLayer.DynamicTiles.Add(DraBin.Instance.DynamicTiles[4]);
				}
			}
			for (int i = 0; i < rectangle.Height; i++)
			{
				for (int j = 0; j < rectangle.Width; j++)
				{
					reader.BaseStream.Seek((long)((ulong)num2 + (ulong)((long)(i * rectangle.Width * 2)) + (ulong)((long)(j * 2))), SeekOrigin.Begin);
					ushort num9 = reader.ReadUInt16();
					reader.BaseStream.Seek((long)((ulong)(num5 + (uint)num9)), SeekOrigin.Begin);
					byte b2 = reader.ReadByte();
					reader.BaseStream.Seek((long)((ulong)(num6 + (uint)num9)), SeekOrigin.Begin);
					byte b3 = reader.ReadByte();
					reader.BaseStream.Seek((long)((ulong)(num7 + (uint)num9)), SeekOrigin.Begin);
					byte b4 = reader.ReadByte();
					reader.BaseStream.Seek((long)((ulong)(num8 + (uint)num9)), SeekOrigin.Begin);
					byte b5 = reader.ReadByte();
					Point value = new Point((b5 >> 4 & 15) * 16, (int)((b5 & 15) * 16));
					mapTileLayer.SetCollisionIndex(j, i, value);
					uint key = (uint)((int)b2 << 24 | (int)b3 << 16 | (int)b4 << 8 | (int)b5);
					MapTile value2;
					if (cachedTiles.ContainsKey(key))
					{
						value2 = cachedTiles[key];
					}
					else
					{
						int num10 = (int)(b2 & 15);
						int num11 = (b2 & 240) >> 4;
						int num12 = ((int)b3 << 4 & 240) + num10 * 256;
						int num13 = (int)(b3 & 240) + num11 * 256;
						Vector2 vector = flag ? VRam.Instance.GenericCluts[(int)b4] : VRam.Instance.MapCluts[(int)b4];
						value2 = new MapTile
						{
							TilePos = new VRamPoint(VRamLayout.MapTilesVRamPosition.X + num12, VRamLayout.MapTilesVRamPosition.Y + num13),
							ClutAsColor = new Color((int)vector.X, (int)vector.Y, 0, 0)
						};
						cachedTiles.Add(key, value2);
					}
					mapTileLayer[j, i] = value2;
				}
			}
			reader.BaseStream.Seek(position, SeekOrigin.Begin);
			return mapTileLayer;
		}

		private static Rectangle DecodeLayerSize(byte[] encodedValue)
		{
			uint num = (uint)((int)encodedValue[2] << 16 | (int)encodedValue[1] << 8 | (int)encodedValue[0]);
			uint num2 = num & 63U;
			uint num3 = num >> 6 & 63U;
			uint num4 = num >> 12 & 63U;
			uint num5 = num >> 18 & 63U;
			return new Rectangle((int)(num2 * 16U), (int)(num3 * 16U), (int)((num4 - num2 + 1U) * 16U), (int)((num5 - num3 + 1U) * 16U));
		}
	}
}
