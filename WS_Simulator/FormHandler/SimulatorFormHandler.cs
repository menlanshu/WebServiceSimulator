using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WS_Simulator.FormHandler
{
    public static class SimulatorFormHandler
    {
        public static char[] ConfigDelimeter = (";").ToCharArray();

        public static bool InitAddressList(ComboBox comboBox, out string errDesc)
        {
            errDesc = "";

            try
            {
                if (ConfigurationManager.ConnectionStrings["WebServiceAddr"] != null)
                {
                    comboBox.Items.Clear();
                    string test = ConfigurationManager.ConnectionStrings["WebServiceAddr"].ConnectionString;
                    foreach (string tempStr in test.Split(ConfigDelimeter))
                    {
                        comboBox.Items.Add(tempStr);
                    }
                    return true;
                }
                else
                {
                    errDesc = "Get WebServiceAddr from configuration file fail";
                    return false;
                }
            }
            catch (Exception err)
            {
                errDesc = $"Excepetion happen when initial address list : {err.Message} " ;
                return false;
            }
        }

        public static bool InitBatchMethoName(ref List<string> batchMethodName, out string errDesc)
        {
            errDesc = "";

            try
            {
                if (ConfigurationManager.ConnectionStrings["BatchMethodName"] != null)
                {
                    batchMethodName = new List<string>();
                    string test = ConfigurationManager.ConnectionStrings["BatchMethodName"].ConnectionString;
                    foreach (string tempStr in test.Split(ConfigDelimeter))
                    {
                        batchMethodName.Add(tempStr);
                    }
                    return true;
                }
                else
                {
                    errDesc = "Get BatchMethodName from configuration file fail";
                    return false;
                }
            }
            catch (Exception err)
            {
                errDesc = $"Excepetion happen when initial batch method name : { err.Message }";
                return false;
            }
        }


        public static bool InitFileExtensionName(ref List<string> fileExtensionName, out string errDesc)
        {
            errDesc = "";

            try
            {
                if (ConfigurationManager.AppSettings["FileExtension"] != null)
                {
                    fileExtensionName = new List<string>();
                    string test = ConfigurationManager.AppSettings["FileExtension"];
                    foreach (string tempStr in test.Split(ConfigDelimeter))
                    {
                        fileExtensionName.Add(tempStr);
                    }
                    return true;
                }
                else
                {
                    errDesc = "Get fileExtensionName from configuration file fail";
                    return false;
                }
            }
            catch (Exception err)
            {
                errDesc = $"Excepetion happen when initial InitFileExtensionName : {err.Message}";
                return false;
            }
        }

        public static bool InitSendMultiNodesFileExtensionName(ref List<string> multiNodesExtensionName, out string errDesc)
        {
            errDesc = "";

            try
            {
                if (ConfigurationManager.AppSettings["NeedSendExtension"] != null)
                {
                    multiNodesExtensionName = new List<string>();
                    string test = ConfigurationManager.AppSettings["NeedSendExtension"];
                    foreach (string tempStr in test.Split(ConfigDelimeter))
                    {
                        multiNodesExtensionName.Add(tempStr);
                    }
                    return true;
                }
                else
                {
                    errDesc = "Get NeedSendExtension from configuration file fail";
                    return false;
                }
            }
            catch (Exception err)
            {
                errDesc = $"Excepetion happen when initial InitSendMultiNodesFileExtensionName : {err.Message}";
                return false;
            }
        }

        public static bool InitMyDBHelper(ref bool DBHelperNeed, DBHelper myDBHelper, out string errDesc)
        {
            errDesc = "";

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
                    errDesc = $"InitMyDBHelper from configuration file fail";
                    return false;
                }
            }
            catch (Exception err)
            {
                errDesc = $"Excepetion happen when initial InitMyDBHelper : {err.Message}";
                return false;
            }
        }

        public static bool InitNeedWaitMessageList(ref List<string> needWaitMessageList, ref int sleepTime, out string errDesc)
        {
            errDesc = "";

            try
            {
                if (ConfigurationManager.AppSettings["NeedTimerWait"] != null)
                {
                    needWaitMessageList = new List<string>();
                    string test = ConfigurationManager.AppSettings["NeedTimerWait"];
                    foreach (string tempStr in test.Split(ConfigDelimeter))
                    {
                        needWaitMessageList.Add(tempStr);
                    }
                    //return true;
                }
                else
                {
                    errDesc = "Get NeedTimerWait from configuration file fail";
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
                    errDesc = "Get WaitTime from configuration file fail";
                    return false;
                }
            }
            catch (Exception err)
            {
                errDesc = $"Excepetion happen when initial needWaitMessageList/WaitTime : {err.Message}";
                return false;
            }
        }


        public static void InitMethodNameOfWebService(ComboBox comboBox, TreeNodeCollection treeNodes)
        {
            comboBox.Items.Clear();

            foreach (TreeNode node in treeNodes)
            {
                AddMethoName(node, comboBox);
            }
        }

        public static void AddMethoName(TreeNode inTreeNode, ComboBox comboBox)
        {
            if (inTreeNode.Nodes.Count == 0)
            {
                comboBox.Items.Add(inTreeNode.Text);
            }
            else
            {
                foreach (TreeNode tempNode in inTreeNode.Nodes)
                {
                    AddMethoName(tempNode, comboBox);
                }
            }
        }
    }
}
