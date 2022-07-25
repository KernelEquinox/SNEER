using System;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;

namespace WpfXnaRenderControl
{
	// Token: 0x02000005 RID: 5
	public class XnaGraphicsDeviceService : IGraphicsDeviceService
	{
		// Token: 0x06000016 RID: 22 RVA: 0x00002438 File Offset: 0x00000638
		private XnaGraphicsDeviceService(IntPtr windowHandle, int width, int height)
		{
			this._parameters = new PresentationParameters
			{
				BackBufferWidth = Math.Max(width, 1),
				BackBufferHeight = Math.Max(height, 1),
				BackBufferFormat = SurfaceFormat.Color,
				DepthStencilFormat = DepthFormat.Depth24,
				DeviceWindowHandle = windowHandle,
				PresentationInterval = PresentInterval.Immediate,
				IsFullScreen = false
			};
			this._GraphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, GraphicsProfile.Reach, this._parameters);
		}

		// Token: 0x06000017 RID: 23 RVA: 0x000024AC File Offset: 0x000006AC
		public static XnaGraphicsDeviceService AddRef(IntPtr windowHandle, int width, int height)
		{
			if (Interlocked.Increment(ref XnaGraphicsDeviceService._referenceCount) == 1)
			{
				XnaGraphicsDeviceService._singletonInstance = new XnaGraphicsDeviceService(windowHandle, width, height);
			}
			return XnaGraphicsDeviceService._singletonInstance;
		}

		// Token: 0x06000018 RID: 24 RVA: 0x000024CD File Offset: 0x000006CD
		public void Release(bool disposing)
		{
			if (Interlocked.Decrement(ref XnaGraphicsDeviceService._referenceCount) == 0)
			{
				if (disposing)
				{
					if (this.DeviceDisposing != null)
					{
						this.DeviceDisposing(this, EventArgs.Empty);
					}
					this._GraphicsDevice.Dispose();
				}
				this._GraphicsDevice = null;
			}
		}

		// Token: 0x06000019 RID: 25 RVA: 0x0000250C File Offset: 0x0000070C
		public void ResetDevice(int width, int height)
		{
			if (this.DeviceResetting != null)
			{
				this.DeviceResetting(this, EventArgs.Empty);
			}
			this._parameters.BackBufferWidth = Math.Max(this._parameters.BackBufferWidth, width);
			this._parameters.BackBufferHeight = Math.Max(this._parameters.BackBufferHeight, height);
			this._GraphicsDevice.Reset(this._parameters);
			if (this.DeviceReset != null)
			{
				this.DeviceReset(this, EventArgs.Empty);
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600001A RID: 26 RVA: 0x00002594 File Offset: 0x00000794
		public GraphicsDevice GraphicsDevice
		{
			get
			{
				return this._GraphicsDevice;
			}
		}

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x0600001B RID: 27 RVA: 0x0000259C File Offset: 0x0000079C
		// (remove) Token: 0x0600001C RID: 28 RVA: 0x000025D4 File Offset: 0x000007D4
		public event EventHandler<EventArgs> DeviceCreated;

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x0600001D RID: 29 RVA: 0x0000260C File Offset: 0x0000080C
		// (remove) Token: 0x0600001E RID: 30 RVA: 0x00002644 File Offset: 0x00000844
		public event EventHandler<EventArgs> DeviceDisposing;

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x0600001F RID: 31 RVA: 0x0000267C File Offset: 0x0000087C
		// (remove) Token: 0x06000020 RID: 32 RVA: 0x000026B4 File Offset: 0x000008B4
		public event EventHandler<EventArgs> DeviceReset;

		// Token: 0x14000004 RID: 4
		// (add) Token: 0x06000021 RID: 33 RVA: 0x000026EC File Offset: 0x000008EC
		// (remove) Token: 0x06000022 RID: 34 RVA: 0x00002724 File Offset: 0x00000924
		public event EventHandler<EventArgs> DeviceResetting;

		// Token: 0x04000006 RID: 6
		private static XnaGraphicsDeviceService _singletonInstance;

		// Token: 0x04000007 RID: 7
		private static int _referenceCount;

		// Token: 0x04000008 RID: 8
		private GraphicsDevice _GraphicsDevice;

		// Token: 0x04000009 RID: 9
		private PresentationParameters _parameters;
	}
}
