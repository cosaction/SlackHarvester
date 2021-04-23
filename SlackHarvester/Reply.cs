// Copyright (c) 2019-2021 Convention of States Action
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using Newtonsoft.Json;

namespace SlackHarvester
{
	[JsonObject(MemberSerialization.OptIn)]
	internal sealed class Reply
	{
		[JsonProperty]
		internal string User { get; set; }

		[JsonProperty]
		internal string Ts { get; set; }
	}
}