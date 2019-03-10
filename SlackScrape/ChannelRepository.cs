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
	internal sealed class ChannelRepository
	{
		internal IReadOnlyDictionary<string, Channel> Channels { get; private set; }

		internal bool HasChannel(string name) => Channels.ContainsKey(name);

		internal Channel Get(string name)
		{
			return Channels[name];
		}

		internal ChannelRepository(string exportedSlackFolder)
		{
			var channels = new Dictionary<string, Channel>();
			var channelsPathname = Path.Combine(exportedSlackFolder, "channels.json");
			if (!File.Exists(channelsPathname))
			{
				throw new InvalidOperationException($"'{channelsPathname}' does not exist.");
			}
			using (var reader = File.OpenText(channelsPathname))
			{
				var token = JToken.ReadFrom(new JsonTextReader(reader));
				foreach (var result in token.Children().ToList())
				{
					var channel = result.ToObject<Channel>();
					channels.Add(channel.Name, channel);
				}
			}
			Channels = channels;
		}
	}
}