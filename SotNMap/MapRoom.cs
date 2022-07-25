using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PSX;

namespace SotNMap
{
	public class MapRoom
	{
		public MapRoom(MapPoint roomOffset, MapPoint dimensions)
		{
			this.Entities = new List<MapEntity>();
			this.RoomPosition = roomOffset;
			this.Dimensions = dimensions;
		}

		public void Init(int zoneOffsetX, int zoneOffsetY)
		{
			this.RoomRectInPixels = new Rectangle
			{
				X = (this.RoomPosition.X - zoneOffsetX) * Constants.RoomWidthInPixels,
				Y = (this.RoomPosition.Y - zoneOffsetY) * Constants.RoomHeightInPixels,
				Width = this.Dimensions.X * Constants.RoomWidthInPixels,
				Height = this.Dimensions.Y * Constants.RoomHeightInPixels
			};
		}

		public Rectangle RoomRectInPixels { get; private set; }

		public MapPoint RoomPosition { get; private set; }

		public MapPoint Dimensions { get; private set; }

		public MapTileLayer BackgroundLayer { get; set; }

		public MapTileLayer ForegroundLayer
		{
			get
			{
				return this._foregroundLayer;
			}
			set
			{
				this._foregroundLayer = value;
				this.CollisionLayer = new MapTileLayer(this._foregroundLayer.Dimensions);
				for (int i = 0; i < this._foregroundLayer.Dimensions.Y; i++)
				{
					for (int j = 0; j < this._foregroundLayer.Dimensions.X; j++)
					{
						Point collisionIndex = this._foregroundLayer.GetCollisionIndex(j, i);
						this.CollisionLayer[j, i] = new MapTile
						{
							TilePos = new VRamPoint(collisionIndex.X, collisionIndex.Y),
							ClutAsColor = Color.White
						};
					}
				}
			}
		}

		public MapTileLayer CollisionLayer { get; private set; }

		public IList<MapEntity> Entities { get; set; }

		public void DrawRoom(SpriteBatch sbatch, Effect paletteDrawingEffect, Matrix viewMatrix, bool drawFront, bool drawBack, Texture2D vramTex, Texture2D pixel, Color flushColor)
		{
			if (drawBack && this.BackgroundLayer != null)
			{
				this.BackgroundLayer.Draw(sbatch, vramTex, this.RoomRectInPixels, 0.5f);
			}
			if (drawFront && this.ForegroundLayer != null)
			{
				this.ForegroundLayer.Draw(sbatch, vramTex, this.RoomRectInPixels, 0.5f);
			}
		}

		public void DrawCollision(SpriteBatch sbatch, Texture2D collisionTex)
		{
			this.CollisionLayer.Draw(sbatch, collisionTex, this.RoomRectInPixels, 0.5f);
		}

		private MapTileLayer _foregroundLayer;
	}
}
