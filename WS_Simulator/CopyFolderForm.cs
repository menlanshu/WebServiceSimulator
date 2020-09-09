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
        private bool _folderTreeBusy;

        public CopyFolderForm(ICopyFolderFormRequester copyFolderFormRequester, TreeView currPathTree, Node folderNode)
        {
            InitializeComponent();

            _requester = copyFolderFormRequester;
            _folderNode = folderNode;

            this.StartPosition = FormStartPosition.CenterScreen;

            _folderTreeBusy = false;
            SimulatorFormHandler.LoadFileTree(this.folderTree, _folderNode.TreeNodeValue);
            ShowOrHidePanel2(true);

            SimulatorFormHandler.LoadDirectoryTree(this.pathTree, currPathTree);

            MakeReplaceFunctionVisible(this.needReplaceCB.Checked);
            InitialImageList();
        }

        private void ShowOrHidePanel2(bool hide)
        {
            if (hide)
            {
                this.splitContainer2.Panel2.Enabled = false;
                this.folderTree.Nodes[0].Checked = true;
            }
            else
            {
                this.splitContainer2.Panel2.Enabled = true;
            }
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
            if (!string.IsNullOrWhiteSpace(this.queryText.Text))
            {
                TreeNode currNode = SimulatorFormHandler.SelectTheTreeNode(this.queryText.Text, this.pathTree.Nodes[0]);
                if (currNode != null)
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

        private void okayButton_Click(object sender, EventArgs e)
        {
            bool okay = false;
            string errDesc = "";

            if (string.IsNullOrWhiteSpace(this.folderNameText.Text))
            {
                MessageBox.Show("Folder Name can not be empty!");
                return;
            }
            else
            {

                //if (SimulatorFormHandler.CheckNodeExist(this.pathTree.SelectedNode, this.folderNameText.Text))
                //{
                //    MessageBox.Show("Folder name already exist in this folder!");
                //    return;
                //}

                if (this.needReplaceCB.Checked)
                {
                    if (string.IsNullOrWhiteSpace(this.oldTextValue.Text) ||
                        string.IsNullOrWhiteSpace(this.newTextValue.Text))
                    {
                        MessageBox.Show("Old Text and New Text both can't be empty!");
                        return;
                    }
                }

                if (this.pathTree.SelectedNode != null)
                {
                    if (!this.needReplaceCB.Checked)
                    {
                        (okay, errDesc) = _requester.SaveFolderToTreeNode((Node)this.pathTree.SelectedNode.Tag,
                            _folderNode, this.folderNameText.Text);
                    }
                    else
                    {
                        (okay, errDesc) = _requester.SaveFolderToTreeNode((Node)this.pathTree.SelectedNode.Tag,
                            _folderNode, this.folderNameText.Text, this.oldTextValue.Text, this.newTextValue.Text);
                    }


                    if (okay)
                    {
                        MessageBox.Show("Copy folder okay");
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show(errDesc);
                    }
                }
                else
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

        private void selectFileCB_CheckedChanged(object sender, EventArgs e)
        {
            if (this.selectFileCB.Checked)
            {
                ShowOrHidePanel2(false);

            }
            else
            {
                ShowOrHidePanel2(true);
            }
        }

        private void folderTree_AfterCheck(object sender, TreeViewEventArgs e)
        {
            var selectNode = e.Node;
            if (selectNode != null)
            {
                if (_folderTreeBusy) return;

                _folderTreeBusy = true;
                try
                {
                    ((Node)selectNode.Tag).TreeNodeValue.Checked = selectNode.Checked;
                    if (selectNode.Nodes.Count > 0)
                    {
                        if (selectNode.Checked == true)
                        {
                            CheckAllChildNodes(selectNode, true);
                        }
                        else
                        {
                            CheckAllChildNodes(selectNode, false);
                        }
                    }
                }
                finally
                {
                    _folderTreeBusy = false;
                }
            }
        }

        private void CheckAllChildNodes(TreeNode treeNode, bool nodeChecked)
        {
            //here iterate in all child nodes of checked/unchecked parent node
            foreach (TreeNode node in treeNode.Nodes)
            {
                node.Checked = nodeChecked;
                ((Node)node.Tag).TreeNodeValue.Checked = nodeChecked;

                if (node.Nodes.Count > 0)
                {
                    // If the current node has child nodes, call the CheckAllChildsNodes method recursively.
                    CheckAllChildNodes(node, nodeChecked);
                }
            }

        }
    }
}
