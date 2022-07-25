using System;
using System.Windows.Input;

namespace SotNEditor
{
	public static class SotNCommands
	{
		// Show the PSX VRAM Preview window
		public static readonly RoutedCommand VRamPreview = new RoutedCommand();

		// Set path to F_GAME.BIN
		public static readonly RoutedCommand SetFGameBinPath = new RoutedCommand();

		// Set path to DRA.BIN
		public static readonly RoutedCommand SetDraBinPath = new RoutedCommand();

		// Toggle visibility of the selected layer
		public static readonly RoutedCommand SetLayer = new RoutedCommand();

		// Set the background color of the map
		public static readonly RoutedCommand SetBackgroundColor = new RoutedCommand();

		// Export the map as an image
		public static readonly RoutedCommand ExportMapImage = new RoutedCommand();
	}
}
