using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PSX;
using SotN;

namespace SotNMap
{
	public class MapTileLayer
	{
		public MapPoint Dimensions { get; private set; }

		public IList<CompressedTileInfo> DynamicTiles { get; private set; }

		public MapTileLayer(MapPoint dimensions)
		{
			this.DynamicTiles = new List<CompressedTileInfo>();
			this.Dimensions = dimensions;
			this._tiles = new MapTile[dimensions.Y, dimensions.X];
			this._collisionCoordinates = new Point[dimensions.Y, dimensions.X];
		}

		public MapTile this[int x, int y]
		{
			get
			{
				MapTile mapTile = this._tiles[y, x];
				return mapTile ?? MapTile.Empty;
			}
			set
			{
				this._tiles[y, x] = value;
			}
		}

		public Point GetCollisionIndex(int x, int y)
		{
			Point result = Point.Zero;
			if (this._collisionCoordinates.GetLength(0) > y && this._collisionCoordinates.GetLength(1) > x)
			{
				result = this._collisionCoordinates[y, x];
			}
			return result;
		}

		public void SetCollisionIndex(int x, int y, Point value)
		{
			this._collisionCoordinates[y, x] = value;
		}

		public void Draw(SpriteBatch sbatch, Texture2D texture, Rectangle roomRectInPixels, float layer)
		{
			for (int i = 0; i < this.DynamicTiles.Count; i++)
			{
				CompressedTileInfo compressedTileInfo = this.DynamicTiles[i];
				VRam.Instance.ImportImageData(compressedTileInfo.Rect, compressedTileInfo.Data);
			}
			for (int j = 0; j < this.Dimensions.Y; j++)
			{
				int num = j * Constants.TileHeightInPixels;
				for (int k = 0; k < this.Dimensions.X; k++)
				{
					int num2 = k * Constants.TileWidthInPixels;
					MapTile mapTile = this[k, j];
					this._destinationRect.X = roomRectInPixels.X + num2;
					this._destinationRect.Y = roomRectInPixels.Y + num;
					for (int l = 0; l < mapTile.DrawingRects.Count<Rectangle>(); l++)
					{
						Rectangle value = mapTile.DrawingRects[l];
						sbatch.Draw(texture, this._destinationRect, new Rectangle?(value), mapTile.ClutAsColor, 0f, Vector2.Zero, SpriteEffects.None, layer);
					}
				}
			}
		}

		public byte Flags;

		public byte LayerLBA;

		public byte Topmost;

		public short DrawFlags;

		private readonly MapTile[,] _tiles;

		private readonly Point[,] _collisionCoordinates;

		private Rectangle _destinationRect = new Rectangle(0, 0, Constants.TileWidthInPixels, Constants.TileHeightInPixels);
	}
}
