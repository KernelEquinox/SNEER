using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PSX;

namespace SotNMap
{
	public class MapTile
	{
		public Color ClutAsColor { get; set; }

		public IList<Rectangle> DrawingRects
		{
			get
			{
				return this._drawingRects;
			}
		}

		public static MapTile Empty
		{
			get
			{
				return MapTile.MapTileEmpty;
			}
		}

		public VRamPoint TilePos
		{
			get
			{
				return this._tilePos;
			}
			set
			{
				this._tilePos = value;
				this._drawingRects = VRam.Instance.GetDrawingRectsFor(value.X, value.Y, Constants.TileWidthInPixels, Constants.TileHeightInPixels);
			}
		}

		private IList<Rectangle> _drawingRects = new List<Rectangle>();

		private static readonly MapTile MapTileEmpty = new MapTile
		{
			ClutAsColor = Color.White,
			TilePos = VRamPoint.Zero
		};

		private VRamPoint _tilePos = VRamPoint.Zero;
	}
}
