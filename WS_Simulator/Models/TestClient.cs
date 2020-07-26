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
        public Action<string> UpdateReplyMessage;
        public Action<string> UpdateAfterReadFile;

        private char[] ConfigDelimeter = (";").ToCharArray();

        public string RootDirectoryPath { get; set; }
        public int CurrentSendNodeCount { get; set; }
        public int CurrentActualSendNodeCount { get; set; }
        public List<int> AutoContextCountList { get; set; }
        public List<string> NeedSendExtensionName { get; set; }
        public GenerateContext AutoGenerateContext { get; private set; }

        public List<TestNode> WaitSendTreeNode { get; private set; } = new List<TestNode>();
        public TreeNode SendStartNode { get; set; }
        public TreeNode SendEndNode { get; set; }

        public bool DurationCheckNeed { get; set; }
        public TestNodeType DurationSendFlag { get; set; }
        public bool IsBatch { get; set; }
        public bool IsDBHelperNeed { get; set; }

        public int CurrentPerfTestCount { get; set; }
        public int PerfMsgCount { get; set; }
        public string MethodName { get; set; }
        public bool IsPerfTest { get; set; }
        public bool ManualStop { get; set; }

        public List<Node> CurrNodeList { get; set; } = new List<Node>();

        public string RequestMessage { get; set; }

        private async Task SendMessageToE3(TreeNode sendNode, string requestMessage)
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
                    UpdateReplyMessage?.Invoke(replyMessage);
                    return;
                }

                string nodeNamePostFix = nodeName.Substring(nodeName.LastIndexOf("."));
                if (nodeNamePostFix.ToUpper() == ".SQL" && !IsDBHelperNeed)
                {
                    replyMessage = "Do not support SQL according to configuration!";
                    UpdateReplyMessage?.Invoke(replyMessage);
                    return;
                }
                else if (nodeNamePostFix.ToUpper() == ".SQL" && IsDBHelperNeed)
                {
                    isSqlFile = true;
                }

                TimerStart?.Invoke();

                if (isSqlFile)
                {
                    await Task.Run(() => DBProcessor.HandleDBAction(requestMessage, UpdateReplyMessage));
                }
                else
                {
                    // TODO - this is not a good practice
                    RequestMessage = requestMessage;
                    await Task.Run(() => WebServiceProcessor.InvokeWebMethod(UpdateReplyMessage));
                }

            }
            catch (Exception err)
            {
                replyMessage = "Excepetion happen in " + System.Reflection.MethodBase.GetCurrentMethod().Name + " : " + err.Message;
                UpdateReplyMessage?.Invoke(replyMessage);
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
                    currentNodeValue = XMLProcessor.GetVlaueByPath(tempPath, RequestMessage, IsBatch, getInnerXml);
                    nodeValue += currentNodeValue;
                }
            }

            return nodeValue;
        }

        public async Task RunAllNodesInDirectory(Action updateCurrentLoopText, Func<TreeNode, string> selectAndSendNode)
        {
            try
            {
                //initial wait send node related variable
                CurrentSendNodeCount = 1;
                CurrentActualSendNodeCount = 0;
                DurationSendFlag = TestNodeType.NORMAL;
                ManualStop = false;

                if (SendStartNode != null)
                {
                    DurationCheckNeed = true;
                }
                else
                {
                    DurationCheckNeed = false;
                }

                // TODO - Optimization
                if (!WaitSendTreeNode.Select(x => x.TreeNodeValue).Contains(SendStartNode))
                {
                    DurationCheckNeed = false;
                }

                await SendAllWaitNodes(updateCurrentLoopText, selectAndSendNode);
            }
            catch (Exception err)
            {
                UpdateReplyMessage?.Invoke($"Excepetion happen in { System.Reflection.MethodBase.GetCurrentMethod().Name} : { err.Message} ");
            }
        }

        public async Task SendAllWaitNodes(Action updateCurrentLoopText, Func<TreeNode, string> selectAndSendNode)
        {

            if (string.IsNullOrEmpty(MethodName))
            {
                UpdateReplyMessage?.Invoke("Method name can not be empty!");
                return;
            }

            bool currentNodeNeedSend;

            foreach (var tempNode in WaitSendTreeNode)
            {
                currentNodeNeedSend = true;
                if (DurationCheckNeed)
                {
                    if (DurationSendFlag == TestNodeType.END)
                    {
                        currentNodeNeedSend = false;
                    }
                    else
                    {
                        if (DurationSendFlag == TestNodeType.NORMAL)
                        {
                            if (tempNode.TreeNodeValue == SendStartNode)
                            {
                                DurationSendFlag = TestNodeType.START;
                                currentNodeNeedSend = true;
                            }
                            else
                            {
                                currentNodeNeedSend = false;
                            }
                        }
                        if (DurationSendFlag == TestNodeType.START)
                        {
                            if (tempNode.TreeNodeValue == SendEndNode)
                            {
                                DurationSendFlag = TestNodeType.END;
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
                    if (DurationSendFlag == TestNodeType.END)
                    {
                        currentNodeNeedSend = false;
                    }
                }

                string tempName = tempNode.TreeNodeName;
                tempName = tempName.Substring(tempName.LastIndexOf("."));

                if (currentNodeNeedSend && NeedSendExtensionName.Contains(tempName))
                {
                    CurrentActualSendNodeCount++;

                    //RequestMessage = selectAndSendNode?.Invoke(tempNode);

                    await SendMessageToE3(tempNode.TreeNodeValue, tempNode.TreeNodeMessage);
                }else
                {
                    UpdateReplyMessage?.Invoke("No need send message.");
                }

            }

            WaitSendTreeNode = new List<TestNode>();

            if (IsPerfTest == true && ManualStop == false)
            {
                if (CurrentPerfTestCount < PerfMsgCount)
                {
                    CurrentPerfTestCount++;
                    updateCurrentLoopText?.Invoke();
                    await RunAllNodesInDirectory(updateCurrentLoopText, selectAndSendNode);
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
        public string AutoChangeContextInfo(string message)
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
                UpdateReplyMessage?.Invoke($"Excepetion happen in { System.Reflection.MethodBase.GetCurrentMethod().Name } : { err.Message }");
            }

            return message;
        }
        private string FormatCount(int count, string type)
        {
            string result = "";

            result = string.Format(type, count);
            return result;
        }

        public void AddTestNode(TreeNode sendNode, Func<string, string, Action<string>, Action<string>, string> loadFile)
        {
            string requestMessage = "";
            WaitSendTreeNode = new List<TestNode>();
            if(sendNode.Nodes.Count > 0)
            {
                foreach (TreeNode node in sendNode.Nodes)
                {
                    requestMessage = loadFile(node.FullPath, RootDirectoryPath, UpdateReplyMessage, null);
                    TestNode newNode = new TestNode(TreeNodeType.File, node, TestStatus.WaitForSend, requestMessage, true);
                    WaitSendTreeNode.Add(newNode);
                }
            }else
            {
                requestMessage = loadFile(sendNode.FullPath, RootDirectoryPath, UpdateReplyMessage, null);
                TestNode newNode = new TestNode(TreeNodeType.File, sendNode, TestStatus.WaitForSend, requestMessage, true);
                WaitSendTreeNode.Add(newNode);
            }
        }


        public bool InTesting()
        {
            if(WaitSendTreeNode.Count > 0)
            {
                return true;
            }else
            {
                return false;
            }    
        }

        public void InitialCurrNodeList(TreeNode rootNode)
        {
            int nodeId = 1;
            CurrNodeList = new List<Node>();

            // assign root node
            Node newNode = new LocalFSNode(TreeNodeType.Root, rootNode);
            newNode.Id = nodeId;
            CurrNodeList.Add(newNode);

            // get all child node of root node
            foreach (TreeNode node in rootNode.Nodes)
            {
                CovertTreeNodeToNodeList(node);
            }
        }

        private void CovertTreeNodeToNodeList(TreeNode currNode)
        {
            int currNodeId = CurrNodeList.Max(x => x.Id);
            if(currNode.Nodes.Count > 0)
            {
                currNodeId++;
                Node newNode = new LocalFSNode(TreeNodeType.Directory, currNode);
                newNode.Id = currNodeId;

                foreach(TreeNode node in currNode.Nodes)
                {
                    CovertTreeNodeToNodeList(node);
                }
            }
            else
            {
                // Covert file node to local file system node
                currNodeId++;
                Node newNode = new LocalFSNode(TreeNodeType.File, currNode);
                newNode.Id = currNodeId;

                CurrNodeList.Add(newNode);
            }
        }

    }
}
