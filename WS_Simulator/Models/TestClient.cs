using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
        public Action<TreeNode, Color> UpdateNodeColor;
        public Action<List<TestNode>> UpdateSendNodeListStatus;
        public Func<Node, string, TreeNodeType, Node> SaveNodeToTree;

        private char[] ConfigDelimeter = (";").ToCharArray();
        private const string ResultFolderName = "Result";
        private const string ResultFilePostFix = "result";

        public string RootDirectoryPath { get; set; }
        public int CurrentSendNodeCount { get; set; }
        public int CurrentActualSendNodeCount { get; set; }
        public List<int> AutoContextCountList { get; set; }
        public List<string> NeedSendExtensionName { get; set; }
        public GenerateContext AutoGenerateContext { get; private set; }

        public List<TestNode> WaitSendTreeNode { get; private set; } = new List<TestNode>();

        public bool AutoSaveReply { get; set; }
        public bool IsBatch { get; set; }
        public bool IsDBHelperNeed { get; set; }

        public int CurrentPerfTestCount { get; set; }
        public int PerfMsgCount { get; set; }
        public string MethodName { get; set; }
        public bool IsPerfTest { get; set; }
        public bool ManualStop { get; set; }

        public List<Node> CurrNodeList { get; set; } = new List<Node>();
        public TestRepository CurrentRepository { get; set; }

        public string RequestMessage { get; set; }
        public string ReplyMessage { get; set; }


        private async Task SendMessageToE3(TreeNode sendNode, string requestMessage)
        {
            string nodeName = sendNode.Text;
            bool isSqlFile = false;

            try
            {
                requestMessage = XMLProcessor.RestoreXml(requestMessage);

                // TODO - See any influence for this mark
                //this.rtbRequest.Text = requestMessage;

                if (string.IsNullOrEmpty(requestMessage))
                {
                    ReplyMessage = "Request message is empty, can't send message to R2R";
                    UpdateReplyMessage?.Invoke(ReplyMessage);
                    return;
                }

                string nodeNamePostFix = nodeName.Substring(nodeName.LastIndexOf("."));
                if (nodeNamePostFix.ToUpper() == ".SQL" && !IsDBHelperNeed)
                {
                    ReplyMessage = "Do not support SQL according to configuration!";
                    UpdateReplyMessage?.Invoke(ReplyMessage);
                    return;
                }
                else if (nodeNamePostFix.ToUpper() == ".SQL" && IsDBHelperNeed)
                {
                    isSqlFile = true;
                }

                TimerStart?.Invoke();

                // TODO - this is not a good practice
                if (WaitSendTreeNode.Count > 0)
                {
                    RequestMessage = requestMessage;
                }

                if (isSqlFile)
                {
                    ReplyMessage = await Task.Run(() => DBProcessor.HandleDBAction(RequestMessage, UpdateReplyMessage));
                }
                else
                {
                    ReplyMessage = await Task.Run(() => WebServiceProcessor.InvokeWebMethod(UpdateReplyMessage));
                }

            }
            catch (Exception err)
            {
                ReplyMessage = "Excepetion happen in " + System.Reflection.MethodBase.GetCurrentMethod().Name + " : " + err.Message;
                UpdateReplyMessage?.Invoke(ReplyMessage);
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
                //DurationSendFlag = TestNodeType.NORMAL;
                ManualStop = false;

                //if (WaitSendTreeNode.Where(x => x.TreeNodeTestType == TestNodeType.START) != null)
                //{
                //    DurationCheckNeed = true;
                //}
                //else
                //{
                //    DurationCheckNeed = false;
                //}

                //TODO - Optimization
                //if (!WaitSendTreeNode.Select(x => x.TreeNodeValue).Contains(SendStartNode))
                //{
                //    DurationCheckNeed = false;
                //}

                await SendAllWaitNodes(updateCurrentLoopText, selectAndSendNode);
            }
            catch (Exception err)
            {
                WaitSendTreeNode.Clear();
                ReplyMessage = $"Excepetion happen in { System.Reflection.MethodBase.GetCurrentMethod().Name} : { err.Message} ";
                UpdateReplyMessage?.Invoke(ReplyMessage);
            }
        }

        public async Task SendAllWaitNodes(Action updateCurrentLoopText, Func<TreeNode, string> selectAndSendNode)
        {

            if (string.IsNullOrEmpty(MethodName))
            {
                ReplyMessage = "Method name can not be empty!";
                UpdateReplyMessage?.Invoke(ReplyMessage);
                return;
            }

            //bool currentNodeNeedSend;

            foreach (var tempNode in WaitSendTreeNode)
            {
                if (ManualStop == true)
                {
                    WaitSendTreeNode.Clear();
                    return;
                }

                // Send Node Start
                tempNode.TreeNodeSendStatus = TestStatus.Sending;
                UpdateSendNodeListStatus?.Invoke(WaitSendTreeNode);
                Stopwatch newWatch = new Stopwatch();
                newWatch.Start();

                string tempName = tempNode.TreeNodeName;
                tempName = tempName.Substring(tempName.LastIndexOf("."));

                //if (currentNodeNeedSend && NeedSendExtensionName.Contains(tempName))
                if (NeedSendExtensionName.Contains(tempName))
                {
                    CurrentActualSendNodeCount++;

                    //RequestMessage = selectAndSendNode?.Invoke(tempNode);

                    await SendMessageToE3(tempNode.TreeNodeValue, tempNode.TreeNodeMessage);
                } else
                {
                    ReplyMessage = "No need send message.";
                    UpdateReplyMessage?.Invoke("No need send message.");
                }

                // Send Node End
                newWatch.Stop();
                tempNode.TestPeriod = newWatch.ElapsedMilliseconds;
                tempNode.TreeNodeSendStatus = TestStatus.Sended;
                UpdateSendNodeListStatus?.Invoke(WaitSendTreeNode);

                tempNode.TreeNodeReplyMessage = ReplyMessage;
                if (AutoSaveReply == true)
                {  
                    await Task.Run(() => SaveReplyMsgToFileMethod(tempNode));
                }

            }

            if (IsPerfTest == true && ManualStop == false)
            {
                if (CurrentPerfTestCount < PerfMsgCount)
                {
                    CurrentPerfTestCount++;
                    updateCurrentLoopText?.Invoke();
                    await RunAllNodesInDirectory(updateCurrentLoopText, selectAndSendNode);
                }else
                {
                    WaitSendTreeNode.Clear();
                }
            }else
            {
                WaitSendTreeNode.Clear();
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

        public void AddTestNode(TreeNode sendNode, Func<Node, string, Action<string>, Action<string>, string> loadFile)
        {
            string requestMessage = "";
            WaitSendTreeNode = new List<TestNode>();
            if(sendNode.Nodes.Count > 0)
            {
                // Check current directory is for duration test or not
                List<Node> sendNodeList = new List<Node>();
                foreach (TreeNode treeNode in sendNode.Nodes)
                {
                    sendNodeList.Add((Node)treeNode.Tag);
                }
                bool durationCheckNeed = sendNodeList.Where(x => x.TreeNodeTestType == TestNodeType.START).Count() > 0;
                bool saveTestNode = false;

                foreach (Node node in sendNodeList)
                {
                    if (node.TreeNodeType == TreeNodeType.File)
                    {
                        if (durationCheckNeed == true && node.TreeNodeTestType == TestNodeType.START)
                        {
                            saveTestNode = true;
                        }
                        else if (durationCheckNeed == false)
                        {
                            saveTestNode = true;
                        }

                        if (saveTestNode == true)
                        {
                            requestMessage = loadFile(node, RootDirectoryPath, UpdateReplyMessage, null);
                            TestNode newNode = new TestNode(node, TestStatus.WaitForSend, requestMessage, true);
                            WaitSendTreeNode.Add(newNode);
                        }

                        if (durationCheckNeed == true && node.TreeNodeTestType == TestNodeType.END)
                        {
                            saveTestNode = false;
                        }
                    }
                }
            }else
            {
                requestMessage = loadFile((Node)sendNode.Tag, RootDirectoryPath, UpdateReplyMessage, null);
                TestNode newNode = new TestNode((Node)sendNode.Tag, TestStatus.WaitForSend, requestMessage, true);
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

        /*
        // Initial Current Node list according to root node of path tree
        public void InitialCurrNodeList(TreeNode rootNode)
        {
            CurrNodeList = new List<Node>();

            // assign root node
            Node newNode = new Node(TreeNodeType.Root, rootNode, null);
            CurrNodeList.Add(newNode);

            // get all child node of root node
            foreach (TreeNode node in rootNode.Nodes)
            {
                CovertTreeNodeToNodeList(node, newNode);
            }
        }

        private void CovertTreeNodeToNodeList(TreeNode currNode, Node motherNode)
        {
            if(currNode.Tag is DirectoryInfo)
            {
                Node newNode = new Node( TreeNodeType.Directory, currNode, motherNode);
                CurrNodeList.Add(newNode);

                foreach(TreeNode node in currNode.Nodes)
                {
                    CovertTreeNodeToNodeList(node, newNode);
                }
            }
            else
            {
                // Covert file node to local file system node
                Node newNode = new Node(TreeNodeType.File, currNode, motherNode);

                CurrNodeList.Add(newNode);
            }
        }*/


        public void SetSendStartNode(TreeNode fileNode)
        {
            // TODO - tree to get a start and end for each directory
            Node startNode = CurrNodeList.Where(x => x.TreeNodeTestType == TestNodeType.START).FirstOrDefault();
            Node endNode = CurrNodeList.Where(x => x.TreeNodeTestType == TestNodeType.END).FirstOrDefault();

            // TODO - Delete those code
            UpdateNodeColor?.Invoke(startNode?.TreeNodeValue, Color.Black);
            UpdateNodeColor?.Invoke(endNode?.TreeNodeValue, Color.Black);

            if (startNode != null)
            {
                startNode.TreeNodeTestType = TestNodeType.NORMAL;
            }
            if(endNode != null)
            {
                endNode.TreeNodeTestType = TestNodeType.NORMAL;
            }

            Node newStartNode = CurrNodeList.Where(x => x.TreeNodeValue == fileNode).FirstOrDefault();
            newStartNode.TreeNodeTestType = TestNodeType.START;

            //UpdateNodeColor?.Invoke(SendStartNode, Color.Black);
            UpdateNodeColor?.Invoke(newStartNode?.TreeNodeValue, Color.Red);
        }

        public void SetSendEndNode(TreeNode fileNode)
        {
            // TODO - tree to get a start and end for each directory
            Node newEndNode = CurrNodeList.Where(x => x.TreeNodeValue == fileNode).FirstOrDefault();
            if (newEndNode != null)
            { 
                newEndNode.TreeNodeTestType = TestNodeType.END;
            }
            //SendEndNode = fileNode;

            //UpdateNodeColor?.Invoke(SendEndNode, Color.Red);
            UpdateNodeColor?.Invoke(newEndNode?.TreeNodeValue, Color.Red);
        }

        public void ClearDurationNode()
        {
            // TODO - tree to get a start and end for each directory
            Node startNode = CurrNodeList.Where(x => x.TreeNodeTestType == TestNodeType.START).FirstOrDefault();
            Node endNode = CurrNodeList.Where(x => x.TreeNodeTestType == TestNodeType.END).FirstOrDefault();

            UpdateNodeColor?.Invoke(startNode?.TreeNodeValue, Color.Black);
            UpdateNodeColor?.Invoke(endNode?.TreeNodeValue, Color.Black);

            if (startNode != null)
            {
                startNode.TreeNodeTestType = TestNodeType.NORMAL;
            }
            if (endNode != null)
            {
                endNode.TreeNodeTestType = TestNodeType.NORMAL;
            }


            //UpdateNodeColor?.Invoke(SendStartNode, Color.Black);
            //UpdateNodeColor?.Invoke(SendEndNode, Color.Black);

            //SendStartNode = null;
            //SendEndNode = null;
        }


        private void SaveReplyMsgToFileMethod(Node currNode)
        {
            if (currNode != null)
            {
                if (currNode.TreeNodeSourceType == SourceNodeType.DB)
                {
                    SaveReplyResultToDB(currNode);
                }
                else
                {
                    SaveReplyResultToLocal(currNode);
                }
            }
        }

        private void SaveReplyResultToDB(Node currNode)
        {
            Node resultDirectory = null;
            Node fileNode = null;
            string fileName = $"{ currNode.TreeNodeName}.{ DateTime.Now.ToString("yyyyMMddhhmmssfff")}.{ResultFilePostFix}";

            var result = CurrNodeList.Where(x => x.MotherNodeId == currNode.MotherNodeId &&
                x.TreeNodeType == TreeNodeType.Directory && x.TreeNodeName == ResultFolderName);
            if (result.Count() == 0)
            {
                resultDirectory = SaveNodeToTree(currNode.MotherNode, ResultFolderName,
                    TreeNodeType.Directory);

                resultDirectory.RepositoryId = CurrentRepository.Id;
                resultDirectory.Repository = CurrentRepository;
                SQLiteDBProcessor.SaveOneNode(resultDirectory);
            }else
            {
                resultDirectory = result.FirstOrDefault();
            }


            if (CurrNodeList.Where(x => x.MotherNodeId == resultDirectory.Id &&
                x.TreeNodeType == TreeNodeType.File && x.TreeNodeName == fileName).Count() == 0)
            {
                fileNode = SaveNodeToTree(resultDirectory, fileName, TreeNodeType.File);

                fileNode.RepositoryId = CurrentRepository.Id;
                fileNode.TreeNodeMessage = currNode.TreeNodeReplyMessage;
                SQLiteDBProcessor.SaveOneNode(fileNode);
            }
        }

        public bool SaveNewNodeToDB(Node currNode)
        {
            Node fileNode = null;

            if (CurrNodeList.Where(x => x.MotherNodeId == currNode.MotherNode?.Id &&
                x.TreeNodeType == TreeNodeType.File && x.TreeNodeName == currNode.TreeNodeName).Count() == 0)
            {
                fileNode = SaveNodeToTree(currNode.MotherNode, currNode.TreeNodeName, TreeNodeType.File);

                fileNode.RepositoryId = CurrentRepository.Id;
                fileNode.TreeNodeMessage = currNode.TreeNodeMessage;
                SQLiteDBProcessor.SaveOneNode(fileNode);

                return true;
            }else
            {
                return false;
            }

        }



        private void SaveReplyResultToLocal(Node currNode)
        {
            Node resultDirectory = null;
            string fileName = $"{ currNode.TreeNodeName}.{ DateTime.Now.ToString("yyyyMMddhhmmssfff")}.{ResultFilePostFix}";

            string relativeFilePah = currNode.TreeNodeValue.FullPath.Substring(8);
            string relativeFolderPath = relativeFilePah.Substring(0, relativeFilePah.LastIndexOf("\\") + 1);
            string directoryPath = $@"{RootDirectoryPath}{relativeFolderPath}{ResultFolderName}\";

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                resultDirectory = SaveNodeToTree(currNode.MotherNode, ResultFolderName,
                    TreeNodeType.Directory);
            }else
            {
                foreach(TreeNode node in currNode.MotherNode.TreeNodeValue.Nodes)
                {
                    if(node.Text == ResultFolderName)
                    {
                        resultDirectory = (Node)node.Tag;
                        break;
                    }
                }
            }

            string fileNameFullPath = $@"{directoryPath }{fileName}";

            if (!File.Exists(fileNameFullPath))
            {
                FileProcessor.SaveFile(fileNameFullPath, currNode.TreeNodeReplyMessage);
                SaveNodeToTree(resultDirectory, 
                    $"{ currNode.TreeNodeName}.{ DateTime.Now.ToString("yyyyMMddhhmmss")}.{ResultFilePostFix}",
                    TreeNodeType.File);
            }
        }

    }
}
