namespace WS_Simulator
{
    partial class SaveSingleFileToDB
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
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("RootNode");
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.pathTree = new System.Windows.Forms.TreeView();
            this.queryButton = new System.Windows.Forms.Button();
            this.nameText = new System.Windows.Forms.TextBox();
            this.queryText = new System.Windows.Forms.TextBox();
            this.nameLabel = new System.Windows.Forms.Label();
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
            this.splitContainer1.Panel2.Controls.Add(this.queryButton);
            this.splitContainer1.Panel2.Controls.Add(this.nameText);
            this.splitContainer1.Panel2.Controls.Add(this.queryText);
            this.splitContainer1.Panel2.Controls.Add(this.nameLabel);
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
            treeNode3.Name = "RootNode";
            treeNode3.Text = "RootNode";
            this.pathTree.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode3});
            this.pathTree.Size = new System.Drawing.Size(516, 548);
            this.pathTree.TabIndex = 0;
            this.pathTree.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pathTree_MouseDown);
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
            // nameText
            // 
            this.nameText.Location = new System.Drawing.Point(28, 264);
            this.nameText.Name = "nameText";
            this.nameText.Size = new System.Drawing.Size(275, 35);
            this.nameText.TabIndex = 5;
            // 
            // queryText
            // 
            this.queryText.Location = new System.Drawing.Point(28, 59);
            this.queryText.Name = "queryText";
            this.queryText.Size = new System.Drawing.Size(275, 35);
            this.queryText.TabIndex = 4;
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Location = new System.Drawing.Point(23, 219);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(111, 30);
            this.nameLabel.TabIndex = 3;
            this.nameLabel.Text = "File Name:";
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
            this.cancelButton.Location = new System.Drawing.Point(28, 465);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(275, 41);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okayButton
            // 
            this.okayButton.Location = new System.Drawing.Point(28, 403);
            this.okayButton.Name = "okayButton";
            this.okayButton.Size = new System.Drawing.Size(275, 41);
            this.okayButton.TabIndex = 0;
            this.okayButton.Text = "Save";
            this.okayButton.UseVisualStyleBackColor = true;
            this.okayButton.Click += new System.EventHandler(this.okayButton_Click);
            // 
            // SaveSingleFileToDB
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 30F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(853, 548);
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.Name = "SaveSingleFileToDB";
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
        private System.Windows.Forms.TextBox nameText;
        private System.Windows.Forms.TextBox queryText;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.Label queryLabel;
    }
}