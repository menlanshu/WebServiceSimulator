using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WS_Simulator.FormHandler;
using WS_Simulator.Interface;
using WS_Simulator.Models;

namespace WS_Simulator
{
    public partial class ReplaceTextForm : Form
    {
        IReplaceTextFormRequester _requester;
        private Node _folderNode;

        public ReplaceTextForm(IReplaceTextFormRequester replaceTextFormRequester,  Node folderNode)
        {
            InitializeComponent();

            _requester = replaceTextFormRequester;
            _folderNode = folderNode;

            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void okayButton_Click(object sender, EventArgs e)
        {
            bool okay = false;
            string errDesc = "";
            

            if(string.IsNullOrWhiteSpace(this.oldTextValue.Text) ||
                string.IsNullOrWhiteSpace(this.newTextValue.Text))
            {
                MessageBox.Show("Old Text and New Text both can't be empty!");
                return;
            }

            if (cbReplaceFileName.Checked)
            {
                (okay, errDesc) = _requester.ReplaceTextFileName(_folderNode, this.oldTextValue.Text, this.newTextValue.Text);
            }

            if(cbReplaceFileContent.Checked)
            {
                (okay, errDesc) = _requester.ReplaceTextFileInfo(_folderNode, this.oldTextValue.Text, this.newTextValue.Text);
            }

            if (okay)
            {
                MessageBox.Show("Replace all files in folder success!");
                this.Close();
            }else
            {
                MessageBox.Show(errDesc);
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
