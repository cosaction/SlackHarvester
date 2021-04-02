// Copyright (c) 2019-2021 Convention of States Action
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System;
using System.Windows.Forms;
using SlackHarvester.COS.SlackHarvester;
using SlackHarvester.COS.SlackHarvester.UI;

namespace SlackHarvester
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			// Glean data from source files.
			// "C:\\Dev\SlackHarvester\COS_Slack_Exports"
			SlackHarvesterServices.ExportedSlackBaseFolder = args[0];
			if (SlackHarvesterServices.CanHarvestSource)
			{
				SlackHarvesterServices.HarvestFromSources();
				SlackHarvesterServices.RemoveSourceFolders();
			}
			if (SlackHarvesterServices.CanDoManualCleanup)
			{
				Application.Run(new SlackHarvesterWnd());
			}
		}
	}
}
