using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WS_Simulator.Models
{
    public class TestNode : Node
    {
        public string TreeNodeMessage { get; set; }
        public TestStatus TreeNodeSendStatus { get; set; }
        public bool NeedSend { get; set; }
        public TestNode(TreeNodeType treeNodeType, TreeNode treeNodeValue, 
            TestStatus testStatus, string treeNodeMessage, bool needSend)
        {
            base.TreeNodeType = treeNodeType;
            base.TreeNodeValue = treeNodeValue;
            base.TreeNodeName = treeNodeValue.Text;

            this.TreeNodeSendStatus = testStatus;
            this.TreeNodeMessage = treeNodeMessage;
            this.NeedSend = needSend;
        }
    }
}
