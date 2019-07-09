// Copyright (c) 2019 Randy Regnier
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System;

namespace SlackScrape
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			SlackHarvester.Harvest(args[0]);
			Console.WriteLine("Done.");
		}
	}
}
