using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WS_Simulator.Models
{
    public class LocalFSNode : Node
    {
        public FileSystemInfo TreeNodeSystemInfo { get; set; }

        public LocalFSNode(TreeNodeType treeNodeType, TreeNode treeNodeValue)
        {
            base.TreeNodeType = treeNodeType;
            base.TreeNodeValue = treeNodeValue;
            base.TreeNodeName = treeNodeValue.Text;
            this.TreeNodeSystemInfo = (FileSystemInfo)treeNodeValue.Tag;
        }
    }
}
