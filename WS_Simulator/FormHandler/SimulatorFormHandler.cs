using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WS_Simulator.DataAccess;
using WS_Simulator.Models;

namespace WS_Simulator.FormHandler
{
    public static class SimulatorFormHandler
    {

        public static char[] ConfigDelimeter = (";").ToCharArray();
        public static string FileImageKey = "file";
        public static string FolderImageKey = "folder";

        public static bool CheckNodeExist(TreeNode motherNode, string name)
        {
            foreach (TreeNode node in motherNode.Nodes)
            {
                if (node.Text == name)
                {
                    return true;
                }
            }

            return false;
        }

        public static async Task InitMethodNameOfWebService(WSConfig wSConfig, TreeNodeCollection treeNodes)
        {
            //comboBox.Items.Clear();
            wSConfig.WSMethodList.Clear();

            foreach (TreeNode node in treeNodes)
            {
                await AddMethoName(node, wSConfig);
            }
        }

        public static async Task AddMethoName(TreeNode inTreeNode, WSConfig wSConfig)
        {
            if (inTreeNode.Nodes.Count == 0)
            {
                wSConfig.WSMethodList.Add(inTreeNode.Text);
            }
            else
            {
                foreach (TreeNode tempNode in inTreeNode.Nodes)
                {
                    await AddMethoName(tempNode, wSConfig);
                }
            }
        }



        public static (bool oKay, string directoryPath) LoadFileTree(
            ref List<Node> nodeList, TreeView pathTree,
            ContextMenuStrip folderMenu, ContextMenuStrip fileMenu,
            List<string> loadExtensionNameList, string directoryPath = ".")
        {
            if (Directory.Exists(directoryPath))
            {
                nodeList = new List<Node>();

                // Handle logic for file node part
                DirectoryInfo tempDirectory = new DirectoryInfo(directoryPath);

                pathTree.Nodes[0].Nodes.Clear();
                pathTree.Nodes[0].Text = "RootNode";
                pathTree.Nodes[0].ContextMenuStrip = folderMenu;

                // assign root node
                Node newNode = new FileNode(TreeNodeType.Root, pathTree.Nodes[0], null, pathTree.Nodes[0].FullPath);
                newNode.TreeNodeSourceType = SourceNodeType.LOCAL;
                nodeList.Add(newNode);

                pathTree.Nodes[0].Tag = newNode;

                foreach (FileSystemInfo tempInfo in tempDirectory.EnumerateFileSystemInfos())
                {
                    LoadWholeTree(ref nodeList, newNode, tempInfo, pathTree.Nodes[0], folderMenu, fileMenu, loadExtensionNameList);
                }

                pathTree.Nodes[0].Expand();

                return (true, directoryPath);
            }

            return (false, "Directory path not exist");
        }

        public static void LoadWholeTree(
            ref List<Node> nodeList, Node motherNode, FileSystemInfo tempSystemInfo, TreeNode tempNode,
            ContextMenuStrip folderMenu, ContextMenuStrip fileMenu,
            List<string> loadExtensionNameList)
        {

            try
            {
                if (tempSystemInfo is DirectoryInfo)
                {
                    TreeNode tempDireNode = new TreeNode(tempSystemInfo.Name);
                    tempDireNode.ContextMenuStrip = folderMenu;
                    tempDireNode.ImageIndex = 1; // ImageKey = "folder";
                    tempDireNode.SelectedImageIndex = 1;
                    tempNode.Nodes.Add(tempDireNode);

                    Node newNode = new FileNode(TreeNodeType.Directory, tempDireNode, motherNode, tempDireNode.FullPath);
                    newNode.TreeNodeSourceType = SourceNodeType.LOCAL;
                    nodeList.Add(newNode);

                    tempDireNode.Tag = newNode;


                    foreach (FileSystemInfo tempInfo in ((DirectoryInfo)tempSystemInfo).EnumerateFileSystemInfos())
                    {
                        LoadWholeTree(ref nodeList, newNode, tempInfo, tempDireNode, folderMenu, fileMenu, loadExtensionNameList);
                    }
                }
                else if (tempSystemInfo is FileInfo)
                {
                    if (loadExtensionNameList.Contains(tempSystemInfo.Extension))
                    {
                        TreeNode tempFileNode = new TreeNode(tempSystemInfo.Name);
                        tempFileNode.ContextMenuStrip = fileMenu;
                        tempFileNode.ImageIndex = 0; // ImageKey = "file";
                        tempFileNode.SelectedImageIndex = 0; ;
                        tempNode.Nodes.Add(tempFileNode);

                        // Covert file node to local file system node
                        Node newNode = new FileNode(TreeNodeType.File, tempFileNode, motherNode, tempFileNode.FullPath);
                        newNode.TreeNodeSourceType = SourceNodeType.LOCAL;
                        nodeList.Add(newNode);

                        tempFileNode.Tag = newNode;

                    }
                }
            }
            catch (Exception err)
            {
                throw new Exception($"Exception happen in LoadWholeTree : { err.Message }");
            }

        }

        public static (bool oKay, string directoryPath) LoadFileTreeFromDB(
            TreeView pathTree,
            ContextMenuStrip folderMenu, ContextMenuStrip fileMenu,
            TestRepository testRepository, string directoryPath = ".")
        {
            if (testRepository != null)
            {
                Node rootNode = testRepository.TestNodeList.Where(x => x.TreeNodeType == TreeNodeType.Root).FirstOrDefault();
                // TODO - Can be changed when fetch data from DB!
                rootNode.TreeNodeSourceType = SourceNodeType.DB;

                if (rootNode != null)
                {
                    pathTree.Nodes[0].Nodes.Clear();
                    pathTree.Nodes[0].Text = rootNode.TreeNodeName;
                    pathTree.Nodes[0].ContextMenuStrip = folderMenu;
                    pathTree.Nodes[0].Tag = rootNode;

                    rootNode.TreeNodeValue = pathTree.Nodes[0];

                    foreach (var node in testRepository.TestNodeList.Where(x => x.MotherNodeId == rootNode.Id).OrderBy(x => x.TreeNodeName))
                    {
                        node.MotherNode = rootNode;
                        LoadWholeTreeFomrDB(pathTree.Nodes[0], folderMenu, fileMenu, testRepository, node);
                    }

                    pathTree.Nodes[0].Expand();

                    return (true, directoryPath);
                }
                else
                {
                    return (false, "Can not find root node");
                }
            }

            return (false, "Test Repository is NUll!");
        }

        public static void LoadWholeTreeFomrDB(
            TreeNode tempNode,
            ContextMenuStrip folderMenu, ContextMenuStrip fileMenu,
            TestRepository testRepository, Node currNode)
        {

            try
            {
                if (currNode.TreeNodeType == TreeNodeType.Directory)
                {
                    currNode.TreeNodeSourceType = SourceNodeType.DB;
                    TreeNode tempDireNode = new TreeNode(currNode.TreeNodeName);

                    tempDireNode.ContextMenuStrip = folderMenu;
                    tempDireNode.Tag = currNode;
                    tempDireNode.ImageIndex = 1; // ImageKey = "folder";
                    tempDireNode.SelectedImageIndex = 1;

                    currNode.TreeNodeValue = tempDireNode;

                    tempNode.Nodes.Add(tempDireNode);

                    foreach (var node in testRepository.TestNodeList.Where(x => x.MotherNodeId == currNode.Id).OrderBy(x => x.TreeNodeName))
                    {
                        node.MotherNode = currNode;
                        LoadWholeTreeFomrDB(tempDireNode, folderMenu, fileMenu, testRepository, node);
                    }
                }
                else if (currNode.TreeNodeType == TreeNodeType.File)
                {
                    currNode.TreeNodeSourceType = SourceNodeType.DB;
                    TreeNode tempFileNode = new TreeNode(currNode.TreeNodeName);

                    tempFileNode.ContextMenuStrip = fileMenu;
                    tempFileNode.Tag = currNode;
                    tempFileNode.ImageIndex = 0; // ImageKey = "file";
                    tempFileNode.SelectedImageIndex = 0; ;

                    currNode.TreeNodeValue = tempFileNode;

                    tempNode.Nodes.Add(tempFileNode);
                }
            }
            catch (Exception err)
            {
                throw new Exception($"Exception happen in LoadWholeTreeFomrDB : { err.Message }");
            }

        }

        public static void LoadDirectoryTree(TreeView pathTree, TreeView oldTree)
        {

            pathTree.Nodes[0].Nodes.Clear();
            pathTree.Nodes[0].Text = "RootNode";
            pathTree.Nodes[0].Tag = oldTree.Nodes[0].Tag;

            foreach (TreeNode node in oldTree.Nodes[0].Nodes)
            {
                LoadWholeDiretoryTree(node, pathTree.Nodes[0]);
            }
            pathTree.Nodes[0].Expand();
        }

        public static void LoadWholeDiretoryTree(TreeNode tempNode, TreeNode motherNode)
        {

            try
            {
                if (((Node)tempNode.Tag).TreeNodeType == TreeNodeType.Directory)
                {
                    TreeNode tempDireNode = new TreeNode(tempNode.Text);
                    motherNode.Nodes.Add(tempDireNode);
                    tempDireNode.ImageIndex = 1; // ImageKey = "folder";
                    tempDireNode.SelectedImageIndex = 1;

                    tempDireNode.Tag = tempNode.Tag;

                    foreach (TreeNode node in tempNode.Nodes)
                    {
                        LoadWholeDiretoryTree(node, tempDireNode);
                    }
                }
            }
            catch (Exception err)
            {
                throw new Exception($"Exception happen in LoadWholeDiretoryTree : { err.Message }");
            }

        }

        public static string LoadTestFile(
            Node node, string rootPath,
            Action<string> updateReplyMessage, Action<string> updateAfterReadFile)
        {
            string filePath = "";
            string replyMessage = "";
            string requestMessage = "";

            try
            {
                if (node != null)
                {
                    if (node.TreeNodeSourceType == SourceNodeType.DB)
                    {
                        requestMessage = SQLiteDBProcessor.GetRequestMessageOfCurrentNode(node);
                        updateAfterReadFile?.Invoke(requestMessage);
                    }
                    else if (node.TreeNodeSourceType == SourceNodeType.LOCAL)
                    {
                        // TODO - Optimize the node path!!
                        filePath = FileProcessor.GetFullPath(rootPath, node.NodeFullPath);

                        requestMessage = FileProcessor.ReadFile(filePath, updateAfterReadFile);
                    }
                }

            }
            catch (Exception err)
            {
                replyMessage = $"Exception happen in LoadTestFile : { err.Message} {Environment.NewLine} File Path: {filePath}";
                updateReplyMessage?.Invoke(replyMessage);
            }

            return requestMessage;
        }


        public static SaveFileDialog CreateAFileDialog(Node node, string defaultPath)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            string openPath = defaultPath;

            //set file type
            sfd.Filter = "All files (*.*)|*.*";
            //sfd.FilterIndex = 1;
            sfd.RestoreDirectory = true;

            // Get the actual path of current node
            if (node != null)
            {
                if (node is FileNode)
                {
                    openPath = ((FileNode)node).DirectoryPath;
                }

                sfd.FileName = node.TreeNodeName;
            }

            sfd.InitialDirectory = openPath;

            return sfd;
        }

        public static void RichBoxTextEscapeXML(RichTextBox inRichTextBox)
        {
            try
            {
                string tempStr = inRichTextBox.Text;
                if (string.IsNullOrEmpty(tempStr))
                {
                    MessageBox.Show(inRichTextBox.Name + " is empty!");
                    return;
                }
                inRichTextBox.Text = XMLProcessor.ToDataFromWebService(tempStr);
                inRichTextBox.SelectionStart = 0;
            }
            catch (Exception ex)
            {
                string msg = "";
                while (ex != null)
                {
                    msg += ex.Message + Environment.NewLine;
                    ex = ex.InnerException;
                }
                inRichTextBox.Text = msg + inRichTextBox.Text;
                inRichTextBox.SelectionStart = 0;
            }

        }

        public static void RichBoxTextToXML(RichTextBox inRichTextBox)
        {
            try
            {
                string tempStr = inRichTextBox.Text;
                if (string.IsNullOrEmpty(tempStr))
                {
                    MessageBox.Show(inRichTextBox.Name + " is empty!");
                    return;
                }
                inRichTextBox.Text = XMLProcessor.FormatXml(XMLProcessor.RestoreXml(tempStr));
                inRichTextBox.SelectionStart = 0;
            }
            catch (Exception ex)
            {
                string msg = "";
                while (ex != null)
                {
                    msg += ex.Message + Environment.NewLine;
                    ex = ex.InnerException;
                }
                inRichTextBox.Text = msg + inRichTextBox.Text;
                inRichTextBox.SelectionStart = 0;
            }
        }
    }
}
