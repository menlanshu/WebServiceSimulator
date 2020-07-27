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
    public partial class SaveToDB : Form
    {
        ISaveToDBFormRequester _requester;
        public SaveToDB(ISaveToDBFormRequester requester)
        {
            _requester = requester;

            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            bool result;
            string errDesc;

            if(this.nameValueText.Text.Length < 0)
            {
                MessageBox.Show("Please key in your test repository name");
            }else
            {
                (result, errDesc) = await Task.Run(() => _requester.SaveRepositoryToDB(this.nameValueText.Text));

                if(result == false)
                {
                    MessageBox.Show(errDesc);
                }else
                {
                    MessageBox.Show("Save to DB success");
                    this.Close();
                }
            }

        }
        
    }
}
