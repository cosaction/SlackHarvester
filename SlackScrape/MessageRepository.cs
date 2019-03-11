// Copyright (c) 2019 Randy Regnier
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SlackScrape
{
	internal sealed class MessageRepository
	{
		private Dictionary<string, Message> Messages { get; set; }

		internal SortedDictionary<string, List<Message>> TopLevelMessages { get; private set; }

		internal MessageRepository(string exportedSlackFolder, Channel channel)
		{
			var skipSubtypes = new HashSet<string>
			{
				"channel_join",
				"channel_leave",
				"bot_message",
				"channel_topic",
				"channel_purpose",
				"channel_name",
				"group_join",
				"group_leave",
				"group_topic",
				"group_purpose",
				"group_name"
			};
			var channelPathname = Path.Combine(exportedSlackFolder, channel.Name);
			if (!Directory.Exists(channelPathname))
			{
				throw new InvalidOperationException($"'{channelPathname}' does not exist.");
			}
			Messages = new Dictionary<string, Message>();
			TopLevelMessages = new SortedDictionary<string, List<Message>>();
			// One, or more, files in each folder.
			foreach (var jsonPathname in Directory.GetFiles(channelPathname, "*.json"))
			{
				var topLevelMessages = new List<Message>();
				using (var reader = File.OpenText(jsonPathname))
				{
					var tokens = JToken.ReadFrom(new JsonTextReader(reader));
					foreach (var token in tokens.Children().ToList())
					{
						var message = token.ToObject<Message>();
						//if (skipSubtypes.Contains(message.Subtype))
						//{
						//	continue;
						//}
						Messages.Add(message.Ts, message);
						if (string.IsNullOrWhiteSpace(message.Thread_Ts) || message.Ts == message.Thread_Ts)
						{
							topLevelMessages.Add(message);
						}
					}
				}
				TopLevelMessages.Add(Path.GetFileNameWithoutExtension(jsonPathname), topLevelMessages);
			}
			// Add threaded messages to top level message, if there are any threaded replies.
			foreach (var topLevelMessageList in TopLevelMessages.Values)
			{
				foreach (var topLevelMessageWithReplies in topLevelMessageList)
				{
					if (topLevelMessageWithReplies.Replies == null)
					{
						continue;
					}
					foreach (var reply in topLevelMessageWithReplies.Replies)
					{
						topLevelMessageWithReplies.ThreadedMessages.Add(Messages[reply.Ts]);
					}
				}
			}
		}
	}
}