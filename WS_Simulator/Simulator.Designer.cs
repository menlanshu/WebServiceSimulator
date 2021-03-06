﻿namespace WS_Simulator
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Simulator));
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.loadFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadFromFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reloadFromFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadFromDBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveCurrTreeToDBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.myTimer = new System.Windows.Forms.Timer(this.components);
            this.rtbContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripToXML = new System.Windows.Forms.ToolStripMenuItem();
            this.toDispatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripClear = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.escapeXmlMessageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cbToDispatcher = new System.Windows.Forms.CheckBox();
            this.folderContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.runMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearStartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.replaceTextOfAllFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateTestCaseFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateCaseConfigFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameFileFollowOrderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lbHowManyNodesSend = new System.Windows.Forms.Label();
            this.lbCurrentCount = new System.Windows.Forms.Label();
            this.fileContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SetStartStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SetEndStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteFromDBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addNumberBeforeFileNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnStop = new System.Windows.Forms.Button();
            this.cbAutoChangeContext = new System.Windows.Forms.CheckBox();
            this.cbPerfTest = new System.Windows.Forms.CheckBox();
            this.tbMsgCount = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lbCurrentLoop = new System.Windows.Forms.Label();
            this.autoSaveReplyCB = new System.Windows.Forms.CheckBox();
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
            this.rtbContextMenu.SuspendLayout();
            this.folderContextMenu.SuspendLayout();
            this.fileContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadFileToolStripMenuItem});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Padding = new System.Windows.Forms.Padding(5, 1, 0, 1);
            this.mainMenu.Size = new System.Drawing.Size(1553, 24);
            this.mainMenu.TabIndex = 0;
            this.mainMenu.Text = "CurrentCount";
            // 
            // loadFileToolStripMenuItem
            // 
            this.loadFileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadFromFolderToolStripMenuItem,
            this.reloadFromFolderToolStripMenuItem,
            this.loadFromDBToolStripMenuItem,
            this.saveCurrTreeToDBToolStripMenuItem});
            this.loadFileToolStripMenuItem.Name = "loadFileToolStripMenuItem";
            this.loadFileToolStripMenuItem.Size = new System.Drawing.Size(104, 22);
            this.loadFileToolStripMenuItem.Text = "Load Repository";
            // 
            // loadFromFolderToolStripMenuItem
            // 
            this.loadFromFolderToolStripMenuItem.Name = "loadFromFolderToolStripMenuItem";
            this.loadFromFolderToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.loadFromFolderToolStripMenuItem.Text = "LoadFromFolder";
            this.loadFromFolderToolStripMenuItem.Click += new System.EventHandler(this.loadFromFolderToolStripMenuItem_Click);
            // 
            // reloadFromFolderToolStripMenuItem
            // 
            this.reloadFromFolderToolStripMenuItem.Name = "reloadFromFolderToolStripMenuItem";
            this.reloadFromFolderToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.reloadFromFolderToolStripMenuItem.Text = "ReloadFromFolder";
            this.reloadFromFolderToolStripMenuItem.Click += new System.EventHandler(this.reloadFromFolderToolStripMenuItem_Click);
            // 
            // loadFromDBToolStripMenuItem
            // 
            this.loadFromDBToolStripMenuItem.Name = "loadFromDBToolStripMenuItem";
            this.loadFromDBToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.loadFromDBToolStripMenuItem.Text = "LoadFromDB";
            this.loadFromDBToolStripMenuItem.Click += new System.EventHandler(this.loadFromDBToolStripMenuItem_Click);
            // 
            // saveCurrTreeToDBToolStripMenuItem
            // 
            this.saveCurrTreeToDBToolStripMenuItem.Name = "saveCurrTreeToDBToolStripMenuItem";
            this.saveCurrTreeToDBToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.saveCurrTreeToDBToolStripMenuItem.Text = "SaveCurrTreeToDB";
            this.saveCurrTreeToDBToolStripMenuItem.Click += new System.EventHandler(this.saveCurrTreeToDBToolStripMenuItem_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.pathTree);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1553, 734);
            this.splitContainer1.SplitterDistance = 300;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 1;
            // 
            // pathTree
            // 
            this.pathTree.AllowDrop = true;
            this.pathTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pathTree.Location = new System.Drawing.Point(0, 0);
            this.pathTree.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pathTree.Name = "pathTree";
            treeNode1.Name = "RootNode";
            treeNode1.Text = "";
            this.pathTree.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.pathTree.Size = new System.Drawing.Size(300, 734);
            this.pathTree.TabIndex = 0;
            this.pathTree.DragEnter += new System.Windows.Forms.DragEventHandler(this.pathTree_DragEnter);
            this.pathTree.DragOver += new System.Windows.Forms.DragEventHandler(this.pathTree_DragOver);
            this.pathTree.KeyDown += new System.Windows.Forms.KeyEventHandler(this.pathTree_KeyDown);
            this.pathTree.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pathTree_MouseDown);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
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
            this.splitContainer2.Size = new System.Drawing.Size(1250, 734);
            this.splitContainer2.SplitterDistance = 400;
            this.splitContainer2.TabIndex = 0;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
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
            this.splitContainer3.Size = new System.Drawing.Size(1250, 400);
            this.splitContainer3.TabIndex = 0;
            // 
            // lbAddress
            // 
            this.lbAddress.AutoSize = true;
            this.lbAddress.Location = new System.Drawing.Point(8, 18);
            this.lbAddress.Name = "lbAddress";
            this.lbAddress.Size = new System.Drawing.Size(56, 17);
            this.lbAddress.TabIndex = 4;
            this.lbAddress.Text = "Address";
            // 
            // lbMethod
            // 
            this.lbMethod.AutoSize = true;
            this.lbMethod.Location = new System.Drawing.Point(646, 18);
            this.lbMethod.Name = "lbMethod";
            this.lbMethod.Size = new System.Drawing.Size(93, 17);
            this.lbMethod.TabIndex = 3;
            this.lbMethod.Text = "Method Name";
            // 
            // cmbAddress
            // 
            this.cmbAddress.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAddress.FormattingEnabled = true;
            this.cmbAddress.Location = new System.Drawing.Point(71, 15);
            this.cmbAddress.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.cmbAddress.Name = "cmbAddress";
            this.cmbAddress.Size = new System.Drawing.Size(557, 25);
            this.cmbAddress.TabIndex = 2;
            this.cmbAddress.TextChanged += new System.EventHandler(this.cmbAddress_TextChanged);
            // 
            // btnSend
            // 
            this.btnSend.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSend.Location = new System.Drawing.Point(1153, 9);
            this.btnSend.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(64, 35);
            this.btnSend.TabIndex = 1;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // cmbMethodName
            // 
            this.cmbMethodName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMethodName.FormattingEnabled = true;
            this.cmbMethodName.Location = new System.Drawing.Point(744, 15);
            this.cmbMethodName.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.cmbMethodName.Name = "cmbMethodName";
            this.cmbMethodName.Size = new System.Drawing.Size(391, 25);
            this.cmbMethodName.TabIndex = 0;
            this.cmbMethodName.TextChanged += new System.EventHandler(this.cmbMethodName_TextChanged);
            // 
            // rtbRequest
            // 
            this.rtbRequest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbRequest.Location = new System.Drawing.Point(0, 0);
            this.rtbRequest.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.rtbRequest.Name = "rtbRequest";
            this.rtbRequest.Size = new System.Drawing.Size(1250, 346);
            this.rtbRequest.TabIndex = 0;
            this.rtbRequest.Text = "";
            this.rtbRequest.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rtbRequest_KeyDown);
            this.rtbRequest.MouseUp += new System.Windows.Forms.MouseEventHandler(this.rtbRequest_MouseUp);
            // 
            // rtbReply
            // 
            this.rtbReply.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbReply.Location = new System.Drawing.Point(0, 0);
            this.rtbReply.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.rtbReply.Name = "rtbReply";
            this.rtbReply.Size = new System.Drawing.Size(1250, 330);
            this.rtbReply.TabIndex = 1;
            this.rtbReply.Text = "";
            this.rtbReply.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rtbReply_KeyDown);
            this.rtbReply.MouseUp += new System.Windows.Forms.MouseEventHandler(this.rtbReply_MouseUp);
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
            this.saveToFileToolStripMenuItem,
            this.escapeXmlMessageToolStripMenuItem});
            this.rtbContextMenu.Name = "pathContextMenu";
            this.rtbContextMenu.Size = new System.Drawing.Size(178, 114);
            // 
            // toolStripToXML
            // 
            this.toolStripToXML.Name = "toolStripToXML";
            this.toolStripToXML.Size = new System.Drawing.Size(177, 22);
            this.toolStripToXML.Text = "ToXML";
            this.toolStripToXML.Click += new System.EventHandler(this.toolStripToXML_Click);
            // 
            // toDispatchToolStripMenuItem
            // 
            this.toDispatchToolStripMenuItem.Name = "toDispatchToolStripMenuItem";
            this.toDispatchToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.toDispatchToolStripMenuItem.Text = "ToDispatch";
            this.toDispatchToolStripMenuItem.Click += new System.EventHandler(this.toDispatchToolStripMenuItem_Click);
            // 
            // toolStripClear
            // 
            this.toolStripClear.Name = "toolStripClear";
            this.toolStripClear.Size = new System.Drawing.Size(177, 22);
            this.toolStripClear.Text = "Clear";
            this.toolStripClear.Click += new System.EventHandler(this.toolStripClear_Click);
            // 
            // saveToFileToolStripMenuItem
            // 
            this.saveToFileToolStripMenuItem.Name = "saveToFileToolStripMenuItem";
            this.saveToFileToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.saveToFileToolStripMenuItem.Text = "SaveToFile";
            this.saveToFileToolStripMenuItem.Click += new System.EventHandler(this.saveToFileToolStripMenuItem_Click);
            // 
            // escapeXmlMessageToolStripMenuItem
            // 
            this.escapeXmlMessageToolStripMenuItem.Name = "escapeXmlMessageToolStripMenuItem";
            this.escapeXmlMessageToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.escapeXmlMessageToolStripMenuItem.Text = "EscapeXmlMessage";
            this.escapeXmlMessageToolStripMenuItem.Click += new System.EventHandler(this.escapeXmlMessageToolStripMenuItem_Click);
            // 
            // cbToDispatcher
            // 
            this.cbToDispatcher.AutoSize = true;
            this.cbToDispatcher.Location = new System.Drawing.Point(203, 3);
            this.cbToDispatcher.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbToDispatcher.Name = "cbToDispatcher";
            this.cbToDispatcher.Size = new System.Drawing.Size(130, 21);
            this.cbToDispatcher.TabIndex = 2;
            this.cbToDispatcher.Text = "AutoToDispatcher";
            this.cbToDispatcher.UseVisualStyleBackColor = true;
            // 
            // folderContextMenu
            // 
            this.folderContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runMenuItem,
            this.stopToolStripMenuItem,
            this.clearStartToolStripMenuItem,
            this.openFileFolderToolStripMenuItem,
            this.copyFolderToolStripMenuItem,
            this.replaceTextOfAllFilesToolStripMenuItem,
            this.generateTestCaseFileToolStripMenuItem,
            this.generateCaseConfigFileToolStripMenuItem,
            this.renameFileFollowOrderToolStripMenuItem});
            this.folderContextMenu.Name = "folderContextMenu";
            this.folderContextMenu.Size = new System.Drawing.Size(257, 202);
            // 
            // runMenuItem
            // 
            this.runMenuItem.Name = "runMenuItem";
            this.runMenuItem.Size = new System.Drawing.Size(256, 22);
            this.runMenuItem.Text = "Run All";
            this.runMenuItem.Click += new System.EventHandler(this.runMenuItem_Click);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.stopToolStripMenuItem.Text = "Stop Run";
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
            // 
            // clearStartToolStripMenuItem
            // 
            this.clearStartToolStripMenuItem.Name = "clearStartToolStripMenuItem";
            this.clearStartToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.clearStartToolStripMenuItem.Text = "Clear Duration";
            this.clearStartToolStripMenuItem.Click += new System.EventHandler(this.clearStartToolStripMenuItem_Click);
            // 
            // openFileFolderToolStripMenuItem
            // 
            this.openFileFolderToolStripMenuItem.Name = "openFileFolderToolStripMenuItem";
            this.openFileFolderToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.openFileFolderToolStripMenuItem.Text = "Open File Folder";
            this.openFileFolderToolStripMenuItem.Click += new System.EventHandler(this.openFileFolderToolStripMenuItem_Click);
            // 
            // copyFolderToolStripMenuItem
            // 
            this.copyFolderToolStripMenuItem.Name = "copyFolderToolStripMenuItem";
            this.copyFolderToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.copyFolderToolStripMenuItem.Text = "Copy Folder";
            this.copyFolderToolStripMenuItem.Click += new System.EventHandler(this.copyFolderToolStripMenuItem_Click);
            // 
            // replaceTextOfAllFilesToolStripMenuItem
            // 
            this.replaceTextOfAllFilesToolStripMenuItem.Name = "replaceTextOfAllFilesToolStripMenuItem";
            this.replaceTextOfAllFilesToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.replaceTextOfAllFilesToolStripMenuItem.Text = "Replace File Info(NameOrContent)";
            this.replaceTextOfAllFilesToolStripMenuItem.Click += new System.EventHandler(this.replaceTextOfAllFilesToolStripMenuItem_Click);
            // 
            // generateTestCaseFileToolStripMenuItem
            // 
            this.generateTestCaseFileToolStripMenuItem.Name = "generateTestCaseFileToolStripMenuItem";
            this.generateTestCaseFileToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.generateTestCaseFileToolStripMenuItem.Text = "GenerateTestCaseFile";
            this.generateTestCaseFileToolStripMenuItem.Click += new System.EventHandler(this.generateTestCaseFileToolStripMenuItem_Click);
            // 
            // generateCaseConfigFileToolStripMenuItem
            // 
            this.generateCaseConfigFileToolStripMenuItem.Name = "generateCaseConfigFileToolStripMenuItem";
            this.generateCaseConfigFileToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.generateCaseConfigFileToolStripMenuItem.Text = "GenerateCaseConfigFile";
            this.generateCaseConfigFileToolStripMenuItem.Click += new System.EventHandler(this.generateCaseConfigFileToolStripMenuItem_Click);
            // 
            // renameFileFollowOrderToolStripMenuItem
            // 
            this.renameFileFollowOrderToolStripMenuItem.Name = "renameFileFollowOrderToolStripMenuItem";
            this.renameFileFollowOrderToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.renameFileFollowOrderToolStripMenuItem.Text = "RenameFileFollowOrder";
            this.renameFileFollowOrderToolStripMenuItem.Click += new System.EventHandler(this.renameFileFollowOrderToolStripMenuItem_Click);
            // 
            // lbHowManyNodesSend
            // 
            this.lbHowManyNodesSend.AutoSize = true;
            this.lbHowManyNodesSend.Location = new System.Drawing.Point(628, 4);
            this.lbHowManyNodesSend.Name = "lbHowManyNodesSend";
            this.lbHowManyNodesSend.Size = new System.Drawing.Size(116, 17);
            this.lbHowManyNodesSend.TabIndex = 4;
            this.lbHowManyNodesSend.Text = "Multi Nodes Send:";
            // 
            // lbCurrentCount
            // 
            this.lbCurrentCount.AutoSize = true;
            this.lbCurrentCount.Location = new System.Drawing.Point(750, 4);
            this.lbCurrentCount.Name = "lbCurrentCount";
            this.lbCurrentCount.Size = new System.Drawing.Size(85, 17);
            this.lbCurrentCount.TabIndex = 5;
            this.lbCurrentCount.Text = "CurrentCount";
            // 
            // fileContextMenu
            // 
            this.fileContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SetStartStripMenuItem,
            this.SetEndStripMenuItem,
            this.deleteFromDBToolStripMenuItem,
            this.addNumberBeforeFileNameToolStripMenuItem});
            this.fileContextMenu.Name = "fileContextMenu";
            this.fileContextMenu.Size = new System.Drawing.Size(225, 92);
            // 
            // SetStartStripMenuItem
            // 
            this.SetStartStripMenuItem.Name = "SetStartStripMenuItem";
            this.SetStartStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.SetStartStripMenuItem.Text = "Set Start";
            this.SetStartStripMenuItem.Click += new System.EventHandler(this.SetStartStripMenuItem_Click);
            // 
            // SetEndStripMenuItem
            // 
            this.SetEndStripMenuItem.Name = "SetEndStripMenuItem";
            this.SetEndStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.SetEndStripMenuItem.Text = "Set End";
            this.SetEndStripMenuItem.Click += new System.EventHandler(this.SetEndStripMenuItem_Click);
            // 
            // deleteFromDBToolStripMenuItem
            // 
            this.deleteFromDBToolStripMenuItem.Name = "deleteFromDBToolStripMenuItem";
            this.deleteFromDBToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.deleteFromDBToolStripMenuItem.Text = "Delete";
            this.deleteFromDBToolStripMenuItem.Click += new System.EventHandler(this.deleteFromDBToolStripMenuItem_Click);
            // 
            // addNumberBeforeFileNameToolStripMenuItem
            // 
            this.addNumberBeforeFileNameToolStripMenuItem.Name = "addNumberBeforeFileNameToolStripMenuItem";
            this.addNumberBeforeFileNameToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.addNumberBeforeFileNameToolStripMenuItem.Text = "AddNumberBeforeFileName";
            this.addNumberBeforeFileNameToolStripMenuItem.Click += new System.EventHandler(this.addNumberBeforeFileNameToolStripMenuItem_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(875, -1);
            this.btnStop.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(56, 27);
            this.btnStop.TabIndex = 6;
            this.btnStop.Text = "STOP";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // cbAutoChangeContext
            // 
            this.cbAutoChangeContext.AutoSize = true;
            this.cbAutoChangeContext.Location = new System.Drawing.Point(333, 3);
            this.cbAutoChangeContext.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbAutoChangeContext.Name = "cbAutoChangeContext";
            this.cbAutoChangeContext.Size = new System.Drawing.Size(142, 21);
            this.cbAutoChangeContext.TabIndex = 7;
            this.cbAutoChangeContext.Text = "AutoChangeContext";
            this.cbAutoChangeContext.UseVisualStyleBackColor = true;
            // 
            // cbPerfTest
            // 
            this.cbPerfTest.AutoSize = true;
            this.cbPerfTest.Location = new System.Drawing.Point(1447, 3);
            this.cbPerfTest.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbPerfTest.Name = "cbPerfTest";
            this.cbPerfTest.Size = new System.Drawing.Size(73, 21);
            this.cbPerfTest.TabIndex = 8;
            this.cbPerfTest.Text = "PerfTest";
            this.cbPerfTest.UseVisualStyleBackColor = true;
            this.cbPerfTest.CheckedChanged += new System.EventHandler(this.cbPerfTest_CheckedChanged);
            // 
            // tbMsgCount
            // 
            this.tbMsgCount.Location = new System.Drawing.Point(1217, 1);
            this.tbMsgCount.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbMsgCount.Name = "tbMsgCount";
            this.tbMsgCount.Size = new System.Drawing.Size(196, 25);
            this.tbMsgCount.TabIndex = 9;
            this.tbMsgCount.Text = "1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1139, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 17);
            this.label1.TabIndex = 10;
            this.label1.Text = "LoopCount";
            // 
            // lbCurrentLoop
            // 
            this.lbCurrentLoop.AutoSize = true;
            this.lbCurrentLoop.Location = new System.Drawing.Point(939, 4);
            this.lbCurrentLoop.Name = "lbCurrentLoop";
            this.lbCurrentLoop.Size = new System.Drawing.Size(84, 17);
            this.lbCurrentLoop.TabIndex = 11;
            this.lbCurrentLoop.Text = "CurrentLoop:";
            // 
            // autoSaveReplyCB
            // 
            this.autoSaveReplyCB.AutoSize = true;
            this.autoSaveReplyCB.Location = new System.Drawing.Point(478, 3);
            this.autoSaveReplyCB.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.autoSaveReplyCB.Name = "autoSaveReplyCB";
            this.autoSaveReplyCB.Size = new System.Drawing.Size(113, 21);
            this.autoSaveReplyCB.TabIndex = 12;
            this.autoSaveReplyCB.Text = "AutoSaveReply";
            this.autoSaveReplyCB.UseVisualStyleBackColor = true;
            this.autoSaveReplyCB.CheckedChanged += new System.EventHandler(this.autoSaveReplyCB_CheckedChanged);
            // 
            // Simulator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1553, 758);
            this.Controls.Add(this.autoSaveReplyCB);
            this.Controls.Add(this.lbCurrentLoop);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbMsgCount);
            this.Controls.Add(this.cbPerfTest);
            this.Controls.Add(this.cbAutoChangeContext);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.lbCurrentCount);
            this.Controls.Add(this.lbHowManyNodesSend);
            this.Controls.Add(this.cbToDispatcher);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.mainMenu);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mainMenu;
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
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
        private System.Windows.Forms.CheckBox cbAutoChangeContext;
        private System.Windows.Forms.CheckBox cbPerfTest;
        private System.Windows.Forms.TextBox tbMsgCount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbCurrentLoop;
        private System.Windows.Forms.ToolStripMenuItem openFileFolderToolStripMenuItem;
        private System.Windows.Forms.CheckBox autoSaveReplyCB;
        private System.Windows.Forms.ToolStripMenuItem loadFromDBToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveCurrTreeToDBToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadFromFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteFromDBToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reloadFromFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem replaceTextOfAllFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem escapeXmlMessageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateTestCaseFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateCaseConfigFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameFileFollowOrderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addNumberBeforeFileNameToolStripMenuItem;
    }
}

