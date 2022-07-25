using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace SotNEditor.Logging
{
	internal class Logger : INotifyPropertyChanged
	{
		public static Logger Instance
		{
			get
			{
				Logger result;
				if ((result = Logger._instance) == null)
				{
					result = (Logger._instance = new Logger());
				}
				return result;
			}
		}

		protected Logger()
		{
		}

		public IList<LogItem> Log
		{
			get
			{
				return this._log;
			}
		}

		public IList<LogItem> Last10LogLines
		{
			get
			{
				return this._log.Reverse<LogItem>().Take(15).Reverse<LogItem>().ToList<LogItem>();
			}
		}

		public LogItem LastLogLine
		{
			get
			{
				return this._log.LastOrDefault<LogItem>();
			}
		}

		public void Output(string text, LogItemType type)
		{
			LogItem logItem = new LogItem
			{
				Type = type,
				Text = text
			};
			this._log.Add(logItem);
			this._streamWriter.WriteLine(string.Concat(new object[]
			{
				logItem.Timestamp,
				": [",
				logItem.Type,
				"] ",
				logItem.Text
			}));
			this.NotifyPropertyChanged("Log");
			this.NotifyPropertyChanged("Last10LogLines");
			this.NotifyPropertyChanged("LastLogLine");
		}

		public void Init()
		{
			try
			{
				this._streamWriter = new StreamWriter("SNEER_log.txt", true);
				this._streamWriter.AutoFlush = true;
			}
			catch (Exception)
			{
				this.Output("Unable to open SNEER_log.txt for writing. The log for this session will not be saved.", LogItemType.Error);
			}
		}

		public void Deinit()
		{
			if (this._streamWriter != null)
			{
				this._streamWriter.Close();
			}
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

		private const int LogPreviewLineCount = 15;

		private static Logger _instance;

		private StreamWriter _streamWriter;

		private IList<LogItem> _log = new List<LogItem>();
	}
}
