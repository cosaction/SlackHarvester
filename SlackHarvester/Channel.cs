// Copyright (c) 2019-2021 Convention of States Action
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SlackHarvester
{
	[JsonObject(MemberSerialization.OptIn)]
	internal sealed class Channel
	{
		[JsonProperty]
		internal string Id { get; set; }

		[JsonProperty]
		internal string Name { get; set; }

		[JsonProperty]
		internal bool Is_Archived { get; set; }

		internal string UiName => Is_Archived ? $"(A)-{Name}" : Name;

		internal SortedDictionary<string, List<Message>> GetTopLevelMessages(IUserRepository userRepository, string exportedSlackFolder)
		{
			var channelPathname = Path.Combine(exportedSlackFolder, Name);
			if (!Directory.Exists(channelPathname))
			{
				throw new InvalidOperationException($"'{channelPathname}' does not exist.");
			}
			var retVal = new SortedDictionary<string, List<Message>>();
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
			var messages = new Dictionary<string, Message>();
			// One, or more, files in each folder.
			foreach (var jsonPathname in Directory.GetFiles(channelPathname, "*.json"))
			{
				var topLevelMessages = new List<Message>();
				using (var reader = File.OpenText(jsonPathname))
				{
					var token = JToken.ReadFrom(new JsonTextReader(reader));
					foreach (var message in token.Children().ToList().Select(result => result.ToObject<Message>()))
					{
						if (skipSubtypes.Contains(message.Subtype))
						{
							continue;
						}
						PrettifyText(message);
						messages.Add(message.Ts, message);
						if (string.IsNullOrWhiteSpace(message.Thread_Ts) || message.Ts == message.Thread_Ts)
						{
							topLevelMessages.Add(message);
						}
					}
				}
				retVal.Add(Path.GetFileNameWithoutExtension(jsonPathname), topLevelMessages);
			}
			// Add threaded messages to top level message, if there are any threaded replies.
			foreach (var topLevelMessageList in retVal.Values)
			{
				foreach (var topLevelMessageWithReplies in topLevelMessageList)
				{
					if (topLevelMessageWithReplies.Replies == null)
					{
						continue;
					}
					foreach (var reply in topLevelMessageWithReplies.Replies)
					{
						topLevelMessageWithReplies.ThreadedMessages.Add(messages[reply.Ts]);
					}
				}
			}
			return retVal;

			void PrettifyText(Message message)
			{
				if (string.IsNullOrWhiteSpace(message.Text))
				{
					message.Text = "NB: No original text.";
					return;
				}
				byte[] moreJunk = { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x18, 0x19, 0x1C, 0x1D };
				var prettifiedText = message.Text.Trim();
				var idNameMap = new List<Tuple<string, string>>();
				var foundOpenBracketIdx = prettifiedText.IndexOf("<@", 0, StringComparison.InvariantCulture);
				while (foundOpenBracketIdx > -1)
				{
					var endBracketIndex = prettifiedText.IndexOf(">", foundOpenBracketIdx, StringComparison.InvariantCulture);
					var userId = prettifiedText.Substring(foundOpenBracketIdx + 2, endBracketIndex - foundOpenBracketIdx - 2);
					var user = userRepository.Get(userId);
					idNameMap.Add(new Tuple<string, string>(userId, user.Name));
					// NB: Don't do the swap here, since that will mess up "endBracketIndex".
					foundOpenBracketIdx = prettifiedText.IndexOf("<@", endBracketIndex, StringComparison.InvariantCulture);
				}
				foreach (var (userId, userName) in idNameMap)
				{
					// Swap user Name.
					prettifiedText = prettifiedText.Replace(userId, userName);
				}
				var xmlFriendly = string.Empty;
				if (!string.IsNullOrWhiteSpace(prettifiedText))
				{
					var sb = new StringBuilder();
					foreach (var character in prettifiedText.Where(character => character >= 0x0020 && character <= 0xD7FF || character >= 0xE000 && character <= 0xFFFD || character == 0x0009 || character == 0x000A || character == 0x000D))
					{
						sb.Append(character);
					}
					xmlFriendly = Encoding.UTF8.GetString(Encoding.Unicode.GetBytes(sb.ToString()).Where(currentByte => !moreJunk.Contains(currentByte)).ToArray());
				}
				if (!string.Equals(prettifiedText, xmlFriendly, StringComparison.InvariantCultureIgnoreCase))
				{
					prettifiedText = xmlFriendly;
				}
				message.Text = string.IsNullOrWhiteSpace(prettifiedText) ? string.Empty : prettifiedText;
				if (string.IsNullOrEmpty(message.Text))
				{
					message.Text = "NB: Original text was all illegal XML characters.";
				}
			}
		}
	}
}