namespace WS_Simulator
{
    partial class ReplaceTextForm
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
            this.newTextValue = new System.Windows.Forms.TextBox();
            this.oldTextValue = new System.Windows.Forms.TextBox();
            this.newNameLabel = new System.Windows.Forms.Label();
            this.oldTextLabel = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okayButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // newTextValue
            // 
            this.newTextValue.Location = new System.Drawing.Point(158, 84);
            this.newTextValue.Name = "newTextValue";
            this.newTextValue.Size = new System.Drawing.Size(151, 35);
            this.newTextValue.TabIndex = 16;
            // 
            // oldTextValue
            // 
            this.oldTextValue.Location = new System.Drawing.Point(158, 37);
            this.oldTextValue.Name = "oldTextValue";
            this.oldTextValue.Size = new System.Drawing.Size(151, 35);
            this.oldTextValue.TabIndex = 15;
            // 
            // newNameLabel
            // 
            this.newNameLabel.AutoSize = true;
            this.newNameLabel.Location = new System.Drawing.Point(33, 87);
            this.newNameLabel.Name = "newNameLabel";
            this.newNameLabel.Size = new System.Drawing.Size(103, 30);
            this.newNameLabel.TabIndex = 14;
            this.newNameLabel.Text = "New Text:";
            // 
            // oldTextLabel
            // 
            this.oldTextLabel.AutoSize = true;
            this.oldTextLabel.Location = new System.Drawing.Point(33, 40);
            this.oldTextLabel.Name = "oldTextLabel";
            this.oldTextLabel.Size = new System.Drawing.Size(94, 30);
            this.oldTextLabel.TabIndex = 13;
            this.oldTextLabel.Text = "Old Text:";
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(34, 216);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(275, 41);
            this.cancelButton.TabIndex = 12;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okayButton
            // 
            this.okayButton.Location = new System.Drawing.Point(34, 154);
            this.okayButton.Name = "okayButton";
            this.okayButton.Size = new System.Drawing.Size(275, 41);
            this.okayButton.TabIndex = 11;
            this.okayButton.Text = "Replace";
            this.okayButton.UseVisualStyleBackColor = true;
            this.okayButton.Click += new System.EventHandler(this.okayButton_Click);
            // 
            // ReplaceTextForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 30F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(342, 297);
            this.Controls.Add(this.newTextValue);
            this.Controls.Add(this.oldTextValue);
            this.Controls.Add(this.newNameLabel);
            this.Controls.Add(this.oldTextLabel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okayButton);
            this.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.Name = "ReplaceTextForm";
            this.Text = "SaveSingleFileToDB";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        System.Windows.Forms.TreeNode treeNode;
        private System.Windows.Forms.TextBox newTextValue;
        private System.Windows.Forms.TextBox oldTextValue;
        private System.Windows.Forms.Label newNameLabel;
        private System.Windows.Forms.Label oldTextLabel;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okayButton;
    }
}