using System;
using PSX;

namespace SotN
{
	public static class VRamLayout
	{
		public static readonly VRamPoint GenericClutsVRamPosition = new VRamPoint(0, 240);

		public static readonly VRamPoint GenericTilesVRamPosition = new VRamPoint(2048, 256);

		public static readonly VRamPoint MapTilesVRamPosition = new VRamPoint(2048, 0);
	}
}
