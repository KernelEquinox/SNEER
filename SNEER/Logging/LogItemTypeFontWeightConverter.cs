using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SotNEditor.Logging
{
	internal class LogItemTypeFontWeightConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			FontWeight fontWeight = FontWeights.Normal;
			if (!(value is LogItemType))
			{
				return fontWeight;
			}
			LogItemType logItemType = (LogItemType)value;
			LogItemType logItemType2 = logItemType;
			if (logItemType2 == LogItemType.Error)
			{
				fontWeight = FontWeights.Bold;
			}
			return fontWeight;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
