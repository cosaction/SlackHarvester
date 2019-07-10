// Copyright (c) 2019 Randy Regnier
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace SlackScrape
{
	internal static class SlackHarvester
	{
		internal static void Harvest(string exportedSlackFolder)
		{
			if (!Directory.Exists(exportedSlackFolder))
			{
				throw new ArgumentException("Exported Slack folder not found.", nameof(exportedSlackFolder));
			}
			foreach (var dirName in Directory.GetDirectories(exportedSlackFolder, "*.*"))
			{
				if (dirName.Contains("cb-preview"))
				{
					// Remove after cb-preview data has been harvested.
					continue;
				}
				var userRepository = new UserRepository(dirName);
				var channelRepository = new ChannelRepository(dirName);
				ProcessChannels(channelRepository.CurrentChannels.Values, dirName, userRepository, "Current");
				ProcessChannels(channelRepository.ArchivedChannels.Values, dirName, userRepository, "Archived");
			}
		}

		private static void ProcessChannels(IEnumerable<Channel> channels, string dirName, UserRepository userRepository, string channelType)
		{
			var doc = new XDocument(new XElement("root"));
			var root = doc.Root;
			foreach (var channel in channels)
			{
				var channelElement = new XElement("channel", new XAttribute("name", channel.Name));
				root.Add(channelElement);
				var messageRepository = new MessageRepository(dirName, channel);
				foreach (var (messageDate, messages) in messageRepository.TopLevelMessages)
				{
					if (!messages.Any())
					{
						continue;
					}
					var messagesForDate = new XElement("messages", new XAttribute("date", messageDate));
					channelElement.Add(messagesForDate);
					foreach (var message in messages.Where(message => !string.IsNullOrWhiteSpace(message.User) && userRepository.HasUser(message.User)))
					{
						if (string.IsNullOrWhiteSpace(message.Text))
						{
							continue;
						}
						if (message.Replies == null)
						{
							messagesForDate.Add(new XElement("message", new XAttribute("type", "solo"), new XAttribute("name", userRepository.Get(message.User).RealName), new XElement("text", message.PrettifyText(userRepository))));
							continue;
						}
						var threadedMessageElement = new XElement("message", new XAttribute("type", "threaded"));
						messagesForDate.Add(threadedMessageElement);
						var mainMessageElement = new XElement("mainMessage", new XAttribute("name", userRepository.Get(message.User).RealName), new XElement("text", message.PrettifyText(userRepository)));
						threadedMessageElement.Add(mainMessageElement);
						foreach (var replyMessageElement in message.ThreadedMessages.Select(messageReply => new XElement("replyMessage", new XAttribute("name", userRepository.Get(messageReply.User).RealName), new XElement("text", messageReply.PrettifyText(userRepository)))))
						{
							mainMessageElement.Add(replyMessageElement);
						}
					}
				}
			}
			doc.Save(Path.Combine(dirName, $"{Path.GetFileNameWithoutExtension(dirName)}-{channelType}-Messages.xml"));
		}
	}
}
