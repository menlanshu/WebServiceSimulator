using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WS_Simulator.DataAccess;
using WS_Simulator.Interface;
using WS_Simulator.Models;

namespace WS_Simulator
{
    public partial class LoadFromDB : Form
    {
        ILoadFromDBFormRequester _requester;
        List<TestRepository> _avaliableTestRepositories = new List<TestRepository>();
        public LoadFromDB(ILoadFromDBFormRequester requester)
        {
            _requester = requester;

            this.StartPosition = FormStartPosition.CenterScreen;

            InitializeComponent();

            _avaliableTestRepositories = SQLiteDBProcessor.GetAllRepository();
            WireUp();
        }

        private void WireUp()
        {
            this.testRepositoryComoboBox.DataSource = null;

            this.testRepositoryComoboBox.DataSource = _avaliableTestRepositories;
            this.testRepositoryComoboBox.DisplayMember = "RepositoryName";
        }

        private void button1_Click(object sender, EventArgs e)
        {

            TestRepository testRepository = (TestRepository)this.testRepositoryComoboBox.SelectedItem;

            if(testRepository != null)
            {
                _requester.CovertTestRepositoryToTree(testRepository);
                this.Close();
            }else
            {
                MessageBox.Show("Please choose a Test Repository To Load!");
            }


        }
        
    }
}
