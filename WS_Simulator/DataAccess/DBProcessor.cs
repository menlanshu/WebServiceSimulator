using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WS_Simulator.DataAccess
{
    public static class DBProcessor
    {
        private static DBHelper _myDBHelper;

        public static bool InitDBHelper(out string errDesc)
        {
            errDesc = "";

            try
            {
                _myDBHelper = new DBHelper("myconn");
                return true;
            }
            catch (Exception err)
            {
                errDesc = $"Excepetion happen when initial InitDBHelper : {err.Message}";
                return false;
            }
        }

        public static string HandleDBAction(string requestMessage, Action<string> updateReplyMessage)
        {
            string replyMessage = "";

            try
            {
                DataTable tempDataTable = _myDBHelper.GetTable(requestMessage);
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
                updateReplyMessage?.Invoke(replyMessage);
            }

            return replyMessage;
        }

        public static string ConvertBetweenDataTableAndXML_AX(DataTable dtNeedCoveret)
        {
            System.IO.TextWriter tw = new System.IO.StringWriter();
            //if TableName is empty, WriteXml() will throw Exception.             
            dtNeedCoveret.TableName = dtNeedCoveret.TableName.Length == 0 ? "Table_AX" : dtNeedCoveret.TableName;
            dtNeedCoveret.WriteXml(tw);
            //dtNeedCoveret.WriteXmlSchema(tw);
            return tw.ToString();
        }
    }
}
