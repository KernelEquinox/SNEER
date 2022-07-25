using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Xna.Framework;
using SotNMap;

namespace SotNEditor
{
	internal class MinimapControl : Canvas
	{
		public Brush Foreground
		{
			get
			{
				return (Brush)base.GetValue(MinimapControl.ForegroundProperty);
			}
			set
			{
				base.SetValue(MinimapControl.ForegroundProperty, value);
			}
		}

		public Brush Hilite
		{
			get
			{
				return (Brush)base.GetValue(MinimapControl.HiliteProperty);
			}
			set
			{
				base.SetValue(MinimapControl.HiliteProperty, value);
			}
		}

		public MinimapControl()
		{
			base.ClipToBounds = true;
		}

		protected override void OnRender(DrawingContext dc)
		{
			base.OnRender(dc);
			if (!(base.DataContext is MapZone))
			{
				return;
			}
			MapZone mapZone = base.DataContext as MapZone;
			Brush brush = this.Foreground.Clone();
			brush.Opacity = 0.5;
			float num = (float)mapZone.ZoneRect.Height / (float)mapZone.ZoneRect.Width;
			base.Height = (double)num * base.ActualWidth;
			double num2 = base.ActualWidth / (double)(mapZone.ZoneRect.Width * Constants.RoomWidthInPixels);
			double num3 = base.ActualHeight / (double)(mapZone.ZoneRect.Height * Constants.RoomHeightInPixels);
			dc.DrawRectangle(base.Background, null, new Rect(0.0, 0.0, base.ActualWidth, base.ActualHeight));
			for (int i = 0; i < mapZone.Rooms.Count; i++)
			{
				MapRoom mapRoom = mapZone.Rooms[i];
				Rect rectangle = new Rect((double)mapRoom.RoomRectInPixels.X * num2, (double)mapRoom.RoomRectInPixels.Y * num3, (double)mapRoom.RoomRectInPixels.Width * num2, (double)mapRoom.RoomRectInPixels.Height * num3);
				dc.DrawRectangle(brush, null, rectangle);
				rectangle.Inflate(-1.0, -1.0);
				dc.DrawRectangle(this.Foreground, null, rectangle);
			}
			Vector2 vector = Vector2.Transform(Vector2.Zero, this.InvertedFinalMatrix);
			Vector2 vector2 = Vector2.Transform(this.ScreenDims, this.InvertedFinalMatrix);
			dc.DrawRectangle(this.Hilite, null, new Rect((double)vector.X * num2, (double)vector.Y * num3, (double)(vector2.X - vector.X) * num2, (double)(vector2.Y - vector.Y) * num3));
		}

		public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register("Foreground", typeof(Brush), typeof(MinimapControl), new UIPropertyMetadata(Brushes.Black));

		public static readonly DependencyProperty HiliteProperty = DependencyProperty.Register("Hilite", typeof(Brush), typeof(MinimapControl), new UIPropertyMetadata(Brushes.Blue));

		public Microsoft.Xna.Framework.Matrix InvertedFinalMatrix;

		public Vector2 ScreenDims;
	}
}
