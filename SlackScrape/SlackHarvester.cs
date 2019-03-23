// Copyright (c) 2019 Randy Regnier
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SlackScrape
{
	internal static class SlackHarvester
	{
		internal static void Harvest()
		{
			var exportedSlackFolder = Path.Combine("/home", "randy", "COS_Slack_Exports");
			if (!Directory.Exists(exportedSlackFolder))
			{
				throw new ArgumentException("Exported Slack folder not found.", nameof(exportedSlackFolder));
			}
			foreach (var dirName in Directory.GetDirectories(exportedSlackFolder, "*.*"))
			{
				var userRepository = new UserRepository(dirName);
				var channelRepository = new ChannelRepository(dirName);
				ProcessChannels(channelRepository.CurrentChannels.Values, dirName, userRepository, "Current");
				ProcessChannels(channelRepository.ArchivedChannels.Values, dirName, userRepository, "Archived");
			}
		}

		private static void ProcessChannels(IEnumerable<Channel> channels, string dirName, UserRepository userRepository, string channelType)
		{
			var comments = new StringBuilder();
			foreach (var channel in channels)
			{
				comments.AppendLine($"********** [Start: {channel.Name}] **********");
				var messageRepository = new MessageRepository(dirName, channel);
				foreach (var (key, value) in messageRepository.TopLevelMessages)
				{
					if (!value.Any())
					{
						continue;
					}
					comments.AppendLine($"********** [Start: {key}] **********");
					foreach (var message in value)
					{
						if (string.IsNullOrWhiteSpace(message.User) || !userRepository.HasUser(message.User))
						{
							continue;
						}
						if (message.Replies == null)
						{
							comments.AppendLine($"[{userRepository.Get(message.User).RealName}]: {message.PrettifyText(userRepository)}]");
							continue;
						}
						comments.AppendLine($"\t********** [Start: thread] **********");
						comments.AppendLine($"\t[{userRepository.Get(message.User).RealName}]: {message.PrettifyText(userRepository)}]");
						foreach (var messageReply in message.ThreadedMessages)
						{
							comments.AppendLine($"\t\t[{userRepository.Get(messageReply.User).RealName}]: {messageReply.PrettifyText(userRepository)}]");
						}
						comments.AppendLine($"\t********** [End: thread] **********");
					}
					comments.AppendLine($"********** [End: {key}] **********");
				}
				comments.AppendLine($"********** [End: {channel.Name}] **********");
			}
			File.WriteAllText(Path.Combine(dirName, $"{Path.GetFileNameWithoutExtension(dirName)}-{channelType}-Messages.txt"), comments.ToString());
		}
	}
}