using System;

namespace SotNMap
{
	public class MapPoint
	{
		public static bool operator ==(MapPoint lhs, MapPoint rhs)
		{
			return lhs.X == rhs.X && lhs.Y == rhs.Y;
		}

		public static bool operator !=(MapPoint lhs, MapPoint rhs)
		{
			return lhs.X != rhs.X || lhs.Y != rhs.Y;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public int X { get; set; }

		public int Y { get; set; }
	}
}
