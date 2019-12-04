namespace SMSC_Simulator
{
    partial class SearchForm
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
            this.lbSearch = new System.Windows.Forms.Label();
            this.lbReplace = new System.Windows.Forms.Label();
            this.txbSearch = new System.Windows.Forms.TextBox();
            this.txbReplace = new System.Windows.Forms.TextBox();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnReplace = new System.Windows.Forms.Button();
            this.lbReplaceNote = new System.Windows.Forms.Label();
            this.cbCase = new System.Windows.Forms.CheckBox();
            this.gbUpDown = new System.Windows.Forms.GroupBox();
            this.cbDown = new System.Windows.Forms.CheckBox();
            this.cbUp = new System.Windows.Forms.CheckBox();
            this.gbUpDown.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbSearch
            // 
            this.lbSearch.AutoSize = true;
            this.lbSearch.Location = new System.Drawing.Point(19, 21);
            this.lbSearch.Name = "lbSearch";
            this.lbSearch.Size = new System.Drawing.Size(41, 13);
            this.lbSearch.TabIndex = 0;
            this.lbSearch.Text = "Search";
            // 
            // lbReplace
            // 
            this.lbReplace.AutoSize = true;
            this.lbReplace.Location = new System.Drawing.Point(19, 53);
            this.lbReplace.Name = "lbReplace";
            this.lbReplace.Size = new System.Drawing.Size(47, 13);
            this.lbReplace.TabIndex = 1;
            this.lbReplace.Text = "Replace";
            // 
            // txbSearch
            // 
            this.txbSearch.Location = new System.Drawing.Point(79, 18);
            this.txbSearch.Name = "txbSearch";
            this.txbSearch.Size = new System.Drawing.Size(178, 20);
            this.txbSearch.TabIndex = 2;
            this.txbSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txbSearch_KeyDown);
            // 
            // txbReplace
            // 
            this.txbReplace.Location = new System.Drawing.Point(79, 50);
            this.txbReplace.Name = "txbReplace";
            this.txbReplace.Size = new System.Drawing.Size(178, 20);
            this.txbReplace.TabIndex = 3;
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(281, 13);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(75, 23);
            this.btnNext.TabIndex = 4;
            this.btnNext.Text = "Next";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnReplace
            // 
            this.btnReplace.Location = new System.Drawing.Point(281, 47);
            this.btnReplace.Name = "btnReplace";
            this.btnReplace.Size = new System.Drawing.Size(75, 23);
            this.btnReplace.TabIndex = 5;
            this.btnReplace.Text = "Replace All";
            this.btnReplace.UseVisualStyleBackColor = true;
            this.btnReplace.Click += new System.EventHandler(this.btnReplace_Click);
            // 
            // lbReplaceNote
            // 
            this.lbReplaceNote.AutoSize = true;
            this.lbReplaceNote.ForeColor = System.Drawing.Color.Red;
            this.lbReplaceNote.Location = new System.Drawing.Point(19, 76);
            this.lbReplaceNote.Name = "lbReplaceNote";
            this.lbReplaceNote.Size = new System.Drawing.Size(127, 13);
            this.lbReplaceNote.TabIndex = 6;
            this.lbReplaceNote.Text = "Replace is case sensitive";
            // 
            // cbCase
            // 
            this.cbCase.AutoSize = true;
            this.cbCase.Location = new System.Drawing.Point(22, 96);
            this.cbCase.Name = "cbCase";
            this.cbCase.Size = new System.Drawing.Size(94, 17);
            this.cbCase.TabIndex = 7;
            this.cbCase.Text = "Case sensitive";
            this.cbCase.UseVisualStyleBackColor = true;
            // 
            // gbUpDown
            // 
            this.gbUpDown.Controls.Add(this.cbDown);
            this.gbUpDown.Controls.Add(this.cbUp);
            this.gbUpDown.Location = new System.Drawing.Point(153, 76);
            this.gbUpDown.Name = "gbUpDown";
            this.gbUpDown.Size = new System.Drawing.Size(203, 45);
            this.gbUpDown.TabIndex = 9;
            this.gbUpDown.TabStop = false;
            this.gbUpDown.Text = "Direction";
            // 
            // cbDown
            // 
            this.cbDown.AutoSize = true;
            this.cbDown.Location = new System.Drawing.Point(121, 16);
            this.cbDown.Name = "cbDown";
            this.cbDown.Size = new System.Drawing.Size(64, 17);
            this.cbDown.TabIndex = 11;
            this.cbDown.Text = "Forward";
            this.cbDown.UseVisualStyleBackColor = true;
            this.cbDown.CheckedChanged += new System.EventHandler(this.cbDown_CheckedChanged);
            // 
            // cbUp
            // 
            this.cbUp.AutoSize = true;
            this.cbUp.Location = new System.Drawing.Point(38, 16);
            this.cbUp.Name = "cbUp";
            this.cbUp.Size = new System.Drawing.Size(66, 17);
            this.cbUp.TabIndex = 10;
            this.cbUp.Text = "Reverse";
            this.cbUp.UseVisualStyleBackColor = true;
            this.cbUp.CheckedChanged += new System.EventHandler(this.cbUp_CheckedChanged);
            // 
            // SearchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(373, 131);
            this.Controls.Add(this.gbUpDown);
            this.Controls.Add(this.cbCase);
            this.Controls.Add(this.lbReplaceNote);
            this.Controls.Add(this.btnReplace);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.txbReplace);
            this.Controls.Add(this.txbSearch);
            this.Controls.Add(this.lbReplace);
            this.Controls.Add(this.lbSearch);
            this.Name = "SearchForm";
            this.Text = "SearchForm";
            this.gbUpDown.ResumeLayout(false);
            this.gbUpDown.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbSearch;
        private System.Windows.Forms.Label lbReplace;
        private System.Windows.Forms.TextBox txbSearch;
        private System.Windows.Forms.TextBox txbReplace;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnReplace;
        private System.Windows.Forms.Label lbReplaceNote;
        private System.Windows.Forms.CheckBox cbCase;
        private System.Windows.Forms.GroupBox gbUpDown;
        private System.Windows.Forms.CheckBox cbDown;
        private System.Windows.Forms.CheckBox cbUp;
    }
}