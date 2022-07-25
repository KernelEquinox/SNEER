using System;

namespace PSX
{
	public class VRamRect
	{
		public VRamRect(VRamPoint topLeft, VRamPoint dimensions)
		{
			this._topLeft = topLeft;
			this._dimensions = dimensions;
		}

		public VRamRect(int xPsxVRam, int y, int wPsxVRam, int h)
		{
			this._topLeft = new VRamPoint(xPsxVRam, y);
			this._dimensions = new VRamPoint(wPsxVRam, h);
		}

		public VRamPoint TopLeft
		{
			get
			{
				return this._topLeft;
			}
			set
			{
				this._topLeft = value;
			}
		}

		public VRamPoint Dimensions
		{
			get
			{
				return this._dimensions;
			}
			set
			{
				this._dimensions = value;
			}
		}

		public static bool operator ==(VRamRect lhs, VRamRect rhs)
		{
			return lhs._topLeft == rhs._topLeft && lhs._dimensions == rhs._dimensions;
		}

		public static bool operator !=(VRamRect lhs, VRamRect rhs)
		{
			return lhs._topLeft != rhs._topLeft || lhs._dimensions != rhs._dimensions;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		private VRamPoint _topLeft;

		private VRamPoint _dimensions;
	}
}
