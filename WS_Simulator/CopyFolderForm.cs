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
    public partial class CopyFolderForm : Form
    {
        ICopyFolderFormRequester _requester;
        private Node _folderNode;

        public CopyFolderForm(ICopyFolderFormRequester copyFolderFormRequester, TreeView currPathTree, Node folderNode)
        {
            InitializeComponent();

            _requester = copyFolderFormRequester;
            _folderNode = folderNode;

            this.StartPosition = FormStartPosition.CenterScreen;
            SimulatorFormHandler.LoadDirectoryTree(this.pathTree, currPathTree);

            MakeReplaceFunctionVisible(this.needReplaceCB.Checked);
            InitialImageList();
        }

        private void MakeReplaceFunctionVisible(bool isVisible)
        {
            this.oldTextLabel.Visible = isVisible;
            this.oldTextValue.Visible = isVisible;
            this.newNameLabel.Visible = isVisible;
            this.newTextValue.Visible = isVisible;
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
            bool okay = false;
            string errDesc = "";
            
            if(string.IsNullOrWhiteSpace(this.folderNameText.Text))
            {
                MessageBox.Show("Folder Name can not be empty!");
                return;
            }else
            {

                if(SimulatorFormHandler.CheckNodeExist(this.pathTree.SelectedNode, this.folderNameText.Text))
                {
                    MessageBox.Show("Folder name already exist in this folder!");
                    return;
                }

                if (this.needReplaceCB.Checked)
                {
                    if(string.IsNullOrWhiteSpace(this.oldTextValue.Text) ||
                        string.IsNullOrWhiteSpace(this.newTextValue.Text))
                    {
                        MessageBox.Show("Old Text and New Text both can't be empty!");
                        return;
                    }
                }

                if (this.pathTree.SelectedNode != null)
                {
                    if(!this.needReplaceCB.Checked)
                    {
                        (okay, errDesc) = _requester.SaveFolderToTreeNode((Node)this.pathTree.SelectedNode.Tag,
                            _folderNode, this.folderNameText.Text);
                    }else
                    {
                        (okay, errDesc) = _requester.SaveFolderToTreeNode((Node)this.pathTree.SelectedNode.Tag,
                            _folderNode, this.folderNameText.Text, this.oldTextValue.Text, this.newTextValue.Text);
                    }


                    if (okay)
                    {
                        MessageBox.Show("Copy folder okay");
                        this.Close();
                    }else
                    {
                        MessageBox.Show(errDesc);
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

        private void needReplaceCB_CheckedChanged(object sender, EventArgs e)
        {
            MakeReplaceFunctionVisible(this.needReplaceCB.Checked);
        }
    }
}
