using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SotNEditor.Logging
{
	internal class LogItemTypeColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			SolidColorBrush result = Brushes.ForestGreen;
			if (!(value is LogItemType))
			{
				return result;
			}
			switch ((LogItemType)value)
			{
			case LogItemType.Warning:
				result = Brushes.RoyalBlue;
				break;
			case LogItemType.Error:
				result = Brushes.Firebrick;
				break;
			}
			return result;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
