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
        public Action TimerStart;

        private char[] ConfigDelimeter = (";").ToCharArray();

        public string RootDirectoryPath { get; set; }
        public int CurrentSendNodeCount { get; set; }
        public int CurrentActualSendNodeCount { get; set; }
        public List<int> AutoContextCountList { get; set; }
        public List<string> NeedSendExtensionName { get; set; }
        public GenerateContext AutoGenerateContext { get; private set; }

        public Dictionary<int, TreeNode> WaitSendTreeNode { get; set; } = new Dictionary<int, TreeNode>();
        public TreeNode SendStartNode { get; set; }
        public TreeNode SendEndNode { get; set; }

        public bool SendTreeNodeList { get; set; }
        public bool DurationCheckNeed { get; set; }
        public NodeType DurationSendFlag { get; set; }
        public int SendIndex { get; set; }
        public int TotalCount { get; set; }
        public bool IsBatch { get; set; }
        public bool IsDBHelperNeed { get; set; }

        public TreeNode CurrentLoopDirectoryNode { get; set; }
        public int CurrentPerfTestCount { get; set; }
        public int PerfMsgCount { get; set; }
        public string MethodName { get; set; }
        public bool IsPerfTest { get; set; }
        public bool ManualStop { get; set; }

        public string RequestMessage { get; set; }

        public async Task SendMessageToE3(string methodName, TreeNode sendNode, string requestMessage,
            Action<string> updateReplyMessage, Action timeStart)
        {
            string nodeName = sendNode.Text;
            bool isSqlFile = false;
            string replyMessage;

            try
            {
                requestMessage = XMLProcessor.RestoreXml(requestMessage);

                // TODO - See any influence for this mark
                //this.rtbRequest.Text = requestMessage;

                if (string.IsNullOrEmpty(requestMessage))
                {
                    replyMessage = "Request message is empty, can't send message to R2R";
                    updateReplyMessage?.Invoke(replyMessage);
                    return;
                }

                string nodeNamePostFix = nodeName.Substring(nodeName.LastIndexOf("."));
                if (nodeNamePostFix.ToUpper() == ".SQL" && !IsDBHelperNeed)
                {
                    replyMessage = "Do not support SQL according to configuration!";
                    updateReplyMessage?.Invoke(replyMessage);
                    return;
                }
                else if (nodeNamePostFix.ToUpper() == ".SQL" && IsDBHelperNeed)
                {
                    isSqlFile = true;
                }

                // TODO - Check this mark has any influence
                //if (!isSqlFile && !cmbMethodName.Items.Contains(methodName))
                //{
                //    MessageBox.Show("You key in a illegal method name for test, please check it");
                //    return;
                //}

                timeStart?.Invoke();

                if (isSqlFile)
                {
                    await Task.Run(() => DBProcessor.HandleDBAction(requestMessage, updateReplyMessage));
                }
                else
                {
                    // TODO - this is not a good practice
                    RequestMessage = requestMessage;
                    await Task.Run(() => WebServiceProcessor.InvokeWebMethod(TotalCount, updateReplyMessage));
                }

            }
            catch (Exception err)
            {
                replyMessage = "Excepetion happen in " + System.Reflection.MethodBase.GetCurrentMethod().Name + " : " + err.Message;
                updateReplyMessage?.Invoke(replyMessage);

                //if (_wsConfig.SendTreeNodeList)
                //{
                //    this.lbCurrentCount.Text = _testClient.CurrentActualSendNodeCount + "/" + _testClient.CurrentSendNodeCount + "/" + waitSendTreeNode.Count;
                //    _testClient.CurrentSendNodeCount++;
                //    SendAllWaitNodes();
                //}
            }
        }

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

        public async Task RunAllNodesInDirectory(Action<string> updateReplyMessage, Action updateCurrentLoopText, Func<TreeNode, string> selectAndSendNode)
        {
            try
            {
                //initial wait send node related variable
                CurrentSendNodeCount = 1;
                CurrentActualSendNodeCount = 0;
                WaitSendTreeNode = new Dictionary<int, TreeNode>();
                SendTreeNodeList = true;
                DurationSendFlag = NodeType.NORMAL;
                ManualStop = false;

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

                    await SendAllWaitNodes(updateReplyMessage, updateCurrentLoopText, selectAndSendNode);
                }
            }
            catch (Exception err)
            {
                updateReplyMessage?.Invoke($"Excepetion happen in { System.Reflection.MethodBase.GetCurrentMethod().Name} : { err.Message} ");
            }
        }

        public async Task SendAllWaitNodes(Action<string> updateReplyMessage, Action updateCurrentLoopText, Func<TreeNode, string> selectAndSendNode)
        {

            if (string.IsNullOrEmpty(MethodName))
            {
                updateReplyMessage?.Invoke("Method name can not be empty!");
                return;
            }

            bool currentNodeNeedSend;

            foreach (var tempNode in WaitSendTreeNode.Values)
            {
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

                    RequestMessage = selectAndSendNode?.Invoke(tempNode);

                    SendIndex = 0;
                    TotalCount = 1;

                    await SendMessageToE3(MethodName, tempNode, RequestMessage, updateReplyMessage, TimerStart);
                }

            }

            SendTreeNodeList = false;
            WaitSendTreeNode = new Dictionary<int, TreeNode>();

            if (IsPerfTest == true && ManualStop == false)
            {
                if (CurrentPerfTestCount < PerfMsgCount)
                {
                    CurrentPerfTestCount++;
                    updateCurrentLoopText?.Invoke();
                    await RunAllNodesInDirectory(updateReplyMessage, updateCurrentLoopText, selectAndSendNode);
                }
            }


            //if (CurrentSendNodeCount <= WaitSendTreeNode.Count)
            //{
            //    if (WaitSendTreeNode.ContainsKey(CurrentSendNodeCount))
            //    {
            //        TreeNode tempNode = WaitSendTreeNode[CurrentSendNodeCount];

            //        currentNodeNeedSend = true;
            //        if (DurationCheckNeed)
            //        {
            //            if (DurationSendFlag == NodeType.END)
            //            {
            //                currentNodeNeedSend = false;
            //                SendTreeNodeList = false;
            //            }
            //            else
            //            {
            //                if (DurationSendFlag == NodeType.NORMAL)
            //                {
            //                    if (tempNode == SendStartNode)
            //                    {
            //                        DurationSendFlag = NodeType.START;
            //                        currentNodeNeedSend = true;
            //                    }
            //                    else
            //                    {
            //                        currentNodeNeedSend = false;
            //                    }
            //                }
            //                if (DurationSendFlag == NodeType.START)
            //                {
            //                    if (tempNode == SendEndNode)
            //                    {
            //                        DurationSendFlag = NodeType.END;
            //                        currentNodeNeedSend = true;
            //                    }
            //                    else
            //                    {
            //                        currentNodeNeedSend = true;
            //                    }
            //                }
            //            }
            //        }
            //        else
            //        {
            //            if (DurationSendFlag == NodeType.END)
            //            {
            //                currentNodeNeedSend = false;
            //                SendTreeNodeList = false;
            //            }
            //        }

            //        string tempName = tempNode.Text;
            //        tempName = tempName.Substring(tempName.LastIndexOf("."));

            //        if (currentNodeNeedSend && NeedSendExtensionName.Contains(tempName))
            //        {
            //            CurrentActualSendNodeCount++;

            //            RequestMessage = selectAndSendNode?.Invoke(tempNode);

            //            SendIndex = 0;
            //            TotalCount = 1;

            //            await SendMessageToE3(MethodName, tempNode, RequestMessage, updateReplyMessage, TimerStart);
            //        }
            //        else
            //        {
            //            updateReplyMessage?.Invoke(replyMessage);
            //        }
            //    }
            //}
            //else
            //{
            //    SendTreeNodeList = false;
            //    WaitSendTreeNode = new Dictionary<int, TreeNode>();
            //    if (IsPerfTest == true)
            //    {
            //        if (CurrentPerfTestCount < PerfMsgCount)
            //        {
            //            CurrentPerfTestCount++;
            //            updateCurrentLoopText?.Invoke();
            //            await RunAllNodesInDirectory(updateReplyMessage, updateCurrentLoopText, selectAndSendNode);
            //        }
            //    }
            //}
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
        public bool InitialGenerateContext(out string errDesc)
        {
            errDesc = "";

            try
            {
                AutoGenerateContext = ConfigurationManager.GetSection("AutoGenerateContext") as GenerateContext;
                InitialGenerateContextCurrentCount(AutoGenerateContext);

                return true;
            }
            catch (Exception err)
            {
                 errDesc = $"Excepetion happen when initial Generate Context Group : { err.Message}";
                return false;
            }
        }

        private void InitialGenerateContextCurrentCount(GenerateContext generateContext)
        {
            //CurrentPerfTestCount = 1;
            AutoContextCountList = new List<int>();
            foreach (ContextGroup currentObject in generateContext.ContextGroupList)
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
