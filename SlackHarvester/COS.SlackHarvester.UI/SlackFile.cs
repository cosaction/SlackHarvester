// Copyright (c) 2019-2021 Convention of States Action
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace SlackHarvester.COS.SlackHarvester.UI
{
	internal sealed class SlackFile
	{
		public event EventHandler SlackFileWasDeleted;

		private string UiName { get; }
		private string Pathname { get; }

		private XDocument Document { get; }

		private bool RemoveEmptyMajorElements()
		{
			var wasChanged = false;
			foreach (var emptyReplyMessageElement in RootData.Descendants("replyMessage").Where(el => !el.HasElements).ToList())
			{
				wasChanged = true;
				emptyReplyMessageElement.Remove();
			}
			foreach (var emptyMainMessageElement in RootData.Descendants("mainMessage").Where(el => !el.HasElements).ToList())
			{
				wasChanged = true;
				emptyMainMessageElement.Remove();
			}
			foreach (var emptyMessageElement in RootData.Descendants("message").Where(el => !el.HasElements).ToList())
			{
				wasChanged = true;
				emptyMessageElement.Remove();
			}
			foreach (var emptyMessagesElement in RootData.Descendants("messagesForDate").Where(el => !el.HasElements).ToList())
			{
				wasChanged = true;
				emptyMessagesElement.Remove();
			}
			foreach (var emptyChannelElement in RootData.Descendants("channel").Where(el => !el.HasElements).ToList())
			{
				wasChanged = true;
				emptyChannelElement.Remove();
			}
			return wasChanged;
		}

		private void OnRaiseSlackFileDeletedIfNeeded()
		{
			if (RootData.HasElements)
			{
				return;
			}
			File.Delete(Pathname);
			SlackFileWasDeleted?.Invoke(this, null);
		}

		internal SlackFile(string pathname)
		{
			Pathname = pathname ?? throw new ArgumentNullException(nameof(pathname));
			Document = XDocument.Load(pathname);
			RootData = Document.Root;
			if (RemoveEmptyMajorElements())
			{
				Document.Save(Pathname);
			}
			UiName = Path.GetFileNameWithoutExtension(pathname).Replace("-Messages", string.Empty);
			OnRaiseSlackFileDeletedIfNeeded();
		}

		internal XElement RootData { set; get; }

		internal void Save()
		{
			RemoveEmptyMajorElements();
			Document.Save(Pathname);
			OnRaiseSlackFileDeletedIfNeeded();
		}

		#region Overrides of Object

		/// <inheritdoc />
		public override string ToString()
		{
			return UiName;
		}

		#endregion
	}
}