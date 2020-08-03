namespace WS_Simulator
{
    partial class CopyFolderForm
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("RootNode");
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.pathTree = new System.Windows.Forms.TreeView();
            this.needReplaceCB = new System.Windows.Forms.CheckBox();
            this.newTextValue = new System.Windows.Forms.TextBox();
            this.oldTextValue = new System.Windows.Forms.TextBox();
            this.newNameLabel = new System.Windows.Forms.Label();
            this.oldTextLabel = new System.Windows.Forms.Label();
            this.queryButton = new System.Windows.Forms.Button();
            this.folderNameText = new System.Windows.Forms.TextBox();
            this.queryText = new System.Windows.Forms.TextBox();
            this.folderNameLabel = new System.Windows.Forms.Label();
            this.queryLabel = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okayButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.pathTree);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.needReplaceCB);
            this.splitContainer1.Panel2.Controls.Add(this.newTextValue);
            this.splitContainer1.Panel2.Controls.Add(this.oldTextValue);
            this.splitContainer1.Panel2.Controls.Add(this.newNameLabel);
            this.splitContainer1.Panel2.Controls.Add(this.oldTextLabel);
            this.splitContainer1.Panel2.Controls.Add(this.queryButton);
            this.splitContainer1.Panel2.Controls.Add(this.folderNameText);
            this.splitContainer1.Panel2.Controls.Add(this.queryText);
            this.splitContainer1.Panel2.Controls.Add(this.folderNameLabel);
            this.splitContainer1.Panel2.Controls.Add(this.queryLabel);
            this.splitContainer1.Panel2.Controls.Add(this.cancelButton);
            this.splitContainer1.Panel2.Controls.Add(this.okayButton);
            this.splitContainer1.Size = new System.Drawing.Size(853, 548);
            this.splitContainer1.SplitterDistance = 516;
            this.splitContainer1.TabIndex = 0;
            // 
            // pathTree
            // 
            this.pathTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pathTree.Location = new System.Drawing.Point(0, 0);
            this.pathTree.Name = "pathTree";
            treeNode1.Name = "RootNode";
            treeNode1.Text = "RootNode";
            this.pathTree.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.pathTree.Size = new System.Drawing.Size(516, 548);
            this.pathTree.TabIndex = 0;
            this.pathTree.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pathTree_MouseDown);
            // 
            // needReplaceCB
            // 
            this.needReplaceCB.AutoSize = true;
            this.needReplaceCB.Location = new System.Drawing.Point(28, 267);
            this.needReplaceCB.Name = "needReplaceCB";
            this.needReplaceCB.Size = new System.Drawing.Size(160, 34);
            this.needReplaceCB.TabIndex = 11;
            this.needReplaceCB.Text = "Need Replace";
            this.needReplaceCB.UseVisualStyleBackColor = true;
            this.needReplaceCB.CheckedChanged += new System.EventHandler(this.needReplaceCB_CheckedChanged);
            // 
            // newTextValue
            // 
            this.newTextValue.Location = new System.Drawing.Point(152, 354);
            this.newTextValue.Name = "newTextValue";
            this.newTextValue.Size = new System.Drawing.Size(151, 35);
            this.newTextValue.TabIndex = 10;
            // 
            // oldTextValue
            // 
            this.oldTextValue.Location = new System.Drawing.Point(152, 307);
            this.oldTextValue.Name = "oldTextValue";
            this.oldTextValue.Size = new System.Drawing.Size(151, 35);
            this.oldTextValue.TabIndex = 9;
            // 
            // newNameLabel
            // 
            this.newNameLabel.AutoSize = true;
            this.newNameLabel.Location = new System.Drawing.Point(27, 357);
            this.newNameLabel.Name = "newNameLabel";
            this.newNameLabel.Size = new System.Drawing.Size(103, 30);
            this.newNameLabel.TabIndex = 8;
            this.newNameLabel.Text = "New Text:";
            // 
            // oldTextLabel
            // 
            this.oldTextLabel.AutoSize = true;
            this.oldTextLabel.Location = new System.Drawing.Point(27, 310);
            this.oldTextLabel.Name = "oldTextLabel";
            this.oldTextLabel.Size = new System.Drawing.Size(94, 30);
            this.oldTextLabel.TabIndex = 7;
            this.oldTextLabel.Text = "Old Text:";
            // 
            // queryButton
            // 
            this.queryButton.Location = new System.Drawing.Point(28, 113);
            this.queryButton.Name = "queryButton";
            this.queryButton.Size = new System.Drawing.Size(275, 44);
            this.queryButton.TabIndex = 6;
            this.queryButton.Text = "Query From Tree";
            this.queryButton.UseVisualStyleBackColor = true;
            this.queryButton.Click += new System.EventHandler(this.queryButton_Click);
            // 
            // folderNameText
            // 
            this.folderNameText.Location = new System.Drawing.Point(28, 217);
            this.folderNameText.Name = "folderNameText";
            this.folderNameText.Size = new System.Drawing.Size(275, 35);
            this.folderNameText.TabIndex = 5;
            // 
            // queryText
            // 
            this.queryText.Location = new System.Drawing.Point(28, 59);
            this.queryText.Name = "queryText";
            this.queryText.Size = new System.Drawing.Size(275, 35);
            this.queryText.TabIndex = 4;
            // 
            // folderNameLabel
            // 
            this.folderNameLabel.AutoSize = true;
            this.folderNameLabel.Location = new System.Drawing.Point(23, 184);
            this.folderNameLabel.Name = "folderNameLabel";
            this.folderNameLabel.Size = new System.Drawing.Size(137, 30);
            this.folderNameLabel.TabIndex = 3;
            this.folderNameLabel.Text = "Folder Name:";
            // 
            // queryLabel
            // 
            this.queryLabel.AutoSize = true;
            this.queryLabel.Location = new System.Drawing.Point(23, 14);
            this.queryLabel.Name = "queryLabel";
            this.queryLabel.Size = new System.Drawing.Size(164, 30);
            this.queryLabel.TabIndex = 2;
            this.queryLabel.Text = "Directory Name:";
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(28, 486);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(275, 41);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okayButton
            // 
            this.okayButton.Location = new System.Drawing.Point(28, 424);
            this.okayButton.Name = "okayButton";
            this.okayButton.Size = new System.Drawing.Size(275, 41);
            this.okayButton.TabIndex = 0;
            this.okayButton.Text = "Copy";
            this.okayButton.UseVisualStyleBackColor = true;
            this.okayButton.Click += new System.EventHandler(this.okayButton_Click);
            // 
            // CopyFolderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 30F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(853, 548);
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.Name = "CopyFolderForm";
            this.Text = "SaveSingleFileToDB";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView pathTree;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okayButton;
        System.Windows.Forms.TreeNode treeNode;
        private System.Windows.Forms.Button queryButton;
        private System.Windows.Forms.TextBox folderNameText;
        private System.Windows.Forms.TextBox queryText;
        private System.Windows.Forms.Label folderNameLabel;
        private System.Windows.Forms.Label queryLabel;
        private System.Windows.Forms.Label newNameLabel;
        private System.Windows.Forms.Label oldTextLabel;
        private System.Windows.Forms.TextBox newTextValue;
        private System.Windows.Forms.TextBox oldTextValue;
        private System.Windows.Forms.CheckBox needReplaceCB;
    }
}