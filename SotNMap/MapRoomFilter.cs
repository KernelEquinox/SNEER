using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace SotNMap
{
	public class MapRoomFilter
	{
		public Matrix InvertedMatrix { get; private set; }

		public void RunQuery(ref Matrix viewMatrix, Vector2 screenDims, Vector2 localMousePos, int? drawThisRoomIndexLast)
		{
			this._drawThisRoomIndexLast = drawThisRoomIndexLast;
			this.InvertedMatrix = Matrix.Invert(viewMatrix);
			this._invertedScreenTL = Vector2.Transform(Vector2.Zero, this.InvertedMatrix);
			this._invertedScreenBR = Vector2.Transform(screenDims, this.InvertedMatrix);
			this._screenRect.X = (int)this._invertedScreenTL.X;
			this._screenRect.Y = (int)this._invertedScreenTL.Y;
			this._screenRect.Width = (int)(this._invertedScreenBR.X - this._invertedScreenTL.X);
			this._screenRect.Height = (int)(this._invertedScreenBR.Y - this._invertedScreenTL.Y);
			Vector2 vector = Vector2.Transform(localMousePos, this.InvertedMatrix);
			this._invertedMousePos.X = (int)vector.X;
			this._invertedMousePos.Y = (int)vector.Y;
			this.Reset();
		}

		public void Reset()
		{
			this._screenFilteredRooms.Clear();
			this._mouseFilteredRooms.Clear();
			this._mouseFilteredEntities.Clear();
		}

		public IList<MapRoom> FilterRoomsByScreenRect(IList<MapRoom> roomList)
		{
			if (!this._screenFilteredRooms.Any<MapRoom>())
			{
				for (int i = 0; i < roomList.Count; i++)
				{
					if (this._drawThisRoomIndexLast == null || i != this._drawThisRoomIndexLast.Value)
					{
						MapRoom mapRoom = roomList[i];
						if (true || this._screenRect.Intersects(mapRoom.RoomRectInPixels) || this._screenRect.Contains(mapRoom.RoomRectInPixels) || mapRoom.RoomRectInPixels.Contains(this._screenRect))
						{
							this._screenFilteredRooms.Add(mapRoom);
						}
					}
				}
				if (this._drawThisRoomIndexLast != null)
				{
					this._screenFilteredRooms.Add(roomList[this._drawThisRoomIndexLast.Value]);
				}
			}
			return this._screenFilteredRooms;
		}

		public IList<MapRoom> FilterRoomsByMousePos(IList<MapRoom> roomList)
		{
			this._mouseFilteredRooms.Clear();
			for (int i = 0; i < roomList.Count; i++)
			{
				MapRoom mapRoom = roomList[i];
				if (mapRoom.RoomRectInPixels.Contains(this._invertedMousePos))
				{
					this._mouseFilteredRooms.Add(mapRoom);
				}
			}
			return this._mouseFilteredRooms;
		}

		public IList<MapEntity> FilterEntitiesByMousePos(MapRoom room, IList<MapEntity> entityList)
		{
			this._mouseFilteredEntities.Clear();
			for (int i = 0; i < entityList.Count; i++)
			{
				MapEntity mapEntity = entityList[i];
				if (room.RoomRectInPixels.X + mapEntity.RoomPosition.X + 8 >= this._invertedMousePos.X && room.RoomRectInPixels.X + mapEntity.RoomPosition.X - 8 <= this._invertedMousePos.X && room.RoomRectInPixels.Y + mapEntity.RoomPosition.Y + 8 >= this._invertedMousePos.Y && room.RoomRectInPixels.Y + mapEntity.RoomPosition.Y - 8 <= this._invertedMousePos.Y)
				{
					this._mouseFilteredEntities.Add(mapEntity);
				}
			}
			return this._mouseFilteredEntities;
		}

		private Vector2 _invertedScreenTL;

		private Vector2 _invertedScreenBR;

		private Point _invertedMousePos;

		private Rectangle _screenRect = Rectangle.Empty;

		private int? _drawThisRoomIndexLast;

		private readonly List<MapRoom> _screenFilteredRooms = new List<MapRoom>();

		private readonly List<MapRoom> _mouseFilteredRooms = new List<MapRoom>();

		private readonly List<MapEntity> _mouseFilteredEntities = new List<MapEntity>();
	}
}
