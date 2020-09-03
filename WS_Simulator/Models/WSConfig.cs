using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebServiceStudio;

namespace WS_Simulator.Models
{
    public class WSConfig
    {
        private static char[] ConfigDelimeter = (";").ToCharArray();

        public readonly string DurSendStart = "Start";
        public readonly string DurEndStart = "End";

        public List<string> WSAddressList { get; set; } = new List<string>();
        public List<string> WSMethodList { get; set; } = new List<string>();
        public List<string> BatchMethodName { get; set; }
        public List<string> FileExtensionName { get; set; }
        public List<string> MultiNodeExtensionName { get; set; }
        public List<string> NeedWaitMessageList { get; set; }
        public int SleepTime { get; set; }
        public int CurrentSendNodeCount { get; set; }
        public int CurrentActualSendNodeCount { get; set; }
        public bool DurationCheckedNeed { get; set; }
        public bool DurationCheckNeed { get; set; }
        public string DurationSendFlag { get; set; }


        public TreeNode SendStartNode { get; set; }
        public TreeNode SendEndNode { get; set; }

        public DBHelper MyDBHelper { get; set; }
        public bool DBHelperNeed { get; set; }
        public bool IsBatch { get; set; }

        public Hashtable NormalCollection { get; set; }
        public Hashtable BatchCollection { get; set; }
        public Hashtable MethodMapping { get; set; }
        public Hashtable DispatchConfig { get; set; }

        public TreeNode CurrLoopDirectoryNode { get; set; }
        public int CurrentPerTestCount { get; set; }
        public int PerfMsgCount { get; set; }


        #region Method

        public bool InitializeWSConfig(out string errDesc)
        {
            errDesc = "";

            bool isOkay = false;

            (isOkay, WSAddressList) = InitListConfiguration("WebServiceAddr", out errDesc);

            if (isOkay == false)
            {
                return false;
            }

            (isOkay, BatchMethodName) = InitListConfiguration("BatchMethodName", out errDesc);

            if (isOkay == false)
            {
                return false;
            }

            (isOkay, FileExtensionName) = InitListConfiguration("FileExtension", out errDesc, ConfigType.APP);

            if (isOkay == false)
            {
                return false;
            }

            (isOkay, MultiNodeExtensionName) = InitListConfiguration("NeedSendExtension", out errDesc, ConfigType.APP);

            if (isOkay == false)
            {
                return false;
            }

            (isOkay, NeedWaitMessageList) = InitListConfiguration("NeedTimerWait", out errDesc, ConfigType.APP);

            if (isOkay == false)
            {
                return false;
            }

            (isOkay, SleepTime) = InitIntConfiguration("WaitTime", out errDesc);

            if (isOkay == false)
            {
                return false;
            }

            (isOkay, DBHelperNeed) = InitBoolConfiguration("needDBOpr", out errDesc);

            if (isOkay == false)
            {
                return false;
            }


            return true;
        }

        // Get list configuration from configuration file
        private (bool isOkay, List<string> listConfig) InitListConfiguration(string key, out string errDesc, ConfigType configType = ConfigType.CONN)
        {
            errDesc = "";

            bool result = false;
            List<string> listConfig = new List<string>();

            try
            {
                switch (configType)
                {
                    case ConfigType.CONN:
                        if (!string.IsNullOrEmpty(ConfigurationManager.ConnectionStrings[key]?.ConnectionString))
                        {
                            listConfig = ConfigurationManager.ConnectionStrings[key].ConnectionString.Split(ConfigDelimeter).ToList();

                            result = true;
                        }
                        else
                        {
                            errDesc = $"Get {key} from configuration file fail";
                        }
                        break;
                    case ConfigType.APP:
                        if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings[key]))
                        {
                            listConfig = ConfigurationManager.AppSettings[key].Split(ConfigDelimeter).ToList();

                            result = true;
                        }
                        else
                        {
                            errDesc = $"Get {key} from configuration file fail";
                        }
                        break;
                    default:
                        break;
                }


            }
            catch (Exception err)
            {
                errDesc = $"Exception happen when initial {key} name : { err.Message }";
            }

            return (result, listConfig);
        }
        // Get bool configuration from configuration file
        private (bool isOkay, bool boolConfig) InitBoolConfiguration(string key, out string errDesc)
        {
            errDesc = "";
            bool result = false;
            bool boolConfig = false;

            try
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings[key]))
                {
                    if(ConfigurationManager.AppSettings[key].ToUpper() == "T")
                    {
                        boolConfig = true;
                    }else
                    {
                        boolConfig = false;
                    }

                    result = true;
                }
                else
                {
                    errDesc = $"Get {key} from configuration file fail";
                }
            }
            catch (Exception err)
            {
                errDesc = $"Exception happen when initial {key} name : { err.Message }";
            }

            return (result, boolConfig);
        }
        // Get integer configuration from configuration file
        private (bool isOkay, int intConfig) InitIntConfiguration(string key, out string errDesc)
        {
            errDesc = "";

            bool result = false;
            int intConfig = 0;

            try
            {
                //WaitTime
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings[key]))
                {
                    if (!int.TryParse(ConfigurationManager.AppSettings[key], out intConfig))
                    {
                        errDesc = $"Parse {key} value to int fail!";
                    }
                    else
                    {
                        result = true;
                    }
                }
                else
                {
                    errDesc = "Get WaitTime from configuration file fail";
                }
            }
            catch (Exception err)
            {
                errDesc = $"Excepetion happen when initial needWaitMessageList/WaitTime : {err.Message}";
            }

            return (result, intConfig);
        }
        // Initial DBHelper configuration





        #endregion
    }
}
