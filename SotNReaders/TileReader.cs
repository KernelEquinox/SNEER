using System;
using System.Collections.Generic;
using System.IO;
using PSX;

namespace SotN
{
	public static class TileReader
	{
		public static IList<string> LoadGenericTilesToVRam(string filename)
		{
			List<string> list = new List<string>();
			try
			{
				using (FileStream fileStream = new FileStream(filename, FileMode.Open))
				{
					if (!TileReader.LoadTilesToVRam(VRamLayout.GenericTilesVRamPosition, fileStream))
					{
						list.Add("Unable to load generic tiles to VRam");
					}
					else
					{
						byte[] array = new byte[TileReader.BlockSizeBytes];
						fileStream.Read(array, 0, TileReader.BlockSizeBytes);
						VRam.Instance.ImportImageData(new VRamRect(new VRamPoint(VRamLayout.GenericClutsVRamPosition.X, VRamLayout.GenericClutsVRamPosition.Y), new VRamPoint(TileReader.BlockWidthVRam * 8, TileReader.BlockHeightVRam / 8)), array);
					}
				}
			}
			catch (Exception ex)
			{
				list.Add("Exception while loading generic tiles to VRam: " + ex.Message + "\nStack Trace: " + ex.StackTrace);
			}
			return list;
		}

		public static IList<string> LoadMapTilesToVRam(string filename)
		{
			List<string> list = new List<string>();
			try
			{
				using (FileStream fileStream = new FileStream(filename, FileMode.Open))
				{
					if (!TileReader.LoadTilesToVRam(VRamLayout.MapTilesVRamPosition, fileStream))
					{
						list.Add("Unable to load map tiles to VRam");
					}
				}
			}
			catch (Exception ex)
			{
				list.Add("Exception while loading map tiles to VRam: " + ex.Message + "\nStack Trace: " + ex.StackTrace);
			}
			return list;
		}

		private static bool LoadTilesToVRam(VRamPoint toPos, FileStream inStream)
		{
			byte[] array = new byte[TileReader.BlockSizeBytes];
			for (int i = 0; i < TileReader.SlotsPerFile; i++)
			{
				for (int j = 0; j <= TileReader.BlockHeightVRam; j += TileReader.BlockHeightVRam)
				{
					for (int k = 0; k <= TileReader.BlockWidthVRam; k += TileReader.BlockWidthVRam)
					{
						try
						{
							inStream.Read(array, 0, TileReader.BlockSizeBytes);
						}
						catch
						{
							return false;
						}
						VRam.Instance.ImportImageData(new VRamRect(new VRamPoint(toPos.X + i * TileReader.BlockWidthVRam * 2 + k, toPos.Y + j), new VRamPoint(TileReader.BlockWidthVRam, TileReader.BlockHeightVRam)), array);
					}
				}
			}
			return true;
		}

		private static readonly int BlockWidthVRam = 128;

		private static readonly int BlockHeightVRam = 128;

		private static readonly int BlockSizeBytes = TileReader.BlockWidthVRam / 2 * TileReader.BlockHeightVRam;

		private static readonly int SlotsPerFile = 8;
	}
}
