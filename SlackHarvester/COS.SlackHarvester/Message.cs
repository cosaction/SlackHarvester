// Copyright (c) 2019-2021 Convention of States Action
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace SlackHarvester.COS.SlackHarvester
{
	[JsonObject(MemberSerialization.OptIn)]
	internal sealed class Message
	{
		private static byte[] _moreJunk = { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x18, 0x19, 0x1C, 0x1D };

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

		internal void PrettifyText(UserRepository userRepository)
		{
			if (string.IsNullOrWhiteSpace(Text))
			{
				Text = "NB: No original text.";
				return;
			}
			var prettifiedText = Text.Trim();
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
			var xmlFriendly = XmlCharacterWhitelist(prettifiedText);
			if (!string.Equals(prettifiedText, xmlFriendly, StringComparison.InvariantCultureIgnoreCase))
			{
				prettifiedText = xmlFriendly;
			}
			Text = string.IsNullOrWhiteSpace(prettifiedText) ? string.Empty : prettifiedText;
			if (string.IsNullOrEmpty(Text))
			{
				Text = "NB: Original text was all illegal XML characters.";
			}
		}

		private static string XmlCharacterWhitelist(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
			{
				return string.Empty;
			}
			var sb = new StringBuilder();
			foreach (var character in input.Where(character => character >= 0x0020 && character <= 0xD7FF || character >= 0xE000 && character <= 0xFFFD || character == 0x0009 || character == 0x000A || character == 0x000D))
			{
				sb.Append(character);
			}
			return Encoding.UTF8.GetString(Encoding.Unicode.GetBytes(sb.ToString()).Where(currentByte => !_moreJunk.Contains(currentByte)).ToArray());
		}
	}
}