using System;
using System.Windows;

namespace SotNEditor.Logging
{
	public class LogItem : DependencyObject
	{
		public DateTime Timestamp
		{
			get
			{
				return (DateTime)base.GetValue(LogItem.TimestampProperty);
			}
			set
			{
				base.SetValue(LogItem.TimestampProperty, value);
			}
		}

		public LogItemType Type
		{
			get
			{
				return (LogItemType)base.GetValue(LogItem.TypeProperty);
			}
			set
			{
				base.SetValue(LogItem.TypeProperty, value);
			}
		}

		public string Text
		{
			get
			{
				return (string)base.GetValue(LogItem.TextProperty);
			}
			set
			{
				base.SetValue(LogItem.TextProperty, value);
			}
		}

		public static readonly DependencyProperty TimestampProperty = DependencyProperty.Register("Timestamp", typeof(DateTime), typeof(LogItem), new UIPropertyMetadata(DateTime.Now));

		public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(LogItemType), typeof(LogItem), new UIPropertyMetadata(LogItemType.Info));

		public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(LogItem), new UIPropertyMetadata(string.Empty));
	}
}
