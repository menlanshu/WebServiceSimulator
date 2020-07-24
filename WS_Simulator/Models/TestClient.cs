using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using WebServiceStudio;
using WS_Simulator.DataAccess;

namespace WS_Simulator.Models
{
    public class TestClient
    {
        private char[] ConfigDelimeter = (";").ToCharArray();

        public int CurrentSendNodeCount { get; set; }
        public int CurrentActualSendNodeCount { get; set; }
        public List<int> AutoContextCountList { get; set; }
        public List<string> NeedSendExtensionName { get; set; }
        public GenerateContext AutoGenerateContext { get; set; }

        public Dictionary<int, TreeNode> WaitSendTreeNode { get; set; } = new Dictionary<int, TreeNode>();
        public TreeNode SendStartNode { get; set; }
        public TreeNode SendEndNode { get; set; }

        public bool SendTreeNodeList { get; set; }
        public bool DurationCheckNeed { get; set; }
        public NodeType DurationSendFlag { get; set; }
        public int SendIndex { get; set; }
        public int TotalCount { get; set; }
        public bool IsBatch { get; set; }

        public TreeNode CurrentLoopDirectoryNode { get; set; }
        public int CurrentPerfTestCount { get; set; }
        public int PerfMsgCount { get; set; }
        public string MethodName { get; set; }
        public bool IsPerfTest { get; set; }

        public string RequestMessage { get; set; }

        public string GetNodeValue(string nodeName)
        {
            string nodeValue = "";
            string path = "";
            bool getInnerXml = false;

            if (RequestMessage != "" && nodeName.Split(' ').Length >= 3)
            {

                if (IsBatch)
                {
                    path = ConfigurationManager.ConnectionStrings[nodeName.Split(' ')[1]] == null ? "" :
                    ConfigurationManager.ConnectionStrings[nodeName.Split(' ')[1]].ConnectionString;
                }
                else
                {
                    path = ConfigurationManager.AppSettings[nodeName.Split(' ')[1]] == null ? "" :
                        ConfigurationManager.AppSettings[nodeName.Split(' ')[1]];
                }
            }

            if (path != "")
            {
                nodeValue = "";
                string currentNodeValue = "";
                foreach (string tempPath in path.Split(ConfigDelimeter))
                {
                    (currentNodeValue, TotalCount) = XMLProcessor.GetVlaueByPath(tempPath, RequestMessage, IsBatch, TotalCount, SendIndex, getInnerXml);
                    nodeValue += currentNodeValue;
                }
            }

            return nodeValue;
        }

        public void RunAllNodesInDirectory(Action<string> updateReplyMessage, Action updateCurrentLoopText, Action<TreeNode> selectAndSendNode)
        {
            try
            {
                //initial wait send node related variable
                CurrentSendNodeCount = 1;
                CurrentActualSendNodeCount = 0;
                WaitSendTreeNode = new Dictionary<int, TreeNode>();
                SendTreeNodeList = true;
                DurationSendFlag = NodeType.NORMAL;

                if (SendStartNode != null)
                {
                    DurationCheckNeed = true;
                }
                else
                {
                    DurationCheckNeed = false;
                }

                if (CurrentLoopDirectoryNode.Nodes.Count > 0)
                {
                    int tempNodeCount = 0;
                    foreach (TreeNode tempNode in CurrentLoopDirectoryNode.Nodes)
                    {
                        if (tempNode.Nodes.Count == 0)
                        {
                            WaitSendTreeNode.Add(++tempNodeCount, tempNode);
                        }
                    }

                    if (!WaitSendTreeNode.ContainsValue(SendStartNode))
                    {
                        DurationCheckNeed = false;
                    }

                    SendAllWaitNodes(updateReplyMessage, updateCurrentLoopText, selectAndSendNode);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Excepetion happen in " + System.Reflection.MethodBase.GetCurrentMethod().Name + " : " + err.Message);
            }
        }

        public void SendAllWaitNodes(Action<string> updateReplyMessage, Action updateCurrentLoopText, Action<TreeNode> selectAndSendNode)
        {
            string replyMessage = "";
            if (string.IsNullOrEmpty(MethodName))
            {
                MessageBox.Show("Method name can not be empty!");
                return;
            }

            bool currentNodeNeedSend;

            if (CurrentSendNodeCount <= WaitSendTreeNode.Count)
            {
                if (WaitSendTreeNode.ContainsKey(CurrentSendNodeCount))
                {
                    TreeNode tempNode = WaitSendTreeNode[CurrentSendNodeCount];

                    currentNodeNeedSend = true;
                    if (DurationCheckNeed)
                    {
                        if (DurationSendFlag == NodeType.END)
                        {
                            currentNodeNeedSend = false;
                            SendTreeNodeList = false;
                        }
                        else
                        {
                            if (DurationSendFlag == NodeType.NORMAL)
                            {
                                if (tempNode == SendStartNode)
                                {
                                    DurationSendFlag = NodeType.START;
                                    currentNodeNeedSend = true;
                                }
                                else
                                {
                                    currentNodeNeedSend = false;
                                }
                            }
                            if (DurationSendFlag == NodeType.START)
                            {
                                if (tempNode == SendEndNode)
                                {
                                    DurationSendFlag = NodeType.END;
                                    currentNodeNeedSend = true;
                                }
                                else
                                {
                                    currentNodeNeedSend = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (DurationSendFlag == NodeType.END)
                        {
                            currentNodeNeedSend = false;
                            SendTreeNodeList = false;
                        }
                    }

                    string tempName = tempNode.Text;
                    tempName = tempName.Substring(tempName.LastIndexOf("."));

                    if (currentNodeNeedSend && NeedSendExtensionName.Contains(tempName))
                    {
                        CurrentActualSendNodeCount++;

                        selectAndSendNode?.Invoke(tempNode);
                    }
                    else
                    {
                        updateReplyMessage?.Invoke(replyMessage);
                    }
                }
            }
            else
            {
                SendTreeNodeList = false;
                WaitSendTreeNode = new Dictionary<int, TreeNode>();
                if (IsPerfTest == true)
                {
                    if (CurrentPerfTestCount < PerfMsgCount)
                    {
                        CurrentPerfTestCount++;
                        updateCurrentLoopText?.Invoke();
                        RunAllNodesInDirectory(updateReplyMessage, updateCurrentLoopText, selectAndSendNode);
                    }
                }
            }
        }

        private int CalculateCurrentCount(int contextCount, string contextMode)
        {
            int result = CurrentPerfTestCount;
            bool assigned = false;

            if (contextMode.ToUpper() == "INCREASE")
            {
                assigned = true;
                result = CurrentPerfTestCount - 1;
            }
            else if (contextMode.ToUpper() == "LOOP")
            {
                assigned = true;
                result = (CurrentPerfTestCount - 1) % (contextCount <= 0 ? 1 : contextCount);
            }
            else
            {
                for (int i = AutoContextCountList.Count() - 1; i >= 0; i--)
                {
                    if (AutoContextCountList[i] <= CurrentPerfTestCount && AutoContextCountList[i] >= contextCount)
                    {
                        if (AutoContextCountList[i] == contextCount)
                        {
                            result = contextCount == 0 ?
                                (assigned ? result : result - 1) : result / contextCount;
                            assigned = true;
                        }
                        else
                        {
                            result = result % AutoContextCountList[i];
                            assigned = true;
                        }
                    }
                }
            }
            return assigned ? result : 0;
        }

        // Initial context
        // TODO - call from where?
        private bool InitialGenerateContext(out string errDesc)
        {
            errDesc = "";

            try
            {
                AutoGenerateContext = ConfigurationManager.GetSection("AutoGenerateContext") as GenerateContext;
                InitialGenerateContextCurrentCount();

                return true;
            }
            catch (Exception err)
            {
                 errDesc = $"Excepetion happen when initial Generate Context Group : { err.Message}";
                return false;
            }
        }

        private void InitialGenerateContextCurrentCount()
        {
            //CurrentPerfTestCount = 1;
            AutoContextCountList = new List<int>();
            foreach (ContextGroup currentObject in AutoGenerateContext.ContextGroupList)
            {
                currentObject.CurrentCount = 0;
                if (!AutoContextCountList.Contains(currentObject.Count))
                {
                    AutoContextCountList.Add(currentObject.Count);
                }
            }
            AutoContextCountList.Sort();
        }
        public string AutoChangeContextInfo(string message, Action<string> updateReplyMessage)
        {
            try
            {
                foreach (ContextGroup singleContextGroup in AutoGenerateContext.ContextGroupList)
                {
                    singleContextGroup.CurrentCount = CalculateCurrentCount(singleContextGroup.Count, singleContextGroup.Mode);
                }

                foreach (ContextGroup singleContextGroup in AutoGenerateContext.ContextGroupList)
                {
                    message = message.Replace(singleContextGroup.Name, 
                        (FormatCount(singleContextGroup.CurrentCount,singleContextGroup.Type)).ToString());
                }

            }
            catch (Exception err)
            {
                updateReplyMessage?.Invoke($"Excepetion happen in { System.Reflection.MethodBase.GetCurrentMethod().Name } : { err.Message }");
            }

            return message;
        }
        private string FormatCount(int count, string type)
        {
            string result = "";

            result = string.Format(type, count);
            return result;
        }


    }
}
