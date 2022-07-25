using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SotNMap;

namespace SotNEditor.HUD
{
	internal class HilitePanels
	{
		public void LoadContent(ContentManager contentMgr)
		{
			this._pixel = contentMgr.Load<Texture2D>("pixel");
			this._hudFont = contentMgr.Load<SpriteFont>("HUDFont");
			this._fontHeight = this._hudFont.MeasureString("|I").Y;
		}

		public void DrawHilitesForRooms(SpriteBatch sBatch, IList<MapRoom> allRooms, ref Matrix viewMatrix)
		{
			this._roomPanels.Clear();
			this._roomTexts.Clear();
			MapZone loadedMap = MainWindow.LoadedMap;
			IList<MapRoom> list = loadedMap.RoomFilter.FilterRoomsByMousePos(loadedMap.Rooms);
			int num = 0;
			for (int i = 0; i < list.Count<MapRoom>(); i++)
			{
				MapRoom mapRoom = list[i];
				Vector2 vector = Vector2.Transform(new Vector2((float)mapRoom.RoomRectInPixels.Left, (float)mapRoom.RoomRectInPixels.Top), viewMatrix);
				Vector2 vector2 = Vector2.Transform(new Vector2((float)mapRoom.RoomRectInPixels.Right, (float)mapRoom.RoomRectInPixels.Bottom), viewMatrix);
				Color color = this._roomPanelColors[i % this._roomPanelColors.Length];
				Rectangle rectangle = new Rectangle((int)(vector.X - 2f - 2f), (int)(vector.Y - 2f - 2f), (int)(vector2.X - vector.X + 4f + 4f), (int)(vector2.Y - vector.Y + 4f + 4f));
				Rectangle destinationRectangle = new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, 2);
				Rectangle destinationRectangle2 = new Rectangle(rectangle.X, rectangle.Y, 2, rectangle.Height);
				Rectangle destinationRectangle3 = new Rectangle(rectangle.Right - 2, rectangle.Y, 2, rectangle.Height);
				Rectangle destinationRectangle4 = new Rectangle(rectangle.X, rectangle.Bottom - 2, rectangle.Width, 2);
				sBatch.Draw(this._pixel, destinationRectangle, color);
				sBatch.Draw(this._pixel, destinationRectangle2, color);
				sBatch.Draw(this._pixel, destinationRectangle3, color);
				sBatch.Draw(this._pixel, destinationRectangle4, color);
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("Room Index: ");
				stringBuilder.Append(allRooms.IndexOf(mapRoom));
				stringBuilder.Append("\nPosition: ");
				stringBuilder.Append(mapRoom.RoomPosition.X);
				stringBuilder.Append(", ");
				stringBuilder.Append(mapRoom.RoomPosition.Y);
				stringBuilder.Append("\nDimensions: ");
				stringBuilder.Append(mapRoom.Dimensions.X);
				stringBuilder.Append(", ");
				stringBuilder.Append(mapRoom.Dimensions.Y);
				if (mapRoom.ForegroundLayer != null)
				{
					stringBuilder.Append("\n\nForeground");
					stringBuilder.Append("\nFlags: ");
					stringBuilder.Append(mapRoom.ForegroundLayer.Flags.ToString("X8"));
					stringBuilder.Append("\nDraw: ");
					stringBuilder.Append(mapRoom.ForegroundLayer.DrawFlags.ToString("X8"));
					stringBuilder.Append("\nTopmost: ");
					stringBuilder.Append(mapRoom.ForegroundLayer.Topmost);
					stringBuilder.Append("\nLayerLBA: ");
					stringBuilder.Append(mapRoom.ForegroundLayer.LayerLBA);
				}
				if (mapRoom.BackgroundLayer != null)
				{
					stringBuilder.Append("\n\nBackground");
					stringBuilder.Append("\nFlags: ");
					stringBuilder.Append(mapRoom.BackgroundLayer.Flags.ToString("X8"));
					stringBuilder.Append("\nDraw: ");
					stringBuilder.Append(mapRoom.BackgroundLayer.DrawFlags.ToString("X8"));
					stringBuilder.Append("\nTopmost: ");
					stringBuilder.Append(mapRoom.BackgroundLayer.Topmost);
					stringBuilder.Append("\nLayerLBA: ");
					stringBuilder.Append(mapRoom.BackgroundLayer.LayerLBA);
				}
				Vector2 vector3 = this._hudFont.MeasureString(stringBuilder);
				int num2 = (int)(vector3.X + 4f);
				int num3 = (int)(vector3.Y + 4f);
				int num4 = this.ClientDimensions.X - num2;
				int num5 = this.ClientDimensions.Y - num3 - num;
				num += num3 + 3;
				Rectangle key = new Rectangle(num4, num5, num2, num3);
				Vector2 key2 = new Vector2((float)(num4 + 2), (float)(num5 + 2));
				this._roomPanels.Add(key, color);
				this._roomTexts.Add(key2, stringBuilder.ToString());
				int num6 = num5 + num3 / 2;
				bool flag = rectangle.Y < num6;
				bool flag2 = rectangle.Bottom < num6;
				int num7;
				if (flag && !flag2)
				{
					num7 = rectangle.Right;
				}
				else
				{
					int num8 = Math.Max(rectangle.Left, 0);
					int num9 = Math.Min(rectangle.Right, this.ClientDimensions.X);
					num7 = (num8 + num9) / 2;
				}
				Rectangle destinationRectangle5 = new Rectangle(num7, num6, key.Left - num7, 2);
				if (!flag)
				{
					Rectangle destinationRectangle6 = new Rectangle(num7, num6, 2, rectangle.Y - num6);
					sBatch.Draw(this._pixel, destinationRectangle6, color);
				}
				else if (flag2)
				{
					Rectangle destinationRectangle7 = new Rectangle(num7, rectangle.Bottom, 2, num6 - rectangle.Bottom + 2);
					sBatch.Draw(this._pixel, destinationRectangle7, color);
				}
				sBatch.Draw(this._pixel, destinationRectangle5, color);
			}
			foreach (KeyValuePair<Rectangle, Color> keyValuePair in this._roomPanels)
			{
				sBatch.Draw(this._pixel, keyValuePair.Key, keyValuePair.Value);
			}
			foreach (KeyValuePair<Vector2, string> keyValuePair2 in this._roomTexts)
			{
				sBatch.DrawString(this._hudFont, keyValuePair2.Value, keyValuePair2.Key, Color.Black);
			}
		}

		public void DrawHilitesForEntities(SpriteBatch sBatch, IList<MapRoom> allRooms, ref Matrix viewMatrix)
		{
			foreach (MapRoom mapRoom in allRooms)
			{
				this._entPanels.Clear();
				this._entTexts.Clear();
				IList<MapEntity> list = MainWindow.LoadedMap.RoomFilter.FilterEntitiesByMousePos(mapRoom, mapRoom.Entities);
				int num = 0;
				for (int i = 0; i < list.Count<MapEntity>(); i++)
				{
					MapEntity mapEntity = list[i];
					Vector2 vector = Vector2.Transform(new Vector2((float)(mapRoom.RoomRectInPixels.X + mapEntity.RoomPosition.X - 8), (float)(mapRoom.RoomRectInPixels.Y + mapEntity.RoomPosition.Y - 8)), viewMatrix);
					Vector2 vector2 = Vector2.Transform(new Vector2((float)(mapRoom.RoomRectInPixels.X + mapEntity.RoomPosition.X + 8), (float)(mapRoom.RoomRectInPixels.Y + mapEntity.RoomPosition.Y + 8)), viewMatrix);
					Color color = this._entPanelColors[i % this._entPanelColors.Length];
					Rectangle rectangle = new Rectangle((int)(vector.X - 2f - 2f), (int)(vector.Y - 2f - 2f), (int)(vector2.X - vector.X + 4f + 4f), (int)(vector2.Y - vector.Y + 4f + 4f));
					Rectangle destinationRectangle = new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, 2);
					Rectangle destinationRectangle2 = new Rectangle(rectangle.X, rectangle.Y, 2, rectangle.Height);
					Rectangle destinationRectangle3 = new Rectangle(rectangle.Right - 2, rectangle.Y, 2, rectangle.Height);
					Rectangle destinationRectangle4 = new Rectangle(rectangle.X, rectangle.Bottom - 2, rectangle.Width, 2);
					sBatch.Draw(this._pixel, destinationRectangle, color);
					sBatch.Draw(this._pixel, destinationRectangle2, color);
					sBatch.Draw(this._pixel, destinationRectangle3, color);
					sBatch.Draw(this._pixel, destinationRectangle4, color);
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append("Entity Index: ");
					stringBuilder.Append(mapRoom.Entities.IndexOf(mapEntity).ToString());
					stringBuilder.Append("\nId: ");
					stringBuilder.Append(mapEntity.Id.ToString());
					stringBuilder.Append("\nInitial State: ");
					stringBuilder.Append(mapEntity.InitialState.ToString());
					stringBuilder.Append("\nDead Flag: ");
					stringBuilder.Append(mapEntity.DeadFlag.ToString());
					stringBuilder.Append("\nUnknown 1: ");
					stringBuilder.Append(mapEntity.Unknown1.ToString());
					stringBuilder.Append("\nUnknown 2: ");
					stringBuilder.Append(mapEntity.Unknown2.ToString());
					stringBuilder.Append("\nUnknown 3: ");
					stringBuilder.Append(mapEntity.Unknown3.ToString());
					Vector2 vector3 = this._hudFont.MeasureString(stringBuilder);
					int width = (int)(vector3.X + 4f);
					int num2 = (int)(vector3.Y + 4f);
					int num3 = 0;
					int num4 = num;
					num += num2 + 3;
					Rectangle key = new Rectangle(num3, num4, width, num2);
					Vector2 key2 = new Vector2((float)(num3 + 2), (float)(num4 + 2));
					this._entPanels.Add(key, color);
					this._entTexts.Add(key2, stringBuilder.ToString());
					int num5 = num4 + num2 / 2;
					bool flag = rectangle.Y < num5;
					bool flag2 = rectangle.Bottom < num5;
					int num6;
					if (flag && !flag2)
					{
						num6 = rectangle.X;
					}
					else
					{
						int num7 = Math.Max(rectangle.X, 0);
						int num8 = Math.Min(rectangle.Right, this.ClientDimensions.X);
						num6 = (num7 + num8) / 2;
					}
					Rectangle destinationRectangle5 = new Rectangle(key.Right, num5, num6 - key.Right, 2);
					if (!flag)
					{
						Rectangle destinationRectangle6 = new Rectangle(num6, num5, 2, rectangle.Y - num5);
						sBatch.Draw(this._pixel, destinationRectangle6, color);
					}
					else if (flag2)
					{
						Rectangle destinationRectangle7 = new Rectangle(num6, rectangle.Bottom, 2, num5 - rectangle.Bottom + 2);
						sBatch.Draw(this._pixel, destinationRectangle7, color);
					}
					sBatch.Draw(this._pixel, destinationRectangle5, color);
				}
				foreach (KeyValuePair<Rectangle, Color> keyValuePair in this._entPanels)
				{
					sBatch.Draw(this._pixel, keyValuePair.Key, keyValuePair.Value);
				}
				foreach (KeyValuePair<Vector2, string> keyValuePair2 in this._entTexts)
				{
					sBatch.DrawString(this._hudFont, keyValuePair2.Value, keyValuePair2.Key, Color.Black);
				}
			}
		}

		private const int PanelPadding = 2;

		private const int PanelSpacing = 3;

		private const int HilitePadding = 2;

		private const int HiliteThickness = 2;

		public Point ClientDimensions = Point.Zero;

		private Color[] _roomPanelColors = new Color[]
		{
			Color.PaleVioletRed,
			Color.SandyBrown,
			Color.SteelBlue,
			Color.RosyBrown
		};

		private Color[] _entPanelColors = new Color[]
		{
			Color.Red,
			Color.RoyalBlue,
			Color.SeaGreen,
			Color.Orange
		};

		private Texture2D _pixel;

		private SpriteFont _hudFont;

		private float _fontHeight;

		private Dictionary<Rectangle, Color> _roomPanels = new Dictionary<Rectangle, Color>();

		private Dictionary<Vector2, string> _roomTexts = new Dictionary<Vector2, string>();

		private Dictionary<Rectangle, Color> _entPanels = new Dictionary<Rectangle, Color>();

		private Dictionary<Vector2, string> _entTexts = new Dictionary<Vector2, string>();
	}
}
