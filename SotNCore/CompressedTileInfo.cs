using System;
using PSX;

namespace SotN
{
	public class CompressedTileInfo
	{
		public byte[] Data { get; private set; }

		public VRamRect Rect { get; private set; }

		public CompressedTileInfo(byte[] data, VRamPoint pos, VRamPoint dims)
		{
			this.Data = data;
			this.Rect = new VRamRect(pos, dims);
		}
	}
}
