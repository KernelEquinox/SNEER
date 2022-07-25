using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WpfXnaRenderControl
{
	// Token: 0x02000004 RID: 4
	public abstract class XnaRenderControlBase : Control
	{
		// Token: 0x06000008 RID: 8 RVA: 0x000020CE File Offset: 0x000002CE
		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000009 RID: 9 RVA: 0x000020D0 File Offset: 0x000002D0
		public ServiceContainer Services
		{
			get
			{
				return this._Services;
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000A RID: 10 RVA: 0x000020D8 File Offset: 0x000002D8
		public GraphicsDevice GraphicsDevice
		{
			get
			{
				if (this._GraphicsDeviceService == null)
				{
					return null;
				}
				return this._GraphicsDeviceService.GraphicsDevice;
			}
		}

		// Token: 0x0600000B RID: 11
		protected abstract void Initialize();

		// Token: 0x0600000C RID: 12
		protected new abstract void Update();

		// Token: 0x0600000D RID: 13
		protected abstract void Draw();

		// Token: 0x0600000E RID: 14 RVA: 0x000020F0 File Offset: 0x000002F0
		protected override void OnCreateControl()
		{
			if (!base.DesignMode)
			{
				this._GraphicsDeviceService = XnaGraphicsDeviceService.AddRef(base.Handle, base.ClientSize.Width, base.ClientSize.Height);
				this._Services.AddService<IGraphicsDeviceService>(this._GraphicsDeviceService);
				Mouse.WindowHandle = base.Handle;
				this.Initialize();
			}
			base.OnCreateControl();
		}

		// Token: 0x0600000F RID: 15 RVA: 0x0000215A File Offset: 0x0000035A
		protected override void Dispose(bool disposing)
		{
			if (this._GraphicsDeviceService != null)
			{
				this._GraphicsDeviceService.Release(disposing);
				this._GraphicsDeviceService = null;
			}
			base.Dispose(disposing);
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002180 File Offset: 0x00000380
		protected override void OnPaint(PaintEventArgs pe)
		{
			this.Update();
			string text = this.BeginDraw();
			if (string.IsNullOrEmpty(text))
			{
				this.Draw();
				this.EndDraw();
			}
			else
			{
				this.PaintUsingSystemDrawing(pe.Graphics, text);
			}
			base.OnPaint(pe);
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000021C4 File Offset: 0x000003C4
		private string BeginDraw()
		{
			if (this._GraphicsDeviceService == null)
			{
				return this.Text + "\n\n" + base.GetType();
			}
			string text = this.HandleDeviceReset();
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			Viewport viewport = new Viewport
			{
				X = 0,
				Y = 0,
				Width = base.ClientSize.Width,
				Height = base.ClientSize.Height,
				MinDepth = 0f,
				MaxDepth = 1f
			};
			this.GraphicsDevice.Viewport = viewport;
			return null;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x0000226C File Offset: 0x0000046C
		private void EndDraw()
		{
			try
			{
				Microsoft.Xna.Framework.Rectangle value = new Microsoft.Xna.Framework.Rectangle(0, 0, base.ClientSize.Width, base.ClientSize.Height);
				this.GraphicsDevice.Present(new Microsoft.Xna.Framework.Rectangle?(value), null, base.Handle);
			}
			catch
			{
			}
		}

		// Token: 0x06000013 RID: 19 RVA: 0x000022D4 File Offset: 0x000004D4
		private string HandleDeviceReset()
		{
			bool flag;
			switch (this.GraphicsDevice.GraphicsDeviceStatus)
			{
			case GraphicsDeviceStatus.Lost:
				return "Graphics device lost";
			case GraphicsDeviceStatus.NotReset:
				flag = true;
				break;
			default:
			{
				PresentationParameters presentationParameters = this.GraphicsDevice.PresentationParameters;
				flag = (base.ClientSize.Width > presentationParameters.BackBufferWidth || base.ClientSize.Height > presentationParameters.BackBufferHeight);
				break;
			}
			}
			if (flag)
			{
				try
				{
					this._GraphicsDeviceService.ResetDevice(base.ClientSize.Width, base.ClientSize.Height);
				}
				catch (Exception arg)
				{
					return "Graphics device reset failed\n\n" + arg;
				}
			}
			return null;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x0000239C File Offset: 0x0000059C
		protected virtual void PaintUsingSystemDrawing(Graphics graphics, string text)
		{
			graphics.Clear(System.Drawing.Color.CornflowerBlue);
			using (Brush brush = new SolidBrush(System.Drawing.Color.Black))
			{
				using (StringFormat stringFormat = new StringFormat())
				{
					stringFormat.Alignment = StringAlignment.Center;
					stringFormat.LineAlignment = StringAlignment.Center;
					graphics.DrawString(text, this.Font, brush, base.ClientRectangle, stringFormat);
				}
			}
		}

		// Token: 0x04000004 RID: 4
		private ServiceContainer _Services = new ServiceContainer();

		// Token: 0x04000005 RID: 5
		private XnaGraphicsDeviceService _GraphicsDeviceService;
	}
}
