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
		internal IReadOnlyDictionary<string, Channel> CurrentChannels { get; }

		internal bool HasCurrentChannel(string name) => CurrentChannels.ContainsKey(name);

		internal Channel GetCurrentChannel(string name)
		{
			return CurrentChannels[name];
		}

		internal IReadOnlyDictionary<string, Channel> ArchivedChannels { get; }

		internal bool HasArchivedChannel(string name) => ArchivedChannels.ContainsKey(name);

		internal Channel GetArchivedChannel(string name)
		{
			return ArchivedChannels[name];
		}

		internal ChannelRepository(string exportedSlackFolder)
		{
			var channelsPathname = Path.Combine(exportedSlackFolder, "channels.json");
			if (!File.Exists(channelsPathname))
			{
				throw new InvalidOperationException($"'{channelsPathname}' does not exist.");
			}
			var currentChannels = new Dictionary<string, Channel>();
			var archivedChannels = new Dictionary<string, Channel>();
			using (var reader = File.OpenText(channelsPathname))
			{
				var token = JToken.ReadFrom(new JsonTextReader(reader));
				foreach (var result in token.Children().ToList())
				{
					var channel = result.ToObject<Channel>();
					if (channel.Is_Archived)
					{
						archivedChannels.Add(channel.Name, channel);
					}
					else
					{
						currentChannels.Add(channel.Name, channel);
					}
				}
			}
			CurrentChannels = currentChannels;
			ArchivedChannels = archivedChannels;
		}
	}
}