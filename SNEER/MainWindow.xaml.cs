using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using Microsoft.Win32;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PSX;
using SotN;
using SotNEditor.HUD;
using SotNEditor.Logging;
using SotNMap;
using Matrix = Microsoft.Xna.Framework.Matrix;

namespace SotNEditor
{
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public partial class MainWindow : Window
	{
		public static MapZone LoadedMap { get; private set; }
		private ConfigValues _config;
		private ContentManager _contentMgr;
		private XnaRenderControl _renderControl;
		private VRamViewer _vRamViewer;

		// String for saving the map as a PNG file
		private string _mapName;
		private string _dirName;

		public MainWindow()
		{
			this.InitializeComponent();
			base.Title = "SotN Editor v0.15b";
			this._renderControl = new XnaRenderControl(this.controlHost);
			this._renderControl.ClientSizeChanged += this._renderControl_ClientSizeChanged;
			this._renderControl.PropertyChanged += this._renderControl_PropertyChanged;
			this._renderControl.SetDrawOption(MapDrawOptions.DrawCollisionFront, false);
			CompositionTarget.Rendering += this.CompositionTarget_Rendering;
			ToolTipService.SetInitialShowDelay(this.StatusBar, 0);
			ToolTipService.SetShowDuration(this.StatusBar, int.MaxValue);
		}

		private void MainWindow_Loaded(object sender, EventArgs e)
		{
			Logger.Instance.Init();
			if (this._renderControl.GraphicsDevice == null)
			{
				Logger.Instance.Output("Error creating GraphicsDevice", LogItemType.Error);
			}
			else
			{
				VRam.InitInstance(this._renderControl.GraphicsDevice);
			}
			this._contentMgr = new ContentManager(this._renderControl.Services);
			this._renderControl.LoadContent(this._contentMgr);
			VRam.Instance.LoadContent(this._contentMgr);
			try
			{
				this._config = ConfigFile.ReadConfig("config.xml");
			}
			catch (FileNotFoundException)
			{
				this._config = new ConfigValues();
			}
			catch (Exception ex)
			{
				this._config = new ConfigValues();
				MessageBox.Show(ex.Message + "\nConfig file will be replaced.", "Error loading config");
			}
			if (string.IsNullOrEmpty(this._config.Dra_BinPath))
			{
				MessageBox.Show("Please find the location of your DRA.BIN file.", "Generating Config File...");
				this.CommandBinding_SetDraBinPath_Executed(this, null);
			}
			if (string.IsNullOrEmpty(this._config.F_Game_BinPath))
			{
				MessageBox.Show("Please find the location of your F_GAME.BIN file.", "Generating Config File...");
				this.CommandBinding_SetFGameBinPath_Executed(this, null);
			}
			Microsoft.Xna.Framework.Color color = new Microsoft.Xna.Framework.Color
			{
				PackedValue = this._config.FlushColor
			};

			if (color == XnaRenderControl.EditorColor)
            {
				rdoBGEditor.IsChecked = true;
            }
			else if (color == XnaRenderControl.GameColor)
			{
				rdoBGGame.IsChecked = true;
			}
			else
            {
				rdoBGTransparent.IsChecked = true;
				color = XnaRenderControl.TransparentColor;
            }
			_renderControl.FlushColor = color;
			Logger.Instance.Output("SNEER Initialized", LogItemType.Info);
		}

		private void MainWindow_Closing(object sender, CancelEventArgs e)
		{
			if (!ConfigFile.WriteConfig("config.xml", this._config) && MessageBox.Show("There was an error writing the config file, are you sure you want to close SNEER?", "Warning", MessageBoxButton.YesNo) == MessageBoxResult.No)
			{
				e.Cancel = true;
			}
		}

		private void MainWindow_Closed(object sender, EventArgs e)
		{
			this.CleanupOpenObjects();
			Logger.Instance.Deinit();
		}

		private void CompositionTarget_Rendering(object sender, EventArgs e)
		{
			this._renderControl.Invalidate();
		}

		private void _renderControl_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (!(sender is XnaRenderControl))
			{
				return;
			}
			if (e.PropertyName == "InvertedFinalMatrix")
			{
				this.minimap.InvertedFinalMatrix = MainWindow.LoadedMap.RoomFilter.InvertedMatrix;
				this.minimap.InvalidateVisual();
			}
		}

		private void _renderControl_ClientSizeChanged(object sender, EventArgs e)
		{
			if (!(sender is XnaRenderControl))
			{
				return;
			}
			XnaRenderControl xnaRenderControl = sender as XnaRenderControl;
			this.minimap.ScreenDims = new Vector2((float)xnaRenderControl.ClientSize.Width, (float)xnaRenderControl.ClientSize.Height);
		}

		private void LoadGenericData()
		{
			if (!string.IsNullOrEmpty(this._config.F_Game_BinPath))
			{
				Logger.Instance.Output("Loading Generic Tiles", LogItemType.Info);
				IList<string> list = TileReader.LoadGenericTilesToVRam(this._config.F_Game_BinPath);
				if (list.Count != 0)
				{
					foreach (string text in list)
					{
						Logger.Instance.Output(text, LogItemType.Error);
					}
				}
			}
			Logger.Instance.Output("Generating Generic CLUTs", LogItemType.Info);
			VRam.Instance.GenerateGenericCluts();
			if (!string.IsNullOrEmpty(this._config.Dra_BinPath))
			{
				Logger.Instance.Output("Reading data from DRA.BIN", LogItemType.Info);
				DraBin.Instance.Init(this._config.Dra_BinPath);
				return;
			}
			Logger.Instance.Output("Didn't read data from DRA.BIN, path not configured", LogItemType.Warning);
		}

		private void CleanupOpenObjects()
		{
			Logger.Instance.Output("Cleaning Open Objects", LogItemType.Info);
			this.minimap.DataContext = null;
			MainWindow.LoadedMap = null;
			if (this._vRamViewer != null)
			{
				this._vRamViewer.Close();
			}
			VRam.Instance.Clear();
			DraBin.Instance.Deinit();
		}

		private void CommandBinding_Open_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Filter = "SotN Map Files|*.bin"
			};
			if (openFileDialog.ShowDialog().Value)
			{
				try
				{
					this.CleanupOpenObjects();
					this.LoadGenericData();
					_dirName = Path.GetDirectoryName(openFileDialog.FileName) + "\\";
					string fileName = Path.GetFileName(openFileDialog.FileName);
					_mapName = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
					string tileFile;
					string layoutFile;
					if (fileName.StartsWith("F_"))
					{
						tileFile = _dirName + fileName;
						this._mapName = this._mapName.Substring(2, this._mapName.Length - 2);
						layoutFile = _dirName + this._mapName;
					}
					else
					{
						tileFile = _dirName + "F_" + fileName;
						layoutFile = _dirName + fileName;
					}
					Logger.Instance.Output("Selected tile file: \"" + tileFile + "\"", LogItemType.Info);
					Logger.Instance.Output("Selected layout file: \"" + layoutFile + "\"", LogItemType.Info);
					Logger.Instance.Output("Loading map tiles", LogItemType.Info);
					IList<string> list = TileReader.LoadMapTilesToVRam(tileFile);
					if (list.Count != 0)
					{
						foreach (string text3 in list)
						{
							Logger.Instance.Output(text3, LogItemType.Error);
						}
						this.CleanupOpenObjects();
					}
					else
					{
						Logger.Instance.Output("Generating map CLUTs", LogItemType.Info);
						VRam.Instance.GenerateMapCluts();
						Logger.Instance.Output("Loading map layout", LogItemType.Info);
						MainWindow.LoadedMap = LayoutReader.LoadMapLayout(layoutFile);
						if (MainWindow.LoadedMap == null)
						{
							this._renderControl.Feedback("Error loading map zone", 2m, FeedbackType.Normal);
							Logger.Instance.Output("Error loading map zone", LogItemType.Error);
							this.CleanupOpenObjects();
						}
						else
						{
							this._renderControl.Feedback("Map zone loaded", 2m, FeedbackType.Normal);
							Logger.Instance.Output(string.Concat(new object[]
							{
								"Rect: ",
								MainWindow.LoadedMap.ZoneRect.X,
								", ",
								MainWindow.LoadedMap.ZoneRect.Y,
								", ",
								MainWindow.LoadedMap.ZoneRect.Width,
								", ",
								MainWindow.LoadedMap.ZoneRect.Height
							}), LogItemType.Info);
							Logger.Instance.Output("Rooms: " + MainWindow.LoadedMap.Rooms.Count, LogItemType.Info);
							Logger.Instance.Output("Map zone loaded", LogItemType.Info);
							base.Title = "SotN Editor v0.15b - " + Path.GetFileNameWithoutExtension(layoutFile);
							this._renderControl.ResetView();
							this.minimap.DataContext = MainWindow.LoadedMap;
							this.minimap.InvalidateVisual();
						}
					}
					// Enable the "Export PNG" button
					btnExport.IsEnabled = true;
					fileMenuExport.IsEnabled = true;
				}
				catch (Exception ex)
				{
					Logger.Instance.Output("Exception loading map zone: " + ex.Message, LogItemType.Error);
					Logger.Instance.Output("Stack Trace:\n" + ex.StackTrace, LogItemType.Error);
					btnExport.IsEnabled = false;
					fileMenuExport.IsEnabled = false;
				}
			}
		}

		private void CommandBinding_VramPreview_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (this._vRamViewer != null)
			{
				this._vRamViewer.Close();
			}
			this._vRamViewer = new VRamViewer(VRam.Instance.RamTexture, VRam.Instance.ClutTexture);
			this._vRamViewer.Closed += this._vRamViewer_Closed;
			this._vRamViewer.Show();
		}

		private void _vRamViewer_Closed(object sender, EventArgs e)
		{
			Logger.Instance.Output("Shutting down", LogItemType.Info);
			this._vRamViewer = null;
		}

		private void CommandBinding_SetFGameBinPath_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Filter = "F_GAME.BIN|F_GAME.BIN"
			};
			if (openFileDialog.ShowDialog() ?? false)
			{
				Logger.Instance.Output("F_GAME.BIN path set to \"" + openFileDialog.FileName + "\"", LogItemType.Info);
				this._config.F_Game_BinPath = openFileDialog.FileName;
				return;
			}
			if (string.IsNullOrWhiteSpace(this._config.F_Game_BinPath))
			{
				Logger.Instance.Output("F_GAME.BIN path not set", LogItemType.Warning);
				MessageBox.Show("You must load the F_GAME.BIN file from the SotN ISO to correctly display save/load rooms!", "Warning");
			}
		}

		private void CommandBinding_SetDraBinPath_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Filter = "DRA.BIN|DRA.BIN"
			};
			if (openFileDialog.ShowDialog() ?? false)
			{
				Logger.Instance.Output("DRA.BIN path set to \"" + openFileDialog.FileName + "\"", LogItemType.Info);
				this._config.Dra_BinPath = openFileDialog.FileName;
				return;
			}
			if (string.IsNullOrWhiteSpace(this._config.Dra_BinPath))
			{
				Logger.Instance.Output("DRA.BIN path not set", LogItemType.Warning);
				MessageBox.Show("You must load the DRA.BIN file from the SotN ISO to correctly display save/load rooms!", "Warning");
			}
		}

		private void CommandBinding_SetLayer_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			CheckBox checkBox = e.OriginalSource as CheckBox;
			string a;
			if ((a = (string)e.Parameter) != null)
			{
				if (a == "Collision")
				{
					Logger.Instance.Output("MapDrawOptions.DrawCollisionFront set to " + checkBox.IsChecked.Value, LogItemType.Info);
					this._renderControl.SetDrawOption(MapDrawOptions.DrawCollisionFront, checkBox.IsChecked.Value);
					return;
				}
				if (a == "Front")
				{
					Logger.Instance.Output("MapDrawOptions.DrawFront set to " + checkBox.IsChecked.Value, LogItemType.Info);
					this._renderControl.SetDrawOption(MapDrawOptions.DrawFront, checkBox.IsChecked.Value);
					return;
				}
				if (!(a == "Back"))
				{
					return;
				}
				Logger.Instance.Output("MapDrawOptions.DrawBack set to " + checkBox.IsChecked.Value, LogItemType.Info);
				this._renderControl.SetDrawOption(MapDrawOptions.DrawBack, checkBox.IsChecked.Value);
			}
		}

		private void CommandBinding_SetBackgroundColor_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			string a;
			if ((a = (string)e.Parameter) != null)
			{
				if (a == "Game")
				{
					Logger.Instance.Output("FlushColor set to \"Game\"", LogItemType.Info);
					this._renderControl.FlushColor = XnaRenderControl.GameColor;
				}
				else if (a == "Editor")
				{
					Logger.Instance.Output("FlushColor set to \"Editor\"", LogItemType.Info);
					this._renderControl.FlushColor = XnaRenderControl.EditorColor;
				}
				else
                {
					Logger.Instance.Output("FlushColor set to \"Transparent\"", LogItemType.Info);
					this._renderControl.FlushColor = XnaRenderControl.TransparentColor;
				}
			}
			this._config.FlushColor = this._renderControl.FlushColor.PackedValue;
		}

		private void CommandBinding_ExportMapImage_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			// Don't do anything if no map was loaded
			if (LoadedMap == null)
            {
				return;
            }

			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				Filter = "Image Files|*.png",
				FileName = this._mapName + ".png"
			};

			if (saveFileDialog.ShowDialog() ?? false)
			{

				// Get the dimensions of the current map zone
				int mapWidth = LoadedMap.ZoneRect.Width * 256;
				int mapHeight = LoadedMap.ZoneRect.Height * 256;

				// Get the allowable dimensions of the texture
				int texWidth = Math.Min(mapWidth, 2048);
				int texHeight = Math.Min(mapHeight, 2048);

				// Get the number of textures that will need to be exported
				int chunkX = (int)Math.Ceiling((double)mapWidth / 2048);
				int chunkY = (int)Math.Ceiling((double)mapHeight / 2048);

				// Create a bitmap for the whole map
				using (Bitmap mapImage = new Bitmap(mapWidth, mapHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
				{
					// Loop through each chunk
					for (int curY = 0; curY < chunkY; curY++)
					{
						for (int curX = 0; curX < chunkX; curX++)
						{

							// Get the offsets
							int offsetX = curX * 2048;
							int offsetY = curY * 2048;

							// Create a render target to catch drawn textures
							RenderTarget2D renderTarget = new RenderTarget2D(_renderControl.GraphicsDevice, texWidth, texHeight, false, SurfaceFormat.Color, DepthFormat.Depth24);
							_renderControl.GraphicsDevice.SetRenderTarget(renderTarget);
							_renderControl.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Transparent);

							// Calculate view matrix to encompass current texture chunk
							Matrix matrix = Matrix.CreateTranslation(-offsetX, -offsetY, 0f);
							Matrix matrix2 = Matrix.CreateTranslation((float)(-(float)Math.Floor((double)((float)mapWidth / 2f))), (float)(-(float)Math.Floor((double)((float)mapHeight / 2f))), 0f);
							Matrix matrix3 = Matrix.CreateTranslation((float)Math.Floor((double)((float)mapWidth / 2f)), (float)Math.Floor((double)((float)mapHeight / 2f)), 0f);
							Matrix matrix4 = Matrix.CreateScale(1, 1, 1f);
							Matrix viewMatrix = matrix * matrix2 * matrix4 * matrix3;

							// Draw the map section onto the texture
							Console.WriteLine(_renderControl.FlushColor);
							LoadedMap.Draw(
								_renderControl._sBatch,
								ref viewMatrix,
								_renderControl._paletteDrawingEffect,
								_renderControl._clutTextureEffectParameter,
								_renderControl._drawOptions,
								_renderControl._pixel,
								_renderControl.FlushColor
							);

							// Clear the render target to begin referencing the texture
							_renderControl.GraphicsDevice.SetRenderTarget(null);

							// Create a new bitmap for the texture in memory
							using (Bitmap bmp = new Bitmap(texWidth, texHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
							{
								BitmapData bmpData;
								System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, texWidth, texHeight);
								byte[] texBytes = new byte[4 * texWidth * texHeight];

								// Fetch the texture data
								renderTarget.GetData(texBytes);

								// RGB <=> BGR
								for (int i = 0; i < texBytes.Length; i += 4)
								{
									byte tmp = texBytes[i];
									texBytes[i] = texBytes[i + 2];
									texBytes[i + 2] = tmp;
								}

								// Copy texture data to the bitmap
								bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
								Marshal.Copy(texBytes, 0, bmpData.Scan0, texBytes.Length);
								bmp.UnlockBits(bmpData);

								// Write the bitmap to the map image bitmap
								using (Graphics g = Graphics.FromImage(mapImage))
								{
									g.DrawImage(bmp, offsetX, offsetY);
								}
							}

						}
					}

					// Save the map image to the specified file
					mapImage.Save(saveFileDialog.FileName, ImageFormat.Png);
				}

				Logger.Instance.Output("Exported map " + _mapName, LogItemType.Info);
				return;
			}
			if (string.IsNullOrWhiteSpace(this._config.Dra_BinPath))
			{
				Logger.Instance.Output("DRA.BIN path not set", LogItemType.Warning);
				MessageBox.Show("You must load the DRA.BIN file from the SotN ISO to correctly display save/load rooms!", "Warning");
			}
		}
	}
}
