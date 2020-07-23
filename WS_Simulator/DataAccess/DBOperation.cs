using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace WS_Simulator
{
    public class DBHelper
    {
        private string connectionstring { get; set; } = System.Configuration.ConfigurationManager.ConnectionStrings["myconn"].ConnectionString;

        public DBHelper()
        {
            OpenConnection();
        }

        /// <summary>
        /// 根据配置节读取连接数据库
        /// </summary>
        /// <param name="key">需要连接的数据库</param>
        public DBHelper(string key)
        {
            this.connectionstring = System.Configuration.ConfigurationManager.ConnectionStrings[key].ConnectionString;
            OpenConnection();
        }

        /// <summary>
        /// 定义Oracle连接
        /// </summary>
        private OracleConnection conn { get; set; }

        private OracleCommand cmd;
        /// <summary>
        /// 打开连接
        /// </summary>
        public void OpenConnection()
        {
            if (conn == null)
                conn = new OracleConnection(connectionstring);
            if (conn.State == ConnectionState.Closed)
                conn.Open();
        }

        /// <summary>
        /// 构建返回Command
        /// </summary>
        /// <param name="cmdText">CommandText</param>
        /// <param name="cmd">需要返回的Command</param>
        /// <param name="cmdType">执行类型</param>
        /// <param name="param">参数列表</param>
        public void BuilderCommand(string cmdText, out OracleCommand cmd, CommandType cmdType, params OracleParameter[] param)
        {
            cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd = conn.CreateCommand();
            cmd.CommandType = cmdType;
            cmd.CommandText = cmdText;
            foreach (var item in param)
            {
                cmd.Parameters.Add(item);
            }
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedName">存储过程名称</param>
        /// <param name="param">参数列表</param>
        /// <returns>返回执行成功条数</returns>
        public int ExecuteStored(string storedName, params OracleParameter[] param)
        {
            BuilderCommand(storedName, out cmd, CommandType.StoredProcedure, param);
            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 获取DataSet
        /// </summary>
        /// <param name="cmdText">执行语句</param>
        /// <returns>DataSet</returns>
        public DataSet GetDateSet(string cmdText, params OracleParameter[] param)
        {
            using (OracleDataAdapter _da = new OracleDataAdapter(cmdText, conn))
            {
                DataSet ds = new DataSet();
                _da.Fill(ds);
                return ds;
            }
        }

        /// <summary>
        /// 获取Table
        /// </summary>
        /// <param name="cmdText">执行语句</param>
        /// <returns>DataTable</returns>
        public DataTable GetTable(string cmdText)
        {
            return GetDateSet(cmdText).Tables[0];
        }

        /// <summary>
        /// 执行sql
        /// </summary>
        /// <param name="cmdText">执行语句</param>
        /// <param name="param">执行所需参数</param>
        /// <returns>成功执行数</returns>
        public int ExecuteSql(string cmdText, params OracleParameter[] param)
        {
            OracleCommand cmd = new OracleCommand();
            BuilderCommand(cmdText, out cmd, CommandType.Text, param);
            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 构建DataReader
        /// </summary>
        /// <param name="cmdText">执行语句</param>
        /// <param name="param">执行所需参数</param>
        /// <returns>DataReader</returns>
        public DataTable BuilderDataReader(string cmdText, params OracleParameter[] param)
        {
            BuilderCommand(cmdText, out cmd, CommandType.Text, param);
            OracleDataReader daReader = cmd.ExecuteReader();
            string data = string.Empty;

            DataTable OutDataTable = new DataTable();
            DataRow dataRow;

            for (int j = 0; j < daReader.FieldCount; j++)
            {
                //获取列名
                OutDataTable.Columns.Add(daReader.GetName(j));
            }

            //循环取数据集合中的数据,存到DataTable中
            do
            {
                while (daReader.Read())
                {
                    dataRow = OutDataTable.NewRow();
                    for (int j = 0; j < daReader.FieldCount; j++)
                    {
                        data = daReader[j].ToString();
                        dataRow[j] = data;
                    }
                    OutDataTable.Rows.Add(dataRow);
                }
            } while (daReader.NextResult());

            return OutDataTable;
        }
    }
}