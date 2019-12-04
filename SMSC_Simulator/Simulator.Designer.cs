namespace SMSC_Simulator
{
    partial class Simulator
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
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("");
            this.treeMethods = new System.Windows.Forms.TreeView();
            this.treeInput = new System.Windows.Forms.TreeView();
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.loadFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reloadFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.pathTree = new System.Windows.Forms.TreeView();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.lbAddress = new System.Windows.Forms.Label();
            this.lbMethod = new System.Windows.Forms.Label();
            this.cmbAddress = new System.Windows.Forms.ComboBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.cmbMethodName = new System.Windows.Forms.ComboBox();
            this.rtbRequest = new System.Windows.Forms.RichTextBox();
            this.rtbReply = new System.Windows.Forms.RichTextBox();
            this.pathContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.sendToolStrip = new System.Windows.Forms.ToolStripMenuItem();
            this.myTimer = new System.Windows.Forms.Timer(this.components);
            this.rtbContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripToXML = new System.Windows.Forms.ToolStripMenuItem();
            this.toDispatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripClear = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cbToDispatcher = new System.Windows.Forms.CheckBox();
            this.folderContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.runMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearStartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lbHowManyNodesSend = new System.Windows.Forms.Label();
            this.lbCurrentCount = new System.Windows.Forms.Label();
            this.fileContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SetStartStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SetEndStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnStop = new System.Windows.Forms.Button();
            this.mainMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.pathContextMenu.SuspendLayout();
            this.rtbContextMenu.SuspendLayout();
            this.folderContextMenu.SuspendLayout();
            this.fileContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeMethods
            // 
            this.treeMethods.LineColor = System.Drawing.Color.Empty;
            this.treeMethods.Location = new System.Drawing.Point(0, 0);
            this.treeMethods.Name = "treeMethods";
            this.treeMethods.Size = new System.Drawing.Size(121, 97);
            this.treeMethods.TabIndex = 0;
            // 
            // treeInput
            // 
            this.treeInput.LineColor = System.Drawing.Color.Empty;
            this.treeInput.Location = new System.Drawing.Point(0, 0);
            this.treeInput.Name = "treeInput";
            this.treeInput.Size = new System.Drawing.Size(121, 97);
            this.treeInput.TabIndex = 0;
            // 
            // mainMenu
            // 
            this.mainMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadFileToolStripMenuItem,
            this.reloadFolderToolStripMenuItem});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Padding = new System.Windows.Forms.Padding(4, 1, 0, 1);
            this.mainMenu.Size = new System.Drawing.Size(1338, 24);
            this.mainMenu.TabIndex = 0;
            this.mainMenu.Text = "CurrentCount";
            // 
            // loadFileToolStripMenuItem
            // 
            this.loadFileToolStripMenuItem.Name = "loadFileToolStripMenuItem";
            this.loadFileToolStripMenuItem.Size = new System.Drawing.Size(72, 22);
            this.loadFileToolStripMenuItem.Text = "LoadFolder";
            this.loadFileToolStripMenuItem.Click += new System.EventHandler(this.loadFileToolStripMenuItem_Click);
            // 
            // reloadFolderToolStripMenuItem
            // 
            this.reloadFolderToolStripMenuItem.Name = "reloadFolderToolStripMenuItem";
            this.reloadFolderToolStripMenuItem.Size = new System.Drawing.Size(82, 22);
            this.reloadFolderToolStripMenuItem.Text = "ReloadFolder";
            this.reloadFolderToolStripMenuItem.Click += new System.EventHandler(this.reloadFolderToolStripMenuItem_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.pathTree);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1338, 502);
            this.splitContainer1.SplitterDistance = 259;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 1;
            // 
            // pathTree
            // 
            this.pathTree.AllowDrop = true;
            this.pathTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pathTree.Location = new System.Drawing.Point(0, 0);
            this.pathTree.Margin = new System.Windows.Forms.Padding(2);
            this.pathTree.Name = "pathTree";
            treeNode4.Name = "RootNode";
            treeNode4.Text = "";
            this.pathTree.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode4});
            this.pathTree.Size = new System.Drawing.Size(259, 502);
            this.pathTree.TabIndex = 0;
            this.pathTree.DragEnter += new System.Windows.Forms.DragEventHandler(this.pathTree_DragEnter);
            this.pathTree.DragOver += new System.Windows.Forms.DragEventHandler(this.pathTree_DragOver);
            this.pathTree.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pathTree_MouseDown);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer3);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.rtbReply);
            this.splitContainer2.Size = new System.Drawing.Size(1076, 502);
            this.splitContainer2.SplitterDistance = 274;
            this.splitContainer2.SplitterWidth = 3;
            this.splitContainer2.TabIndex = 0;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.lbAddress);
            this.splitContainer3.Panel1.Controls.Add(this.lbMethod);
            this.splitContainer3.Panel1.Controls.Add(this.cmbAddress);
            this.splitContainer3.Panel1.Controls.Add(this.btnSend);
            this.splitContainer3.Panel1.Controls.Add(this.cmbMethodName);
            this.splitContainer3.Panel1.SizeChanged += new System.EventHandler(this.splitContainer3_Panel1_SizeChanged);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.rtbRequest);
            this.splitContainer3.Size = new System.Drawing.Size(1076, 274);
            this.splitContainer3.SplitterDistance = 44;
            this.splitContainer3.SplitterWidth = 3;
            this.splitContainer3.TabIndex = 0;
            // 
            // lbAddress
            // 
            this.lbAddress.AutoSize = true;
            this.lbAddress.Location = new System.Drawing.Point(7, 16);
            this.lbAddress.Name = "lbAddress";
            this.lbAddress.Size = new System.Drawing.Size(45, 13);
            this.lbAddress.TabIndex = 4;
            this.lbAddress.Text = "Address";
            // 
            // lbMethod
            // 
            this.lbMethod.AutoSize = true;
            this.lbMethod.Location = new System.Drawing.Point(556, 17);
            this.lbMethod.Name = "lbMethod";
            this.lbMethod.Size = new System.Drawing.Size(74, 13);
            this.lbMethod.TabIndex = 3;
            this.lbMethod.Text = "Method Name";
            // 
            // cmbAddress
            // 
            this.cmbAddress.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAddress.FormattingEnabled = true;
            this.cmbAddress.Location = new System.Drawing.Point(61, 13);
            this.cmbAddress.Margin = new System.Windows.Forms.Padding(2);
            this.cmbAddress.Name = "cmbAddress";
            this.cmbAddress.Size = new System.Drawing.Size(478, 21);
            this.cmbAddress.TabIndex = 2;
            this.cmbAddress.TextChanged += new System.EventHandler(this.cmbAddress_TextChanged);
            // 
            // btnSend
            // 
            this.btnSend.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSend.Location = new System.Drawing.Point(989, 9);
            this.btnSend.Margin = new System.Windows.Forms.Padding(2);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(55, 27);
            this.btnSend.TabIndex = 1;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // cmbMethodName
            // 
            this.cmbMethodName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMethodName.FormattingEnabled = true;
            this.cmbMethodName.Location = new System.Drawing.Point(638, 13);
            this.cmbMethodName.Margin = new System.Windows.Forms.Padding(2);
            this.cmbMethodName.Name = "cmbMethodName";
            this.cmbMethodName.Size = new System.Drawing.Size(336, 21);
            this.cmbMethodName.TabIndex = 0;
            // 
            // rtbRequest
            // 
            this.rtbRequest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbRequest.Location = new System.Drawing.Point(0, 0);
            this.rtbRequest.Margin = new System.Windows.Forms.Padding(2);
            this.rtbRequest.Name = "rtbRequest";
            this.rtbRequest.Size = new System.Drawing.Size(1076, 227);
            this.rtbRequest.TabIndex = 0;
            this.rtbRequest.Text = "";
            this.rtbRequest.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rtbRequest_KeyDown);
            this.rtbRequest.MouseUp += new System.Windows.Forms.MouseEventHandler(this.rtbRequest_MouseUp);
            // 
            // rtbReply
            // 
            this.rtbReply.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbReply.Location = new System.Drawing.Point(0, 0);
            this.rtbReply.Margin = new System.Windows.Forms.Padding(2);
            this.rtbReply.Name = "rtbReply";
            this.rtbReply.Size = new System.Drawing.Size(1076, 225);
            this.rtbReply.TabIndex = 1;
            this.rtbReply.Text = "";
            this.rtbReply.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rtbReply_KeyDown);
            this.rtbReply.MouseUp += new System.Windows.Forms.MouseEventHandler(this.rtbReply_MouseUp);
            // 
            // pathContextMenu
            // 
            this.pathContextMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.pathContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sendToolStrip});
            this.pathContextMenu.Name = "pathContextMenu";
            this.pathContextMenu.Size = new System.Drawing.Size(99, 26);
            // 
            // sendToolStrip
            // 
            this.sendToolStrip.Name = "sendToolStrip";
            this.sendToolStrip.Size = new System.Drawing.Size(98, 22);
            this.sendToolStrip.Text = "Send";
            this.sendToolStrip.Click += new System.EventHandler(this.sendToolStrip_Click);
            // 
            // myTimer
            // 
            this.myTimer.Tick += new System.EventHandler(this.myTimer_Tick);
            // 
            // rtbContextMenu
            // 
            this.rtbContextMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.rtbContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripToXML,
            this.toDispatchToolStripMenuItem,
            this.toolStripClear,
            this.saveToFileToolStripMenuItem});
            this.rtbContextMenu.Name = "pathContextMenu";
            this.rtbContextMenu.Size = new System.Drawing.Size(128, 92);
            // 
            // toolStripToXML
            // 
            this.toolStripToXML.Name = "toolStripToXML";
            this.toolStripToXML.Size = new System.Drawing.Size(127, 22);
            this.toolStripToXML.Text = "ToXML";
            this.toolStripToXML.Click += new System.EventHandler(this.toolStripToXML_Click);
            // 
            // toDispatchToolStripMenuItem
            // 
            this.toDispatchToolStripMenuItem.Name = "toDispatchToolStripMenuItem";
            this.toDispatchToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.toDispatchToolStripMenuItem.Text = "ToDispatch";
            this.toDispatchToolStripMenuItem.Click += new System.EventHandler(this.toDispatchToolStripMenuItem_Click);
            // 
            // toolStripClear
            // 
            this.toolStripClear.Name = "toolStripClear";
            this.toolStripClear.Size = new System.Drawing.Size(127, 22);
            this.toolStripClear.Text = "Clear";
            this.toolStripClear.Click += new System.EventHandler(this.toolStripClear_Click);
            // 
            // saveToFileToolStripMenuItem
            // 
            this.saveToFileToolStripMenuItem.Name = "saveToFileToolStripMenuItem";
            this.saveToFileToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.saveToFileToolStripMenuItem.Text = "SaveToFile";
            this.saveToFileToolStripMenuItem.Click += new System.EventHandler(this.saveToFileToolStripMenuItem_Click);
            // 
            // cbToDispatcher
            // 
            this.cbToDispatcher.AutoSize = true;
            this.cbToDispatcher.Location = new System.Drawing.Point(174, 2);
            this.cbToDispatcher.Name = "cbToDispatcher";
            this.cbToDispatcher.Size = new System.Drawing.Size(112, 17);
            this.cbToDispatcher.TabIndex = 2;
            this.cbToDispatcher.Text = "AutoToDispatcher";
            this.cbToDispatcher.UseVisualStyleBackColor = true;
            // 
            // folderContextMenu
            // 
            this.folderContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runMenuItem,
            this.stopToolStripMenuItem,
            this.clearStartToolStripMenuItem});
            this.folderContextMenu.Name = "folderContextMenu";
            this.folderContextMenu.Size = new System.Drawing.Size(144, 70);
            // 
            // runMenuItem
            // 
            this.runMenuItem.Name = "runMenuItem";
            this.runMenuItem.Size = new System.Drawing.Size(143, 22);
            this.runMenuItem.Text = "Run All";
            this.runMenuItem.Click += new System.EventHandler(this.runMenuItem_Click);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.stopToolStripMenuItem.Text = "Stop Run";
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
            // 
            // clearStartToolStripMenuItem
            // 
            this.clearStartToolStripMenuItem.Name = "clearStartToolStripMenuItem";
            this.clearStartToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.clearStartToolStripMenuItem.Text = "Clear Duration";
            this.clearStartToolStripMenuItem.Click += new System.EventHandler(this.clearStartToolStripMenuItem_Click);
            // 
            // lbHowManyNodesSend
            // 
            this.lbHowManyNodesSend.AutoSize = true;
            this.lbHowManyNodesSend.Location = new System.Drawing.Point(342, 4);
            this.lbHowManyNodesSend.Name = "lbHowManyNodesSend";
            this.lbHowManyNodesSend.Size = new System.Drawing.Size(94, 13);
            this.lbHowManyNodesSend.TabIndex = 4;
            this.lbHowManyNodesSend.Text = "Multi Nodes Send:";
            // 
            // lbCurrentCount
            // 
            this.lbCurrentCount.AutoSize = true;
            this.lbCurrentCount.Location = new System.Drawing.Point(440, 4);
            this.lbCurrentCount.Name = "lbCurrentCount";
            this.lbCurrentCount.Size = new System.Drawing.Size(69, 13);
            this.lbCurrentCount.TabIndex = 5;
            this.lbCurrentCount.Text = "CurrentCount";
            // 
            // fileContextMenu
            // 
            this.fileContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SetStartStripMenuItem,
            this.SetEndStripMenuItem});
            this.fileContextMenu.Name = "fileContextMenu";
            this.fileContextMenu.Size = new System.Drawing.Size(118, 48);
            // 
            // SetStartStripMenuItem
            // 
            this.SetStartStripMenuItem.Name = "SetStartStripMenuItem";
            this.SetStartStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.SetStartStripMenuItem.Text = "Set Start";
            this.SetStartStripMenuItem.Click += new System.EventHandler(this.SetStartStripMenuItem_Click);
            // 
            // SetEndStripMenuItem
            // 
            this.SetEndStripMenuItem.Name = "SetEndStripMenuItem";
            this.SetEndStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.SetEndStripMenuItem.Text = "Set End";
            this.SetEndStripMenuItem.Click += new System.EventHandler(this.SetEndStripMenuItem_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(547, -1);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(48, 22);
            this.btnStop.TabIndex = 6;
            this.btnStop.Text = "STOP";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // Simulator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1338, 526);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.lbCurrentCount);
            this.Controls.Add(this.lbHowManyNodesSend);
            this.Controls.Add(this.cbToDispatcher);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.mainMenu);
            this.MainMenuStrip = this.mainMenu;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Simulator";
            this.Text = "Simulator";
            this.Load += new System.EventHandler(this.Simulator_Load);
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel1.PerformLayout();
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.pathContextMenu.ResumeLayout(false);
            this.rtbContextMenu.ResumeLayout(false);
            this.folderContextMenu.ResumeLayout(false);
            this.fileContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem loadFileToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView pathTree;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.RichTextBox rtbRequest;
        private System.Windows.Forms.RichTextBox rtbReply;
        private System.Windows.Forms.ContextMenuStrip pathContextMenu;
        private System.Windows.Forms.ToolStripMenuItem sendToolStrip;
        private System.Windows.Forms.ComboBox cmbMethodName;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Timer myTimer;
        private System.Windows.Forms.ContextMenuStrip rtbContextMenu;
        private System.Windows.Forms.ToolStripMenuItem toolStripToXML;
        private System.Windows.Forms.ToolStripMenuItem toolStripClear;
        private System.Windows.Forms.Label lbAddress;
        private System.Windows.Forms.Label lbMethod;
        private System.Windows.Forms.ComboBox cmbAddress;
        private System.Windows.Forms.ToolStripMenuItem saveToFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toDispatchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reloadFolderToolStripMenuItem;
        private System.Windows.Forms.CheckBox cbToDispatcher;
        private System.Windows.Forms.ContextMenuStrip folderContextMenu;
        private System.Windows.Forms.ToolStripMenuItem runMenuItem;
        private System.Windows.Forms.Label lbHowManyNodesSend;
        private System.Windows.Forms.Label lbCurrentCount;
        private System.Windows.Forms.ContextMenuStrip fileContextMenu;
        private System.Windows.Forms.ToolStripMenuItem SetStartStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SetEndStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearStartToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.Button btnStop;
    }
}

