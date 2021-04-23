// Copyright (c) 2021 Convention of States Action
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SlackHarvester
{
	internal sealed class SlackWorkspace
	{
		private string ExportedSlackWorkspaceFolder { get; }
		private IUserRepository Users { get; set; }
		private HashSet<string> EssentiallyEmptyTextContent { get; }
		private List<Channel> Channels { get; }

		internal string WorkspaceName { get; set; }

		internal SlackWorkspace(string exportedSlackWorkspaceFolder)
		{
			ExportedSlackWorkspaceFolder = exportedSlackWorkspaceFolder;
			WorkspaceName = Path.GetFileName(ExportedSlackWorkspaceFolder);
			Channels = new List<Channel>();
			EssentiallyEmptyTextContent = new HashSet<string>
			{
				"NB: No original text.",
				"NB: Original text was all illegal XML characters."
			};
		}

		internal SlackFile ProcessWorkspace()
		{
			// Get user repository
			Users = new UserRepository(ExportedSlackWorkspaceFolder);
			// Load channels
			var channelsPathname = Path.Combine(ExportedSlackWorkspaceFolder, "channels.json");
			if (!File.Exists(channelsPathname))
			{
				throw new InvalidOperationException($"'{channelsPathname}' does not exist.");
			}
			using (var reader = File.OpenText(channelsPathname))
			{
				var token = JToken.ReadFrom(new JsonTextReader(reader));
				foreach (var channel in token.Children().ToList().Select(result => result.ToObject<Channel>()))
				{
					Channels.Add(channel);
				}
			}
			return !ProcessChannels(out var slackFileDocument) ?
				null
				: new SlackFile(Path.Combine(SlackHarvesterServices.ManualProcessingFolder, $"{Path.GetFileName(ExportedSlackWorkspaceFolder).Replace(".", "_")}.xml"), slackFileDocument, true);
		}

		private bool ProcessChannels(out XDocument slackFileDocument)
		{
			slackFileDocument = new XDocument();
			var root = new XElement("root");
			slackFileDocument.Add(root);
			foreach (var channel in Channels)
			{
				var channelElement = new XElement("channel", new XAttribute("name", channel.Name), new XAttribute("uiName", channel.UiName));
				foreach (var dictionary in channel.GetTopLevelMessages(Users, ExportedSlackWorkspaceFolder))
				{
					if (!dictionary.Value.Any())
					{
						continue;
					}
					var messagesForDate = new XElement("messagesForDate", new XAttribute("date", dictionary.Key));
					foreach (var message in dictionary.Value.Where(message => !string.IsNullOrWhiteSpace(message.User) && Users.HasUser(message.User)))
					{
						var noUsefulTextMessage = EssentiallyEmptyTextContent.Contains(message.Text);
						if (message.Replies == null)
						{
							if (!noUsefulTextMessage)
							{
								messagesForDate.Add(new XElement("message", new XAttribute("type", "solo"), new XAttribute("name", Users.Get(message.User).RealName), new XElement("text", message.Text)));
							}
							continue;
						}
						var threadedMessageElement = new XElement("message", new XAttribute("type", "threaded"));
						messagesForDate.Add(threadedMessageElement);
						var mainMessageElement = new XElement("mainMessage", new XAttribute("name", Users.Get(message.User).RealName), new XElement("text", message.Text));
						threadedMessageElement.Add(mainMessageElement);
						foreach (var replyMessageElement in message.ThreadedMessages.Select(messageReply => new XElement("replyMessage", new XAttribute("name", Users.Get(messageReply.User).RealName), new XElement("text", messageReply.Text))))
						{
							if (!EssentiallyEmptyTextContent.Contains(replyMessageElement.Element("text").Value))
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
			return root.HasElements;
		}

		private sealed class UserRepository : IUserRepository
		{
			private Dictionary<string, User> Users { get; set; }
			private IUserRepository AsIUserRepository => this;

			internal UserRepository(string exportedSlackFolder)
			{
				var usersPathname = Path.Combine(exportedSlackFolder, "users.json");
				if (!File.Exists(usersPathname))
				{
					throw new InvalidOperationException($"'{usersPathname}' does not exist.");
				}
				Users = new Dictionary<string, User>();
				using (var reader = File.OpenText(usersPathname))
				{
					var token = JToken.ReadFrom(new JsonTextReader(reader));
					foreach (var user in token.Children().ToList().Select(result => result.ToObject<User>()))
					{
						Users.Add(user.Id, user);
					}
				}
				if (AsIUserRepository.HasUser("USLACKBOT"))
				{
					return;
				}
				var slackBotUser = new User
				{
					Id = "USLACKBOT",
					Real_Name = "SLACKBOT",
					Deleted = false
				};
				Users.Add("USLACKBOT", slackBotUser);
			}

			bool IUserRepository.HasUser(string id) => Users.ContainsKey(id);

			User IUserRepository.Get(string id)
			{
				return Users[id];
			}
		}
	}
}