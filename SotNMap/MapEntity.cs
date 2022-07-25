using System;
using Microsoft.Xna.Framework.Graphics;

namespace SotNMap
{
	public class MapEntity
	{
		public MapPoint RoomPosition;

		public Texture2D Texture;

		public short Id;

		public byte Unknown1;

		public byte Unknown2;

		public byte Unknown3;

		public byte DeadFlag;

		public ushort InitialState;
	}
}
