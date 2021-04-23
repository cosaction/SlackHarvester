// Copyright (c) 2019-2021 Convention of States Action
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace SlackHarvester
{
	/// <summary>
	/// The idea of this window is to take the output of the basic harvester and then remove all material that is not suited for being a HD KB FAQ.
	///
	/// [NB: One ought not run the core harvester multiple times on the same set of data, or it will make more manual cleanup work.]
	/// </summary>
	public partial class SlackHarvesterWnd : Form
	{
		private List<SlackFile> _slackFiles;
		private bool _docIsDirty;

		public SlackHarvesterWnd()
		{
			InitializeComponent();
		}

		internal SlackHarvesterWnd(List<SlackFile> slackFilesToManuallyProcess)
			: this()
		{
			// Populate the toolStripComboBoxSlackFiles combo box.
			_slackFiles = slackFilesToManuallyProcess;
			foreach (var slackFile in _slackFiles)
			{
				if (!slackFile.RootData.HasElements)
				{
					// It was empty, so was deleted in the constructor, so skip adding it.
					continue;
				}
				slackFile.SlackFileWasDeleted += SlackFileWasDeletedHandler;
				_toolStripComboBoxSlackFiles.Items.Add(slackFile);
			}
		}

		private void SlackFileWasDeletedHandler(object sender, EventArgs e)
		{
			var slackFile = (SlackFile)sender;
			_slackFiles.Remove(slackFile);
			_toolStripComboBoxSlackFiles.Items.Remove(slackFile);
			if (_toolStripComboBoxSlackFiles.Items.Count > 0)
			{
				_toolStripComboBoxSlackFiles.SelectedIndex = 0;
			}
			else
			{
				// Nothing left to do, so close program.
				Close();
			}
		}

		private void ToolStripComboBoxSlackFiles_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Wipe out contents of _splitContainer, and replace with newly selected content
			var comboBox = (ToolStripComboBox)sender;
			PopulateTreeView((SlackFile)comboBox.SelectedItem);
		}

		private void PopulateTreeView(SlackFile slackFile)
		{
			var priorSlackFile = _treeView.Tag as SlackFile;
			priorSlackFile?.Save();
			_treeView.Nodes.Clear();
			_treeView.Tag = slackFile;
			var elementsWithNoChildElements = new List<XElement>();
			foreach (var channelElement in slackFile.RootData.Elements("channel"))
			{
				if (!channelElement.HasElements)
				{
					elementsWithNoChildElements.Add(channelElement);
					continue;
				}
				var channelNode = new TreeNode(channelElement.Attribute("uiName").Value)
				{
					Tag = channelElement
				};
				_treeView.Nodes.Add(channelNode);
				foreach (var messageElement in channelElement.Elements("messagesForDate"))
				{
					if (!messageElement.HasElements)
					{
						elementsWithNoChildElements.Add(messageElement);
						continue;
					}
					var dateNode = new TreeNode(messageElement.Attribute("date").Value)
					{
						Tag = messageElement
					};
					channelNode.Nodes.Add(dateNode);
				}
			}
			if (!elementsWithNoChildElements.Any())
			{
				return;
			}
			foreach (var elementWithNoChildElements in elementsWithNoChildElements)
			{
				elementWithNoChildElements.Remove();
			}
			slackFile.Save();
		}

		private void SlackHarvesterWnd_Load(object sender, EventArgs e)
		{
			if (_toolStripComboBoxSlackFiles.Items.Count > 0)
			{
				_toolStripComboBoxSlackFiles.SelectedIndex = 0;
			}
			else
			{
				// Nothing to do, so close.
				Close();
			}
		}

		private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
		{
			var selectedTreeNode = _treeView.SelectedNode;
			if (selectedTreeNode == null)
			{
				return;
			}
			if (_treeView.Nodes.Contains(selectedTreeNode))
			{
				_treeView.SelectedNode = null;
				return;
			}
			// Set content in right side of split container (ListView).
			_listView.Items.Clear();
			_listView.Groups.Clear();
			_listView.SuspendLayout();
			var selectedDateElement = (XElement)selectedTreeNode.Tag;
			foreach (var messageElement in selectedDateElement.Elements())
			{
				switch (messageElement.Attribute("type").Value)
				{
					case "threaded":
						var mainMessageElement = messageElement.Element("mainMessage");
						var threadedGroup = new ListViewGroup(mainMessageElement.Attribute("name").Value, HorizontalAlignment.Left);
						foreach (var textElement in mainMessageElement.Descendants("text"))
						{
							var textElementParent = textElement.Parent;
							var threadedListViewItem = new ListViewItem(new[] { string.Empty, "T", $"{textElementParent.Attribute("name").Value}: {textElement.Value}" }, threadedGroup)
							{
								Tag = textElement
							};
							_listView.Items.Add(threadedListViewItem);
						}
						_listView.Groups.Add(threadedGroup);
						break;
					case "solo":
						var soloListItem = new ListViewItem(new[] { string.Empty, "S", $"{messageElement.Attribute("name").Value}: {messageElement.Element("text").Value}" })
						{
							Tag = messageElement
						};
						_listView.Items.Add(soloListItem);
						break;
				}
				_listView.Columns[2].Width = -2;
				_listView.ResumeLayout(true);
			}
		}

		private void SlackHarvesterWnd_FormClosing(object sender, FormClosingEventArgs e)
		{
			// Save current doc.
			if (_docIsDirty)
			{
				((SlackFile)_treeView.Tag).Save();
				_docIsDirty = false;
			}
		}

		private void DeleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// Selected tree node, not list item.
			var selectedTreeNode = _treeView.SelectedNode;
			if (selectedTreeNode == null)
			{
				return;
			}
			if (MessageBox.Show(@"Are you sure you want to delete the entire tree node?", @"Delete Main Node", MessageBoxButtons.OKCancel) == DialogResult.OK)
			{
				RemoveTreeNode(selectedTreeNode);
			}
		}

		private void RemoveTreeNode(TreeNode gonerTreeNode)
		{
			_treeView.SuspendLayout();
			while (true)
			{
				var selectedElement = (XElement)gonerTreeNode.Tag;
				selectedElement.Remove();
				var parentNode = gonerTreeNode.Parent;
				gonerTreeNode.Remove();
				if (parentNode == null)
				{
					break;
				}
				selectedElement = (XElement)parentNode.Tag;
				if (!selectedElement.HasElements)
				{
					// Delete parent node, since its xml is now empty, as well.
					gonerTreeNode = parentNode;
					continue;
				}
				break;
			}
			_treeView.ResumeLayout(false);
			((SlackFile)_treeView.Tag).Save();
			_docIsDirty = false;
		}

		private void ContextMenuStripListItem_Opening(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// Enable/Disable "deleteAllToolStripMenuItem", depending on there being at least one checked list item.
			deleteAllToolStripMenuItem.Enabled = _listView.CheckedItems.Count > 0 || _listView.SelectedItems.Count > 0;
		}

		private void DeleteAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// Checked list items to delete.
			if (MessageBox.Show(@"Are you sure you want to delete all of the checked/selected list items?", @"Delete All Item(s)", MessageBoxButtons.OKCancel) != DialogResult.OK)
			{
				return;
			}
			var selectedListViewItems = _listView.CheckedItems.Count > 0 ? _listView.CheckedItems.Cast<ListViewItem>().Reverse().ToList() : _listView.SelectedItems.Cast<ListViewItem>().Reverse().ToList();
			_docIsDirty = true;
			var selectedTreeNode = _treeView.SelectedNode;
			var selectedElement = (XElement)selectedTreeNode.Tag;
			if (selectedElement.HasElements)
			{
				var goners = new List<ListViewItem>(selectedListViewItems.Count);
				var groups = new HashSet<ListViewGroup>();
				foreach (var selectedListViewItem in selectedListViewItems)
				{
					if (selectedListViewItem.Group != null)
					{
						groups.Add(selectedListViewItem.Group);
					}
					goners.Add(selectedListViewItem);
					var currentElement = (XElement)selectedListViewItem.Tag;
					var ancestorElements = currentElement.Ancestors().ToList();
					currentElement.Remove();
					ProcessAncestors(ancestorElements);
				}
				foreach (var goner in goners)
				{
					goner.Remove();
				}
				foreach (var gonerGroup in groups.Where(group => @group.Items.Count == 0))
				{
					_listView.Groups.Remove(gonerGroup);
				}
			}
			else
			{
				_treeView.SuspendLayout();
				selectedElement.Remove();
				selectedTreeNode.Remove();
				_treeView.ResumeLayout(true);
				_listView.SuspendLayout();
				_listView.Items.Clear();
				_listView.Groups.Clear();
				_listView.ResumeLayout(true);
			}
			if (_docIsDirty)
			{
				((SlackFile)_treeView.Tag).Save();
				_docIsDirty = false;
			}

			void ProcessAncestors(IEnumerable<XElement> ancestorElements)
			{
				foreach (var ancestorElement in ancestorElements)
				{
					if (ancestorElement.HasElements)
					{
						break;
					}
					switch (ancestorElement.Name.LocalName)
					{
						case "root":
							// The entire document is empty. It will be deleted and another one selected, if there is one.
							return;
						case "channel":
						case "messagesForDate":
							// The tree view needs to be updated, since one of its elements is being removed
							PopulateTreeView((SlackFile)_treeView.Tag);
							break;
						case "message":
						case "replyMessage":
						case "mainMessage":
							break;
					}
					if (ancestorElement.Parent != null)
					{
						ancestorElement.Remove();
					}
				}
			}
		}

		private void ShowFullText_Click(object sender, EventArgs e)
		{
			var selectedText = string.Empty;
			var spacer = string.Empty;
			var listViewSelectedItems = _listView.SelectedItems;
			foreach (ListViewItem item in listViewSelectedItems)
			{

				var selectedElement = (XElement)item.Tag;
				selectedText = selectedText + spacer + selectedElement.Value;
				spacer = " ";
			}
			MessageBox.Show(selectedText, "Full Text", MessageBoxButtons.OK);
		}

		private void SlackHarvesterWnd_Resize(object sender, EventArgs e)
		{
			_listView.Columns[2].Width = -2;
		}

		private void _treeView_KeyDown(object sender, KeyEventArgs e)
		{
			var selectedTreeNode = _treeView.SelectedNode;
			if (selectedTreeNode == null)
			{
				return;
			}
			if (_treeView.Nodes.Contains(selectedTreeNode))
			{
				// Skip top level node.
				_treeView.SelectedNode = null;
				return;
			}
			if (e.KeyCode == Keys.Delete)
			{
				// Delete the selected node.
				RemoveTreeNode(selectedTreeNode);
				e.Handled = true;
			}
		}
	}
}