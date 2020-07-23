using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Configuration;
using System.Xml;
using System.Threading;
using WebServiceStudio;
using System.Reflection;
using System.Web.Services.Protocols;
using System.Net;
using System.Diagnostics;

namespace SMSC_Simulator
{
    public partial class Simulator : Form
    {
        private string m_DirectoryPath;
        private char[] Delimeter = (",").ToCharArray();
        public static string defaultValue = "NA";
        public static char[] ConfigDelimeter = (";").ToCharArray();
        public static string requestMessage = "";
        private static string replyMessage = "";
        private int waitSecond = 0;

        private RichTextBox selectRichTextBox;
        private List<string> batchMethodName;
        private List<string> fileExtensionName;
        private List<string> multiNodesExtensionName;
        private List<string> needWaitMessageList;
        private int sleepTime;

        public delegate void UpdateReplyMessage();
        public static UpdateReplyMessage myUpdateReplyMessage;

        private Wsdl wsdl = null;
        private TreeView treeMethods;
        private TreeView treeInput;
        private Dictionary<int, TreeNode> waitSendTreeNode = new Dictionary<int, TreeNode>();
        private int currentSendNodeCount;
        private int currentActualSendNodeCount;
        private bool SendTreeNodeList;
        private TreeNode sendStartNode;
        private TreeNode sendEndNode;
        private bool durationCheckNeed;
        private string durationSendFlag;
        private const string durSendStart = "Start";
        private const string durSendEnd = "End";

        DBHelper myDBHelper;
        bool DBHelperNeed;

        public static int sendIndex;
        public static int totalCount;
        public static bool isBatch;

        SearchForm testSearch;
        string requestSourceStr = "";
        bool requestIsCaseSensitive = false;
        string replySourceStr = "";
        bool replyIsCaseSensitive = false;

        System.Collections.Hashtable normalCollection;
        System.Collections.Hashtable batchCollection;
        System.Collections.Hashtable methodMapping;
        System.Collections.Hashtable dispatcherConfig;


        //auto generate context function
        GenerateContext autoGenerateContext;
        List<int> autoContextCountList;
        TreeNode currentLoopDirectoryNode;
        int currentPerfTestCount;
        int perfMsgCount;


        public Simulator()
        {
            InitializeComponent();
            this.Resize += new System.EventHandler(this.frmSimulator_Resize);
            wsdl = new Wsdl();
        }

        private void frmSimulator_Resize(object sender, EventArgs e)
        {
            //this.splitContainer3.Panel1.Size = new System.Drawing.Size(this.splitContainer3.Panel1.Width, 50);
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

        private void InitMethodName()
        {
            this.cmbMethodName.Items.Clear();
            foreach (TreeNode tempNode in this.treeMethods.Nodes)
            {
                AddMethoName(tempNode);
            }
            //foreach (TreeNode tempNode in this.treeMethods.Nodes)
            //{
            //    methodName = tempNode.Text;
            //    foreach (TreeNode tempNode2 in tempNode.Nodes)
            //    {
            //        methodName = tempNode2.Text;
            //if (methodName != "")
            //{
            //    cmbMethodName.Items.Clear();
            //    foreach (string tempStr in methodName.Split(Delimeter))
            //    {
            //        cmbMethodName.Items.Add(tempStr);
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("No method can be loaded!");
            //}
            //    }
            //}
        }

        private void AddMethoName(TreeNode inTreeNode)
        {
            if (inTreeNode.Nodes.Count == 0)
            {
                this.cmbMethodName.Items.Add(inTreeNode.Text);
            }
            else
            {
                foreach (TreeNode tempNode in inTreeNode.Nodes)
                {
                    AddMethoName(tempNode);
                }
            }
        }

        private void LoadFileTree(string directoryPath = ".")
        {
            if (Directory.Exists(directoryPath))
            {
                m_DirectoryPath = directoryPath;
                DirectoryInfo tempDirectory = new DirectoryInfo(directoryPath);
                this.pathTree.Nodes[0].Nodes.Clear();
                this.pathTree.Nodes[0].Text = "RootNode";
                this.pathTree.Nodes[0].ContextMenuStrip = this.folderContextMenu;
                foreach (FileSystemInfo tempInfo in tempDirectory.EnumerateFileSystemInfos())
                {
                    LoadWholeTree(tempInfo, this.pathTree.Nodes[0]);
                }
                this.pathTree.Nodes[0].Expand();
            }
        }

        private void LoadWholeTree(FileSystemInfo tempSystemInfo, TreeNode tempNode)
        {

            try
            {
                if (tempSystemInfo is DirectoryInfo)
                {
                    TreeNode tempDireNode = new TreeNode(tempSystemInfo.Name);
                    tempDireNode.ContextMenuStrip = this.folderContextMenu;
                    tempDireNode.Tag = tempSystemInfo;
                    tempNode.Nodes.Add(tempDireNode);
                    foreach (FileSystemInfo tempInfo in ((DirectoryInfo)tempSystemInfo).EnumerateFileSystemInfos())
                    {
                        LoadWholeTree(tempInfo, tempDireNode);
                    }
                }
                else if (tempSystemInfo is FileInfo)
                {
                    if (this.fileExtensionName.Contains(tempSystemInfo.Extension))
                    {
                        TreeNode tempFileNode = new TreeNode(tempSystemInfo.Name);
                        tempFileNode.ContextMenuStrip = this.fileContextMenu;
                        tempFileNode.Tag = tempSystemInfo;
                        //tempFileNode.ContextMenuStrip = this.pathContextMenu;
                        tempNode.Nodes.Add(tempFileNode);
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Exception happen in LoadWholeTree : " + err.Message);
            }

        }

        private void ClearAllTabs()
        {
            treeMethods.Nodes.Clear();
            TreeNodeProperty.ClearIncludedTypes();
            treeInput.Nodes.Clear();
        }

        private void Simulator_Load(object sender, EventArgs e)
        {
            try
            {
                WSSWebRequestCreate.RegisterPrefixes();
                if (!InitAddressList()) this.Dispose();
                if (!InitBatchMethoName()) this.Dispose();
                if (!InitFileExtensionName()) this.Dispose();
                if (!InitialGenerateContext()) this.Dispose();

                SendTreeNodeList = false;
                if (!InitSendMultiNodesFileExtensionName()) this.Dispose();
                if (!InitNeedWaitMessageList()) this.Dispose();
                if (!InitMyDBHelper()) this.Dispose();

                SetupAssemblyResolver();

                LoadFileTree();

                myUpdateReplyMessage = new UpdateReplyMessage(UpdateRTBReplyMsg);

                myTimer.Interval = 1000;
                myTimer.Enabled = false;

                this.rtbRequest.ContextMenuStrip = this.rtbContextMenu;
                this.rtbReply.ContextMenuStrip = this.rtbContextMenu;
            }
            catch (Exception err)
            {
                MessageBox.Show("Excepetion happen in " + System.Reflection.MethodBase.GetCurrentMethod().Name + " : " + err.Message);
            }
        }

        private bool InitialGenerateContext()
        {
            try
            {
                autoGenerateContext = ConfigurationManager.GetSection("AutoGenerateContext") as GenerateContext;
                InitialGenerateContextCurrentCount();


                return true;
            }
            catch (Exception err)
            {
                MessageBox.Show("Excepetion happen when initial Generate Context Group : " + err.Message);
                return false;
            }
        }

        private void InitialGenerateContextCurrentCount()
        {
            currentPerfTestCount = 1;
            autoContextCountList = new List<int>();
            foreach (ContextGroup currentObject in autoGenerateContext.ContextGroupList)
            {
                currentObject.CurrentCount = 0;
                if (!autoContextCountList.Contains(currentObject.Count))
                {
                    autoContextCountList.Add(currentObject.Count);
                }
            }
            autoContextCountList.Sort();
        }

        private bool InitBatchMethoName()
        {
            try
            {
                if (ConfigurationManager.ConnectionStrings["BatchMethodName"] != null)
                {
                    batchMethodName = new List<string>();
                    string test = ConfigurationManager.ConnectionStrings["BatchMethodName"].ConnectionString;
                    foreach (string tempStr in test.Split(ConfigDelimeter))
                    {
                        this.batchMethodName.Add(tempStr);
                    }
                    return true;
                }
                else
                {
                    MessageBox.Show("Get BatchMethodName from configuration file fail");
                    return false;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Excepetion happen when initial batch method name : " + err.Message);
                return false;
            }
        }

        private bool InitFileExtensionName()
        {
            try
            {
                if (ConfigurationManager.AppSettings["FileExtension"] != null)
                {
                    fileExtensionName = new List<string>();
                    string test = ConfigurationManager.AppSettings["FileExtension"];
                    foreach (string tempStr in test.Split(ConfigDelimeter))
                    {
                        this.fileExtensionName.Add(tempStr);
                    }
                    return true;
                }
                else
                {
                    MessageBox.Show("Get fileExtensionName from configuration file fail");
                    return false;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Excepetion happen when initial InitFileExtensionName : " + err.Message);
                return false;
            }
        }

        private bool InitSendMultiNodesFileExtensionName()
        {
            try
            {
                if (ConfigurationManager.AppSettings["NeedSendExtension"] != null)
                {
                    multiNodesExtensionName = new List<string>();
                    string test = ConfigurationManager.AppSettings["NeedSendExtension"];
                    foreach (string tempStr in test.Split(ConfigDelimeter))
                    {
                        this.multiNodesExtensionName.Add(tempStr);
                    }
                    return true;
                }
                else
                {
                    MessageBox.Show("Get NeedSendExtension from configuration file fail");
                    return false;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Excepetion happen when initial InitSendMultiNodesFileExtensionName : " + err.Message);
                return false;
            }
        }

        private bool InitMyDBHelper()
        {
            try
            {
                if (ConfigurationManager.AppSettings["needDBOpr"] != null)
                {
                    string test = ConfigurationManager.AppSettings["needDBOpr"];
                    DBHelperNeed = false;
                    if (test == "T")
                    {
                        DBHelperNeed = true;
                        myDBHelper = new DBHelper("myconn");
                    }
                    return true;
                }
                else
                {
                    MessageBox.Show("InitMyDBHelper from configuration file fail");
                    return false;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Excepetion happen when initial InitMyDBHelper : " + err.Message);
                return false;
            }
        }


        private bool InitNeedWaitMessageList()
        {
            try
            {
                if (ConfigurationManager.AppSettings["NeedTimerWait"] != null)
                {
                    needWaitMessageList = new List<string>();
                    string test = ConfigurationManager.AppSettings["NeedTimerWait"];
                    foreach (string tempStr in test.Split(ConfigDelimeter))
                    {
                        this.needWaitMessageList.Add(tempStr);
                    }
                    //return true;
                }
                else
                {
                    MessageBox.Show("Get NeedTimerWait from configuration file fail");
                    //return false;
                }

                if (ConfigurationManager.AppSettings["WaitTime"] != null)
                {
                    sleepTime = 0;
                    sleepTime = Convert.ToInt32(ConfigurationManager.AppSettings["WaitTime"]);
                    return true;
                }
                else
                {
                    MessageBox.Show("Get WaitTime from configuration file fail");
                    return false;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Excepetion happen when initial needWaitMessageList/WaitTime : " + err.Message);
                return false;
            }
        }

        private bool InitAddressList()
        {
            try
            {
                if (ConfigurationManager.ConnectionStrings["WebServiceAddr"] != null)
                {
                    this.cmbAddress.Items.Clear();
                    string test = ConfigurationManager.ConnectionStrings["WebServiceAddr"].ConnectionString;
                    foreach (string tempStr in test.Split(ConfigDelimeter))
                    {
                        this.cmbAddress.Items.Add(tempStr);
                    }
                    return true;
                }
                else
                {
                    MessageBox.Show("Get WebServiceAddr from configuration file fail");
                    return false;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Excepetion happen when initial address list : " + err.Message);
                return false;
            }
        }

        public Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            Assembly proxyAssembly = wsdl.ProxyAssembly;
            if ((proxyAssembly != null) && (proxyAssembly.GetName().ToString() == args.Name))
            {
                return proxyAssembly;
            }
            return null;
        }

        private void SetupAssemblyResolver()
        {
            ResolveEventHandler handler = new ResolveEventHandler(OnAssemblyResolve);
            AppDomain.CurrentDomain.AssemblyResolve += handler;
        }

        private void FillInvokeTab()
        {
            Assembly proxyAssembly = wsdl.ProxyAssembly;
            if (proxyAssembly != null)
            {
                treeMethods.Nodes.Clear();
                foreach (System.Type type in proxyAssembly.GetTypes())
                {
                    if (TreeNodeProperty.IsWebService(type))
                    {
                        TreeNode node = treeMethods.Nodes.Add(type.Name);
                        HttpWebClientProtocol proxy = (HttpWebClientProtocol)Activator.CreateInstance(type);
                        ProxyProperty property = new ProxyProperty(proxy);
                        property.RecreateSubtree(null);
                        node.Tag = property.TreeNode;
                        proxy.Credentials = CredentialCache.DefaultCredentials;
                        if (proxy is SoapHttpClientProtocol protocol2)
                        {
                            protocol2.CookieContainer = new CookieContainer();
                            protocol2.AllowAutoRedirect = true;
                        }
                        foreach (MethodInfo info in type.GetMethods())
                        {
                            if (TreeNodeProperty.IsWebMethod(info))
                            {
                                node.Nodes.Add(info.Name).Tag = info;
                            }
                        }
                    }
                }
                TreeNode currentNode = new TreeNode();
                currentNode = FindCurrentNode(this.treeMethods.Nodes, "SMSC_R2R_CMP_CalcRecipeSettings");
                if (currentNode != null && (currentNode.Tag is MethodInfo))
                {
                    MethodInfo tag = currentNode.Tag as MethodInfo;
                }
                //treeMethods.ExpandAll();
            }
        }

        private void HandleDBAction()
        {
            try
            {
                replyMessage = "";
                DataTable tempDataTable = myDBHelper.GetTable(requestMessage);
                if (tempDataTable == null)
                {
                    replyMessage = "DB Action Error";
                }
                else
                {
                    replyMessage = ConvertBetweenDataTableAndXML_AX(tempDataTable);
                }

            }
            catch (Exception err)
            {
                replyMessage += "Excepetion happen in " + System.Reflection.MethodBase.GetCurrentMethod().Name + " : " + err.Message;
            }
            finally
            {
                this.BeginInvoke(myUpdateReplyMessage);
            }
        }

        public string ConvertBetweenDataTableAndXML_AX(DataTable dtNeedCoveret)
        {
            System.IO.TextWriter tw = new System.IO.StringWriter();
            //if TableName is empty, WriteXml() will throw Exception.             
            dtNeedCoveret.TableName = dtNeedCoveret.TableName.Length == 0 ? "Table_AX" : dtNeedCoveret.TableName;
            dtNeedCoveret.WriteXml(tw);
            //dtNeedCoveret.WriteXmlSchema(tw);
            return tw.ToString();
        }


        private void InvokeWebMethod()
        {
            string replyHeader = "";
            try
            {
                MethodProperty currentMethodProperty = GetCurrentMethodProperty();
                if (currentMethodProperty != null)
                {
                    HttpWebClientProtocol proxy = currentMethodProperty.GetProxyProperty().GetProxy();
                    RequestProperties properties = new RequestProperties(proxy);
                    replyMessage = "<Reply>";
                    MethodInfo method = currentMethodProperty.GetMethod();
                    System.Type declaringType = method.DeclaringType;
                    for (int tempInfoCount = 0; tempInfoCount < totalCount; tempInfoCount++)
                    {
                        try
                        {

                            WSSWebRequest.RequestTrace = properties;
                            object[] parameters = currentMethodProperty.ReadChildren() as object[];
                            object result = method.Invoke(proxy, BindingFlags.Public, null, parameters, null);
                            //MethodProperty property2 = new MethodProperty(currentMethodProperty.GetProxyProperty(), method, result, parameters);
                        }
                        catch (Exception err)
                        {
                            replyMessage += err.Message + Environment.NewLine;
                        }
                        finally
                        {
                            WSSWebRequest.RequestTrace = null;
                            //richRequest.Text = properties.requestPayLoad;
                            //replyMessage = properties.responsePayLoad;
                            if (properties != null && !string.IsNullOrEmpty(properties.responsePayLoad))
                            {
                                replyHeader = GetReplyHeader(properties.responsePayLoad);
                                if (replyHeader.Contains("ResponseCode: 200 (OK)"))
                                {
                                    replyMessage += GetReplyBody(properties.responsePayLoad) + Environment.NewLine;
                                }
                            }
                            else
                            {
                                replyMessage += "For some reason, no reponse." + Environment.NewLine;
                            }
                        }
                    }
                    replyMessage += "</Reply>";
                }
                else
                {
                    replyMessage += "currentMethodProperty = null, can not get current Method Property";
                }
            }
            catch (Exception err)
            {
                replyMessage += "Excepetion happen in " + System.Reflection.MethodBase.GetCurrentMethod().Name + " : " + err.Message;
            }
            finally
            {
                this.BeginInvoke(myUpdateReplyMessage);
            }
        }

        private string GetReplyHeader(string inReply)
        {
            string outHeader;
            List<string> striparr = inReply.Split(new string[] { "\n" }, StringSplitOptions.None).ToList();

            foreach (string tempStr in striparr)
            {
                outHeader = tempStr;
                return outHeader;
            }

            return "Get Header Fail!";
        }

        private string GetReplyBody(string inReply)
        {
            string outBody = "";
            bool isBody = false;
            List<string> striparr = inReply.Split(new string[] { "\n" }, StringSplitOptions.None).ToList();

            foreach (string tempStr in striparr)
            {
                if (isBody)
                {
                    outBody += tempStr;
                }
                else
                {
                    if (tempStr == "")
                    {
                        isBody = true;
                    }
                }
            }
            if (outBody != "")
            {
                outBody = outBody.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>","");
                outBody = outBody.Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", "");
                return outBody;
            }

            return "Get Body Fail!";
        }

        public static string GetNodeValue(string nodeName)
        {
            string nodeValue = "";
            string path = "";
            bool getInnerXml = false;

            //path = ConfigurationManager.ConnectionStrings["E3TestRootName"] == null ? "" :
            //        ConfigurationManager.ConnectionStrings["E3TestRootName"].ConnectionString;
            if (requestMessage != "" && nodeName.Split(' ').Length >= 3)
            {
                //XmlDocument testXmlDoc = new XmlDocument();
                //if (SMSC_Simulator.Simulator.requestMessage == "") return "";

                //testXmlDoc.LoadXml(requestMessage);
                //if (testXmlDoc == null)
                //{
                //    return "";
                //}
                //else if (path != "")
                //{
                //    if (testXmlDoc.SelectSingleNode(path) != null)
                //    {
                //        path = path + "/" + nodeName.Split(' ')[1];
                //        getInnerXml = true;
                //    }
                //    else
                //    {
                        if (isBatch)
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
                //}

                if (path != "")
                {
                    nodeValue = "";
                    foreach (string tempPath in path.Split(ConfigDelimeter))
                    {
                        nodeValue += GetVlaueByPath(tempPath, getInnerXml);
                    }
                }
            //}

            return nodeValue;
        }

        private static string GetVlaueByPath(string path, bool getInnerXml = false)
        {
            XmlDocument testXmlDoc = new XmlDocument();
            string pathValue = "";
            if (SMSC_Simulator.Simulator.requestMessage == "") return "";

            testXmlDoc.LoadXml(SMSC_Simulator.Simulator.requestMessage);

            if (testXmlDoc.SelectSingleNode(path) == null)
            {
                pathValue = SMSC_Simulator.Simulator.defaultValue;
            }
            else
            {
                if (testXmlDoc.SelectSingleNode(path) == null)
                {
                    pathValue = $"<defaultValue>{defaultValue}</defaultValue>";
                }
                else
                {
                    if (testXmlDoc.SelectSingleNode(path).InnerText != testXmlDoc.SelectSingleNode(path).InnerXml)
                    {
                        XmlNodeList tempNodeList = testXmlDoc.SelectNodes(path);
                        if (tempNodeList.Count > 1 && !isBatch && sendIndex == 0)
                        {
                            totalCount = tempNodeList.Count;
                            pathValue = tempNodeList[sendIndex] == null?
                                $"<defaultValue>{defaultValue}</defaultValue>": tempNodeList[sendIndex].OuterXml;
                        }
                        else if (tempNodeList.Count == 1)
                        {
                            pathValue = testXmlDoc.SelectSingleNode(path) == null ?
                                $"<defaultValue>{defaultValue}</defaultValue>" : 
                                (getInnerXml? testXmlDoc.SelectSingleNode(path).InnerXml :testXmlDoc.SelectSingleNode(path).OuterXml);
                        }
                        else if (sendIndex < totalCount)
                        {
                            pathValue = tempNodeList[sendIndex] == null ?
                                $"<defaultValue>{defaultValue}</defaultValue>" : tempNodeList[sendIndex].OuterXml;
                        }
                    }
                    else
                    {
                        pathValue = testXmlDoc.SelectSingleNode(path) == null ?
                            defaultValue : testXmlDoc.SelectSingleNode(path).InnerText;
                    }
                }
            }

            return pathValue;
        }

        private MethodProperty GetCurrentMethodProperty()
        {
            if ((treeInput.Nodes == null) || (treeInput.Nodes.Count == 0))
            {
                MessageBox.Show(this, "Select a web method to execute");
                return null;
            }
            TreeNode node = treeInput.Nodes[0];
            if (!(node.Tag is MethodProperty tag))
            {
                MessageBox.Show(this, "Select a method to execute");
                return null;
            }
            return tag;
        }

        private void UpdateRTBReplyMsg()
        {
            if (!this.btnSend.Enabled)
            {
                this.rtbReply.Clear();
                this.rtbReply.Text = replyMessage;

                this.btnSend.Enabled = true;
                waitSecond = 0;
                myTimer.Enabled = false;
            }

            if (SendTreeNodeList)
            {
                this.lbCurrentCount.Text = currentActualSendNodeCount + "/" + currentSendNodeCount + "/" + waitSendTreeNode.Count;
                currentSendNodeCount++;
                if (NeedWait(this.pathTree.SelectedNode.Text))
                {
                    Thread.Sleep(sleepTime);
                }
                SendAllWaitNodes();
            }
        }

        private bool NeedWait(string fileName)
        {
            if (fileName == "")
            {
                return false;
            }
            else {
                foreach (string tempStr in needWaitMessageList)
                {
                    if (fileName.Contains(tempStr))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void pathTree_MouseDown(object sender, MouseEventArgs e)
        {
            if (SendTreeNodeList)
            {
                return;
            }

            if (this.pathTree.SelectedNode != null)
            {
                this.pathTree.SelectedNode.BackColor = Color.White;
            }
            this.pathTree.SelectedNode = this.pathTree.GetNodeAt(e.X, e.Y);
            if (this.pathTree.SelectedNode != null)
            {
                this.pathTree.SelectedNode.BackColor = Color.LightGreen;
            }

            LoadTestFile();
        }

        private void LoadTestFile()
        {
            string filePath = "";
            try
            {
                if (this.pathTree.SelectedNode != null)
                {
                    filePath = m_DirectoryPath + this.pathTree.SelectedNode.FullPath.Substring(8);

                    ReadFileStream(filePath);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Exception happen in LoadTestFile : " + err.Message + Environment.NewLine
                    + "File Path : " + filePath);
            }
        }

        private void ReadFileStream(string filePath)
        {
            FileStream tempFileStream = null;
            StreamReader tempReader = null;
            string tempFileStr = "";

            try
            {

                if (File.Exists(filePath))
                {

                    tempFileStream = new FileStream(filePath, FileMode.Open);
                    tempReader = new StreamReader(tempFileStream);
                    tempFileStr = tempReader.ReadToEnd();

                    this.rtbRequest.Clear();
                    this.rtbRequest.Text = tempFileStr;

                    if (cbToDispatcher.Checked == true)
                    {
                        selectRichTextBox = this.rtbRequest;
                        toDispatchToolStripMenuItem_Click(false, null);
                    }

                    if (cbAutoChangeContext.Checked == true)
                    {
                        selectRichTextBox = this.rtbRequest;
                        AutoChangeContextInfo(selectRichTextBox);
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Exception happen in ReadFileStream : " + err.Message);
            }
            finally
            {
                if (tempFileStream != null) tempFileStream.Close();
                if (tempReader != null) tempReader.Close();
            }


        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            SendTreeNodeList = false;
            SendButtonClick();
        }

        private void SendButtonClick()
        {
            sendIndex = 0;
            totalCount = 1;
            isBatch = batchMethodName.Contains(this.cmbMethodName.Text);

            SendMessageToE3();
        }

        private bool SetInputNode()
        {
            TreeNode currentNode = new TreeNode();
            currentNode = FindCurrentNode(this.treeMethods.Nodes, this.cmbMethodName.Text);
            if (currentNode != null && (currentNode.Tag is MethodInfo))
            {
                MethodInfo tag = currentNode.Tag as MethodInfo;
                treeInput.Nodes.Clear();
                MethodProperty property = new MethodProperty(GetProxyPropertyFromNode(currentNode), tag);
                property.RecreateSubtree(null);
                treeInput.Nodes.Add(property.TreeNode);
                //currentNode.Tag = property.TreeNode;
                return true;
            }
            else
            {
                return false;
            }
        }

        private TreeNode FindCurrentNode(TreeNodeCollection inNodeCollection, string nodeName)
        {
            foreach (TreeNode node in inNodeCollection)
            {
                if (node.Text == nodeName)
                {
                    return node;
                }
                else if (node.Nodes.Count != 0)
                {
                    return FindCurrentNode(node.Nodes, nodeName);
                }
            }

            return null;
        }

        private ProxyProperty GetProxyPropertyFromNode(TreeNode treeNode)
        {
            while (treeNode.Parent != null)
            {
                treeNode = treeNode.Parent;
            }
            if (treeNode.Tag is TreeNode tag)
            {
                return (tag.Tag as ProxyProperty);
            }
            return null;
        }

        private void SendMessageToE3()
        {
            Thread handleObj;
            string tempName = this.pathTree.SelectedNode.Text;
            bool isSqlFile = false;

            try
            {
                requestMessage = RestoreXml(this.rtbRequest.Text);
                this.rtbRequest.Text = requestMessage;

                if (string.IsNullOrEmpty(requestMessage))
                {
                    MessageBox.Show("Request message is empty, can't send message to R2R");
                    return;
                }

                tempName = tempName.Substring(tempName.LastIndexOf("."));
                if (tempName.ToUpper() == ".SQL" && !DBHelperNeed)
                {
                    MessageBox.Show("Do not support SQL according to configuration!");
                    return;
                }
                else if (tempName.ToUpper() == ".SQL" && DBHelperNeed)
                {
                    isSqlFile = true;
                }

                if (!isSqlFile && !cmbMethodName.Items.Contains(cmbMethodName.Text))
                {
                    MessageBox.Show("You key in a illegal method name for test, please check it");
                    return;
                }


                SetInputNode();

                myTimer.Enabled = true;
                this.btnSend.Enabled = false;
                waitSecond = 0;
                myTimer.Start();

                if (isSqlFile)
                {
                    handleObj = new Thread(new ThreadStart(delegate
                    { HandleDBAction(); }));
                    handleObj.Start();
                }
                else
                {
                    handleObj = new Thread(new ThreadStart(delegate
                    { InvokeWebMethod(); }));
                    handleObj.Start();
                }

            }
            catch (Exception err)
            {
                this.rtbReply.Clear();
                this.rtbReply.Text = "Excepetion happen in " + System.Reflection.MethodBase.GetCurrentMethod().Name + " : " + err.Message;
                this.btnSend.Enabled = true;
                waitSecond = 0;
                myTimer.Enabled = false;

                if (SendTreeNodeList)
                {
                    this.lbCurrentCount.Text = currentActualSendNodeCount + "/" + currentSendNodeCount + "/" + waitSendTreeNode.Count;
                    currentSendNodeCount++;
                    SendAllWaitNodes();
                }
            }
        }

        private bool SaveFile(string fileName, string contents)
        {
            if (System.IO.File.Exists(fileName) && (MessageBox.Show(this, "File " + fileName + " already exists. Overwrite?", "Warning", MessageBoxButtons.YesNo) != DialogResult.Yes))
            {
                return false;
            }
            FileStream stream = System.IO.File.OpenWrite(fileName);
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(contents);
            writer.Flush();
            stream.SetLength(stream.Position);
            stream.Close();
            return true;
        }

        private void Handle_SMSC_R2R_CMP_CalcRecipeSettings()
        {
            R2RService.SMSC_R2R_ControllerServiceSoapClient tempClient = new R2RService.SMSC_R2R_ControllerServiceSoapClient("SMSC_R2R_ControllerServiceSoap12");

            R2RService.eventInfo requestSchema = new R2RService.eventInfo();
            XmlDocument testXmlDoc = new XmlDocument();

            try
            {
                testXmlDoc.LoadXml(requestMessage);
                if (testXmlDoc != null)
                {
                    requestSchema.FabName = testXmlDoc.SelectSingleNode("/msg/msgBody/fabName") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/fabName").InnerText;
                    requestSchema.EventName = testXmlDoc.SelectSingleNode("/msg/msgBody/eventName") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/eventName").InnerText;
                    requestSchema.EqpId = testXmlDoc.SelectSingleNode("/msg/msgBody/eqpId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/eqpId").InnerText;
                    requestSchema.Area = testXmlDoc.SelectSingleNode("/msg/msgBody/area") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/area").InnerText;
                    requestSchema.BatchId = testXmlDoc.SelectSingleNode("/msg/msgBody/batchId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/batchId").InnerText;
                    requestSchema.LotCnt = testXmlDoc.SelectSingleNode("/msg/msgBody/lotCnt") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/lotCnt").InnerText;
                    requestSchema.MessageId = testXmlDoc.SelectSingleNode("/msg/msgBody/messageId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/messageId").InnerText;
                    requestSchema.ExContext = testXmlDoc.SelectSingleNode("/msg/msgBody/exContext") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/exContext").InnerText;
                    requestSchema.TxId = DateTime.Now.ToLongDateString();

                    XmlNodeList tempNodeList = testXmlDoc.SelectNodes("/msg/msgBody/lotInfoList/lotInfo");
                    replyMessage = "";
                    if (tempNodeList.Count == 0)
                    {
                        replyMessage = "Can't get lotInfo from message!";
                        return;
                    }

                    foreach (XmlNode tempXmlNode in tempNodeList)
                    {
                        if (tempXmlNode == null)
                        {
                            replyMessage += "Lot Info Is Null" + Environment.NewLine;
                            continue;
                        }
                        else
                        {
                            requestSchema.XmlString += tempXmlNode.OuterXml;
                        }

                        tempClient.SMSC_R2R_CMP_CalcRecipeSettings(requestSchema);
                        /*R2RReplyService.eventInfo tempReplySchema = tempClient.SMSC_R2R_CMP_CalcRecipeSettings(requestSchema);

                        if (tempReplySchema.Result == "0")
                        {
                            replyMessage += RestoreXml(tempReplySchema.XmlString)+ Environment.NewLine + Environment.NewLine;
                        }
                        else
                        {
                            replyMessage += "Result: " + tempReplySchema.Result + Environment.NewLine;
                            replyMessage += "Errdesc: " + tempReplySchema.ErrDesc + Environment.NewLine;
                            replyMessage += "ReplyMsg: " + RestoreXml(tempReplySchema.XmlString)+ Environment.NewLine;
                        }*/

                    }

                }
            }
            catch (Exception err)
            {
                replyMessage = "Excepetion happen in " + System.Reflection.MethodBase.GetCurrentMethod().Name + " : " + err.Message;
            }
            finally
            {
                this.BeginInvoke(myUpdateReplyMessage);
            }
        }

        private void Handle_SMSC_R2R_CMP_Metrology()
        {
            R2RService.SMSC_R2R_ControllerServiceSoapClient tempClient = new R2RService.SMSC_R2R_ControllerServiceSoapClient("SMSC_R2R_ControllerServiceSoap12");

            R2RService.eventInfo1 requestSchema = new R2RService.eventInfo1();
            XmlDocument testXmlDoc = new XmlDocument();

            try
            {
                testXmlDoc.LoadXml(requestMessage);
                if (testXmlDoc != null)
                {
                    requestSchema.FabName = testXmlDoc.SelectSingleNode("/msg/msgBody/fabName") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/fabName").InnerText;
                    requestSchema.EventName = testXmlDoc.SelectSingleNode("/msg/msgBody/eventName") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/eventName").InnerText;
                    requestSchema.EqpId = testXmlDoc.SelectSingleNode("/msg/msgBody/eqpId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/eqpId").InnerText;
                    requestSchema.Area = testXmlDoc.SelectSingleNode("/msg/msgBody/area") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/area").InnerText;
                    requestSchema.BatchId = testXmlDoc.SelectSingleNode("/msg/msgBody/batchId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/batchId").InnerText;
                    requestSchema.LotCnt = testXmlDoc.SelectSingleNode("/msg/msgBody/lotCnt") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/lotCnt").InnerText;
                    requestSchema.MessageId = testXmlDoc.SelectSingleNode("/msg/msgBody/messageId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/messageId").InnerText;
                    requestSchema.ExContext = testXmlDoc.SelectSingleNode("/msg/msgBody/exContext") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/exContext").InnerText;
                    requestSchema.TxId = DateTime.Now.ToLongDateString();

                    XmlNodeList tempNodeList = testXmlDoc.SelectNodes("/msg/msgBody/lotInfoList/lotInfo");
                    replyMessage = "";
                    if (tempNodeList.Count == 0)
                    {
                        replyMessage = "Can't get lotInfo from message!";
                        return;
                    }

                    foreach (XmlNode tempXmlNode in tempNodeList)
                    {
                        if (tempXmlNode == null)
                        {
                            replyMessage += "Lot Info Is Null" + Environment.NewLine;
                            continue;
                        }
                        else
                        {
                            requestSchema.XmlString += tempXmlNode.OuterXml;
                        }

                        tempClient.SMSC_R2R_CMP_Metrology(requestSchema);
                        /*R2RReplyService.eventInfo1 tempReplySchema = tempClient.SMSC_R2R_CMP_Metrology(requestSchema);

                        if (tempReplySchema.Result == "0")
                        {
                            replyMessage += RestoreXml(tempReplySchema.XmlString)+ Environment.NewLine + Environment.NewLine;
                        }
                        else
                        {
                            replyMessage += "Result: " + tempReplySchema.Result + Environment.NewLine;
                            replyMessage += "Errdesc: " + tempReplySchema.ErrDesc + Environment.NewLine;
                            replyMessage += "ReplyMsg: " + RestoreXml(tempReplySchema.XmlString)+ Environment.NewLine;
                        }*/

                    }

                }
            }
            catch (Exception err)
            {
                replyMessage = "Excepetion happen in " + System.Reflection.MethodBase.GetCurrentMethod().Name + " : " + err.Message;
            }
            finally
            {
                this.BeginInvoke(myUpdateReplyMessage);
            }
        }

        private void Handle_SMSC_R2R_CMP_UsedSettings()
        {
            R2RService.SMSC_R2R_ControllerServiceSoapClient tempClient = new R2RService.SMSC_R2R_ControllerServiceSoapClient("SMSC_R2R_ControllerServiceSoap12");

            R2RService.eventInfo2 requestSchema = new R2RService.eventInfo2();
            XmlDocument testXmlDoc = new XmlDocument();

            try
            {
                testXmlDoc.LoadXml(requestMessage);
                if (testXmlDoc != null)
                {
                    requestSchema.FabName = testXmlDoc.SelectSingleNode("/msg/msgBody/fabName") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/fabName").InnerText;
                    requestSchema.EventName = testXmlDoc.SelectSingleNode("/msg/msgBody/eventName") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/eventName").InnerText;
                    requestSchema.EqpId = testXmlDoc.SelectSingleNode("/msg/msgBody/eqpId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/eqpId").InnerText;
                    requestSchema.Area = testXmlDoc.SelectSingleNode("/msg/msgBody/area") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/area").InnerText;
                    requestSchema.BatchId = testXmlDoc.SelectSingleNode("/msg/msgBody/batchId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/batchId").InnerText;
                    requestSchema.LotCnt = testXmlDoc.SelectSingleNode("/msg/msgBody/lotCnt") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/lotCnt").InnerText;
                    requestSchema.MessageId = testXmlDoc.SelectSingleNode("/msg/msgBody/messageId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/messageId").InnerText;
                    requestSchema.ExContext = testXmlDoc.SelectSingleNode("/msg/msgBody/exContext") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/exContext").InnerText;
                    requestSchema.TxId = DateTime.Now.ToLongDateString();

                    XmlNodeList tempNodeList = testXmlDoc.SelectNodes("/msg/msgBody/lotInfoList/lotInfo");
                    replyMessage = "";
                    if (tempNodeList.Count == 0)
                    {
                        replyMessage = "Can't get lotInfo from message!";
                        return;
                    }

                    foreach (XmlNode tempXmlNode in tempNodeList)
                    {
                        if (tempXmlNode == null)
                        {
                            replyMessage += "Lot Info Is Null" + Environment.NewLine;
                            continue;
                        }
                        else
                        {
                            requestSchema.XmlString += tempXmlNode.OuterXml;
                        }

                        tempClient.SMSC_R2R_CMP_UsedSettings(requestSchema);
                        /*R2RReplyService.eventInfo2 tempReplySchema = tempClient.SMSC_R2R_CMP_UsedSettings(requestSchema);

                        if (tempReplySchema.Result == "0")
                        {
                            replyMessage += RestoreXml(tempReplySchema.XmlString)+ Environment.NewLine + Environment.NewLine;
                        }
                        else
                        {
                            replyMessage += "Result: " + tempReplySchema.Result + Environment.NewLine;
                            replyMessage += "Errdesc: " + tempReplySchema.ErrDesc + Environment.NewLine;
                            replyMessage += "ReplyMsg: " + RestoreXml(tempReplySchema.XmlString)+ Environment.NewLine;
                        }*/

                    }

                }
            }
            catch (Exception err)
            {
                replyMessage = "Excepetion happen in " + System.Reflection.MethodBase.GetCurrentMethod().Name + " : " + err.Message;
            }
            finally
            {
                this.BeginInvoke(myUpdateReplyMessage);
            }
        }

        private void Handle_SMSC_R2R_CVD_CalcRecipeSettings()
        {
            R2RService.SMSC_R2R_ControllerServiceSoapClient tempClient = new R2RService.SMSC_R2R_ControllerServiceSoapClient("SMSC_R2R_ControllerServiceSoap12");

            R2RService.eventInfo3 requestSchema = new R2RService.eventInfo3();
            XmlDocument testXmlDoc = new XmlDocument();

            try
            {
                testXmlDoc.LoadXml(requestMessage);
                if (testXmlDoc != null)
                {
                    requestSchema.FabName = testXmlDoc.SelectSingleNode("/msg/msgBody/fabName") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/fabName").InnerText;
                    requestSchema.EventName = testXmlDoc.SelectSingleNode("/msg/msgBody/eventName") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/eventName").InnerText;
                    requestSchema.EqpId = testXmlDoc.SelectSingleNode("/msg/msgBody/eqpId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/eqpId").InnerText;
                    requestSchema.Area = testXmlDoc.SelectSingleNode("/msg/msgBody/area") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/area").InnerText;
                    requestSchema.BatchId = testXmlDoc.SelectSingleNode("/msg/msgBody/batchId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/batchId").InnerText;
                    requestSchema.LotCnt = testXmlDoc.SelectSingleNode("/msg/msgBody/lotCnt") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/lotCnt").InnerText;
                    requestSchema.MessageId = testXmlDoc.SelectSingleNode("/msg/msgBody/messageId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/messageId").InnerText;
                    requestSchema.ExContext = testXmlDoc.SelectSingleNode("/msg/msgBody/exContext") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/exContext").InnerText;
                    requestSchema.TxId = DateTime.Now.ToLongDateString();

                    XmlNodeList tempNodeList = testXmlDoc.SelectNodes("/msg/msgBody/lotInfoList/lotInfo");
                    replyMessage = "";
                    if (tempNodeList.Count == 0)
                    {
                        replyMessage = "Can't get lotInfo from message!";
                        return;
                    }

                    foreach (XmlNode tempXmlNode in tempNodeList)
                    {
                        if (tempXmlNode == null)
                        {
                            replyMessage += "Lot Info Is Null" + Environment.NewLine;
                            continue;
                        }
                        else
                        {
                            requestSchema.XmlString += tempXmlNode.OuterXml;
                        }

                        tempClient.SMSC_R2R_CVD_CalcRecipeSettings(requestSchema);
                        /*R2RReplyService.eventInfo3 tempReplySchema = tempClient.SMSC_R2R_CVD_CalcRecipeSettings(requestSchema);

                        if (tempReplySchema.Result == "0")
                        {
                            replyMessage += RestoreXml(tempReplySchema.XmlString)+ Environment.NewLine + Environment.NewLine;
                        }
                        else
                        {
                            replyMessage += "Result: " + tempReplySchema.Result + Environment.NewLine;
                            replyMessage += "Errdesc: " + tempReplySchema.ErrDesc + Environment.NewLine;
                            replyMessage += "ReplyMsg: " + RestoreXml(tempReplySchema.XmlString)+ Environment.NewLine;
                        }*/

                    }

                }
            }
            catch (Exception err)
            {
                replyMessage = "Excepetion happen in " + System.Reflection.MethodBase.GetCurrentMethod().Name + " : " + err.Message;
            }
            finally
            {
                this.BeginInvoke(myUpdateReplyMessage);
            }
        }

        private void Handle_SMSC_R2R_CVD_Metrology()
        {
            R2RService.SMSC_R2R_ControllerServiceSoapClient tempClient = new R2RService.SMSC_R2R_ControllerServiceSoapClient("SMSC_R2R_ControllerServiceSoap12");

            R2RService.eventInfo4 requestSchema = new R2RService.eventInfo4();
            XmlDocument testXmlDoc = new XmlDocument();

            try
            {
                testXmlDoc.LoadXml(requestMessage);
                if (testXmlDoc != null)
                {
                    requestSchema.FabName = testXmlDoc.SelectSingleNode("/msg/msgBody/fabName") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/fabName").InnerText;
                    requestSchema.EventName = testXmlDoc.SelectSingleNode("/msg/msgBody/eventName") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/eventName").InnerText;
                    requestSchema.EqpId = testXmlDoc.SelectSingleNode("/msg/msgBody/eqpId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/eqpId").InnerText;
                    requestSchema.Area = testXmlDoc.SelectSingleNode("/msg/msgBody/area") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/area").InnerText;
                    requestSchema.BatchId = testXmlDoc.SelectSingleNode("/msg/msgBody/batchId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/batchId").InnerText;
                    requestSchema.LotCnt = testXmlDoc.SelectSingleNode("/msg/msgBody/lotCnt") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/lotCnt").InnerText;
                    requestSchema.MessageId = testXmlDoc.SelectSingleNode("/msg/msgBody/messageId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/messageId").InnerText;
                    requestSchema.ExContext = testXmlDoc.SelectSingleNode("/msg/msgBody/exContext") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/exContext").InnerText;
                    requestSchema.TxId = DateTime.Now.ToLongDateString();

                    XmlNodeList tempNodeList = testXmlDoc.SelectNodes("/msg/msgBody/lotInfoList/lotInfo");
                    replyMessage = "";
                    if (tempNodeList.Count == 0)
                    {
                        replyMessage = "Can't get lotInfo from message!";
                        return;
                    }

                    foreach (XmlNode tempXmlNode in tempNodeList)
                    {
                        if (tempXmlNode == null)
                        {
                            replyMessage += "Lot Info Is Null" + Environment.NewLine;
                            continue;
                        }
                        else
                        {
                            requestSchema.XmlString += tempXmlNode.OuterXml;
                        }

                        tempClient.SMSC_R2R_CVD_Metrology(requestSchema);
                        /*R2RReplyService.eventInfo4 tempReplySchema = tempClient.SMSC_R2R_CVD_Metrology(requestSchema);

                        if (tempReplySchema.Result == "0")
                        {
                            replyMessage += RestoreXml(tempReplySchema.XmlString)+ Environment.NewLine + Environment.NewLine;
                        }
                        else
                        {
                            replyMessage += "Result: " + tempReplySchema.Result + Environment.NewLine;
                            replyMessage += "Errdesc: " + tempReplySchema.ErrDesc + Environment.NewLine;
                            replyMessage += "ReplyMsg: " + RestoreXml(tempReplySchema.XmlString)+ Environment.NewLine;
                        }*/

                    }

                }
            }
            catch (Exception err)
            {
                replyMessage = "Excepetion happen in " + System.Reflection.MethodBase.GetCurrentMethod().Name + " : " + err.Message;
            }
            finally
            {
                this.BeginInvoke(myUpdateReplyMessage);
            }
        }

        private void Handle_SMSC_R2R_CVD_UsedSettings()
        {
            R2RService.SMSC_R2R_ControllerServiceSoapClient tempClient = new R2RService.SMSC_R2R_ControllerServiceSoapClient("SMSC_R2R_ControllerServiceSoap12");

            R2RService.eventInfo5 requestSchema = new R2RService.eventInfo5();
            XmlDocument testXmlDoc = new XmlDocument();

            try
            {
                testXmlDoc.LoadXml(requestMessage);
                if (testXmlDoc != null)
                {
                    requestSchema.FabName = testXmlDoc.SelectSingleNode("/msg/msgBody/fabName") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/fabName").InnerText;
                    requestSchema.EventName = testXmlDoc.SelectSingleNode("/msg/msgBody/eventName") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/eventName").InnerText;
                    requestSchema.EqpId = testXmlDoc.SelectSingleNode("/msg/msgBody/eqpId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/eqpId").InnerText;
                    requestSchema.Area = testXmlDoc.SelectSingleNode("/msg/msgBody/area") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/area").InnerText;
                    requestSchema.BatchId = testXmlDoc.SelectSingleNode("/msg/msgBody/batchId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/batchId").InnerText;
                    requestSchema.LotCnt = testXmlDoc.SelectSingleNode("/msg/msgBody/lotCnt") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/lotCnt").InnerText;
                    requestSchema.MessageId = testXmlDoc.SelectSingleNode("/msg/msgBody/messageId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/messageId").InnerText;
                    requestSchema.ExContext = testXmlDoc.SelectSingleNode("/msg/msgBody/exContext") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/exContext").InnerText;
                    requestSchema.TxId = DateTime.Now.ToLongDateString();

                    XmlNodeList tempNodeList = testXmlDoc.SelectNodes("/msg/msgBody/lotInfoList/lotInfo");
                    replyMessage = "";
                    if (tempNodeList.Count == 0)
                    {
                        replyMessage = "Can't get lotInfo from message!";
                        return;
                    }

                    foreach (XmlNode tempXmlNode in tempNodeList)
                    {
                        if (tempXmlNode == null)
                        {
                            replyMessage += "Lot Info Is Null" + Environment.NewLine;
                            continue;
                        }
                        else
                        {
                            requestSchema.XmlString += tempXmlNode.OuterXml;
                        }

                        tempClient.SMSC_R2R_CVD_UsedSettings(requestSchema);
                        /*R2RReplyService.eventInfo5 tempReplySchema = tempClient.SMSC_R2R_CVD_UsedSettings(requestSchema);

                        if (tempReplySchema.Result == "0")
                        {
                            replyMessage += RestoreXml(tempReplySchema.XmlString)+ Environment.NewLine + Environment.NewLine;
                        }
                        else
                        {
                            replyMessage += "Result: " + tempReplySchema.Result + Environment.NewLine;
                            replyMessage += "Errdesc: " + tempReplySchema.ErrDesc + Environment.NewLine;
                            replyMessage += "ReplyMsg: " + RestoreXml(tempReplySchema.XmlString)+ Environment.NewLine;
                        }*/

                    }

                }
            }
            catch (Exception err)
            {
                replyMessage = "Excepetion happen in " + System.Reflection.MethodBase.GetCurrentMethod().Name + " : " + err.Message;
            }
            finally
            {
                this.BeginInvoke(myUpdateReplyMessage);
            }
        }

        private void Handle_SMSC_R2R_CommonFunction()
        {
            R2RService.SMSC_R2R_ControllerServiceSoapClient tempClient = new R2RService.SMSC_R2R_ControllerServiceSoapClient("SMSC_R2R_ControllerServiceSoap12");

            R2RService.eventInfo6 requestSchema = new R2RService.eventInfo6();
            XmlDocument testXmlDoc = new XmlDocument();

            try
            {
                testXmlDoc.LoadXml(requestMessage);
                if (testXmlDoc != null)
                {
                    requestSchema.FabName = testXmlDoc.SelectSingleNode("/msg/msgBody/fabName") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/fabName").InnerText;
                    requestSchema.EventName = testXmlDoc.SelectSingleNode("/msg/msgBody/eventName") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/eventName").InnerText;
                    requestSchema.EqpId = testXmlDoc.SelectSingleNode("/msg/msgBody/eqpId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/eqpId").InnerText;
                    requestSchema.Area = testXmlDoc.SelectSingleNode("/msg/msgBody/area") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/area").InnerText;
                    requestSchema.BatchId = testXmlDoc.SelectSingleNode("/msg/msgBody/batchId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/batchId").InnerText;
                    requestSchema.LotCnt = testXmlDoc.SelectSingleNode("/msg/msgBody/lotCnt") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/lotCnt").InnerText;
                    requestSchema.MessageId = testXmlDoc.SelectSingleNode("/msg/msgBody/messageId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/messageId").InnerText;
                    requestSchema.ExContext = testXmlDoc.SelectSingleNode("/msg/msgBody/exContext") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/exContext").InnerText;
                    requestSchema.TxId = DateTime.Now.ToLongDateString();

                    XmlNodeList tempNodeList = testXmlDoc.SelectNodes("/msg/msgBody/lotInfoList/lotInfo");
                    replyMessage = "";
                    if (tempNodeList.Count == 0)
                    {
                        replyMessage = "Can't get lotInfo from message!";
                        return;
                    }

                    foreach (XmlNode tempXmlNode in tempNodeList)
                    {
                        if (tempXmlNode == null)
                        {
                            replyMessage += "Lot Info Is Null" + Environment.NewLine;
                            continue;
                        }
                        else
                        {
                            requestSchema.XmlString += tempXmlNode.OuterXml;
                        }

                        tempClient.SMSC_R2R_CommonFunction(requestSchema);
                        /*R2RReplyService.eventInfo6 tempReplySchema = tempClient.SMSC_R2R_CommonFunction(requestSchema);

                        if (tempReplySchema.Result == "0")
                        {
                            replyMessage += RestoreXml(tempReplySchema.XmlString)+ Environment.NewLine + Environment.NewLine;
                        }
                        else
                        {
                            replyMessage += "Result: " + tempReplySchema.Result + Environment.NewLine;
                            replyMessage += "Errdesc: " + tempReplySchema.ErrDesc + Environment.NewLine;
                            replyMessage += "ReplyMsg: " + RestoreXml(tempReplySchema.XmlString)+ Environment.NewLine;
                        }*/

                    }

                }
            }
            catch (Exception err)
            {
                replyMessage = "Excepetion happen in " + System.Reflection.MethodBase.GetCurrentMethod().Name + " : " + err.Message;
            }
            finally
            {
                this.BeginInvoke(myUpdateReplyMessage);
            }
        }

        private void Handle_SMSC_R2R_Etch_CalcRecipeSettings()
        {
            R2RService.SMSC_R2R_ControllerServiceSoapClient tempClient = new R2RService.SMSC_R2R_ControllerServiceSoapClient("SMSC_R2R_ControllerServiceSoap12");

            R2RService.eventInfo7 requestSchema = new R2RService.eventInfo7();
            XmlDocument testXmlDoc = new XmlDocument();

            try
            {
                testXmlDoc.LoadXml(requestMessage);
                if (testXmlDoc != null)
                {
                    requestSchema.FabName = testXmlDoc.SelectSingleNode("/msg/msgBody/fabName") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/fabName").InnerText;
                    requestSchema.EventName = testXmlDoc.SelectSingleNode("/msg/msgBody/eventName") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/eventName").InnerText;
                    requestSchema.EqpId = testXmlDoc.SelectSingleNode("/msg/msgBody/eqpId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/eqpId").InnerText;
                    requestSchema.Area = testXmlDoc.SelectSingleNode("/msg/msgBody/area") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/area").InnerText;
                    requestSchema.BatchId = testXmlDoc.SelectSingleNode("/msg/msgBody/batchId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/batchId").InnerText;
                    requestSchema.LotCnt = testXmlDoc.SelectSingleNode("/msg/msgBody/lotCnt") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/lotCnt").InnerText;
                    requestSchema.MessageId = testXmlDoc.SelectSingleNode("/msg/msgBody/messageId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/messageId").InnerText;
                    requestSchema.ExContext = testXmlDoc.SelectSingleNode("/msg/msgBody/exContext") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/exContext").InnerText;
                    requestSchema.TxId = DateTime.Now.ToLongDateString();

                    XmlNodeList tempNodeList = testXmlDoc.SelectNodes("/msg/msgBody/lotInfoList/lotInfo");
                    replyMessage = "";
                    if (tempNodeList.Count == 0)
                    {
                        replyMessage = "Can't get lotInfo from message!";
                        return;
                    }

                    foreach (XmlNode tempXmlNode in tempNodeList)
                    {
                        if (tempXmlNode == null)
                        {
                            replyMessage += "Lot Info Is Null" + Environment.NewLine;
                            continue;
                        }
                        else
                        {
                            requestSchema.XmlString += tempXmlNode.OuterXml;
                        }

                        tempClient.SMSC_R2R_Etch_CalcRecipeSettings(requestSchema);
                        /*R2RReplyService.eventInfo7 tempReplySchema = tempClient.SMSC_R2R_Etch_CalcRecipeSettings(requestSchema);

                        if (tempReplySchema.Result == "0")
                        {
                            replyMessage += RestoreXml(tempReplySchema.XmlString)+ Environment.NewLine + Environment.NewLine;
                        }
                        else
                        {
                            replyMessage += "Result: " + tempReplySchema.Result + Environment.NewLine;
                            replyMessage += "Errdesc: " + tempReplySchema.ErrDesc + Environment.NewLine;
                            replyMessage += "ReplyMsg: " + RestoreXml(tempReplySchema.XmlString)+ Environment.NewLine;
                        }*/

                    }

                }
            }
            catch (Exception err)
            {
                replyMessage = "Excepetion happen in " + System.Reflection.MethodBase.GetCurrentMethod().Name + " : " + err.Message;
            }
            finally
            {
                this.BeginInvoke(myUpdateReplyMessage);
            }
        }

        private void Handle_SMSC_R2R_Etch_Metrology()
        {
            R2RService.SMSC_R2R_ControllerServiceSoapClient tempClient = new R2RService.SMSC_R2R_ControllerServiceSoapClient("SMSC_R2R_ControllerServiceSoap12");

            R2RService.eventInfo8 requestSchema = new R2RService.eventInfo8();
            XmlDocument testXmlDoc = new XmlDocument();

            try
            {
                testXmlDoc.LoadXml(requestMessage);
                if (testXmlDoc != null)
                {
                    requestSchema.FabName = testXmlDoc.SelectSingleNode("/msg/msgBody/fabName") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/fabName").InnerText;
                    requestSchema.EventName = testXmlDoc.SelectSingleNode("/msg/msgBody/eventName") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/eventName").InnerText;
                    requestSchema.EqpId = testXmlDoc.SelectSingleNode("/msg/msgBody/eqpId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/eqpId").InnerText;
                    requestSchema.Area = testXmlDoc.SelectSingleNode("/msg/msgBody/area") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/area").InnerText;
                    requestSchema.BatchId = testXmlDoc.SelectSingleNode("/msg/msgBody/batchId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/batchId").InnerText;
                    requestSchema.LotCnt = testXmlDoc.SelectSingleNode("/msg/msgBody/lotCnt") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/lotCnt").InnerText;
                    requestSchema.MessageId = testXmlDoc.SelectSingleNode("/msg/msgBody/messageId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/messageId").InnerText;
                    requestSchema.ExContext = testXmlDoc.SelectSingleNode("/msg/msgBody/exContext") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/exContext").InnerText;
                    requestSchema.TxId = DateTime.Now.ToLongDateString();

                    XmlNodeList tempNodeList = testXmlDoc.SelectNodes("/msg/msgBody/lotInfoList/lotInfo");
                    replyMessage = "";
                    if (tempNodeList.Count == 0)
                    {
                        replyMessage = "Can't get lotInfo from message!";
                        return;
                    }

                    foreach (XmlNode tempXmlNode in tempNodeList)
                    {
                        if (tempXmlNode == null)
                        {
                            replyMessage += "Lot Info Is Null" + Environment.NewLine;
                            continue;
                        }
                        else
                        {
                            requestSchema.XmlString += tempXmlNode.OuterXml;
                        }

                        tempClient.SMSC_R2R_Etch_Metrology(requestSchema);
                        /*R2RReplyService.eventInfo8 tempReplySchema = tempClient.SMSC_R2R_Etch_Metrology(requestSchema);

                        if (tempReplySchema.Result == "0")
                        {
                            replyMessage += RestoreXml(tempReplySchema.XmlString)+ Environment.NewLine + Environment.NewLine;
                        }
                        else
                        {
                            replyMessage += "Result: " + tempReplySchema.Result + Environment.NewLine;
                            replyMessage += "Errdesc: " + tempReplySchema.ErrDesc + Environment.NewLine;
                            replyMessage += "ReplyMsg: " + RestoreXml(tempReplySchema.XmlString)+ Environment.NewLine;
                        }*/

                    }

                }
            }
            catch (Exception err)
            {
                replyMessage = "Excepetion happen in " + System.Reflection.MethodBase.GetCurrentMethod().Name + " : " + err.Message;
            }
            finally
            {
                this.BeginInvoke(myUpdateReplyMessage);
            }
        }

        private void Handle_SMSC_R2R_Etch_UsedSettings()
        {
            R2RService.SMSC_R2R_ControllerServiceSoapClient tempClient = new R2RService.SMSC_R2R_ControllerServiceSoapClient("SMSC_R2R_ControllerServiceSoap12");

            R2RService.eventInfo9 requestSchema = new R2RService.eventInfo9();
            XmlDocument testXmlDoc = new XmlDocument();

            try
            {
                testXmlDoc.LoadXml(requestMessage);
                if (testXmlDoc != null)
                {
                    requestSchema.FabName = testXmlDoc.SelectSingleNode("/msg/msgBody/fabName") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/fabName").InnerText;
                    requestSchema.EventName = testXmlDoc.SelectSingleNode("/msg/msgBody/eventName") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/eventName").InnerText;
                    requestSchema.EqpId = testXmlDoc.SelectSingleNode("/msg/msgBody/eqpId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/eqpId").InnerText;
                    requestSchema.Area = testXmlDoc.SelectSingleNode("/msg/msgBody/area") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/area").InnerText;
                    requestSchema.BatchId = testXmlDoc.SelectSingleNode("/msg/msgBody/batchId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/batchId").InnerText;
                    requestSchema.LotCnt = testXmlDoc.SelectSingleNode("/msg/msgBody/lotCnt") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/lotCnt").InnerText;
                    requestSchema.MessageId = testXmlDoc.SelectSingleNode("/msg/msgBody/messageId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/messageId").InnerText;
                    requestSchema.ExContext = testXmlDoc.SelectSingleNode("/msg/msgBody/exContext") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/exContext").InnerText;
                    requestSchema.TxId = DateTime.Now.ToLongDateString();

                    XmlNodeList tempNodeList = testXmlDoc.SelectNodes("/msg/msgBody/lotInfoList/lotInfo");
                    replyMessage = "";
                    if (tempNodeList.Count == 0)
                    {
                        replyMessage = "Can't get lotInfo from message!";
                        return;
                    }

                    foreach (XmlNode tempXmlNode in tempNodeList)
                    {
                        if (tempXmlNode == null)
                        {
                            replyMessage += "Lot Info Is Null" + Environment.NewLine;
                            continue;
                        }
                        else
                        {
                            requestSchema.XmlString += tempXmlNode.OuterXml;
                        }

                        tempClient.SMSC_R2R_Etch_UsedSettings(requestSchema);
                        /*R2RReplyService.eventInfo9 tempReplySchema = tempClient.SMSC_R2R_Etch_UsedSettings(requestSchema);

                        if (tempReplySchema.Result == "0")
                        {
                            replyMessage += RestoreXml(tempReplySchema.XmlString)+ Environment.NewLine + Environment.NewLine;
                        }
                        else
                        {
                            replyMessage += "Result: " + tempReplySchema.Result + Environment.NewLine;
                            replyMessage += "Errdesc: " + tempReplySchema.ErrDesc + Environment.NewLine;
                            replyMessage += "ReplyMsg: " + RestoreXml(tempReplySchema.XmlString)+ Environment.NewLine;
                        }*/

                    }

                }
            }
            catch (Exception err)
            {
                replyMessage = "Excepetion happen in " + System.Reflection.MethodBase.GetCurrentMethod().Name + " : " + err.Message;
            }
            finally
            {
                this.BeginInvoke(myUpdateReplyMessage);
            }
        }

        private void Handle_SMSC_R2R_Furnace_CalcRecipeSettings()
        {
            R2RService.SMSC_R2R_ControllerServiceSoapClient tempClient = new R2RService.SMSC_R2R_ControllerServiceSoapClient("SMSC_R2R_ControllerServiceSoap12");

            R2RService.eventInfo10 requestSchema = new R2RService.eventInfo10();
            XmlDocument testXmlDoc = new XmlDocument();

            try
            {
                testXmlDoc.LoadXml(requestMessage);
                if (testXmlDoc != null)
                {
                    requestSchema.FabName = testXmlDoc.SelectSingleNode("/msg/msgBody/fabName") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/fabName").InnerText;
                    requestSchema.EventName = testXmlDoc.SelectSingleNode("/msg/msgBody/eventName") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/eventName").InnerText;
                    requestSchema.EqpId = testXmlDoc.SelectSingleNode("/msg/msgBody/eqpId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/eqpId").InnerText;
                    requestSchema.Area = testXmlDoc.SelectSingleNode("/msg/msgBody/area") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/area").InnerText;
                    requestSchema.BatchId = testXmlDoc.SelectSingleNode("/msg/msgBody/batchId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/batchId").InnerText;
                    requestSchema.LotCnt = testXmlDoc.SelectSingleNode("/msg/msgBody/lotCnt") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/lotCnt").InnerText;
                    requestSchema.MessageId = testXmlDoc.SelectSingleNode("/msg/msgBody/messageId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/messageId").InnerText;
                    requestSchema.ExContext = testXmlDoc.SelectSingleNode("/msg/msgBody/exContext") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/exContext").InnerText;
                    requestSchema.TxId = DateTime.Now.ToLongDateString();

                    replyMessage = "";
                    if (testXmlDoc.SelectSingleNode("/msg/msgBody/lotInfoList") == null ||
                       testXmlDoc.SelectSingleNode("/msg/msgBody/batchParaInfo") == null)
                    {
                        replyMessage = "Can't get lotInfoList or batchParaInfo!";
                        return;
                    }
                    else
                    {
                        requestSchema.XmlString = testXmlDoc.SelectSingleNode("/msg/msgBody/lotInfoList").OuterXml +
                            testXmlDoc.SelectSingleNode("/msg/msgBody/batchParaInfo").OuterXml;
                    }

                    tempClient.SMSC_R2R_Furnace_CalcRecipeSettings(requestSchema);
                    /*R2RReplyService.eventInfo10 tempReplySchema = tempClient.SMSC_R2R_Furnace_CalcRecipeSettings(requestSchema);

                    if (tempReplySchema.Result == "0")
                    {
                        replyMessage = RestoreXml(tempReplySchema.XmlString);
                    }
                    else
                    {
                        replyMessage += "Result: " + tempReplySchema.Result + Environment.NewLine;
                        replyMessage += "Errdesc: " + tempReplySchema.ErrDesc + Environment.NewLine;
                        replyMessage += "ReplyMsg: " + RestoreXml(tempReplySchema.XmlString)+ Environment.NewLine;
                    }*/

                }
            }
            catch (Exception err)
            {
                replyMessage = "Excepetion happen in " + System.Reflection.MethodBase.GetCurrentMethod().Name + " : " + err.Message;
            }
            finally
            {
                this.BeginInvoke(myUpdateReplyMessage);
            }
        }

        private void Handle_SMSC_R2R_Furnace_Metrology()
        {
            R2RService.SMSC_R2R_ControllerServiceSoapClient tempClient = new R2RService.SMSC_R2R_ControllerServiceSoapClient("SMSC_R2R_ControllerServiceSoap12");

            R2RService.eventInfo11 requestSchema = new R2RService.eventInfo11();
            XmlDocument testXmlDoc = new XmlDocument();

            try
            {
                testXmlDoc.LoadXml(requestMessage);
                if (testXmlDoc != null)
                {
                    requestSchema.FabName = testXmlDoc.SelectSingleNode("/msg/msgBody/fabName") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/fabName").InnerText;
                    requestSchema.EventName = testXmlDoc.SelectSingleNode("/msg/msgBody/eventName") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/eventName").InnerText;
                    requestSchema.EqpId = testXmlDoc.SelectSingleNode("/msg/msgBody/eqpId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/eqpId").InnerText;
                    requestSchema.Area = testXmlDoc.SelectSingleNode("/msg/msgBody/area") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/area").InnerText;
                    requestSchema.BatchId = testXmlDoc.SelectSingleNode("/msg/msgBody/batchId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/batchId").InnerText;
                    requestSchema.LotCnt = testXmlDoc.SelectSingleNode("/msg/msgBody/lotCnt") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/lotCnt").InnerText;
                    requestSchema.MessageId = testXmlDoc.SelectSingleNode("/msg/msgBody/messageId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/messageId").InnerText;
                    requestSchema.ExContext = testXmlDoc.SelectSingleNode("/msg/msgBody/exContext") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/exContext").InnerText;
                    requestSchema.TxId = DateTime.Now.ToLongDateString();

                    XmlNodeList tempNodeList = testXmlDoc.SelectNodes("/msg/msgBody/lotInfoList/lotInfo");
                    replyMessage = "";
                    if (tempNodeList.Count == 0)
                    {
                        replyMessage = "Can't get lotInfo from message!";
                        return;
                    }

                    foreach (XmlNode tempXmlNode in tempNodeList)
                    {
                        if (tempXmlNode == null)
                        {
                            replyMessage += "Lot Info Is Null" + Environment.NewLine;
                            continue;
                        }
                        else
                        {
                            requestSchema.XmlString += tempXmlNode.OuterXml;
                        }

                        tempClient.SMSC_R2R_Furnace_Metrology(requestSchema);
                        /*R2RReplyService.eventInfo11 tempReplySchema = tempClient.SMSC_R2R_Furnace_Metrology(requestSchema);

                        if (tempReplySchema.Result == "0")
                        {
                            replyMessage += RestoreXml(tempReplySchema.XmlString) + Environment.NewLine + Environment.NewLine;
                        }
                        else
                        {
                            replyMessage += "Result: " + tempReplySchema.Result + Environment.NewLine;
                            replyMessage += "Errdesc: " + tempReplySchema.ErrDesc + Environment.NewLine;
                            replyMessage += "ReplyMsg: " + RestoreXml(tempReplySchema.XmlString) + Environment.NewLine;
                        }*/

                    }

                }
            }
            catch (Exception err)
            {
                replyMessage = "Excepetion happen in " + System.Reflection.MethodBase.GetCurrentMethod().Name + " : " + err.Message;
            }
            finally
            {
                this.BeginInvoke(myUpdateReplyMessage);
            }
        }

        private void Handle_SMSC_R2R_Furnace_UsedSettings()
        {
            R2RService.SMSC_R2R_ControllerServiceSoapClient tempClient = new R2RService.SMSC_R2R_ControllerServiceSoapClient("SMSC_R2R_ControllerServiceSoap12");

            R2RService.eventInfo12 requestSchema = new R2RService.eventInfo12();
            XmlDocument testXmlDoc = new XmlDocument();

            try
            {
                testXmlDoc.LoadXml(requestMessage);
                if (testXmlDoc != null)
                {
                    requestSchema.FabName = testXmlDoc.SelectSingleNode("/msg/msgBody/fabName") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/fabName").InnerText;
                    requestSchema.EventName = testXmlDoc.SelectSingleNode("/msg/msgBody/eventName") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/eventName").InnerText;
                    requestSchema.EqpId = testXmlDoc.SelectSingleNode("/msg/msgBody/eqpId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/eqpId").InnerText;
                    requestSchema.Area = testXmlDoc.SelectSingleNode("/msg/msgBody/area") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/area").InnerText;
                    requestSchema.BatchId = testXmlDoc.SelectSingleNode("/msg/msgBody/batchId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/batchId").InnerText;
                    requestSchema.LotCnt = testXmlDoc.SelectSingleNode("/msg/msgBody/lotCnt") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/lotCnt").InnerText;
                    requestSchema.MessageId = testXmlDoc.SelectSingleNode("/msg/msgBody/messageId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/messageId").InnerText;
                    requestSchema.ExContext = testXmlDoc.SelectSingleNode("/msg/msgBody/exContext") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/exContext").InnerText;
                    requestSchema.TxId = DateTime.Now.ToLongDateString();

                    replyMessage = "";
                    if (testXmlDoc.SelectSingleNode("/msg/msgBody/lotInfoList") == null ||
                       testXmlDoc.SelectSingleNode("/msg/msgBody/batchParaInfo") == null)
                    {
                        replyMessage = "Can't get lotInfoList or batchParaInfo!";
                        return;
                    }
                    else
                    {
                        requestSchema.XmlString = testXmlDoc.SelectSingleNode("/msg/msgBody/lotInfoList").OuterXml +
                            testXmlDoc.SelectSingleNode("/msg/msgBody/batchParaInfo").OuterXml;
                    }

                    tempClient.SMSC_R2R_Furnace_UsedSettings(requestSchema);
                    /*R2RReplyService.eventInfo12 tempReplySchema = tempClient.SMSC_R2R_Furnace_UsedSettings(requestSchema);

                    if (tempReplySchema.Result == "0")
                    {
                        replyMessage = RestoreXml(tempReplySchema.XmlString);
                    }
                    else
                    {
                        replyMessage += "Result: " + tempReplySchema.Result + Environment.NewLine;
                        replyMessage += "Errdesc: " + tempReplySchema.ErrDesc + Environment.NewLine;
                        replyMessage += "ReplyMsg: " + RestoreXml(tempReplySchema.XmlString)+ Environment.NewLine;
                    }*/

                }
            }
            catch (Exception err)
            {
                replyMessage = "Excepetion happen in " + System.Reflection.MethodBase.GetCurrentMethod().Name + " : " + err.Message;
            }
            finally
            {
                this.BeginInvoke(myUpdateReplyMessage);
            }
        }

        private void Handle_SMSC_R2R_Litho_CalcRecipeSettings()
        {
            R2RService.SMSC_R2R_ControllerServiceSoapClient tempClient = new R2RService.SMSC_R2R_ControllerServiceSoapClient("SMSC_R2R_ControllerServiceSoap12");

            R2RService.eventInfo13 requestSchema = new R2RService.eventInfo13();
            XmlDocument testXmlDoc = new XmlDocument();

            try
            {
                testXmlDoc.LoadXml(requestMessage);
                if (testXmlDoc != null)
                {
                    requestSchema.FabName = testXmlDoc.SelectSingleNode("/msg/msgBody/fabName") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/fabName").InnerText;
                    requestSchema.EventName = testXmlDoc.SelectSingleNode("/msg/msgBody/eventName") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/eventName").InnerText;
                    requestSchema.EqpId = testXmlDoc.SelectSingleNode("/msg/msgBody/eqpId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/eqpId").InnerText;
                    requestSchema.Area = testXmlDoc.SelectSingleNode("/msg/msgBody/area") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/area").InnerText;
                    requestSchema.BatchId = testXmlDoc.SelectSingleNode("/msg/msgBody/batchId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/batchId").InnerText;
                    requestSchema.LotCnt = testXmlDoc.SelectSingleNode("/msg/msgBody/lotCnt") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/lotCnt").InnerText;
                    requestSchema.MessageId = testXmlDoc.SelectSingleNode("/msg/msgBody/messageId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/messageId").InnerText;
                    requestSchema.ExContext = testXmlDoc.SelectSingleNode("/msg/msgBody/exContext") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/exContext").InnerText;
                    requestSchema.TxId = DateTime.Now.ToLongDateString();

                    XmlNodeList tempNodeList = testXmlDoc.SelectNodes("/msg/msgBody/lotInfoList/lotInfo");
                    replyMessage = "";
                    if (tempNodeList.Count == 0)
                    {
                        replyMessage = "Can't get lotInfo from message!";
                        return;
                    }

                    foreach (XmlNode tempXmlNode in tempNodeList)
                    {
                        if (tempXmlNode == null)
                        {
                            replyMessage += "Lot Info Is Null" + Environment.NewLine;
                            continue;
                        }
                        else
                        {
                            requestSchema.XmlString += tempXmlNode.OuterXml;
                        }

                        tempClient.SMSC_R2R_Litho_CalcRecipeSettings(requestSchema);
                        /*R2RReplyService.eventInfo13 tempReplySchema = tempClient.SMSC_R2R_Litho_CalcRecipeSettings(requestSchema);

                        if (tempReplySchema.Result == "0")
                        {
                            replyMessage += RestoreXml(tempReplySchema.XmlString)+ Environment.NewLine + Environment.NewLine;
                        }
                        else
                        {
                            replyMessage += "Result: " + tempReplySchema.Result + Environment.NewLine;
                            replyMessage += "Errdesc: " + tempReplySchema.ErrDesc + Environment.NewLine;
                            replyMessage += "ReplyMsg: " + RestoreXml(tempReplySchema.XmlString)+ Environment.NewLine;
                        }*/

                    }

                }
            }
            catch (Exception err)
            {
                replyMessage = "Excepetion happen in " + System.Reflection.MethodBase.GetCurrentMethod().Name + " : " + err.Message;
            }
            finally
            {
                this.BeginInvoke(myUpdateReplyMessage);
            }
        }

        private void Handle_SMSC_R2R_Litho_Metrology()
        {
            R2RService.SMSC_R2R_ControllerServiceSoapClient tempClient = new R2RService.SMSC_R2R_ControllerServiceSoapClient("SMSC_R2R_ControllerServiceSoap12");

            R2RService.eventInfo14 requestSchema = new R2RService.eventInfo14();
            XmlDocument testXmlDoc = new XmlDocument();

            try
            {
                testXmlDoc.LoadXml(requestMessage);
                if (testXmlDoc != null)
                {
                    requestSchema.FabName = testXmlDoc.SelectSingleNode("/msg/msgBody/fabName") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/fabName").InnerText;
                    requestSchema.EventName = testXmlDoc.SelectSingleNode("/msg/msgBody/eventName") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/eventName").InnerText;
                    requestSchema.EqpId = testXmlDoc.SelectSingleNode("/msg/msgBody/eqpId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/eqpId").InnerText;
                    requestSchema.Area = testXmlDoc.SelectSingleNode("/msg/msgBody/area") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/area").InnerText;
                    requestSchema.BatchId = testXmlDoc.SelectSingleNode("/msg/msgBody/batchId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/batchId").InnerText;
                    requestSchema.LotCnt = testXmlDoc.SelectSingleNode("/msg/msgBody/lotCnt") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/lotCnt").InnerText;
                    requestSchema.MessageId = testXmlDoc.SelectSingleNode("/msg/msgBody/messageId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/messageId").InnerText;
                    requestSchema.ExContext = testXmlDoc.SelectSingleNode("/msg/msgBody/exContext") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/exContext").InnerText;
                    requestSchema.TxId = DateTime.Now.ToLongDateString();

                    XmlNodeList tempNodeList = testXmlDoc.SelectNodes("/msg/msgBody/lotInfoList/lotInfo");
                    replyMessage = "";
                    if (tempNodeList.Count == 0)
                    {
                        replyMessage = "Can't get lotInfo from message!";
                        return;
                    }

                    foreach (XmlNode tempXmlNode in tempNodeList)
                    {
                        if (tempXmlNode == null)
                        {
                            replyMessage += "Lot Info Is Null" + Environment.NewLine;
                            continue;
                        }
                        else
                        {
                            requestSchema.XmlString += tempXmlNode.OuterXml;
                        }

                        tempClient.SMSC_R2R_Litho_Metrology(requestSchema);
                        /*R2RReplyService.eventInfo14 tempReplySchema = tempClient.SMSC_R2R_Litho_Metrology(requestSchema);

                        if (tempReplySchema.Result == "0")
                        {
                            replyMessage += RestoreXml(tempReplySchema.XmlString)+ Environment.NewLine + Environment.NewLine;
                        }
                        else
                        {
                            replyMessage += "Result: " + tempReplySchema.Result + Environment.NewLine;
                            replyMessage += "Errdesc: " + tempReplySchema.ErrDesc + Environment.NewLine;
                            replyMessage += "ReplyMsg: " + RestoreXml(tempReplySchema.XmlString)+ Environment.NewLine;
                        }*/

                    }

                }
            }
            catch (Exception err)
            {
                replyMessage = "Excepetion happen in " + System.Reflection.MethodBase.GetCurrentMethod().Name + " : " + err.Message;
            }
            finally
            {
                this.BeginInvoke(myUpdateReplyMessage);
            }
        }

        private void Handle_SMSC_R2R_Litho_UsedSettings()
        {
            R2RService.SMSC_R2R_ControllerServiceSoapClient tempClient = new R2RService.SMSC_R2R_ControllerServiceSoapClient("SMSC_R2R_ControllerServiceSoap12");

            R2RService.eventInfo15 requestSchema = new R2RService.eventInfo15();
            XmlDocument testXmlDoc = new XmlDocument();

            try
            {
                testXmlDoc.LoadXml(requestMessage);
                if (testXmlDoc != null)
                {
                    requestSchema.FabName = testXmlDoc.SelectSingleNode("/msg/msgBody/fabName") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/fabName").InnerText;
                    requestSchema.EventName = testXmlDoc.SelectSingleNode("/msg/msgBody/eventName") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/eventName").InnerText;
                    requestSchema.EqpId = testXmlDoc.SelectSingleNode("/msg/msgBody/eqpId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/eqpId").InnerText;
                    requestSchema.Area = testXmlDoc.SelectSingleNode("/msg/msgBody/area") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/area").InnerText;
                    requestSchema.BatchId = testXmlDoc.SelectSingleNode("/msg/msgBody/batchId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/batchId").InnerText;
                    requestSchema.LotCnt = testXmlDoc.SelectSingleNode("/msg/msgBody/lotCnt") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/lotCnt").InnerText;
                    requestSchema.MessageId = testXmlDoc.SelectSingleNode("/msg/msgBody/messageId") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/messageId").InnerText;
                    requestSchema.ExContext = testXmlDoc.SelectSingleNode("/msg/msgBody/exContext") == null ?
                        defaultValue : testXmlDoc.SelectSingleNode("/msg/msgBody/exContext").InnerText;
                    requestSchema.TxId = DateTime.Now.ToLongDateString();

                    XmlNodeList tempNodeList = testXmlDoc.SelectNodes("/msg/msgBody/lotInfoList/lotInfo");
                    replyMessage = "";
                    if (tempNodeList.Count == 0)
                    {
                        replyMessage = "Can't get lotInfo from message!";
                        return;
                    }

                    foreach (XmlNode tempXmlNode in tempNodeList)
                    {
                        if (tempXmlNode == null)
                        {
                            replyMessage += "Lot Info Is Null" + Environment.NewLine;
                            continue;
                        }
                        else
                        {
                            requestSchema.XmlString += tempXmlNode.OuterXml;
                        }

                        tempClient.SMSC_R2R_Litho_UsedSettings(requestSchema);
                        /*R2RReplyService.eventInfo15 tempReplySchema = tempClient.SMSC_R2R_Litho_UsedSettings(requestSchema);

                        if (tempReplySchema.Result == "0")
                        {
                            replyMessage += RestoreXml(tempReplySchema.XmlString)+ Environment.NewLine + Environment.NewLine;
                        }
                        else
                        {
                            replyMessage += "Result: " + tempReplySchema.Result + Environment.NewLine;
                            replyMessage += "Errdesc: " + tempReplySchema.ErrDesc + Environment.NewLine;
                            replyMessage += "ReplyMsg: " + RestoreXml(tempReplySchema.XmlString)+ Environment.NewLine;
                        }*/

                    }

                }
            }
            catch (Exception err)
            {
                replyMessage = "Excepetion happen in " + System.Reflection.MethodBase.GetCurrentMethod().Name + " : " + err.Message;
            }
            finally
            {
                this.BeginInvoke(myUpdateReplyMessage);
            }
        }



        private void AssignParameter(object requestSchema)
        {
            //requestSchema = new requestSchema.GetType();
            //XmlDocument testXmlDoc = new XmlDocument();
            //testXmlDoc.LoadXml(this.rtbRequest.Text);
            //if (testXmlDoc != null)
            //{
            //    requestSchema.FabName = testXmlDoc.SelectSingleNode("/msg/msgBody/fabName") == null ?
            //        "" : testXmlDoc.SelectSingleNode("/msg/msgBody/fabName").InnerText;
            //    requestSchema.EventName = testXmlDoc.SelectSingleNode("/msg/msgBody/eventName") == null ?
            //        "" : testXmlDoc.SelectSingleNode("/msg/msgBody/eventName").InnerText;
            //    requestSchema.EqpId = testXmlDoc.SelectSingleNode("/msg/msgBody/eqpId") == null ?
            //        "" : testXmlDoc.SelectSingleNode("/msg/msgBody/eqpId").InnerText;
            //    requestSchema.Area = testXmlDoc.SelectSingleNode("/msg/msgBody/area") == null ?
            //        "" : testXmlDoc.SelectSingleNode("/msg/msgBody/area").InnerText;
            //    requestSchema.BatchId = testXmlDoc.SelectSingleNode("/msg/msgBody/batchId") == null ?
            //        "" : testXmlDoc.SelectSingleNode("/msg/msgBody/batchId").InnerText;
            //    requestSchema.LotCnt = testXmlDoc.SelectSingleNode("/msg/msgBody/lotCnt") == null ?
            //        "" : testXmlDoc.SelectSingleNode("/msg/msgBody/lotCnt").InnerText;
            //    requestSchema.MessageId = testXmlDoc.SelectSingleNode("/msg/msgBody/messageId") == null ?
            //        "" : testXmlDoc.SelectSingleNode("/msg/msgBody/messageId").InnerText;
            //    requestSchema.ExContext = testXmlDoc.SelectSingleNode("/msg/msgBody/exContext") == null ?
            //        "" : testXmlDoc.SelectSingleNode("/msg/msgBody/exContext").InnerText;
            //    requestSchema.TxId = DateTime.Now.ToLongDateString();
            //    requestSchema.XmlString = testXmlDoc.SelectSingleNode("/msg/msgBody/lotInfoList").OuterXml +
            //        testXmlDoc.SelectSingleNode("/msg/msgBody/batchParaInfo").OuterXml;
        }

        private void myTimer_Tick(object sender, EventArgs e)
        {
            this.rtbReply.Text = "Please wait web service reply.........." + (++waitSecond).ToString();
        }

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
                inRichTextBox.Text = FormatXml(RestoreXml(tempStr));
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

        private string FormatXml(string sUnformattedXml)
        {
            XmlDocument xd = new XmlDocument();
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            XmlTextWriter xtw = null;
            try
            {
                xd.LoadXml(sUnformattedXml);
                xtw = new XmlTextWriter(sw);
                xtw.Formatting = Formatting.Indented;
                xtw.Indentation = 8;
                xtw.IndentChar = ' ';
                xd.WriteTo(xtw);
            }
            finally
            {
                if (xtw != null)
                    xtw.Close();
            }
            return sb.ToString();
        }

        public static string RestoreXml(string xmlStrInWebService)
        {
            string s = xmlStrInWebService;
            while (s.Contains("&lt;") || s.Contains("&gt;") || 
                s.Contains("&apos;") || s.Contains("&aquot;") || s.Contains("&amp;"))
            {
                s = s.Replace("&lt;", "<");
                s = s.Replace("&gt;", ">");
                s = s.Replace("&apos;", "'");
                s = s.Replace("&aquot;", "\"");
                s = s.Replace("&amp;", "&");
            }
            return s;
        }

        public static string ToDataFromWebService(string xml)
        {
            string s = xml;
            s = s.Replace("&", "&amp;");
            s = s.Replace("\"", "&aquot;");
            s = s.Replace("'", "&apos;");
            s = s.Replace(">", "&gt;");
            s = s.Replace("<", "&lt;");
            return s;
        }

        private void tbnRequestToXML_Click(object sender, EventArgs e)
        {
            RichBoxTextToXML(this.rtbRequest);
        }

        private void btnReplyToXML_Click(object sender, EventArgs e)
        {
            RichBoxTextToXML(this.rtbReply);
        }

        private void sendToolStrip_Click(object sender, EventArgs e)
        {
            SendTreeNodeList = false;
            SendMessageToE3();
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
            dialog.SelectedPath = this.m_DirectoryPath;
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
                        LoadFileTree(dialog.SelectedPath);
                    }
                }
            }
        }

        private void cmbAddress_TextChanged(object sender, EventArgs e)
        {
            wsdl.Reset();
            wsdl.Paths.Add(this.cmbAddress.Text);
            wsdl.Generate();
            FillInvokeTab();
            WebServiceStudio.Configuration.MasterConfig.InvokeSettings.AddUri(this.cmbAddress.Text);
            InitMethodName();
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

                sfd.InitialDirectory = this.m_DirectoryPath;

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string localFilePath = sfd.FileName.ToString();
                    string fileNameExt = localFilePath.Substring(localFilePath.LastIndexOf("\\") + 1);

                    SaveFile(localFilePath, selectRichTextBox.Text);
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

            LoadFileTree(path);
        }

        private void toDispatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int configLocation = 0;
            bool showError = true;

            if (sender.GetType() == typeof(bool))
            {
                showError = (bool)sender;
            }

            if (selectRichTextBox != null)
            {
                if (IsE3EventInfo(out configLocation))
                {
                    RichBoxTextToXML(selectRichTextBox);
                    normalCollection = (System.Collections.Hashtable)ConfigurationManager.GetSection("dispatcherMapping" + configLocation.ToString());
                    batchCollection = (System.Collections.Hashtable)ConfigurationManager.GetSection("batchDispatcherMapping" + configLocation.ToString());
                    methodMapping = (System.Collections.Hashtable)ConfigurationManager.GetSection("dispatcherMethodMapping");
                    dispatcherConfig = (System.Collections.Hashtable)ConfigurationManager.GetSection("dispatchConfig");

                    if (normalCollection == null || batchCollection == null ||
                        methodMapping == null || dispatcherConfig == null)
                    {
                        if(showError) MessageBox.Show("The config for transfer message to dispatcher is not enough!");
                        return;
                    }

                    ChangeToDispatcherMessage(selectRichTextBox);
                }
                else
                {
                    if (showError) MessageBox.Show("Request message is not controller test meesage!");
                }
                //need confirm null value
            }
        }

        private bool IsE3EventInfo(out int dispatcherConfig)
        {
            string path = "";
            string fullPathStr = "";
            dispatcherConfig = 1;

            try
            {
                if (this.rtbRequest.Text == "") return false;


                fullPathStr = ConfigurationManager.ConnectionStrings["E3TestRootName"] == null ? "" :
                        ConfigurationManager.ConnectionStrings["E3TestRootName"].ConnectionString;


                if (fullPathStr != "")
                {
                    for (int configLocaion = 0; configLocaion < fullPathStr.Split(ConfigDelimeter).Count(); configLocaion++)
                    {
                        path = "";
                        path = fullPathStr.Split(ConfigDelimeter)[configLocaion];
                        if (path != "")
                        {
                            XmlDocument testXmlDoc = new XmlDocument();
                            testXmlDoc.LoadXml(this.rtbRequest.Text);
                            if (testXmlDoc == null)
                            {
                                return false;
                            }
                            if (testXmlDoc.SelectSingleNode(path) != null)
                            {
                                dispatcherConfig = dispatcherConfig + configLocaion;
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Excepetion happen in " + System.Reflection.MethodBase.GetCurrentMethod().Name + " : " + err.Message);
            }

            return false;
        }

        private void ChangeToDispatcherMessage(RichTextBox inRichTextBox)
        {
            string tempStr = inRichTextBox.Text;
            string path = @".\DispatcherFormat.xml";
            XmlDocument xDoc = new XmlDocument();
            XmlDocument xRequestDoc = new XmlDocument();
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            XmlTextWriter xtw = null;

            try
            {
                if (File.Exists(path) && this.rtbRequest.Text != "")
                {
                    xDoc.Load(path);
                    xRequestDoc.LoadXml(this.rtbRequest.Text);
                    XmlNodeList nodeList = xDoc.SelectNodes("/");

                    XmlNamespaceManager xnm = new XmlNamespaceManager(xRequestDoc.NameTable);
                    xnm.AddNamespace("xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
                    xnm.AddNamespace("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");

                    ReplaceDispatcher(nodeList, xRequestDoc);

                    xtw = new XmlTextWriter(sw);
                    xtw.Formatting = Formatting.Indented;
                    xtw.Indentation = 8;
                    xtw.IndentChar = ' ';
                    xDoc.WriteTo(xtw);
                    //xDoc.Save(xmlPath);
                }
                else
                {
                    MessageBox.Show("Can't find DispatcherFormat.xml in running folder!");
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Excepetion happen in " + System.Reflection.MethodBase.GetCurrentMethod().Name + " : " + err.Message);
            } finally
            {
                if (xtw != null) xtw.Close();
            }

            this.rtbRequest.Text = sb.ToString();
        }

        private void AutoChangeContextInfo(RichTextBox inRichTextBox)
        {
            string tempStr = inRichTextBox.Text;

            try
            {
                foreach (ContextGroup singleContextGroup in autoGenerateContext.ContextGroupList)
                {
                    singleContextGroup.CurrentCount = CalculateCurrentCount(singleContextGroup.Count, singleContextGroup.Mode);
                }

                foreach (ContextGroup singleContextGroup in autoGenerateContext.ContextGroupList)
                {
                    tempStr = tempStr.Replace(singleContextGroup.Name, 
                        (FormatCount(singleContextGroup.CurrentCount,singleContextGroup.Type)).ToString());
                }

            }
            catch (Exception err)
            {
                MessageBox.Show("Excepetion happen in " + System.Reflection.MethodBase.GetCurrentMethod().Name + " : " + err.Message);
            }
            
            inRichTextBox.Text = tempStr;
        }

        private string FormatCount(int count, string type)
        {
            string result = "";

            result = string.Format(type, count);
            return result;
        }

        private int CalculateCurrentCount(int contextCount, string contextMode)
        {
            int result = currentPerfTestCount;
            bool assigned = false;

            if (contextMode.ToUpper() == "INCREASE")
            {
                assigned = true;
                result = currentPerfTestCount - 1;
            }
            else if (contextMode.ToUpper() == "LOOP")
            {
                assigned = true;
                result = (currentPerfTestCount - 1) % (contextCount <= 0 ? 1 : contextCount);
            }
            else
            {
                for (int i = autoContextCountList.Count() - 1; i >= 0; i--)
                {
                    if (autoContextCountList[i] <= currentPerfTestCount && autoContextCountList[i] >= contextCount)
                    {
                        if (autoContextCountList[i] == contextCount)
                        {
                            result = contextCount == 0 ?
                                (assigned ? result : result - 1) : result / contextCount;
                            assigned = true;
                        }
                        else
                        {
                            result = result % autoContextCountList[i];
                            assigned = true;
                        }
                    }
                }
            }
            return assigned ? result : 0;
        }

        private void ReplaceDispatcher(XmlNodeList nodeList, XmlDocument requestDoc)
        {
            string autoChangeMe = "";
            foreach (XmlNode tempNode in nodeList)
            {
                if (tempNode.NodeType != XmlNodeType.Text && tempNode.NodeType != XmlNodeType.Document)
                {
                    string tempPath = GetNodePath(tempNode);

                    if (dispatcherConfig.ContainsKey("AutoChange"))
                    {
                        if (dispatcherConfig["AutoChange"].ToString() == tempPath)
                        {
                            autoChangeMe = GetAutoChangeName();
                            if (autoChangeMe != "") tempNode.InnerText = autoChangeMe;
                        }
                    }

                    if (dispatcherConfig.ContainsKey("IsBatch") && this.pathTree.SelectedNode != null &&
                        this.pathTree.SelectedNode.Text.Contains(dispatcherConfig["IsBatch"].ToString()))
                    {
                        if (batchCollection.ContainsKey(tempPath))
                        {
                            if (tempNode.ChildNodes.Count == 1 && tempNode.InnerText == tempNode.InnerXml)
                            {
                                if (requestDoc.SelectSingleNode(batchCollection[tempPath].ToString()) != null)
                                {
                                    tempNode.InnerText = requestDoc.SelectSingleNode(batchCollection[tempPath].ToString()).InnerText;
                                }
                            }
                            else
                            {
                                if (requestDoc.SelectSingleNode(batchCollection[tempPath].ToString()) != null)
                                {
                                    tempNode.InnerXml = requestDoc.SelectSingleNode(batchCollection[tempPath].ToString()).InnerXml;
                                }
                            }
                            continue;
                        }
                    }
                    else
                    {
                        if (normalCollection.ContainsKey(tempPath))
                        {
                            if (tempNode.ChildNodes.Count == 1)
                            {
                                if (requestDoc.SelectSingleNode(normalCollection[tempPath].ToString()) != null)
                                {
                                    tempNode.InnerText = requestDoc.SelectSingleNode(normalCollection[tempPath].ToString()).InnerText;
                                }
                            }
                            else
                            {
                                if (requestDoc.SelectSingleNode(normalCollection[tempPath].ToString()) != null)
                                {
                                    tempNode.InnerXml = requestDoc.SelectSingleNode(normalCollection[tempPath].ToString()).InnerXml;
                                }
                            }
                            continue;
                        }
                    }
                }

                if (tempNode.ChildNodes.Count > 0)
                {
                    ReplaceDispatcher(tempNode.ChildNodes, requestDoc);
                }
            }
        }

        private string GetNodePath(XmlNode node)
        {
            string nodePath = node.Name;
            while (node.ParentNode != null && node.ParentNode.NodeType !=XmlNodeType.Document
                && node.ParentNode.NodeType != XmlNodeType.XmlDeclaration
                && node.ParentNode.NodeType != XmlNodeType.Text)
            {
                node = node.ParentNode;
                nodePath = node.Name + "/" + nodePath;
            }
            nodePath = "/" + nodePath;
            return nodePath;
        }

        private string GetAutoChangeName()
        {
            string tempStr = "";
            if (this.pathTree.SelectedNode != null)
            {
                foreach (var tempVar in methodMapping.Keys)
                {
                    if (this.pathTree.SelectedNode.Text.Contains(tempVar.ToString()))
                    {
                        tempStr = methodMapping[tempVar.ToString()].ToString();
                    }
                }
            }

            return tempStr;
        }

        private void splitContainer3_Panel1_SizeChanged(object sender, EventArgs e)
        {
            AutoChangeFunctionListButton();
        }

        private void reloadFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadFileTree(m_DirectoryPath);
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
                currentPerfTestCount = 1;
                if (cbPerfTest.Checked == true)
                {
                    if (this.tbMsgCount.Text.Contains(";"))
                    {
                        if (this.tbMsgCount.Text.Split(ConfigDelimeter).Count() == 2)
                        {
                            perfMsgCount = Convert.ToInt32(this.tbMsgCount.Text.Split(ConfigDelimeter)[0]);
                            currentPerfTestCount = Convert.ToInt32(this.tbMsgCount.Text.Split(ConfigDelimeter)[1]);
                        }
                        else
                        {
                            MessageBox.Show("Only support PerfMsgCount and Start Test Count split by ; in PerfMsgCount TextBox");
                        }
                    }
                    else
                    {
                        perfMsgCount = Convert.ToInt32(this.tbMsgCount.Text);
                    }
                }
                else
                {
                    perfMsgCount = 1;
                }

                this.lbCurrentLoop.Text = "CurrentLoop: " + currentPerfTestCount.ToString();
                RunAllNodesInDirectory(true);
            }
            catch (Exception err)
            {
                MessageBox.Show("Excepetion happen in " + System.Reflection.MethodBase.GetCurrentMethod().Name + " : " + err.Message);
            }
        }


        private void RunAllNodesInDirectory(bool firstLoop)
        {
            TreeNode directoryNode;
            if(firstLoop)
            {
                directoryNode = currentLoopDirectoryNode = this.pathTree.SelectedNode;
            }else
            {
                directoryNode = currentLoopDirectoryNode;
            }

            try
            {
                //initial wait send node related variable
                currentSendNodeCount = 1;
                currentActualSendNodeCount = 0;
                waitSendTreeNode = new Dictionary<int, TreeNode>();
                SendTreeNodeList = true;
                durationSendFlag = "";

                if (sendStartNode != null)
                {
                    durationCheckNeed = true;
                }
                else
                {
                    durationCheckNeed = false;
                }

                if (directoryNode.Nodes.Count > 0)
                {
                    int tempNodeCount = 0;
                    foreach (TreeNode tempNode in directoryNode.Nodes)
                    {
                        if (tempNode.Nodes.Count == 0)
                        {
                            waitSendTreeNode.Add(++tempNodeCount, tempNode);
                        }
                    }

                    if (!waitSendTreeNode.ContainsValue(sendStartNode))
                    {
                        durationCheckNeed = false;
                    }

                    SendAllWaitNodes();
                }
            }
            catch(Exception err)
            {
                MessageBox.Show("Excepetion happen in " + System.Reflection.MethodBase.GetCurrentMethod().Name + " : " + err.Message);
            }
        }

        private void SendAllWaitNodes()
        {
            bool currentNodeNeedSend;

            if (currentSendNodeCount <= waitSendTreeNode.Count)
            {
                if (waitSendTreeNode.ContainsKey(currentSendNodeCount))
                {
                    TreeNode tempNode = waitSendTreeNode[currentSendNodeCount];

                    currentNodeNeedSend = true;
                    if (durationCheckNeed)
                    {
                        if (durationSendFlag == durSendEnd)
                        {
                            currentNodeNeedSend = false;
                            SendTreeNodeList = false;
                        }
                        else
                        {
                            if (durationSendFlag == "")
                            {
                                if (tempNode == sendStartNode)
                                {
                                    durationSendFlag = durSendStart;
                                    currentNodeNeedSend = true;
                                }
                                else
                                {
                                    currentNodeNeedSend = false;
                                }
                            }
                            if (durationSendFlag == durSendStart)
                            {
                                if (tempNode == sendEndNode)
                                {
                                    durationSendFlag = durSendEnd;
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
                        if (durationSendFlag == durSendEnd)
                        {
                            currentNodeNeedSend = false;
                            SendTreeNodeList = false;
                        }
                    }

                    string tempName = tempNode.Text;
                    tempName = tempName.Substring(tempName.LastIndexOf("."));

                    if (currentNodeNeedSend && multiNodesExtensionName.Contains(tempName))
                    {
                        currentActualSendNodeCount++;
                        this.pathTree.SelectedNode = tempNode;

                        LoadTestFile();

                        SendButtonClick();
                    }
                    else
                    {
                        this.lbCurrentCount.Text = currentActualSendNodeCount + "/" + currentSendNodeCount + "/" + waitSendTreeNode.Count;
                        currentSendNodeCount++;
                        SendAllWaitNodes();
                    }
                }
            }
            else
            {
                SendTreeNodeList = false;
                waitSendTreeNode = new Dictionary<int, TreeNode>();
                if (cbPerfTest.Checked == true)
                {
                    if (currentPerfTestCount < perfMsgCount)
                    {
                        currentPerfTestCount++;
                        this.lbCurrentLoop.Text = "CurrentLoop: " + currentPerfTestCount.ToString();
                        RunAllNodesInDirectory(false);
                    }
                }
            }
        }

        private void SetStartStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode fileNode = this.pathTree.SelectedNode;

            if (fileNode.Nodes.Count == 0)
            {
                if (sendStartNode != null)
                {
                    sendStartNode.ForeColor = Color.Black;
                }
                if (sendEndNode != null)
                {
                    sendEndNode.ForeColor = Color.Black;
                }
                sendStartNode = fileNode;
                sendStartNode.ForeColor = Color.Red;
                sendEndNode = null;
            }

        }

        private void SetEndStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode fileNode = this.pathTree.SelectedNode;

            if (fileNode.Nodes.Count == 0)
            {
                sendEndNode = fileNode;
                sendEndNode.ForeColor = Color.Red;
            }
        }

        private void clearStartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sendStartNode != null)
            {
                sendStartNode.ForeColor = Color.Black;
            }
            if (sendEndNode != null)
            {
                sendEndNode.ForeColor = Color.Black;
            }
            sendStartNode = null;
            sendEndNode = null;
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            durationSendFlag = durSendEnd;
            SendTreeNodeList = false;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            this.rtbReply.Clear();
            this.rtbReply.Text = "Manual Stop Waiting";
            this.btnSend.Enabled = true;
            waitSecond = 0;
            myTimer.Enabled = false;

            durationSendFlag = durSendEnd;
            SendTreeNodeList = false;
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
    }
}

