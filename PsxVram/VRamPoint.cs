using System;

namespace PSX
{
	public class VRamPoint
	{
		public static VRamPoint Zero
		{
			get
			{
				return VRamPoint._zero;
			}
		}

		public VRamPoint(int x, int y)
		{
			this.X = x;
			this.Y = y;
		}

		public int X { get; set; }

		public int Y { get; set; }

		private static readonly VRamPoint _zero = new VRamPoint(0, 0);
	}
}
