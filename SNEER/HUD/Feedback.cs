using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SotNEditor.HUD
{
	internal class Feedback
	{
		public void AddFeedback(string text, decimal timeMultiplier = 1m, FeedbackType type = FeedbackType.Normal)
		{
			if (type != FeedbackType.Normal)
			{
				FeedbackEntry feedbackEntry = this._feedbackEntries.SingleOrDefault((FeedbackEntry f) => f.Type == type);
				if (feedbackEntry != null)
				{
					feedbackEntry.Reset(text, timeMultiplier);
					return;
				}
			}
			this._feedbackEntries.Add(new FeedbackEntry(timeMultiplier)
			{
				Text = text,
				Type = type
			});
		}

		public void LoadContent(ContentManager contentMgr)
		{
			this._pixel = contentMgr.Load<Texture2D>("pixel");
			this._hudFont = contentMgr.Load<SpriteFont>("HUDFont");
		}

		public void Update()
		{
			DateTime now = DateTime.Now;
			this._toRemove.Clear();
			for (int i = 0; i < this._feedbackEntries.Count; i++)
			{
				FeedbackEntry feedbackEntry = this._feedbackEntries[i];
				if (now >= feedbackEntry.EndTime)
				{
					this._toRemove.Add(feedbackEntry);
				}
			}
			for (int j = 0; j < this._toRemove.Count; j++)
			{
				this._feedbackEntries.Remove(this._toRemove[j]);
			}
		}

		public void Draw(SpriteBatch sbatch)
		{
			sbatch.Begin();
			for (int i = 0; i < this._feedbackEntries.Count; i++)
			{
				FeedbackEntry feedbackEntry = this._feedbackEntries[i];
				Vector2 vector = this._hudFont.MeasureString(feedbackEntry.Text);
				int num = (int)(vector.X + 4f);
				int height = (int)(vector.Y + 4f);
				int num2 = this.ClientDimensions.X - num - 4;
				int num3 = (int)((vector.Y + 4f + 3f) * (float)i + 4f);
				Rectangle destinationRectangle = new Rectangle(num2, num3, num, height);
				Vector2 position = new Vector2((float)(num2 + 2), (float)(num3 + 2));
				sbatch.Draw(this._pixel, destinationRectangle, this.FeedbackPanelColor);
				sbatch.DrawString(this._hudFont, feedbackEntry.Text, position, this.FeedbackPanelTextColor);
			}
			sbatch.End();
		}

		private const int FeedbackPanelMargin = 4;

		private const int FeedbackPanelPadding = 2;

		private const int FeedbackPanelSpacing = 3;

		private readonly Color FeedbackPanelColor = Color.DarkCyan;

		private readonly Color FeedbackPanelTextColor = Color.Black;

		public Point ClientDimensions = Point.Zero;

		private readonly List<FeedbackEntry> _feedbackEntries = new List<FeedbackEntry>();

		private Texture2D _pixel;

		private SpriteFont _hudFont;

		private List<FeedbackEntry> _toRemove = new List<FeedbackEntry>();
	}
}
