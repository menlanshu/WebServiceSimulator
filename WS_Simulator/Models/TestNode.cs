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
        public TestStatus TreeNodeSendStatus { get; set; }
        public bool NeedSend { get; set; }
        public long TestPeriod { get; set; }

        public TestNode(Node baseNode,
            TestStatus testStatus, string treeNodeMessage, bool needSend) : 
            base(baseNode.TreeNodeType, baseNode.TreeNodeValue, baseNode.MotherNode, baseNode.NodeFullPath)
        {
            this.TreeNodeSendStatus = testStatus;
            this.TreeNodeMessage = treeNodeMessage;
            this.NeedSend = needSend;

            this.TreeNodeSourceType = baseNode.TreeNodeSourceType;
            this.MotherNodeId = baseNode.MotherNodeId;
        }
    }
}
