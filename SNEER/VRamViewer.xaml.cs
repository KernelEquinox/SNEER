using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Xna.Framework.Graphics;

namespace SotNEditor
{
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public partial class VRamViewer : Window
	{
		public VRamViewer(Texture2D vramImage, Texture2D clutImage)
		{
			this.InitializeComponent();
			byte[] array = new byte[clutImage.Width * clutImage.Height * 4];
			clutImage.GetData<byte>(array);
			for (int i = 0; i < array.Length; i += 4)
			{
				byte b = array[i];
				array[i] = array[i + 2];
				array[i + 2] = b;
			}
			BitmapSource dataContext = BitmapSource.Create(clutImage.Width, clutImage.Height, 96.0, 96.0, PixelFormats.Bgr32, BitmapPalettes.BlackAndWhite, array, clutImage.Width * 4);
			this.ClutPreviewImage.DataContext = dataContext;
			array = new byte[vramImage.Width * vramImage.Height * 4];
			vramImage.GetData<byte>(array);
			byte[] array2 = new byte[vramImage.Width * vramImage.Height];
			for (int j = 0; j < array.Length; j += 4)
			{
				array2[j / 4] = array[j];
			}
			dataContext = BitmapSource.Create(vramImage.Width, vramImage.Height, 96.0, 96.0, PixelFormats.Indexed8, BitmapPalettes.Gray16, array2, vramImage.Width);
			this.VRamPreviewImage.DataContext = dataContext;
		}
	}
}
