using System;
using System.Collections.Generic;
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
        public TreeNode TreeNodeValue { get; set; }
        public TreeNodeType TreeNodeType { get; set; }
        public TestNodeType TreeNodeTestType { get; set; } = TestNodeType.NORMAL;

        public Node(int id, TreeNodeType treeNodeType, TreeNode treeNodeValue)
        {
            Id = id;
            TreeNodeType = treeNodeType;
            TreeNodeValue = treeNodeValue;
            TreeNodeName = treeNodeValue.Text;
        }
    }
}
