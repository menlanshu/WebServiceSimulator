using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WS_Simulator.Models
{
    public class TestNode 
    {
        public Node NodeInTree { get; set; }
        public string CurrentNodeSendMessage { get; set; }
        public string CurrentNodeReplyMessage { get; set; }
        public TestStatus TreeNodeSendStatus { get; set; }
        public bool NeedWait { get; set; } = false;
        public int NeedWaitTime { get; set; }
        public long TestPeriod { get; set; }

        public TestNode(Node baseNode, TestStatus testStatus, string treeNodeMessage)
        {
            NodeInTree = baseNode;

            this.CurrentNodeSendMessage = treeNodeMessage;

            this.TreeNodeSendStatus = testStatus;

        }
    }
}
