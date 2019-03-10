// Copyright (c) 2019 Randy Regnier
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using Newtonsoft.Json;

namespace SlackScrape
{
	[JsonObject(MemberSerialization.OptIn)]
	internal sealed class User
	{
		[JsonProperty]
		internal string Id { get; set; }

		[JsonProperty]
		internal string Name { get; set; }

		[JsonProperty]
		internal string Real_Name { get; set; }

		[JsonProperty]
		internal UserProfile Profile { get; set; }

		[JsonProperty]
		internal bool Deleted { get; set; }

		internal string RealName => Deleted ? Profile.Real_Name : Real_Name;
	}
}