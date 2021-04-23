// Copyright (c) 2019-2021 Convention of States Action
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System;
using System.Linq;
using System.Windows.Forms;

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
			var slackFilesToManuallyProcess = SlackHarvesterServices.HarvestSlackData(args[0]);
			if (slackFilesToManuallyProcess.Any())
			{
				Application.Run(new SlackHarvesterWnd(slackFilesToManuallyProcess));
			}
			else
			{
				MessageBox.Show("Nothing to do!", "Manual Processing", MessageBoxButtons.OK);
			}
		}
	}
}