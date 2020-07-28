namespace WS_Simulator
{
    partial class LoadFromDB
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
            this.loadFromDBButton = new System.Windows.Forms.Button();
            this.testRepositoryComoboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Segoe UI", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleLabel.Location = new System.Drawing.Point(12, 19);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(399, 40);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "Load Test Repository From DB";
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Location = new System.Drawing.Point(14, 70);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(219, 30);
            this.nameLabel.TabIndex = 1;
            this.nameLabel.Text = "Test Repository Name:";
            // 
            // loadFromDBButton
            // 
            this.loadFromDBButton.Location = new System.Drawing.Point(71, 176);
            this.loadFromDBButton.Name = "loadFromDBButton";
            this.loadFromDBButton.Size = new System.Drawing.Size(279, 39);
            this.loadFromDBButton.TabIndex = 2;
            this.loadFromDBButton.Text = "Load From DB";
            this.loadFromDBButton.UseVisualStyleBackColor = true;
            this.loadFromDBButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // testRepositoryComoboBox
            // 
            this.testRepositoryComoboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.testRepositoryComoboBox.FormattingEnabled = true;
            this.testRepositoryComoboBox.Location = new System.Drawing.Point(19, 113);
            this.testRepositoryComoboBox.Name = "testRepositoryComoboBox";
            this.testRepositoryComoboBox.Size = new System.Drawing.Size(385, 38);
            this.testRepositoryComoboBox.TabIndex = 3;
            // 
            // LoadFromDB
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 30F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(428, 242);
            this.Controls.Add(this.testRepositoryComoboBox);
            this.Controls.Add(this.loadFromDBButton);
            this.Controls.Add(this.nameLabel);
            this.Controls.Add(this.titleLabel);
            this.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.Name = "LoadFromDB";
            this.Text = "Load Test Repository From DB";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.Button loadFromDBButton;
        private System.Windows.Forms.ComboBox testRepositoryComoboBox;
    }
}