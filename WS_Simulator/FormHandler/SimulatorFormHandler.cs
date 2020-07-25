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

            return (false, "");
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

        public static string LoadTestFile(TreeNode selectNode, string rootPath, 
            Action<string> updateReplyMessage, Action<string> updateAfterReadFile)
        {
            string filePath = "";
            string replyMessage = "";
            string requestMessage = "";

            try
            {
                if (selectNode != null)
                {
                    filePath = rootPath + selectNode.FullPath.Substring(8);

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
