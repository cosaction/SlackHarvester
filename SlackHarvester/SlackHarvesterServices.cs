// Copyright (c) 2019-2021 Convention of States Action
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SlackHarvester
{
	internal static class SlackHarvesterServices
	{
		private static string _exportedSlackBaseFolder;
		private static string _exportedSlackSourceFolder;
		private static string _manualProcessingFolder;
		private const string ZIPSources = "ZIPSources";

		private static string ExportedSlackBaseFolder
		{
			set
			{
				if (!Directory.Exists(value))
				{
					throw new ArgumentException($"'{value}' does not exist.");
				}
				_exportedSlackBaseFolder = value;
			}
			get => _exportedSlackBaseFolder;
		}

		private static string ExportedSlackSourceFolder
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_exportedSlackSourceFolder))
				{
					_exportedSlackSourceFolder = Path.Combine(_exportedSlackBaseFolder, "Source");
				}
				return _exportedSlackSourceFolder;
			}
		}

		internal static string ManualProcessingFolder
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_manualProcessingFolder))
				{
					_manualProcessingFolder = Path.Combine(_exportedSlackBaseFolder, "ManualProcessing");
				}
				return _manualProcessingFolder;
			}
		}

		internal static List<SlackFile> HarvestSlackData(string exportedSlackBaseFolder)
		{
			ExportedSlackBaseFolder = exportedSlackBaseFolder;
			var slackFilesToManuallyProcess = new List<SlackFile>();
			var exportedSlackFolders = Directory.GetDirectories(ExportedSlackSourceFolder)
				.Where(folder => !folder.EndsWith(ZIPSources)).ToList();
			if (Directory.Exists(ExportedSlackBaseFolder) && exportedSlackFolders.Any())
			{
				// Delete any xml files in ManualProcessingFolder
				foreach (var file in Directory.GetFiles(ManualProcessingFolder))
				{
					File.Delete(file);
				}
				slackFilesToManuallyProcess.AddRange(exportedSlackFolders
					.Select(currentExportedSlackSourceFolder => new SlackWorkspace(currentExportedSlackSourceFolder))
					.Select(slackWorkspace => slackWorkspace.ProcessWorkspace()));
				foreach (var gonerFolder in exportedSlackFolders)
				{
					Directory.Delete(gonerFolder, true);
				}
			}
			else if (Directory.Exists(ManualProcessingFolder) && Directory.GetFiles(ManualProcessingFolder, "*.xml").Any())
			{
				// Load extant xml files, if any, since there were no new Slack exports to process.
				slackFilesToManuallyProcess.AddRange(Directory.GetFiles(SlackHarvesterServices.ManualProcessingFolder, "*.xml", SearchOption.TopDirectoryOnly)
					.Select(xmlPathname => new SlackFile(xmlPathname)).Where(slackFile => slackFile.RootData.HasElements));
			}
			return slackFilesToManuallyProcess;
		}
	}
}