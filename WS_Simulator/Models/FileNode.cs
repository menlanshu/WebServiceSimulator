using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WS_Simulator.DataAccess;

namespace WS_Simulator.Models
{
    public class FileNode: Node
    {
        public static Func<Node, string, TreeNodeType, Node> SaveNodeToTree;
        public static Action<string> UpdateAfterReadFile;

        public static string RootDirectoryPath { get; set; }

        private const string ResultFolderName = "Result";
        private const string DateTimeFormatStr = "yyyyMMddhhmmssfff";
        private const string ResultFilePostFix = "result";

        public FileNode(TreeNodeType treeNodeType, TreeNode treeNodeValue, Node motherNode, string fullPath) : 
            base(treeNodeType, treeNodeValue, motherNode, fullPath)
        {
        }

        public FileNode()
        { }

        public override string GetCurrentMessage(bool updateControl)
        {
            string filePath = "";
            string requestMessage = "";

            filePath = FileProcessor.GetFullPath(RootDirectoryPath, NodeFullPath);

            requestMessage = FileProcessor.ReadFile(filePath, 
                updateControl ? UpdateAfterReadFile : null);

            return requestMessage;
        }

        public Node SaveCurrentNodeToMotherNode(Node motherNode, string requestMessage)
        {
            string directoryPath = FileProcessor.GetFullDirectoryPath(RootDirectoryPath, 
                TreeNodeType == TreeNodeType.Directory ? $@"{NodeFullPath}\" : NodeFullPath);

            if (this.TreeNodeType == TreeNodeType.Directory)
            {
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                    return (Node)SaveNodeToTree?.Invoke(motherNode, TreeNodeName, TreeNodeType.Directory);
                }
            }
            else if (this.TreeNodeType == TreeNodeType.File)
            {
                string fileNameFullPath = $@"{directoryPath }{TreeNodeName}";

                if (!File.Exists(fileNameFullPath))
                {
                    FileProcessor.SaveFile(fileNameFullPath, requestMessage);
                    return SaveNodeToTree?.Invoke(motherNode, TreeNodeName,
                        TreeNodeType.File);
                }
            }

            return null;
        }

        public override void SaveReplyResult(string currentNodeReplyMessage)
        {
            Node resultDirectory = null;
            string fileName = $"{ TreeNodeName}.{ DateTime.Now.ToString(DateTimeFormatStr)}.{ResultFilePostFix}";

            string directoryPath = $@"{FileProcessor.GetFullDirectoryPath(RootDirectoryPath, 
                TreeNodeType == TreeNodeType.Directory ? $@"{NodeFullPath}\" : NodeFullPath)}{ResultFolderName}\";

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                resultDirectory = SaveNodeToTree?.Invoke(MotherNode, ResultFolderName,
                    TreeNodeType.Directory);
            }
            else
            {
                foreach (TreeNode node in MotherNode.TreeNodeValue.Nodes)
                {
                    if (node.Text == ResultFolderName)
                    {
                        resultDirectory = (Node)node.Tag;
                        break;
                    }
                }
            }

            string fileNameFullPath = $@"{directoryPath }{fileName}";

            if (!File.Exists(fileNameFullPath))
            {
                FileProcessor.SaveFile(fileNameFullPath, currentNodeReplyMessage);
                SaveNodeToTree?.Invoke(resultDirectory, fileName, TreeNodeType.File);
            }
        }
    }
}
