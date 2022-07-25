using System;

namespace SotNMap
{
	[Flags]
	public enum MapDrawOptions
	{
		None = 0,
		DrawFront = 1,
		DrawBack = 2,
		DrawCollisionFront = 4,
		DrawCollisionBack = 8,
		DrawEntities = 16,
		All = 31
	}
}
