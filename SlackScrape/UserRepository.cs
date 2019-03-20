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
	internal sealed class UserRepository
	{
		private Dictionary<string, User> Users { get; set; }

		internal bool HasUser(string id) => Users.ContainsKey(id);

		internal User Get(string id)
		{
			return Users[id];
		}

		internal UserRepository(string dirName)
		{
			var usersPathname = Path.Combine(dirName, "users.json");
			if (!File.Exists(usersPathname))
			{
				throw new InvalidOperationException($"'{usersPathname}' does not exist.");
			}
			Users = new Dictionary<string, User>();
			using (var reader = File.OpenText(usersPathname))
			{
				var token = JToken.ReadFrom(new JsonTextReader(reader));
				foreach (var userToken in token.Children().ToList())
				{
					var user = userToken.ToObject<User>();
					Users.Add(user.Id, user);
				}
			}
			if (HasUser("USLACKBOT"))
			{
				return;
			}
			var slackBotUser = new User
			{
				Id = "USLACKBOT",
				Real_Name = "SLACKBOT",
				Deleted = false
			};
			Users.Add("USLACKBOT", slackBotUser);
		}
	}
}