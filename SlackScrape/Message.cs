// Copyright (c) 2019 Randy Regnier
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
			var retVal = Text.Trim();
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
			var xmlFriendly = XmlCharacterWhitelist(retVal);
			if (!string.Equals(retVal, xmlFriendly, StringComparison.InvariantCultureIgnoreCase))
			{
				retVal = xmlFriendly;
			}
			return retVal;
		}

		private static string XmlCharacterWhitelist(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
			{
				return string.Empty;
			}
			var sb = new StringBuilder();
			foreach (var ch in input.Where(ch => ch >= 0x0020 && ch <= 0xD7FF || ch >= 0xE000 && ch <= 0xFFFD || ch == 0x0009 || ch == 0x000A || ch == 0x000D))
			{
				sb.Append(ch);
			}
			return NullRemover(sb.ToString());
		}

		private static string NullRemover(string input)
		{
			int idx;
			var asBytes = Encoding.UTF8.GetBytes(input);
			var temp = new byte[asBytes.Length];
			for (idx = 0; idx < asBytes.Length - 1; idx++)
			{
				if (asBytes[idx] == 0x00)
				{
					break;
				}
				temp[idx] = asBytes[idx];
			}
			var nullLessData = new byte[idx];
			for (idx = 0; idx < nullLessData.Length; idx++)
			{
				nullLessData[idx] = temp[idx];
			}
			return Encoding.UTF8.GetString(nullLessData);
		}
	}
}
