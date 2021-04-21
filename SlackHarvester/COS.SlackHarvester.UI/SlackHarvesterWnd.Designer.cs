// Copyright (c) 2019-2021 Convention of States Action
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

namespace SlackHarvester.COS.SlackHarvester.UI
{
    partial class SlackHarvesterWnd
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			this._toolStripContainer = new System.Windows.Forms.ToolStripContainer();
			this._splitContainer = new System.Windows.Forms.SplitContainer();
			this._treeView = new System.Windows.Forms.TreeView();
			this._contextMenuStripTreeView = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this._listView = new System.Windows.Forms.ListView();
			this._checkBox = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this._typeColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this._dataColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this._contextMenuStripListItem = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.deleteAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.showFullText = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this._toolStripComboBoxSlackFiles = new System.Windows.Forms.ToolStripComboBox();
			this._toolStripContainer.ContentPanel.SuspendLayout();
			this._toolStripContainer.TopToolStripPanel.SuspendLayout();
			this._toolStripContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._splitContainer)).BeginInit();
			this._splitContainer.Panel1.SuspendLayout();
			this._splitContainer.Panel2.SuspendLayout();
			this._splitContainer.SuspendLayout();
			this._contextMenuStripTreeView.SuspendLayout();
			this._contextMenuStripListItem.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _toolStripContainer
			// 
			this._toolStripContainer.BottomToolStripPanelVisible = false;
			// 
			// _toolStripContainer.ContentPanel
			// 
			this._toolStripContainer.ContentPanel.Controls.Add(this._splitContainer);
			this._toolStripContainer.ContentPanel.Size = new System.Drawing.Size(1133, 545);
			this._toolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this._toolStripContainer.LeftToolStripPanelVisible = false;
			this._toolStripContainer.Location = new System.Drawing.Point(0, 0);
			this._toolStripContainer.Name = "_toolStripContainer";
			this._toolStripContainer.RightToolStripPanelVisible = false;
			this._toolStripContainer.Size = new System.Drawing.Size(1133, 570);
			this._toolStripContainer.TabIndex = 0;
			this._toolStripContainer.Text = "toolStripContainer";
			// 
			// _toolStripContainer.TopToolStripPanel
			// 
			this._toolStripContainer.TopToolStripPanel.Controls.Add(this.toolStrip1);
			// 
			// _splitContainer
			// 
			this._splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this._splitContainer.Location = new System.Drawing.Point(0, 0);
			this._splitContainer.Name = "_splitContainer";
			// 
			// _splitContainer.Panel1
			// 
			this._splitContainer.Panel1.Controls.Add(this._treeView);
			// 
			// _splitContainer.Panel2
			// 
			this._splitContainer.Panel2.Controls.Add(this._listView);
			this._splitContainer.Size = new System.Drawing.Size(1133, 545);
			this._splitContainer.SplitterDistance = 122;
			this._splitContainer.TabIndex = 0;
			// 
			// _treeView
			// 
			this._treeView.ContextMenuStrip = this._contextMenuStripTreeView;
			this._treeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this._treeView.FullRowSelect = true;
			this._treeView.HideSelection = false;
			this._treeView.Location = new System.Drawing.Point(0, 0);
			this._treeView.Name = "_treeView";
			this._treeView.Size = new System.Drawing.Size(122, 545);
			this._treeView.TabIndex = 0;
			this._treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeView_AfterSelect);
			this._treeView.KeyDown += new System.Windows.Forms.KeyEventHandler(this._treeView_KeyDown);
			// 
			// _contextMenuStripTreeView
			// 
			this._contextMenuStripTreeView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem});
			this._contextMenuStripTreeView.Name = "_contextMenuStripTreeView";
			this._contextMenuStripTreeView.Size = new System.Drawing.Size(210, 26);
			// 
			// deleteToolStripMenuItem
			// 
			this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
			this.deleteToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
			this.deleteToolStripMenuItem.Text = "Delete Whole Tree Node...";
			this.deleteToolStripMenuItem.Click += new System.EventHandler(this.DeleteToolStripMenuItem_Click);
			// 
			// _listView
			// 
			this._listView.Activation = System.Windows.Forms.ItemActivation.OneClick;
			this._listView.CheckBoxes = true;
			this._listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this._checkBox,
            this._typeColumn,
            this._dataColumn});
			this._listView.ContextMenuStrip = this._contextMenuStripListItem;
			this._listView.Dock = System.Windows.Forms.DockStyle.Fill;
			this._listView.FullRowSelect = true;
			this._listView.GridLines = true;
			this._listView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this._listView.HideSelection = false;
			this._listView.Location = new System.Drawing.Point(0, 0);
			this._listView.Name = "_listView";
			this._listView.Size = new System.Drawing.Size(1007, 545);
			this._listView.TabIndex = 0;
			this._listView.UseCompatibleStateImageBehavior = false;
			this._listView.View = System.Windows.Forms.View.Details;
			// 
			// _checkBox
			// 
			this._checkBox.Text = "";
			this._checkBox.Width = 20;
			// 
			// _typeColumn
			// 
			this._typeColumn.Text = "T";
			this._typeColumn.Width = 20;
			// 
			// _dataColumn
			// 
			this._dataColumn.Text = "Data";
			this._dataColumn.Width = 760;
			// 
			// _contextMenuStripListItem
			// 
			this._contextMenuStripListItem.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteAllToolStripMenuItem,
            this.showFullText});
			this._contextMenuStripListItem.Name = "_contextMenuStripListItem";
			this._contextMenuStripListItem.Size = new System.Drawing.Size(286, 48);
			this._contextMenuStripListItem.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuStripListItem_Opening);
			// 
			// deleteAllToolStripMenuItem
			// 
			this.deleteAllToolStripMenuItem.Name = "deleteAllToolStripMenuItem";
			this.deleteAllToolStripMenuItem.Size = new System.Drawing.Size(285, 22);
			this.deleteAllToolStripMenuItem.Text = "Delete All Checked/Selected Messages...";
			this.deleteAllToolStripMenuItem.Click += new System.EventHandler(this.DeleteAllToolStripMenuItem_Click);
			// 
			// showFullText
			// 
			this.showFullText.Name = "showFullText";
			this.showFullText.Size = new System.Drawing.Size(285, 22);
			this.showFullText.Text = "Show All Text";
			this.showFullText.Click += new System.EventHandler(this.ShowFullText_Click);
			// 
			// toolStrip1
			// 
			this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._toolStripComboBoxSlackFiles});
			this.toolStrip1.Location = new System.Drawing.Point(3, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(194, 25);
			this.toolStrip1.TabIndex = 1;
			// 
			// _toolStripComboBoxSlackFiles
			// 
			this._toolStripComboBoxSlackFiles.AutoSize = false;
			this._toolStripComboBoxSlackFiles.DropDownWidth = 160;
			this._toolStripComboBoxSlackFiles.MaxDropDownItems = 20;
			this._toolStripComboBoxSlackFiles.Name = "_toolStripComboBoxSlackFiles";
			this._toolStripComboBoxSlackFiles.Size = new System.Drawing.Size(180, 23);
			this._toolStripComboBoxSlackFiles.SelectedIndexChanged += new System.EventHandler(this.ToolStripComboBoxSlackFiles_SelectedIndexChanged);
			// 
			// SlackHarvesterWnd
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1133, 570);
			this.Controls.Add(this._toolStripContainer);
			this.Name = "SlackHarvesterWnd";
			this.Text = "Slack Harvester";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SlackHarvesterWnd_FormClosing);
			this.Load += new System.EventHandler(this.SlackHarvesterWnd_Load);
			this.Resize += new System.EventHandler(this.SlackHarvesterWnd_Resize);
			this._toolStripContainer.ContentPanel.ResumeLayout(false);
			this._toolStripContainer.TopToolStripPanel.ResumeLayout(false);
			this._toolStripContainer.TopToolStripPanel.PerformLayout();
			this._toolStripContainer.ResumeLayout(false);
			this._toolStripContainer.PerformLayout();
			this._splitContainer.Panel1.ResumeLayout(false);
			this._splitContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this._splitContainer)).EndInit();
			this._splitContainer.ResumeLayout(false);
			this._contextMenuStripTreeView.ResumeLayout(false);
			this._contextMenuStripListItem.ResumeLayout(false);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);

        }

		#endregion

		private System.Windows.Forms.ToolStripContainer _toolStripContainer;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripComboBox _toolStripComboBoxSlackFiles;
        private System.Windows.Forms.SplitContainer _splitContainer;
        private System.Windows.Forms.TreeView _treeView;
        private System.Windows.Forms.ContextMenuStrip _contextMenuStripTreeView;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ListView _listView;
        private System.Windows.Forms.ColumnHeader _typeColumn;
        private System.Windows.Forms.ColumnHeader _dataColumn;
        private System.Windows.Forms.ColumnHeader _checkBox;
        private System.Windows.Forms.ContextMenuStrip _contextMenuStripListItem;
        private System.Windows.Forms.ToolStripMenuItem deleteAllToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem showFullText;
	}
}

