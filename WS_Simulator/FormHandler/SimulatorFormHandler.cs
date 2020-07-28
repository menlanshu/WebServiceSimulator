using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
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

        public static bool NeedWait(string fileName, List<string> needWaitmessageList)
        {
            if (fileName == "")
            {
                return false;
            }
            else
            {
                foreach (string tempStr in needWaitmessageList)
                {
                    if (fileName.Contains(tempStr))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static (bool oKay, string directoryPath) LoadFileTree(TreeView pathTree, ContextMenuStrip folderMenu, ContextMenuStrip fileMenu, 
            List<string> loadExtensionNameList, string directoryPath = ".")
        {
            if (Directory.Exists(directoryPath))
            {
                DirectoryInfo tempDirectory = new DirectoryInfo(directoryPath);

                pathTree.Nodes[0].Nodes.Clear();
                pathTree.Nodes[0].Text = "RootNode";
                pathTree.Nodes[0].ContextMenuStrip = folderMenu;
                pathTree.Nodes[0].Tag = tempDirectory;

                foreach (FileSystemInfo tempInfo in tempDirectory.EnumerateFileSystemInfos())
                {
                    LoadWholeTree(tempInfo, pathTree.Nodes[0], folderMenu, fileMenu, loadExtensionNameList);
                }

                pathTree.Nodes[0].Expand();

                return (true, directoryPath);
            }

            return (false, "Directory path not exist");
        }

        public static void LoadWholeTree(FileSystemInfo tempSystemInfo, TreeNode tempNode, 
            ContextMenuStrip folderMenu, ContextMenuStrip fileMenu, List<string> loadExtensionNameList)
        {

            try
            {
                if (tempSystemInfo is DirectoryInfo)
                {
                    TreeNode tempDireNode = new TreeNode(tempSystemInfo.Name);

                    tempDireNode.ContextMenuStrip = folderMenu;
                    tempDireNode.Tag = tempSystemInfo;

                    tempNode.Nodes.Add(tempDireNode);

                    foreach (FileSystemInfo tempInfo in ((DirectoryInfo)tempSystemInfo).EnumerateFileSystemInfos())
                    {
                        LoadWholeTree(tempInfo, tempDireNode, folderMenu, fileMenu, loadExtensionNameList);
                    }
                }
                else if (tempSystemInfo is FileInfo)
                {
                    if (loadExtensionNameList.Contains(tempSystemInfo.Extension))
                    {
                        TreeNode tempFileNode = new TreeNode(tempSystemInfo.Name);

                        tempFileNode.ContextMenuStrip = fileMenu;
                        tempFileNode.Tag = tempSystemInfo;

                        tempNode.Nodes.Add(tempFileNode);
                    }
                }
            }
            catch (Exception err)
            {
                throw new Exception($"Exception happen in LoadWholeTree : { err.Message }");
            }

        }

        public static (bool oKay, string directoryPath) LoadFileTreeFromDB(TreeView pathTree, ContextMenuStrip folderMenu, ContextMenuStrip fileMenu,
            TestRepository testRepository, string directoryPath = ".")
        {
            if (testRepository != null)
            {
                Node rootNode = testRepository.TestNodeList.Where(x => x.TreeNodeType == TreeNodeType.Root).FirstOrDefault();

                if (rootNode != null)
                {
                    pathTree.Nodes[0].Nodes.Clear();
                    pathTree.Nodes[0].Text = rootNode.TreeNodeName;
                    pathTree.Nodes[0].ContextMenuStrip = folderMenu;
                    pathTree.Nodes[0].Tag = rootNode;

                    foreach (var node in testRepository.TestNodeList.Where(x => x.MotherNode == rootNode))
                    {
                        LoadWholeTreeFomrDB(pathTree.Nodes[0], folderMenu, fileMenu, testRepository,  node);
                    }

                    pathTree.Nodes[0].Expand();

                    return (true, directoryPath);
                }else
                {
                    return (false, "Can not find root node");
                }
            }

            return (false, "Test Repository is NUll!");
        }

        public static void LoadWholeTreeFomrDB(TreeNode tempNode, ContextMenuStrip folderMenu, ContextMenuStrip fileMenu,
            TestRepository testRepository, Node currNode)
        {

            try
            {
                if (currNode.TreeNodeType == TreeNodeType.Directory)
                {
                    TreeNode tempDireNode = new TreeNode(currNode.TreeNodeName);

                    tempDireNode.ContextMenuStrip = folderMenu;
                    tempDireNode.Tag = currNode;

                    tempNode.Nodes.Add(tempDireNode);

                    foreach (var node in testRepository.TestNodeList.Where(x => x.MotherNode == currNode))
                    {
                        LoadWholeTreeFomrDB(tempDireNode, folderMenu, fileMenu, testRepository, node);
                    }
                }
                else if (currNode.TreeNodeType == TreeNodeType.File)
                {
                    TreeNode tempFileNode = new TreeNode(currNode.TreeNodeName);

                    tempFileNode.ContextMenuStrip = fileMenu;
                    tempFileNode.Tag = currNode;

                    tempNode.Nodes.Add(tempFileNode);
                }
            }
            catch (Exception err)
            {
                throw new Exception($"Exception happen in LoadWholeTreeFomrDB : { err.Message }");
            }

        }

        public static string LoadTestFile(TreeNode node, string rootPath, 
            Action<string> updateReplyMessage, Action<string> updateAfterReadFile)
        {
            string filePath = "";
            string replyMessage = "";
            string requestMessage = "";

            try
            {
                if (node.Tag is Node)
                {
                    requestMessage = ((Node)node.Tag).TreeNodeMessage;
                    updateAfterReadFile?.Invoke(requestMessage);
                }
                else
                {
                    // TODO - Optimize the node path!!
                    filePath = rootPath + node.FullPath.Substring(8);

                    requestMessage = FileProcessor.ReadFile(filePath, updateAfterReadFile);
                }
            }
            catch (Exception err)
            {
                replyMessage = $"Exception happen in LoadTestFile : { err.Message} {Environment.NewLine} File Path: {filePath}";
                updateReplyMessage?.Invoke(replyMessage);
            }

            return requestMessage;
        }




    }
}
