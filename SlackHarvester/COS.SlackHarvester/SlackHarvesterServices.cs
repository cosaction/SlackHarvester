// Copyright (c) 2019-2021 Convention of States Action
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace SlackHarvester.COS.SlackHarvester
{
	internal static class SlackHarvesterServices
	{
		private static string _exportedSlackBaseFolder;
		private static string _exportedSlackSourceFolder;
		private static string _manualProcessingFolder;
		private const string Source = "Source";
		private const string Current = "Current";
		private const string Archived = "Archived";
		private const string ManualProcessing = "ManualProcessing";

		internal static string ExportedSlackBaseFolder
		{
			set
			{
				if (!Directory.Exists(value))
				{
					throw new ArgumentException($"'{value}' does not exist.");
				}
				_exportedSlackBaseFolder = value;
			}
			get => _exportedSlackBaseFolder;
		}

		private static string ExportedSlackSourceFolder
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_exportedSlackSourceFolder))
				{
					_exportedSlackSourceFolder = Path.Combine(_exportedSlackBaseFolder, Source);
				}
				return _exportedSlackSourceFolder;
			}
		}

		internal static string ManualProcessingFolder
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_manualProcessingFolder))
				{
					_manualProcessingFolder = Path.Combine(_exportedSlackBaseFolder, ManualProcessing);
				}
				return _manualProcessingFolder;
			}
		}

		internal static bool CanDoManualCleanup => Directory.Exists(ManualProcessingFolder) && Directory.GetFiles(ManualProcessingFolder, "*.xml").Any();

		internal static bool CanHarvestSource => Directory.Exists(ExportedSlackBaseFolder) && Directory.GetDirectories(ExportedSlackSourceFolder).Any();

		internal static void RemoveSourceFolders()
		{
			foreach (var gonerFolder in Directory.GetDirectories(ExportedSlackSourceFolder))
			{
				Directory.Delete(gonerFolder, true);
			}
		}

		internal static void HarvestFromSources()
		{
			UserRepository userRepository = null;
			foreach (var currentExportedSlackSourceFolder in Directory.GetDirectories(ExportedSlackSourceFolder))
			{
				userRepository = new UserRepository(currentExportedSlackSourceFolder);
				var channelRepository = new ChannelRepository(currentExportedSlackSourceFolder);
				ProcessChannelRepository(channelRepository, currentExportedSlackSourceFolder);
			}

			void ProcessChannelRepository(ChannelRepository channelRepository, string currentExportedSlackSourceFolder)
			{
				ProcessChannels(channelRepository.CurrentChannels.Values, Current);
				ProcessChannels(channelRepository.ArchivedChannels.Values, Archived);

				void ProcessChannels(IEnumerable<Channel> channels, string channelType)
				{
					var essentiallyEmptyTextContent = new HashSet<string>
					{
					"NB: No original text.",
					"NB: Original text was all illegal XML characters."
					};
					var root = new XElement("root");
					foreach (var channel in channels)
					{
						var messageRepository = new MessageRepository(userRepository, currentExportedSlackSourceFolder, channel);
						var channelElement = new XElement("channel", new XAttribute("name", channel.Name));
						foreach (var dictionary in messageRepository.TopLevelMessages)
						{
							if (!dictionary.Value.Any())
							{
								continue;
							}
							var messagesForDate = new XElement("messagesForDate", new XAttribute("date", dictionary.Key));
							foreach (var message in dictionary.Value.Where(message => !string.IsNullOrWhiteSpace(message.User) && userRepository.HasUser(message.User)))
							{
								var noUsefulTextMessage = essentiallyEmptyTextContent.Contains(message.Text);
								if (message.Replies == null)
								{
									if (!noUsefulTextMessage)
									{
										messagesForDate.Add(new XElement("message", new XAttribute("type", "solo"), new XAttribute("name", userRepository.Get(message.User).RealName), new XElement("text", message.Text)));
									}
									continue;
								}
								var threadedMessageElement = new XElement("message", new XAttribute("type", "threaded"));
								messagesForDate.Add(threadedMessageElement);
								var mainMessageElement = new XElement("mainMessage", new XAttribute("name", userRepository.Get(message.User).RealName), new XElement("text", message.Text));
								threadedMessageElement.Add(mainMessageElement);
								foreach (var replyMessageElement in message.ThreadedMessages.Select(messageReply => new XElement("replyMessage", new XAttribute("name", userRepository.Get(messageReply.User).RealName), new XElement("text", messageReply.Text))))
								{
									if (!essentiallyEmptyTextContent.Contains(replyMessageElement.Element("text").Value))
									{
										mainMessageElement.Add(replyMessageElement);
									}
								}
							}
							if (messagesForDate.HasElements)
							{
								channelElement.Add(messagesForDate);
							}
						}
						if (channelElement.HasElements)
						{
							root.Add(channelElement);
						}
					}
					if (!root.HasElements)
					{
						return;
					}
					var doc = new XDocument(root);
					// Write out to ManualProcessingFolder
					doc.Save(Path.Combine(ManualProcessingFolder, $"{Path.GetFileName(currentExportedSlackSourceFolder).Replace(".", "_")}-{channelType}-Messages.xml"));
				}
			}
		}
	}
}
