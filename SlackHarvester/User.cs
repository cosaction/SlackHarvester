// Copyright (c) 2019-2021 Convention of States Action
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using Newtonsoft.Json;

namespace SlackHarvester
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
		internal bool Deleted { get; set; }

		[JsonProperty]
		private UserProfile Profile { get; set; }

		internal string RealName => Deleted ? Profile.Real_Name : Real_Name;
	}
}