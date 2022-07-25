using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Interop;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SotNEditor.HUD;
using SotNMap;
using WpfXnaRenderControl;

namespace SotNEditor
{
	public class XnaRenderControl : XnaRenderControlBase, INotifyPropertyChanged
	{
		public Matrix InvertedFinalMatrix
		{
			get
			{
				Matrix result = Matrix.Identity;
				MapZone loadedMap = MainWindow.LoadedMap;
				if (loadedMap != null)
				{
					result = loadedMap.RoomFilter.InvertedMatrix;
				}
				return result;
			}
		}

		public Microsoft.Xna.Framework.Color FlushColor
		{
			get
			{
				return this._flushColor;
			}
			set
			{
				this._flushColor = value;
			}
		}

		public void SetDrawOption(MapDrawOptions drawOption, bool value)
		{
			if (value)
			{
				this._drawOptions |= drawOption;
				return;
			}
			this._drawOptions &= ~drawOption;
		}

		public void ResetView()
		{
			this._zoom = 1f;
			this._totalDrawOffsetX = 0f;
			this._totalDrawOffsetY = 0f;
			this.GenerateViewMatrix();
		}

		public XnaRenderControl(WindowsFormsHost parent)
		{
			this._host = parent;
			parent.Child = this;
			base.MouseEnter += this.XnaRenderControl_MouseEnter;
			base.MouseMove += this.XnaRenderControl_MouseMove;
			base.MouseDown += this.XnaRenderControl_MouseDown;
			base.MouseUp += this.XnaRenderControl_MouseUp;
			base.MouseWheel += this.XnaRenderControl_MouseWheel;
			base.ClientSizeChanged += this.XnaRenderControl_ClientSizeChanged;
			base.KeyPress += this.XnaRenderControl_KeyPress;
			this.GenerateViewMatrix();
		}

		protected override void Initialize()
		{
			this._sBatch = new SpriteBatch(base.GraphicsDevice);
			this._feedback = new Feedback();
			this._hilitePanels = new HilitePanels();
		}

		public void LoadContent(ContentManager contentMgr)
		{
			this._pixel = contentMgr.Load<Texture2D>(@"pixel");
			this._entity = contentMgr.Load<Texture2D>(@"entity");
			this._paletteDrawingEffect = contentMgr.Load<Effect>(@"drawtile");
			this._clutTextureEffectParameter = this._paletteDrawingEffect.Parameters["ClutTexture"];
			this._feedback.LoadContent(contentMgr);
			this._hilitePanels.LoadContent(contentMgr);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged(string property)
		{
			PropertyChangedEventHandler propertyChanged;
			lock (this)
			{
				propertyChanged = this.PropertyChanged;
			}
			if (propertyChanged != null)
			{
				propertyChanged(this, new PropertyChangedEventArgs(property));
			}
		}

		private void XnaRenderControl_ClientSizeChanged(object sender, EventArgs e)
		{
			this._feedback.ClientDimensions.X = base.ClientRectangle.Width;
			this._feedback.ClientDimensions.Y = base.ClientRectangle.Height;
			this._hilitePanels.ClientDimensions.X = base.ClientRectangle.Width;
			this._hilitePanels.ClientDimensions.Y = base.ClientRectangle.Height;
			this._screenDims = new Vector2((float)base.Width, (float)base.Height);
			this._dirty = true;
			this.GenerateViewMatrix();
		}

		private void XnaRenderControl_KeyPress(object sender, KeyPressEventArgs e)
		{
			char keyChar = e.KeyChar;
			if (keyChar != ' ')
			{
				return;
			}
			MapZone loadedMap = MainWindow.LoadedMap;
			if (loadedMap != null)
			{
				IList<MapRoom> list = loadedMap.RoomFilter.FilterRoomsByMousePos(loadedMap.Rooms);
				if (list.Count > 1)
				{
					if (this._drawThisRoomIndexLast != null)
					{
						int num = list.IndexOf(loadedMap.Rooms[this._drawThisRoomIndexLast.Value]);
						num = (num + 1) % list.Count;
						this._drawThisRoomIndexLast = new int?(loadedMap.Rooms.IndexOf(list[num]));
					}
					else
					{
						this._drawThisRoomIndexLast = new int?(loadedMap.Rooms.IndexOf(list[0]));
					}
					this._dirty = true;
				}
			}
		}

		private void XnaRenderControl_MouseEnter(object sender, EventArgs e)
		{
			((IKeyboardInputSink)this._host).TabInto(new TraversalRequest(FocusNavigationDirection.First));
		}

		private void XnaRenderControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			System.Drawing.Point point = base.PointToClient(Control.MousePosition);
			this._localMousePos = new Vector2((float)point.X, (float)point.Y);
			this._dirty = true;
		}

		private void XnaRenderControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				this._mouseIsDown = true;
				this._mouseDownPoint = base.PointToClient(Control.MousePosition);
				this._mouseDrawOffsetX = 0f;
				this._mouseDrawOffsetY = 0f;
				this.GenerateViewMatrix();
			}
		}

		private void XnaRenderControl_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				this._mouseIsDown = false;
				this._totalDrawOffsetX += this._mouseDrawOffsetX;
				this._totalDrawOffsetY += this._mouseDrawOffsetY;
				this._mouseDrawOffsetX = 0f;
				this._mouseDrawOffsetY = 0f;
				this.GenerateViewMatrix();
			}
		}

		private void XnaRenderControl_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			float zoom = this._zoom;
			if (e.Delta > 0)
			{
				this._zoom += 1f;
			}
			else
			{
				this._zoom -= 1f;
			}
			if (this._zoom > 8f)
			{
				this._zoom = 8f;
			}
			else if (this._zoom < 1f)
			{
				this._zoom = 1f;
			}
			if (zoom != this._zoom)
			{
				this._feedback.AddFeedback("Zoom: " + this._zoom, 0.75m, FeedbackType.Zoom);
				this.GenerateViewMatrix();
			}
		}

		private void GenerateViewMatrix()
		{
			Matrix matrix = Matrix.CreateTranslation(-this._totalDrawOffsetX - this._mouseDrawOffsetX, -this._totalDrawOffsetY - this._mouseDrawOffsetY, 0f);
			Matrix matrix2 = Matrix.CreateTranslation((float)(-(float)Math.Floor((double)((float)base.ClientRectangle.Width / 2f))), (float)(-(float)Math.Floor((double)((float)base.ClientRectangle.Height / 2f))), 0f);
			Matrix matrix3 = Matrix.CreateTranslation((float)Math.Floor((double)((float)base.ClientRectangle.Width / 2f)), (float)Math.Floor((double)((float)base.ClientRectangle.Height / 2f)), 0f);
			Matrix matrix4 = Matrix.CreateScale(this._zoom, this._zoom, 1f);
			this._viewMatrix = matrix * matrix2 * matrix4 * matrix3;
			this._dirty = true;
		}

		protected override void Update()
		{
			if (this._mouseIsDown)
			{
				System.Drawing.Point point = base.PointToClient(Control.MousePosition);
				this._mouseDrawOffsetX = (float)Math.Floor((double)((float)(this._mouseDownPoint.X - point.X) / this._zoom));
				this._mouseDrawOffsetY = (float)Math.Floor((double)((float)(this._mouseDownPoint.Y - point.Y) / this._zoom));
				this.GenerateViewMatrix();
			}
			MapZone loadedMap = MainWindow.LoadedMap;
			if (loadedMap != null)
			{
				if (this._dirty)
				{
					loadedMap.RoomFilter.RunQuery(ref this._viewMatrix, this._screenDims, this._localMousePos, this._drawThisRoomIndexLast);
					this.NotifyPropertyChanged("InvertedFinalMatrix");
					this._dirty = false;
				}
				else
				{
					loadedMap.RoomFilter.Reset();
				}
				IList<MapRoom> source = loadedMap.RoomFilter.FilterRoomsByMousePos(loadedMap.Rooms);
				if (!source.Any<MapRoom>())
				{
					this._drawThisRoomIndexLast = null;
				}
			}
			this._feedback.Update();
		}

		protected override void Draw()
		{
			base.GraphicsDevice.Clear(this.FlushColor);
			MapZone loadedMap = MainWindow.LoadedMap;
			if (loadedMap != null)
			{
				loadedMap.Draw(this._sBatch, ref this._viewMatrix, this._paletteDrawingEffect, this._clutTextureEffectParameter, this._drawOptions, this._pixel, this.FlushColor);
				this._sBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, this._viewMatrix);
				IList<MapRoom> list = loadedMap.RoomFilter.FilterRoomsByScreenRect(loadedMap.Rooms);
				for (int i = 0; i < list.Count; i++)
				{
					MapRoom mapRoom = list[i];
					for (int j = 0; j < mapRoom.Entities.Count; j++)
					{
						MapEntity mapEntity = mapRoom.Entities[j];
						if (mapEntity.RoomPosition.X != -1 || mapEntity.RoomPosition.Y != -1)
						{
							Microsoft.Xna.Framework.Rectangle destinationRectangle = new Microsoft.Xna.Framework.Rectangle(mapRoom.RoomRectInPixels.X + mapEntity.RoomPosition.X, mapRoom.RoomRectInPixels.Y + mapEntity.RoomPosition.Y, 16, 16);
							this._sBatch.Draw(this._entity, destinationRectangle, new Microsoft.Xna.Framework.Rectangle?(this._entSrcRect), Microsoft.Xna.Framework.Color.White, 0f, this._entOffset, SpriteEffects.None, 0.5f);
						}
					}
				}
				this._sBatch.End();

				this._sBatch.Begin();
				this._hilitePanels.DrawHilitesForRooms(this._sBatch, loadedMap.Rooms, ref this._viewMatrix);
				this._hilitePanels.DrawHilitesForEntities(this._sBatch, loadedMap.Rooms, ref this._viewMatrix);
				this._sBatch.End();
			}
			this._feedback.Draw(this._sBatch);
		}

		public void Feedback(string text, decimal timeMultiplier = 1m, FeedbackType type = FeedbackType.Normal)
		{
			this._feedback.AddFeedback(text, timeMultiplier, type);
		}

		public static readonly Microsoft.Xna.Framework.Color EditorColor = Microsoft.Xna.Framework.Color.LightGray;

		public static readonly Microsoft.Xna.Framework.Color GameColor = Microsoft.Xna.Framework.Color.Black;

		// Transparency for PNG export
		public static readonly Microsoft.Xna.Framework.Color TransparentColor = new Microsoft.Xna.Framework.Color(255, 255, 255, 0);

		private WindowsFormsHost _host;

		private Microsoft.Xna.Framework.Color _flushColor = XnaRenderControl.EditorColor;

		public MapDrawOptions _drawOptions = MapDrawOptions.All;

		private float _zoom = 1f;

		private float _totalDrawOffsetX;

		private float _totalDrawOffsetY;

		private float _mouseDrawOffsetX;

		private float _mouseDrawOffsetY;

		private System.Drawing.Point _mouseDownPoint = new System.Drawing.Point(-1, -1);

		private bool _mouseIsDown;

		public SpriteBatch _sBatch;

		public Texture2D _pixel;

		private Texture2D _entity;

		public Effect _paletteDrawingEffect;

		public EffectParameter _clutTextureEffectParameter;

		private Feedback _feedback;

		private HilitePanels _hilitePanels;

		private Vector2 _screenDims;

		private Vector2 _localMousePos;

		private int? _drawThisRoomIndexLast;

		public Matrix _viewMatrix = Matrix.Identity;

		private bool _dirty;

		private readonly Microsoft.Xna.Framework.Rectangle _entSrcRect = new Microsoft.Xna.Framework.Rectangle(0, 0, 16, 16);

		private readonly Vector2 _entOffset = new Vector2(8f, 8f);
	}
}
