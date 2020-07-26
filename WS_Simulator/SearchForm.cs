using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WS_Simulator.Interface;

namespace WS_Simulator
{
    public partial class SearchForm : Form
    {
        ISearchFormRequester _requestForm;
        string m_ComponentName;

        public SearchForm(Simulator inSourceForm, string componentName, string selectText = "")
        {
            InitializeComponent();

            _requestForm = inSourceForm;
            m_ComponentName = componentName;
            this.txbSearch.Text = selectText;

            this.FormBorderStyle = FormBorderStyle.Fixed3D;

            if (m_ComponentName.Contains("Request"))
            {
                this.Text = "Request message search form";
            }
            else if (m_ComponentName.Contains("Reply"))
            {
                this.Text = "Reply message search form";
            }
        }

        public void ShowForm(string componentName, string selectText = "")
        {
            m_ComponentName = componentName;
            this.txbSearch.Text = selectText;

            if (m_ComponentName.Contains("Request"))
            {
                this.Text = "Request message search form";
            }
            else if (m_ComponentName.Contains("Reply"))
            {
                this.Text = "Reply message search form";
            }
            
            this.Focus();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (this.txbSearch.Text.Length <= 0)
            {
                MessageBox.Show("Search Text can not be empty");
            }
            else
            {
                _requestForm.SearchFormRequest(m_ComponentName, this.txbSearch.Text, !cbUp.Checked, cbCase.Checked);
            }

        }

        private void btnReplace_Click(object sender, EventArgs e)
        {
            if (this.txbSearch.Text.Length <= 0 || this.txbReplace.Text.Length <= 0)
            {
                MessageBox.Show("Search&Replace Text can not be empty");
            }
            else
            {
                _requestForm.ReplaceFormRequest(m_ComponentName, this.txbSearch.Text, this.txbReplace.Text);
            }
        }

        private void cbUp_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cbUp.Checked)
            {
                this.cbDown.Checked = false;
            }
        }

        private void cbDown_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cbDown.Checked)
            {
                this.cbUp.Checked = false;
            }
        }

        private void txbSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Enter)
            {
                btnNext_Click(sender, e);
            }
        }
    }
}
