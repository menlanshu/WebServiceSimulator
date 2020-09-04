using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using WebServiceStudio;
using System.Diagnostics;
using WS_Simulator.FormHandler;
using WS_Simulator.Models;
using WS_Simulator.DataAccess;
using WS_Simulator.Interface;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics.Eventing.Reader;

namespace WS_Simulator
{
    public partial class Simulator : Form, ISearchFormRequester, ISaveToDBFormRequester, 
        ILoadFromDBFormRequester, ISaveSingleFieToDB, ICopyFolderFormRequester, IReplaceTextFormRequester
    {
        private Action UpdateCurrLoopText;

        private Action<string> UpdateReplyMessage;
        private Func<TreeNode, string> SelectNodeAndSend;

        public static char[] ConfigDelimeter = (";").ToCharArray();
        private int waitSecond = 0;
        private RichTextBox selectRichTextBox;

        SearchForm testSearch;
        string requestSourceStr = "";
        bool requestIsCaseSensitive = false;
        string replySourceStr = "";
        bool replyIsCaseSensitive = false;

        private WSConfig _wsConfig = new WSConfig();
        private TestClient _testClient = new TestClient();

        #region Initial region
        public Simulator()
        {
            InitializeComponent();

            // Initial control form of this form
            InitilizeControl();

            InitializeFormEventAndDelegate();
        }

        private void WireUpForms()
        {
            this.cmbAddress.DataSource = null;
            this.cmbAddress.DataSource = _wsConfig.WSAddressList;
        }

        private void WireUpWSMethodList()
        {
            this.cmbMethodName.DataSource = null;
            this.cmbMethodName.DataSource = _wsConfig.WSMethodList;
        }

        private void Simulator_Load(object sender, EventArgs e)
        {
            try
            {

                WSConfigInitilization();

                TestClientInitialization();

                NodeInitialization();

                InitialImageList();

            }
            catch (Exception err)
            {
                MessageBox.Show("Excepetion happen in " + System.Reflection.MethodBase.GetCurrentMethod().Name + " : " + err.Message);
            }
        }

        private void NodeInitialization()
        {
            FileNode.SaveNodeToTree = SaveNodeToTreeMethod;
            DBNode.SaveNodeToTree = SaveNodeToTreeMethod;

            FileNode.UpdateAfterReadFile = UpdateAfterReadFileMethod;
            DBNode.UpdateAfterReadFile = UpdateAfterReadFileMethod;
        }

        private void WSConfigInitilization()
        {
            string errDesc = "";

            _wsConfig = new WSConfig();
            if (_wsConfig.InitializeWSConfig(out errDesc) == false)
            {
                ShowErrorMessage(errDesc);
                return;
            }

            if (_wsConfig.DBHelperNeed)
            {
                if (!DBProcessor.InitDBHelper(out errDesc))
                {
                    ShowErrorMessage(errDesc);
                    return;
                }
            }
        }

        private void TestClientInitialization()
        {
            string errDesc = "";

            if (_testClient.InitialGenerateContext(out errDesc) == false)
            {
                ShowErrorMessage(errDesc);
                return;
            }

            _testClient.NeedSendExtensionName = _wsConfig.MultiNodeExtensionName;
            _testClient.IsDBHelperNeed = _wsConfig.DBHelperNeed;
            _testClient.NeedWaitmessageList = _wsConfig.NeedWaitMessageList;
            _testClient.NeedWaitTime = _wsConfig.SleepTime;

            _testClient.TimerStart += TimerStartSet;
            _testClient.UpdateAfterReadFile = UpdateAfterReadFileMethod;
            _testClient.UpdateReplyMessage = UpdateRTBReplyMsg;
            _testClient.UpdateNodeColor = UpdateTreeNodeColor;
            _testClient.UpdateSendNodeListStatus = UpdateRequestBoxWaitNodeStatus;
            _testClient.SaveNodeToTree = SaveNodeToTree;

            WireUpForms();

            // Initial a list of Node
            List<Node> initialNodeList = new List<Node>();

            _testClient.CurrentRepository = null;
            (bool okay, string directoryPath) = SimulatorFormHandler.LoadFileTree(ref initialNodeList,
                this.pathTree, folderContextMenu, fileContextMenu, _wsConfig.FileExtensionName);

            if (okay)
            {
                _testClient.RootDirectoryPath = directoryPath;
                _testClient.CurrNodeList = initialNodeList;
            }
            else
            {
                MessageBox.Show(this, "Load Current Directory fail", "Reminder");
            }
        }

        private void InitialImageList()
        {
            ImageList myImageList = new ImageList();
            myImageList.Images.Add(SimulatorFormHandler.FileImageKey, Image.FromFile("file.ico"));
            myImageList.Images.Add(SimulatorFormHandler.FolderImageKey, Image.FromFile("folder.ico"));

            this.pathTree.ImageList = myImageList;
        }

        // Initial Control size and font size
        private void InitilizeControl()
        {
            // Initial name of current form
            this.Text = "Web Service Simulator";

            // Initial location of form
            this.StartPosition = FormStartPosition.CenterScreen;

            // initial font size of receive/reply text box
            this.rtbRequest.Font = new Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbReply.Font = new Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            this.rtbRequest.ContextMenuStrip = this.rtbContextMenu;
            this.rtbReply.ContextMenuStrip = this.rtbContextMenu;

            // My timer defaul setting
            myTimer.Interval = 1000;
            myTimer.Enabled = false;

        }


        // Initial Control event and delegate
        private void InitializeFormEventAndDelegate()
        {
            NullablePrimitiveProperty.GetNodeValue += _testClient.GetNodeValue;

            this.Resize += frmSimulator_Resize;

            this.UpdateReplyMessage += UpdateRTBReplyMsg;
            this.UpdateCurrLoopText += UpdateCurrentLoopTextMethod;
            this.SelectNodeAndSend += SelectNodeAndSendMethod;
        }
        private void UpdateCurrentLoopTextMethod()
        {
            this.BeginInvoke((Action)(() => this.lbCurrentLoop.Text = "CurrentLoop: " + _testClient.CurrentPerfTestCount.ToString()));
        }
        private void TimerStartSet()
        {
            this.Invoke((Action)(() =>
            {
                this.btnSend.Enabled = false;
                waitSecond = 0;

                myTimer.Enabled = true;
                myTimer.Start();
            }));

        }
        private void UpdateRTBReplyMsg(string replyMessage)
        {
            this.BeginInvoke((Action<string>)((replyMsgText) => {
                if (!this.btnSend.Enabled)
                {
                    waitSecond = 0;
                    myTimer.Enabled = false;

                    this.rtbReply.Clear();
                    this.rtbReply.Text = replyMsgText;

                    this.btnSend.Enabled = true;
                }
                else
                {
                    this.rtbReply.Clear();
                    this.rtbReply.Text = replyMsgText;
                }

                this.lbCurrentCount.Text = _testClient.CurrentActualSendNodeCount + "/" + _testClient.CurrentSendNodeCount + "/" + _testClient.WaitSendTreeNode.Count;
                _testClient.CurrentSendNodeCount++;

            }), replyMessage
            );
        }
        private void UpdateAfterReadFileMethod(string requestMessage)
        {
            this.BeginInvoke((Action<string>)(
                (fileInfo) =>
                {
                    this.rtbRequest.Clear();
                    this.rtbRequest.Text = fileInfo;

                    if (cbToDispatcher.Checked == true)
                    {
                        selectRichTextBox = this.rtbRequest;
                        toDispatchToolStripMenuItem_Click(this, null);
                    }

                    if (cbAutoChangeContext.Checked == true)
                    {
                        this.rtbRequest.Text = _testClient.AutoChangeContextInfo(this.rtbRequest.Text);
                    }
                }), requestMessage);
        }
        private void ShowErrorMessage(string errDesc)
        {
            MessageBox.Show(errDesc);
            this.Dispose();
        }
        private string SelectNodeAndSendMethod(TreeNode currNode)
        {
            string requestMessage = (string)this.Invoke((Func<TreeNode, string>)((node) =>
            {
                this.pathTree.SelectedNode = node;

                return ((Node)node.Tag).GetCurrentMessage(true);

                //return SimulatorFormHandler.LoadTestFile((Node)node.Tag, _testClient.RootDirectoryPath, UpdateReplyMessage, UpdateAfterReadFile);

            }), currNode
            );

            return requestMessage;
        }

        private Node SaveNodeToTree(Node motherNode, string nodeName, TreeNodeType treeNodeType)
        {
            Node newNode =  (Node)this.Invoke(
                (Func<Node, string, TreeNodeType, Node>)(
                (inmotherNode, innodeName, intreeNodeType) =>
                {
                    return SaveNodeToTreeMethod(inmotherNode, innodeName, intreeNodeType);
                }
                ), 
                motherNode, nodeName, treeNodeType);

            return newNode;
        }

        private Node SaveNodeToTreeMethod(Node inMotherNode, string inNodeName, TreeNodeType inTreeNodeType)
        {
            return (Node)this.Invoke((Func<Node, string, TreeNodeType, Node>)((motherNode, nodeName, treeNodeType) =>
            {
                TreeNode newTreeNode = null;
                Node newNode = null;
                if (motherNode != null)
                {
                    if (motherNode is DBNode)
                    {
                        Node findMotherNode = _testClient.CurrNodeList.Where(x => x.Id == motherNode.Id).FirstOrDefault();
                        motherNode = findMotherNode == null ? motherNode : findMotherNode;
                    }

                    if (treeNodeType == TreeNodeType.File)
                    {
                        // Add node to current path tree
                        newTreeNode = new TreeNode();
                        //newTreeNode.Text = $@"{ currNode.TreeNodeName}.{DateTime.Now.ToString("yyyyMMddhhmmss")}.result";
                        newTreeNode.Text = nodeName;
                        newTreeNode.ContextMenuStrip = this.fileContextMenu;
                        newTreeNode.ImageIndex = 0; // ImageKey = "file";
                        newTreeNode.SelectedImageIndex = 0;
                        // Add new tree node to current tree
                        motherNode.TreeNodeValue.Nodes.Add(newTreeNode);

                        // Add node to current Node tree
                        if(motherNode is DBNode)
                        {
                            newNode = new DBNode(TreeNodeType.File, newTreeNode, motherNode, newTreeNode.FullPath);
                        }
                        else if(motherNode is FileNode)
                        {
                            newNode = new FileNode(TreeNodeType.File, newTreeNode, motherNode, newTreeNode.FullPath);
                        }

                        newNode.TreeNodeSourceType = motherNode.TreeNodeSourceType;

                        newTreeNode.Tag = newNode;

                        _testClient.CurrNodeList.Add(newNode);
                    }
                    else if (treeNodeType == TreeNodeType.Directory)
                    {
                        // Add node to current path tree
                        newTreeNode = new TreeNode();
                        //newTreeNode.Text = $@"{ currNode.TreeNodeName}.{DateTime.Now.ToString("yyyyMMddhhmmss")}.result";
                        newTreeNode.Text = nodeName;
                        newTreeNode.ContextMenuStrip = this.folderContextMenu;
                        newTreeNode.ImageIndex = 1; // ImageKey = "folder";
                        newTreeNode.SelectedImageIndex = 1;

                        // Add new tree node to current tree
                        motherNode.TreeNodeValue.Nodes.Add(newTreeNode);

                        // Add node to current Node tree
                        if (motherNode is DBNode)
                        {
                            newNode = new DBNode(TreeNodeType.Directory, newTreeNode, motherNode, newTreeNode.FullPath);
                        }else if (motherNode is FileNode)
                        {
                            newNode = new FileNode(TreeNodeType.Directory, newTreeNode, motherNode, newTreeNode.FullPath);
                        }
                        newNode.TreeNodeSourceType = motherNode.TreeNodeSourceType;

                        newTreeNode.Tag = newNode;

                        _testClient.CurrNodeList.Add(newNode);
                    }

                }
                return newNode;
            }), inMotherNode, inNodeName, inTreeNodeType);
        }
        #endregion

        #region event handle region
        // When form resize, auto change the locaion of buttong and address text box
        private void frmSimulator_Resize(object sender, EventArgs e)
        {
            AutoChangeFunctionListButton();
        }

        private void splitContainer3_Panel1_SizeChanged(object sender, EventArgs e)
        {
            AutoChangeFunctionListButton();
        }

        private void AutoChangeFunctionListButton()
        {
            this.cmbAddress.Width = (int)((this.splitContainer3.Panel1.Width - this.lbAddress.Width - this.lbMethod.Width - this.btnSend.Width) * 0.6);
            this.cmbMethodName.Width = (int)((this.splitContainer3.Panel1.Width - this.lbAddress.Width - this.lbMethod.Width - this.btnSend.Width) * 0.3);
            this.lbMethod.Location = new System.Drawing.Point(this.cmbAddress.Location.X + this.cmbAddress.Width + 10, this.lbMethod.Location.Y);
            this.cmbMethodName.Location = new System.Drawing.Point(this.lbMethod.Location.X + this.lbMethod.Width + 10, this.cmbMethodName.Location.Y);
            this.btnSend.Location = new System.Drawing.Point(this.cmbMethodName.Location.X + this.cmbMethodName.Width + 10, this.btnSend.Location.Y);
        }

        private async void cmbAddress_TextChanged(object sender, EventArgs e)
        {
            await WebServiceProcessor.Reset(this.cmbAddress.Text);

            // Initial Node info of whole method in current web service
            await Task.Run(() => WebServiceProcessor.FillInvokeTab());

            WebServiceStudio.Configuration.MasterConfig.InvokeSettings.AddUri(this.cmbAddress.Text);

            // Get web service Method list of current webservice
            await SimulatorFormHandler.InitMethodNameOfWebService(_wsConfig, WebServiceProcessor.TreeMethods.Nodes);

            WireUpWSMethodList();
        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            string methodName = this.cmbMethodName.Text;

            if (string.IsNullOrEmpty(methodName))
            {
                MessageBox.Show("Method Name can not be empty!");
                return;
            }

            if (this.pathTree.SelectedNode != null && ((Node)this.pathTree.SelectedNode.Tag).TreeNodeType == TreeNodeType.File)
            {
                _testClient.AddTestNode(this.pathTree.SelectedNode);
                if (_testClient.WaitSendTreeNode.Count == 1)
                {
                    _testClient.WaitSendTreeNode[0].CurrentNodeSendMessage = this.rtbRequest.Text;
                }

                _testClient.SendType = SendType.SEND;
                await _testClient.RunAllNodesInDirectory(UpdateCurrLoopText, SelectNodeAndSend);
            }
        }

        private void pathTree_MouseDown(object sender, MouseEventArgs e)
        {

            if(_testClient.InTesting())
            {
                return;
            }

            // Set old node to white
            if (this.pathTree.SelectedNode != null)
            {
                this.pathTree.SelectedNode.BackColor = Color.White;
            }

            // Set color of current node
            this.pathTree.SelectedNode = this.pathTree.GetNodeAt(e.X, e.Y);
            if (this.pathTree.SelectedNode != null)
            {
                this.pathTree.SelectedNode.BackColor = Color.LightGreen;
                ((Node)this.pathTree.SelectedNode.Tag).GetCurrentMessage(true);
            }
        }

        private void myTimer_Tick(object sender, EventArgs e)
        {
            this.rtbReply.Text = "Please wait web service reply.........." + (++waitSecond).ToString();
        }

        private void UpdateRequestBoxWaitNodeStatus(List<TestNode> testNodes)
        {
            if (_testClient.SendType == SendType.RUNALL)
            {
                StringBuilder currentStatus = new StringBuilder();

                foreach (var testNode in testNodes)
                {
                    currentStatus.AppendLine($"Test Node: {testNode.NodeInTree.TreeNodeName}");
                    currentStatus.AppendLine($"Status: {testNode.TreeNodeSendStatus}  TotalCostTime: {testNode.TestPeriod}ms");
                    if(testNode.NeedWait)
                    {
                        currentStatus.AppendLine($"Need wait for {testNode.NeedWaitTime}ms after send finish according to configuration.");
                    }
                    currentStatus.AppendLine();
                }

                this.rtbRequest.Clear();
                this.rtbRequest.Text = currentStatus.ToString();
            }

        }

        #endregion

        

        private void tbnRequestToXML_Click(object sender, EventArgs e)
        {
            SimulatorFormHandler.RichBoxTextToXML(this.rtbRequest);
        }

        private void btnReplyToXML_Click(object sender, EventArgs e)
        {
            SimulatorFormHandler.RichBoxTextToXML(this.rtbReply);
        }

        private void rtbRequest_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.toolStripToXML.Visible = true;
                this.toDispatchToolStripMenuItem.Visible = true;
                this.toolStripClear.Visible = true;
                this.saveToFileToolStripMenuItem.Visible = true;
                rtbRequest.ContextMenuStrip.Show();
                selectRichTextBox = rtbRequest;
            }
        }

        private void rtbReply_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.toolStripToXML.Visible = true;
                this.toDispatchToolStripMenuItem.Visible = false;
                this.toolStripClear.Visible = true;
                this.saveToFileToolStripMenuItem.Visible = true;
                this.rtbReply.ContextMenuStrip.Show();
                selectRichTextBox = rtbReply;
            }
        }

        private void toolStripToXML_Click(object sender, EventArgs e)
        {
            if (selectRichTextBox != null)
            {
                SimulatorFormHandler.RichBoxTextToXML(selectRichTextBox);
            }
        }

        private void toolStripClear_Click(object sender, EventArgs e)
        {
            if (selectRichTextBox != null)
            {
                selectRichTextBox.Clear();
            }
        }

        private void saveToFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectRichTextBox != null)
            {
                if (_testClient.CurrentRepository == null)
                {
                    Node selectNode = (Node)(this.pathTree.SelectedNode.Tag);
                    string fileName = selectNode.TreeNodeName;

                    if (selectRichTextBox.Name.ToUpper().Contains("REPLY"))
                    {
                        fileName = fileName.Substring(0, fileName.LastIndexOf(".")) + "_Result.txt";
                    }

                    SaveFileDialog sfd = SimulatorFormHandler.
                        CreateAFileDialog(selectNode, _testClient.RootDirectoryPath, fileName);

                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        string localFilePath = sfd.FileName.ToString();
                        //string fileNameExt = localFilePath.Substring(localFilePath.LastIndexOf("\\") + 1);

                        FileProcessor.SaveFile(localFilePath, selectRichTextBox.Text);
                    }
                }else
                {
                    SaveSingleFileToDB frm = new SaveSingleFileToDB(this, this.pathTree, selectRichTextBox.Text);
                    frm.Show();
                }
            }
        }

        private void pathTree_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All; 
            else
                e.Effect = DragDropEffects.None;
        }

        private void pathTree_DragOver(object sender, DragEventArgs e)
        {
            string path = "";
            try
            {
                path = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            }
            catch (Exception err)
            {
                MessageBox.Show("Excepetion happen in " + System.Reflection.MethodBase.GetCurrentMethod().Name + " : " + err.Message);
            }

            // Initial a list of Node
            List<Node> initialNodeList = new List<Node>();
            _testClient.CurrentRepository = null;
            (bool okay, string directoryPath) = SimulatorFormHandler.LoadFileTree(ref initialNodeList, this.pathTree, folderContextMenu, fileContextMenu, _wsConfig.FileExtensionName, path);
            if (okay)
            {
                _testClient.RootDirectoryPath = directoryPath;
                _testClient.CurrNodeList = initialNodeList;
            }
            else
            {
                MessageBox.Show(this, "Load Current Directory fail", "Reminder");
            }
            
        }

        private void toDispatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int configLocation = 0;
            string errDesc = "";
            string requestMessage;

            if (selectRichTextBox != null && this.pathTree.SelectedNode != null)
            {
                if (XMLProcessor.IsE3EventInfo(this.rtbRequest.Text, out configLocation, out errDesc))
                {
                    SimulatorFormHandler.RichBoxTextToXML(selectRichTextBox);

                    requestMessage = selectRichTextBox.Text;

                    if (XMLProcessor.ChangeToDispatcherMessage(this.pathTree.SelectedNode.Text, ref requestMessage, out errDesc))
                    {
                        selectRichTextBox.Text = requestMessage;
                    }else
                    {
                        UpdateReplyMessage?.Invoke(errDesc);
                    }
                }
                else
                {
                    UpdateReplyMessage?.Invoke(errDesc);
                }
            }
        }

        private void reloadFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Initial a list of Node
            List<Node> initialNodeList = new List<Node>();

            _testClient.CurrentRepository = null;
            (bool okay, string directoryPath)  = SimulatorFormHandler.LoadFileTree(ref initialNodeList, this.pathTree, folderContextMenu, fileContextMenu, 
                _wsConfig.FileExtensionName, _testClient.RootDirectoryPath);
            if (okay)
            {
                _testClient.RootDirectoryPath = directoryPath;
                _testClient.CurrNodeList = initialNodeList;
            }
            else
            {
                MessageBox.Show(this, "Load Current Directory fail", "Reminder");
            }
        }

        #region utility function

        public void ReplaceFormRequest(string searchComponentName, string sourceStr, string destStr)
        {
            string tempMessage = "";

            if (searchComponentName == this.rtbRequest.Name)
            {
                tempMessage = this.rtbRequest.Text;

                tempMessage = tempMessage.Replace(sourceStr, destStr);
                this.rtbRequest.Text = tempMessage;
                MessageBox.Show("Replace Done!");
            }
            else if(searchComponentName == this.rtbReply.Name)
            {
                tempMessage = this.rtbReply.Text;

                tempMessage = tempMessage.Replace(sourceStr, destStr);
                this.rtbReply.Text = tempMessage;
                MessageBox.Show("Replace Done!");
            }
        }

        public void SearchFormRequest(string searchComponentName, string inSrouceStr, bool isDownSearch = true, bool isCaseSensitive = true)
        {
            string tempMessage = "";
            string sourceStr = "";
            RichTextBox tempRTBBox;

            if (searchComponentName == this.rtbRequest.Name)
            {
                tempRTBBox = this.rtbRequest;

                requestSourceStr = inSrouceStr;
                requestIsCaseSensitive = isCaseSensitive;
            }
            else
            {
                tempRTBBox = this.rtbReply;

                replySourceStr = inSrouceStr;
                replyIsCaseSensitive = isCaseSensitive;
            }


            int strLocation;

            if (isCaseSensitive)
            {
                tempMessage = tempRTBBox.Text;
                sourceStr = inSrouceStr;
            }
            else
            {
                tempMessage = tempRTBBox.Text.ToUpper();
                sourceStr = inSrouceStr.ToUpper();
            }

            if (isDownSearch)
            {
                strLocation = tempMessage.IndexOf(sourceStr,
                    tempRTBBox.SelectionStart == 0 ?
                    tempRTBBox.SelectionStart : tempRTBBox.SelectionStart + 1);

                if (strLocation < 0)
                {
                    MessageBox.Show("Can't find " + inSrouceStr + " from current position.");
                    return;
                }

                tempRTBBox.Select(strLocation, sourceStr.Length);
                tempRTBBox.Focus();
            }
            else
            {
                tempMessage = new string(tempMessage.Reverse().ToArray());
                sourceStr = new string(sourceStr.Reverse().ToArray());
                strLocation = tempMessage.IndexOf(sourceStr, tempMessage.Length -
                    (tempRTBBox.SelectionStart == 0 ?
                    tempRTBBox.SelectionStart : tempRTBBox.SelectionStart - 1));

                if (strLocation < 0)
                {
                    MessageBox.Show("Can't find " + inSrouceStr + " from current position.");
                    return;
                }

                tempRTBBox.Select(tempMessage.Length - strLocation - sourceStr.Length, sourceStr.Length);
                tempRTBBox.Focus();
            }
        }

        #endregion

        private void rtbRequest_KeyDown(object sender, KeyEventArgs e)
        {
            rtb_KeyDown(this.rtbRequest, e);
        }

        private void rtb_KeyDown(RichTextBox inRichTextBox, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F && e.Modifiers == Keys.Control)
            {
                if (inRichTextBox.Name == this.rtbRequest.Name)
                {
                    requestSourceStr = inRichTextBox.SelectedText;
                }
                else
                {
                    replySourceStr = inRichTextBox.SelectedText;
                }

                if (testSearch == null || testSearch.IsDisposed)
                {
                    testSearch = new SearchForm(this, inRichTextBox.Name, inRichTextBox.SelectedText);
                    testSearch.Show();
                }
                else
                {
                    testSearch.ShowForm(inRichTextBox.Name, inRichTextBox.SelectedText);
                }
            }
            else if (e.KeyCode == Keys.F3 && e.Modifiers == Keys.Shift)
            {
                if (inRichTextBox.Name == this.rtbRequest.Name)
                {
                    if (!string.IsNullOrEmpty(requestSourceStr))
                    {
                        SearchFormRequest(inRichTextBox.Name, requestSourceStr, false, requestIsCaseSensitive);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(replySourceStr))
                    {
                        SearchFormRequest(inRichTextBox.Name, replySourceStr, false, requestIsCaseSensitive);
                    }
                }
            }
            else if (e.KeyCode == Keys.F3 && e.Modifiers == Keys.None)
            {
                if (inRichTextBox.Name == this.rtbRequest.Name)
                {
                    if (!string.IsNullOrEmpty(requestSourceStr))
                    {
                        SearchFormRequest(inRichTextBox.Name, requestSourceStr, true, requestIsCaseSensitive);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(replySourceStr))
                    {
                        SearchFormRequest(inRichTextBox.Name, replySourceStr, true, requestIsCaseSensitive);
                    }
                }
            }
        }

        private void rtbReply_KeyDown(object sender, KeyEventArgs e)
        {
            rtb_KeyDown(this.rtbReply, e);
        }

        private void runMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                _testClient.CurrentPerfTestCount = 1;
                if (cbPerfTest.Checked == true)
                {
                    if (this.tbMsgCount.Text.Contains(";"))
                    {
                        if (this.tbMsgCount.Text.Split(ConfigDelimeter).Count() == 2)
                        {
                            _testClient.PerfMsgCount = Convert.ToInt32(this.tbMsgCount.Text.Split(ConfigDelimeter)[0]);
                            _testClient.CurrentPerfTestCount = Convert.ToInt32(this.tbMsgCount.Text.Split(ConfigDelimeter)[1]);
                        }
                        else
                        {
                            MessageBox.Show("Only support PerfMsgCount and Start Test Count split by ; in PerfMsgCount TextBox");
                            return;
                        }
                    }
                    else
                    {
                        _testClient.PerfMsgCount = Convert.ToInt32(this.tbMsgCount.Text);
                    }
                }
                else
                {
                    _testClient.PerfMsgCount = 1;
                }

                UpdateCurrLoopText?.Invoke();
                //_testClient.AddTestNode(this.pathTree.SelectedNode,
                //(nodePath, rootPath, updateReplyMessage, updateAfterReadFile) =>
                //SimulatorFormHandler.LoadTestFile(nodePath, rootPath, updateReplyMessage, updateAfterReadFile));

                _testClient.AddTestNode(this.pathTree.SelectedNode);

                _testClient.SendType = SendType.RUNALL;
                _testClient.RunAllNodesInDirectory(UpdateCurrLoopText, SelectNodeAndSend);
            }
            catch (Exception err)
            {
                MessageBox.Show("Excepetion happen in " + System.Reflection.MethodBase.GetCurrentMethod().Name + " : " + err.Message);
            }
        }

        private void SetStartStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode fileNode = this.pathTree.SelectedNode;

            if (fileNode.Nodes.Count == 0)
            {
                _testClient.SetSendStartNode(fileNode);
            }

        }

        private void UpdateTreeNodeColor(TreeNode treeNode, Color toColor)
        {
            if (treeNode != null)
            {
                treeNode.ForeColor = toColor;
            }
        }

        private void SetEndStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode fileNode = this.pathTree.SelectedNode;

            if (fileNode.Nodes.Count == 0)
            {
                _testClient.SetSendEndNode(fileNode);
            }
        }

        private void clearStartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _testClient.ClearDurationNode();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _testClient.ManualStop = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            this.rtbReply.Clear();
            this.rtbReply.Text = "Manual Stop Waiting";
            this.btnSend.Enabled = true;
            waitSecond = 0;
            myTimer.Enabled = false;

            //_testClient.DurationSendFlag = TestNodeType.END;
            _testClient.ManualStop = true;
        }

        private void openFileFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode directoryNode = this.pathTree.SelectedNode;

            if(directoryNode != null)
            {
                if (directoryNode.FullPath != "")
                {
                    //Process.Start($@"{((DirectoryInfo)directoryNode.Tag).FullName}");
                    Process.Start($@"{FileProcessor.GetFullPath(_testClient.RootDirectoryPath, directoryNode.FullPath)}");
                }
            }
        }

        private async void cmbMethodName_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.cmbMethodName.Text))
            {
                string methodName = this.cmbMethodName.Text;
                _testClient.MethodName = methodName;
                _testClient.IsBatch = _wsConfig.BatchMethodName.Contains(methodName);
                await Task.Run(() => WebServiceProcessor.SetInputNode(methodName));
            }

        }

        private void cbPerfTest_CheckedChanged(object sender, EventArgs e)
        {
            _testClient.IsPerfTest = cbPerfTest.Checked;
        }

        private void autoSaveReplyCB_CheckedChanged(object sender, EventArgs e)
        {
            _testClient.AutoSaveReply = autoSaveReplyCB.Checked;
        }

        private void saveCurrTreeToDBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_testClient.CurrentRepository == null)
            {
                SaveToDB frm = new SaveToDB(this);
                frm.Show();
            }else
            {
                MessageBox.Show("Can't save repository from DB to DB!");
            }
        }

        private void loadFromFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _testClient.CurrentRepository = null;
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.SelectedPath = _testClient.RootDirectoryPath;
            dialog.Description = "Please choose the folder for test xml files. ";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    MessageBox.Show(this, "Directory path can not be empty.", "Reminder");
                    return;
                }
                else
                {
                    if (!Directory.Exists(dialog.SelectedPath))
                    {
                        MessageBox.Show(this, "Directory path not exist.", "Reminder");
                        return;
                    }
                    else
                    {
                        // Initial a list of Node
                        List<Node> initialNodeList = new List<Node>();
                        
                        _testClient.CurrentRepository = null;
                        (bool okay, string directoryPath) = SimulatorFormHandler.LoadFileTree(
                            ref initialNodeList, this.pathTree, 
                            folderContextMenu, fileContextMenu, _wsConfig.FileExtensionName, dialog.SelectedPath);
                        if (okay)
                        {
                            _testClient.RootDirectoryPath = directoryPath;
                            _testClient.CurrNodeList = initialNodeList;
                        }
                        else
                        {
                            MessageBox.Show(this, "Load Current Directory fail", "Reminder");
                        }
                    }
                }
            }
        }

        public (bool, string) SaveRepositoryToDB(string name)
        {
            bool result = false;
            string errDesc = "";

            if (!SQLiteDBProcessor.CheckRepositoryName(name))
            {
                TestRepository testRepository = new TestRepository();
                testRepository.RepositoryName = name;


                FileProcessor.LoadMessageForAllNodes(_testClient.CurrNodeList, _testClient.RootDirectoryPath);
                testRepository.TestNodeList = _testClient.CurrNodeList;
                SQLiteDBProcessor.SaveDataToDB(testRepository);

                _testClient.CurrNodeList.ForEach(x => x.TreeNodeMessage = "");

                result = true;
            }else
            {
                errDesc = "Repository Name Already exist, please change it";
            }

            return (result, errDesc);
        }

        private void loadFromDBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadFromDB frm = new LoadFromDB(this);
            frm.Show();
        }

        public void CovertTestRepositoryToTree(TestRepository testRepository)
        {
            _testClient.CurrentRepository = testRepository;
            SimulatorFormHandler.LoadFileTreeFromDB(this.pathTree, folderContextMenu, fileContextMenu, _testClient.CurrentRepository);
            _testClient.CurrNodeList = testRepository.TestNodeList;
        }

        private void deleteFromDBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.pathTree.SelectedNode != null)
            {
                if (_testClient.CurrentRepository == null)
                {
                    Node currNode = (Node)this.pathTree.SelectedNode.Tag;
                    this.pathTree.SelectedNode.Remove();
                    _testClient.CurrNodeList.Remove(currNode);
                }
                else
                {
                    Node currNode = (Node)this.pathTree.SelectedNode.Tag;
                    this.pathTree.SelectedNode.Remove();
                    _testClient.CurrNodeList.Remove(currNode);
                    SQLiteDBProcessor.DeleteOneNode(currNode);
                }
            }
        }

        private void reloadFromFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Node selectNode = null;
            if (this.pathTree.SelectedNode != null)
            {
                selectNode = (Node)this.pathTree.SelectedNode.Tag;
            }

            // Initial a list of Node
            List<Node> initialNodeList = new List<Node>();

            _testClient.CurrentRepository = null;
            (bool okay, string directoryPath) = SimulatorFormHandler.LoadFileTree(ref initialNodeList, this.pathTree, folderContextMenu, fileContextMenu,
                _wsConfig.FileExtensionName, _testClient.RootDirectoryPath);
            if (okay)
            {
                _testClient.RootDirectoryPath = directoryPath;
                _testClient.CurrNodeList = initialNodeList;

                if (selectNode != null)
                {
                    Node getNode = initialNodeList.Where(x => x.NodeFullPath == selectNode.NodeFullPath).FirstOrDefault();

                    this.pathTree.SelectedNode = getNode?.TreeNodeValue;
                    this.pathTree.SelectedNode.Expand();
                }
            }
            else
            {
                MessageBox.Show(this, "Load Current Directory fail", "Reminder");
            }
        }

        public (bool okay, string errDesc) SaveFolderToTreeNode(Node motherNode, Node folderNode, 
            string newFolderName, string oldText = "", string newText = "")
        {
            bool okay = false;
            string errDesc = "";
            string fileName = "";

            if (_testClient.CurrentRepository != null)
            {
                // TODO - Support DB
                errDesc = "This function only support local file now!";
                return (okay, errDesc);
            }

            // Add Folder node
            FileNode node = new FileNode { 
                TreeNodeType = TreeNodeType.Directory,
                TreeNodeName = newFolderName,
                NodeFullPath = $@"{motherNode.NodeFullPath}\{newFolderName}"
            };
            Node dirnode = node.SaveCurrentNodeToMotherNode(motherNode, "");

            if (dirnode != null)
            {
                // Add Each Node of this folder
                foreach (TreeNode currNode in folderNode.TreeNodeValue.Nodes)
                {
                    if (((Node)currNode.Tag).TreeNodeType == TreeNodeType.File)
                    {
                        // Get Folder node name
                        if (string.IsNullOrWhiteSpace(oldText) || string.IsNullOrWhiteSpace(newText))
                        {
                            fileName = ((Node)currNode.Tag).TreeNodeName;
                        }
                        else
                        {
                            fileName = ((Node)currNode.Tag).TreeNodeName.Replace(oldText, newText);
                        }

                        // Add Filte node
                        FileNode fileNode = new FileNode {
                            TreeNodeType = TreeNodeType.File,
                            TreeNodeName = fileName,
                            NodeFullPath = $@"{dirnode.NodeFullPath}\{fileName}"
                        };
                        if (string.IsNullOrWhiteSpace(oldText) || string.IsNullOrWhiteSpace(newText))
                        {
                            fileNode.SaveCurrentNodeToMotherNode(dirnode, ((Node)currNode.Tag).GetCurrentMessage(false));
                        }else
                        {
                            fileNode.SaveCurrentNodeToMotherNode(dirnode, ((Node)currNode.Tag).GetCurrentMessage(false).Replace(oldText, newText));
                        }
                    }
                }

                okay = true;
            }else
            {
                errDesc = $"Add folder to {motherNode.TreeNodeName} fail!";
            }

            return (okay, errDesc);
        }

        private void copyFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(this.pathTree.SelectedNode != null)
            {
                CopyFolderForm frm = new CopyFolderForm(this, this.pathTree, (Node)this.pathTree.SelectedNode.Tag);
                frm.Show();
            }
        }

        public (bool okay, string errDesc) ReplaceTextFileInfo(Node folderNode, string oldText = "", string newText = "")
        {
            bool okay = false;
            string errDesc = "";
            string newMessage = "";

            if (_testClient.CurrentRepository != null)
            {
                // TODO - Support DB
                errDesc = "This function only support local file now!";
                return (okay, errDesc);
            }

            // Add Each Node of this folder
            foreach (TreeNode currNode in folderNode.TreeNodeValue.Nodes)
            {
                if (((Node)currNode.Tag).TreeNodeType == TreeNodeType.File)
                {
                    if (string.IsNullOrWhiteSpace(oldText) || string.IsNullOrWhiteSpace(newText))
                    {
                        // DO nothing
                    }
                    else
                    {
                        newMessage = ((Node)currNode.Tag).GetCurrentMessage(false).Replace(oldText, newText);
                        ((Node)currNode.Tag).UpdateCurrentMessage(newMessage);
                    }
                }
            }

            okay = true;

            return (okay, errDesc);
        }

        private void replaceTextOfAllFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.pathTree.SelectedNode != null)
            {
                ReplaceTextForm frm = new ReplaceTextForm(this, (Node)this.pathTree.SelectedNode.Tag);
                frm.Show();
            }
        }

        private void escapeXmlMessageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectRichTextBox != null)
            {
                SimulatorFormHandler.RichBoxTextEscapeXML(selectRichTextBox);
            }
        }

        private void generateTestCaseFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode directoryNode = this.pathTree.SelectedNode;

            if (directoryNode != null)
            {
                if (directoryNode.FullPath != "")
                {
                    string directoryPath = FileProcessor.GetFullPath(_testClient.RootDirectoryPath, directoryNode.FullPath);
                    TestCaseDocumentHandler.GenerateTestCaseFile(directoryPath);
                }
            }

            reloadFromFolderToolStripMenuItem_Click(sender, e);
        }

        private void generateCaseConfigFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode directoryNode = this.pathTree.SelectedNode;

            if (directoryNode != null)
            {
                if (directoryNode.FullPath != "")
                {
                    string directoryPath = FileProcessor.GetFullPath(_testClient.RootDirectoryPath, directoryNode.FullPath);
                    TestCaseDocumentHandler.GenerateCaseConfigFile(directoryPath);
                }
            }

            reloadFromFolderToolStripMenuItem_Click(sender, e);
        }
    }
}

