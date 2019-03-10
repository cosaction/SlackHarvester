// Copyright (c) 2019 Randy Regnier
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using Newtonsoft.Json;

namespace SlackScrape
{
	[JsonObject(MemberSerialization.OptIn)]
	internal sealed class Channel
	{
		[JsonProperty]
		internal string Id { get; set; }

		[JsonProperty]
		internal string Name { get; set; }
	}
}