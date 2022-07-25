using System;

namespace SotNEditor.HUD
{
	public class FeedbackEntry
	{
		public FeedbackEntry(decimal timeMultiplier)
		{
			this.EndTime = DateTime.Now + new TimeSpan(0, 0, 0, 0, (int)(FeedbackEntry.EntryLifetimeMilliseconds * timeMultiplier));
		}

		public void Reset(string text, decimal timeMultiplier)
		{
			this.Text = text;
			this.EndTime = DateTime.Now + new TimeSpan(0, 0, 0, 0, (int)(FeedbackEntry.EntryLifetimeMilliseconds * timeMultiplier));
		}

		private static readonly int EntryLifetimeMilliseconds = 1000;

		public DateTime EndTime;

		public string Text;

		public FeedbackType Type;
	}
}
