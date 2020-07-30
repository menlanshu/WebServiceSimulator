using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WS_Simulator.FormHandler;
using WS_Simulator.Interface;
using WS_Simulator.Models;

namespace WS_Simulator
{
    public partial class SaveSingleFileToDB : Form
    {
        ISaveSingleFieToDB _requester;
        string _requestMessage;
        public SaveSingleFileToDB(ISaveSingleFieToDB saveSingleFieToDB, TreeView currPathTree, string requestMessage)
        {
            InitializeComponent();

            _requester = saveSingleFieToDB;
            _requestMessage = requestMessage;

            this.StartPosition = FormStartPosition.CenterScreen;
            SimulatorFormHandler.LoadDirectoryTree(this.pathTree, currPathTree);

            InitialImageList();
        }

        private void InitialImageList()
        {
            ImageList myImageList = new ImageList();
            myImageList.Images.Add(SimulatorFormHandler.FileImageKey, Image.FromFile("file.ico"));
            myImageList.Images.Add(SimulatorFormHandler.FolderImageKey, Image.FromFile("folder.ico"));

            this.pathTree.ImageList = myImageList;
        }

        private void pathTree_MouseDown(object sender, MouseEventArgs e)
        {
            // Set old node to white
            if (this.pathTree.SelectedNode != null)
            {
                this.pathTree.SelectedNode.BackColor = Color.White;
            }

            // Set color of current node
            this.pathTree.SelectedNode = this.pathTree.GetNodeAt(e.X, e.Y);
            if (this.pathTree.SelectedNode != null)
            {
                this.pathTree.SelectedNode.BackColor = Color.LightGreen;
            }
        }

        private void queryButton_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrWhiteSpace(this.queryText.Text))
            {
                TreeNode currNode = SelectTheTreeNode(this.queryText.Text, this.pathTree.Nodes[0]);
                if(currNode != null)
                {
                    if (this.pathTree.SelectedNode != null)
                    {
                        this.pathTree.SelectedNode.BackColor = Color.White;
                    }

                    this.pathTree.SelectedNode = currNode;
                    this.pathTree.SelectedNode.BackColor = Color.LightGreen;
                }
            }
        }

        private TreeNode SelectTheTreeNode(string directoryName, TreeNode currentNode)
        {
            if(currentNode == null)
            {
                return null;
            }

            if (currentNode.Text.ToLower().Contains(directoryName.ToLower()))
            {
                return currentNode;
            }
            else
            {
                if (currentNode.Nodes.Count == 0)
                {
                    return null;
                }
                else
                {
                    foreach (TreeNode node in currentNode.Nodes)
                    {
                        TreeNode newNode = SelectTheTreeNode(directoryName, node);
                        if(newNode != null)
                        {
                            return newNode;
                        }
                    }
                }
            }

            return null;
        }

        private void okayButton_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(this.nameText.Text))
            {
                MessageBox.Show("File Name can not be empty!");
            }else
            {
                if(this.pathTree.SelectedNode != null)
                {
                    Node newNode = new Node();
                    newNode.MotherNode = (Node)this.pathTree.SelectedNode.Tag;
                    newNode.TreeNodeMessage = _requestMessage;
                    newNode.TreeNodeName = this.nameText.Text;

                    if(_requester.RequestSaveFile(newNode))
                    {
                        MessageBox.Show("Add okay");
                        this.Close();
                    }else
                    {
                        MessageBox.Show("Seems same record already exists in DB!");
                    }
                }else
                {
                    MessageBox.Show("Select a directory to save file.");
                }

            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
