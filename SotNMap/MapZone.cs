using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PSX;

namespace SotNMap
{
	public class MapZone
	{
		public Rectangle ZoneRect { get; private set; }

		public List<MapRoom> Rooms
		{
			get
			{
				return _rooms;
			}
		}

		public MapRoomFilter RoomFilter
		{
			get
			{
				return _roomFilter;
			}
			set
			{
				_roomFilter = value;
			}
		}

		public MapZone(Rectangle zoneRect)
		{
			ZoneRect = zoneRect;
		}

		public void Draw(SpriteBatch sbatch, ref Matrix viewMatrix, Effect paletteDrawingEffect, EffectParameter clutTextureEffectParam, MapDrawOptions drawOptions, Texture2D pixel, Color flushColor)
		{
			Texture2D ramTexture = VRam.Instance.RamTexture;
			Texture2D collisionTexture = VRam.Instance.CollisionTexture;
			bool flag = (drawOptions & MapDrawOptions.DrawFront) != MapDrawOptions.None;
			bool flag2 = (drawOptions & MapDrawOptions.DrawBack) != MapDrawOptions.None;
			bool flag3 = (drawOptions & MapDrawOptions.DrawCollisionFront) != MapDrawOptions.None;
			clutTextureEffectParam.SetValue(VRam.Instance.ClutTexture);
			IList<MapRoom> list = this.RoomFilter.FilterRoomsByScreenRect(this._rooms);

			// Process backwards to fix a few rendering bugs in NZ1 due to overlapping rooms
			for (int i = list.Count - 1; i >= 0; i--)
			{
				MapRoom mapRoom = list[i];
				sbatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, viewMatrix);
				sbatch.Draw(pixel, mapRoom.RoomRectInPixels, null, flushColor, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
				sbatch.End();
				if ((flag && mapRoom.ForegroundLayer != null) || (flag2 && mapRoom.BackgroundLayer != null))
				{
					sbatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, paletteDrawingEffect, viewMatrix);
					paletteDrawingEffect.CurrentTechnique.Passes[0].Apply();
					mapRoom.DrawRoom(sbatch, paletteDrawingEffect, viewMatrix, flag, flag2, ramTexture, pixel, flushColor);
					sbatch.End();
				}
				if (flag3 && mapRoom.CollisionLayer != null)
				{
					sbatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, viewMatrix);
					mapRoom.DrawCollision(sbatch, collisionTexture);
					sbatch.End();
				}
			}
		}

		private readonly List<MapRoom> _rooms = new List<MapRoom>();

		private MapRoomFilter _roomFilter = new MapRoomFilter();
	}
}
