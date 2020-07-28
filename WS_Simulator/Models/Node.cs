using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WS_Simulator.Models
{
    public class Node
    {
        public int Id { get; set; }
        public string TreeNodeName { get; set; }
        [NotMapped]
        public TreeNode TreeNodeValue { get; set; }
        public string TreeNodeMessage { get; set; }
        public string TreeNodeReplyMessage { get; set; }

        public int? MotherNodeId { get; set; }
        public Node MotherNode { get; set; }
        public TreeNodeType TreeNodeType { get; set; }
        public TestNodeType TreeNodeTestType { get; set; } = TestNodeType.NORMAL;


        public int? RepositoryId { get; set; }
        public TestRepository Repository { get; set; }

        public Node(TreeNodeType treeNodeType, TreeNode treeNodeValue, Node motherNode)
        {
            TreeNodeType = treeNodeType;
            TreeNodeValue = treeNodeValue;
            TreeNodeName = treeNodeValue.Text;

            MotherNode = motherNode;
        }

        public Node()
        { 
        }
    }
}
