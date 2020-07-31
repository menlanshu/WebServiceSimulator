using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WS_Simulator.DataAccess;

namespace WS_Simulator.Models
{
    public class DBNode : Node
    {
        public static Func<Node, string, TreeNodeType, Node> SaveNodeToTree;
        public static Action<string> UpdateAfterReadFile;

        private const string ResultFolderName = "Result";
        private const string DateTimeFormatStr = "yyyyMMddhhmmssfff";
        private const string ResultFilePostFix = "result";

        public DBNode(TreeNodeType treeNodeType, TreeNode treeNodeValue, Node motherNode, string fullPath) :
            base(treeNodeType, treeNodeValue, motherNode, fullPath)
        {
        }
        
        public DBNode()
        {

        }

        public override string GetCurrentMessage(bool updateControl)
        {
            string requestMessage = "";

            requestMessage = SQLiteDBProcessor.GetRequestMessageOfCurrentNode(this);
            if (updateControl)
            {
                UpdateAfterReadFile?.Invoke(requestMessage);
            }

            return requestMessage;
        }

        public override void SaveReplyResult(string currentNodeReplyMessage)
        {
            Node resultDirectory = null;
            Node fileNode = null;
            string fileName = $"{ TreeNodeName}.{ DateTime.Now.ToString(DateTimeFormatStr)}.{ResultFilePostFix}";

            var result = SQLiteDBProcessor.GetResultDirectoryNodeOfCurrentFolder(this, ResultFolderName);
            if (result == null)
            {
                resultDirectory = SaveNodeToTree?.Invoke(MotherNode, ResultFolderName,
                    TreeNodeType.Directory);

                resultDirectory.RepositoryId = this.RepositoryId;
                resultDirectory.Repository = this.Repository;
                SQLiteDBProcessor.SaveOneNode(resultDirectory);
            }
            else
            {
                resultDirectory = result;
            }


            result = SQLiteDBProcessor.GetChildNodeOfCurrentDirectory(resultDirectory, fileName);
            if (result == null)
            {
                fileNode = SaveNodeToTree?.Invoke(resultDirectory, fileName, TreeNodeType.File);

                fileNode.RepositoryId = this.RepositoryId;
                fileNode.Repository = this.Repository;
                fileNode.TreeNodeMessage = currentNodeReplyMessage;
                SQLiteDBProcessor.SaveOneNode(fileNode);
            }
        }

        public bool SaveNewNodeToDB()
        {
            Node fileNode = null;

            var result = SQLiteDBProcessor.GetChildNodeOfCurrentDirectory(MotherNode, TreeNodeName);
            if (result == null)
            {
                fileNode = SaveNodeToTree?.Invoke(MotherNode, TreeNodeName, TreeNodeType.File);

                fileNode.RepositoryId = this.RepositoryId;
                fileNode.Repository = this.Repository;
                fileNode.TreeNodeMessage = TreeNodeMessage;
                SQLiteDBProcessor.SaveOneNode(fileNode);

                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
