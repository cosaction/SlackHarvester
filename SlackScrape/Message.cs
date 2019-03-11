// Copyright (c) 2019 Randy Regnier
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System;
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
			var idNameMap = new List<Tuple<string, string>>();
			var foundOpenBracketIdx = retVal.IndexOf("<@", 0, StringComparison.InvariantCulture);
			while (foundOpenBracketIdx > -1)
			{
				var endBracketIndex = retVal.IndexOf(">", foundOpenBracketIdx, StringComparison.InvariantCulture);
				var userId = retVal.Substring(foundOpenBracketIdx + 2, endBracketIndex - foundOpenBracketIdx -2);
				var user = userRepository.Get(userId);
				idNameMap.Add(new Tuple<string, string>(userId, user.Name));
				foundOpenBracketIdx = retVal.IndexOf("<@", endBracketIndex, StringComparison.InvariantCulture);
			}
			foreach (var (userId, userName) in idNameMap)
			{
				// Swap user Name.
				retVal = retVal.Replace(userId, userName);
			}
			return retVal;
		}
	}
}