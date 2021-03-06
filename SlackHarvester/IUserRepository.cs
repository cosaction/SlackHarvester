// Copyright (c) 2021 Convention of States Action
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

namespace SlackHarvester
{
	internal interface IUserRepository
	{
		bool HasUser(string id);
		User Get(string id);
	}
}