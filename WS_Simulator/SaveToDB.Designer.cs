namespace WS_Simulator
{
    partial class SaveToDB
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
            this.titleLabel = new System.Windows.Forms.Label();
            this.nameLabel = new System.Windows.Forms.Label();
            this.saveToDBButton = new System.Windows.Forms.Button();
            this.nameValueText = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Segoe UI", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleLabel.Location = new System.Drawing.Point(5, 19);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(372, 40);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "Save Current Tree List To DB";
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Location = new System.Drawing.Point(2, 80);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(219, 30);
            this.nameLabel.TabIndex = 1;
            this.nameLabel.Text = "Test Repository Name:";
            // 
            // saveToDBButton
            // 
            this.saveToDBButton.Location = new System.Drawing.Point(48, 176);
            this.saveToDBButton.Name = "saveToDBButton";
            this.saveToDBButton.Size = new System.Drawing.Size(279, 39);
            this.saveToDBButton.TabIndex = 2;
            this.saveToDBButton.Text = "Savet To DB";
            this.saveToDBButton.UseVisualStyleBackColor = true;
            this.saveToDBButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // nameValueText
            // 
            this.nameValueText.Location = new System.Drawing.Point(7, 113);
            this.nameValueText.Name = "nameValueText";
            this.nameValueText.Size = new System.Drawing.Size(363, 35);
            this.nameValueText.TabIndex = 3;
            // 
            // SaveToDB
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 30F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(382, 242);
            this.Controls.Add(this.nameValueText);
            this.Controls.Add(this.saveToDBButton);
            this.Controls.Add(this.nameLabel);
            this.Controls.Add(this.titleLabel);
            this.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.Name = "SaveToDB";
            this.Text = "Save Test Repository to DB";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.Button saveToDBButton;
        private System.Windows.Forms.TextBox nameValueText;
    }
}