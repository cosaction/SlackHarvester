// Copyright (c) 2019 Randy Regnier
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System.Collections.Generic;
using Newtonsoft.Json;

namespace SlackScrape
{
	[JsonObject(MemberSerialization.OptIn)]
	internal sealed class Message
	{
		[JsonProperty]
		internal string User { get; set; }

		[JsonProperty]
		internal string Type { get; set; }

		[JsonProperty]
		internal string Subtype { get; set; }

		[JsonProperty]
		internal string Text { get; set; }

		[JsonProperty]
		internal string Ts { get; set; }

		[JsonProperty]
		internal string Thread_Ts { get; set; }

		[JsonProperty]
		internal List<Reply> Replies { get; set; }

		internal List<Message> ThreadedMessages = new List<Message>();

		internal string PrettifyText(UserRepository userRepository)
		{
			var retVal = Text;
			var startIdx = 0;
			var foundOpenBracketIdx = Text.IndexOf("<@", startIdx);
			while (foundOpenBracketIdx > -1)
			{
				startIdx = foundOpenBracketIdx;
				var endBracketIndex = Text.IndexOf(">", foundOpenBracketIdx + 1);
				var userId = Text.Substring(startIdx + 2, endBracketIndex - startIdx -2);
				var user = userRepository.Get(userId);
				retVal = Text.Replace(userId, user.Name);
				// Swap user Name.
				foundOpenBracketIdx = Text.IndexOf("<@", foundOpenBracketIdx + 1);
			}
			return retVal;
		}
	}
}