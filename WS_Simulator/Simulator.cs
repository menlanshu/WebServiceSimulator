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

namespace WS_Simulator
{
    public partial class Simulator : Form, ISearchFormRequester
    {
        private Action TimerStart;
        private Action UpdateCurrLoopText;

        private Action<string> UpdateReplyMessage;
        private Action<string> UpdateAfterReadFile;
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
            string errDesc = "";
            try
            {

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


                if(_testClient.InitialGenerateContext(out errDesc) == false)
                { 
                    ShowErrorMessage(errDesc);
                    return;
                }

                _testClient.NeedSendExtensionName = _wsConfig.MultiNodeExtensionName;
                _testClient.IsDBHelperNeed = _wsConfig.DBHelperNeed;
                _testClient.TimerStart += TimerStartSet;

                WireUpForms();

                (bool okay, string directoryPath) = SimulatorFormHandler.LoadFileTree(this.pathTree, folderContextMenu, fileContextMenu, _wsConfig.FileExtensionName);
                if(okay)
                {
                    _testClient.RootDirectoryPath = directoryPath;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Excepetion happen in " + System.Reflection.MethodBase.GetCurrentMethod().Name + " : " + err.Message);
            }
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
            this.UpdateAfterReadFile += UpdateAfterReadFileMethod;
            this.UpdateCurrLoopText += UpdateCurrentLoopTextMethod;
            this.SelectNodeAndSend += SelectNodeAndSendMethod;
            this.TimerStart += TimerStartSet;
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
                if (SimulatorFormHandler.NeedWait(this.pathTree.SelectedNode.Text, _wsConfig.NeedWaitMessageList))
                {
                    Thread.Sleep(_wsConfig.SleepTime);
                }

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
                        this.rtbRequest.Text = _testClient.AutoChangeContextInfo(this.rtbRequest.Text, UpdateReplyMessage);
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

                return SimulatorFormHandler.LoadTestFile(node, _testClient.RootDirectoryPath, UpdateReplyMessage, UpdateAfterReadFile);

            }), currNode
            );

            return requestMessage;
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

            _testClient.AddTestNode(this.pathTree.SelectedNode);
            _testClient.RequestMessage = this.rtbRequest.Text;

            //await _testClient.SendMessageToE3(this.pathTree.SelectedNode, this.rtbRequest.Text, UpdateReplyMessage, TimerStart);
            await _testClient.RunAllNodesInDirectory(UpdateReplyMessage, UpdateCurrLoopText, SelectNodeAndSend);
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
                SimulatorFormHandler.LoadTestFile(this.pathTree.SelectedNode, _testClient.RootDirectoryPath, UpdateReplyMessage, UpdateAfterReadFile);
            }
        }

        private void myTimer_Tick(object sender, EventArgs e)
        {
            this.rtbReply.Text = "Please wait web service reply.........." + (++waitSecond).ToString();
        }

        #endregion

        private void RichBoxTextToXML(RichTextBox inRichTextBox)
        {
            try
            {
                string tempStr = inRichTextBox.Text;
                if (string.IsNullOrEmpty(tempStr))
                {
                    MessageBox.Show(inRichTextBox.Name + " is empty!");
                    return;
                }
                inRichTextBox.Text = XMLProcessor.FormatXml(XMLProcessor.RestoreXml(tempStr));
                inRichTextBox.SelectionStart = 0;
            }
            catch (Exception ex)
            {
                string msg = "";
                while (ex != null)
                {
                    msg += ex.Message + Environment.NewLine;
                    ex = ex.InnerException;
                }
                inRichTextBox.Text = msg + inRichTextBox.Text;
                inRichTextBox.SelectionStart = 0;
            }
        }

        private void tbnRequestToXML_Click(object sender, EventArgs e)
        {
            RichBoxTextToXML(this.rtbRequest);
        }

        private void btnReplyToXML_Click(object sender, EventArgs e)
        {
            RichBoxTextToXML(this.rtbReply);
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
                RichBoxTextToXML(selectRichTextBox);
            }
        }

        private void toolStripClear_Click(object sender, EventArgs e)
        {
            if (selectRichTextBox != null)
            {
                selectRichTextBox.Clear();
            }
        }

        private void loadFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
                        (bool okay, string directoryPath) = SimulatorFormHandler.LoadFileTree(this.pathTree, folderContextMenu, fileContextMenu, _wsConfig.FileExtensionName, dialog.SelectedPath);
                        if (okay)
                        {
                            _testClient.RootDirectoryPath = directoryPath;
                        }
                    }
                }
            }
        }

        private void saveToFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectRichTextBox != null)
            {
                SaveFileDialog sfd = new SaveFileDialog();

                //set file type
                sfd.Filter = "Test File（*.xml）|*.xml";

                sfd.FilterIndex = 1;
                sfd.RestoreDirectory = true;

                sfd.InitialDirectory = _testClient.RootDirectoryPath;

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string localFilePath = sfd.FileName.ToString();
                    string fileNameExt = localFilePath.Substring(localFilePath.LastIndexOf("\\") + 1);
                    
                    FileProcessor.SaveFile(localFilePath, selectRichTextBox.Text); ;

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

            (bool okay, string directoryPath) = SimulatorFormHandler.LoadFileTree(this.pathTree, folderContextMenu, fileContextMenu, _wsConfig.FileExtensionName, path);
            if (okay)
            {
                _testClient.RootDirectoryPath = directoryPath;
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
                    RichBoxTextToXML(selectRichTextBox);

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
            (bool okay, string directoryPath)  = SimulatorFormHandler.LoadFileTree(this.pathTree, folderContextMenu, fileContextMenu, 
                _wsConfig.FileExtensionName, _testClient.RootDirectoryPath);
            if (okay)
            {
                _testClient.RootDirectoryPath = directoryPath;
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
                _testClient.AddTestNode(this.pathTree.SelectedNode);
                _testClient.RunAllNodesInDirectory(UpdateReplyMessage, UpdateCurrLoopText, SelectNodeAndSend);
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
                if (_testClient.SendStartNode != null)
                {
                    _testClient.SendStartNode.ForeColor = Color.Black;
                }
                if (_testClient.SendEndNode != null)
                {
                    _testClient.SendEndNode.ForeColor = Color.Black;
                }
                _testClient.SendStartNode = fileNode;
                _testClient.SendStartNode.ForeColor = Color.Red;
                _testClient.SendEndNode = null;
            }

        }

        private void SetEndStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode fileNode = this.pathTree.SelectedNode;

            if (fileNode.Nodes.Count == 0)
            {
                _testClient.SendEndNode = fileNode;
                _testClient.SendEndNode.ForeColor = Color.Red;
            }
        }

        private void clearStartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_testClient.SendStartNode != null)
            {
                _testClient.SendStartNode.ForeColor = Color.Black;
            }
            if (_testClient.SendEndNode != null)
            {
                _testClient.SendEndNode.ForeColor = Color.Black;
            }
            _testClient.SendStartNode = null;
            _testClient.SendEndNode = null;
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _testClient.DurationSendFlag = NodeType.END;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            this.rtbReply.Clear();
            this.rtbReply.Text = "Manual Stop Waiting";
            this.btnSend.Enabled = true;
            waitSecond = 0;
            myTimer.Enabled = false;

            _testClient.DurationSendFlag = NodeType.END;
            _testClient.ManualStop = true;
        }

        private void openFileFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode directoryNode = this.pathTree.SelectedNode;

            if(directoryNode != null)
            {
                if (directoryNode.FullPath != "")
                {
                    Process.Start($@"{((DirectoryInfo)directoryNode.Tag).FullName}");
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
    }
}

