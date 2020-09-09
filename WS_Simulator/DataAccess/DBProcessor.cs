using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WS_Simulator.DataAccess
{
    public static class DBProcessor
    {
        private static DBHelper _myDBHelper;
        private static readonly string _declareStr = "DECLARE";
        private static readonly string _beginStr = "BEGIN";
        private static readonly string _endStr = "END";

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
                DataTable tempDataTable = _myDBHelper.GetTable(requestMessage.RemoveSemicolonFromMessage());
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

        public static string FormatSqlToProcedure(string sqlMessage)
        {
            string result = sqlMessage;

            if (sqlMessage.ToUpper().Contains(_beginStr) && sqlMessage.ToUpper().Contains(_endStr))
            {
                return result;
            }
            else
            {
                result = $"{_declareStr}\n{_beginStr}\n{sqlMessage}\n{_endStr};";
            }

            return result;
        }

        private static string RemoveSemicolonFromMessage(this string sqlMessage)
        {
            string result = sqlMessage;
            string pattern = @";\s+$";
            Regex rgx = new Regex(pattern);

            if (sqlMessage.ToUpper().Contains(_beginStr) && sqlMessage.ToUpper().Contains(_endStr))
            {
                return result;
            }

            if(rgx.Match(sqlMessage).Success)
            {
                result = rgx.Replace(sqlMessage, "");
            }

            return result;
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
