using System;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using eFakturADM.Shared.Utility;

namespace eFakturADM.Logic.Core
{
    public class sqlsrv
    {
        SqlConnection conn = new SqlConnection();

        public sqlsrv()
        {
            try
            {
                conn = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings["eFakturADM.Connection.String"]);
                //int a = conn.ConnectionTimeout;
                conn.Open();
            }
            catch (Exception err)
            {
                //WriteLog.CreateLog("SQL CONNECT (" + DateTime.Now + ") : " + err.Message, "SQLServer", "Query");
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error,  "SQL CONNECT (" + DateTime.Now + ") : " + err.Message, MethodBase.GetCurrentMethod(), err);
                conn.Close();
                conn.Dispose();
            }
        }

        public DataSet sql_select(string cmd)
        {
            DataSet ds = new DataSet();

            try
            {
                using (SqlDataAdapter data = new SqlDataAdapter(cmd, conn))
                {
                    data.Fill(ds);
                }
            }
            catch (Exception err)
            {
                ds.Clear();
                var errMsg = "SQL SELECT (" + DateTime.Now + ") : " + err.Message;
                errMsg = Environment.NewLine + "SELECT CMD (" + DateTime.Now + ") : " + cmd;
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, errMsg, MethodBase.GetCurrentMethod(), err);
                //WriteLog.CreateLog("SQL SELECT (" + DateTime.Now + ") : " + err.Message, "SQLServer", "Query");
                //WriteLog.CreateLog("SELECT CMD (" + DateTime.Now + ") : " + cmd, "SQLServer", "Query");
            }

            return ds;
        }

        public void string_cmd(string cmd)
        {
            try
            {
                using (SqlCommand data = new SqlCommand())
                {
                    data.CommandText = cmd;
                    data.CommandType = CommandType.Text;

                    data.Connection = conn;
                    data.CommandTimeout = int.MaxValue;
                    data.ExecuteScalar();
                }
            }
            catch (Exception err)
            {
                //WriteLog.CreateLog("SQL COMMAND (" + DateTime.Now + ") : " + err.Message, "SQLServer", "Query");
                //WriteLog.CreateLog("STRING CMD (" + DateTime.Now + ") : " + cmd, "SQLServer", "Query");
                var errMsg = "SQL COMMAND (" + DateTime.Now + ") : " + err.Message;
                errMsg = Environment.NewLine + "STRING CMD (" + DateTime.Now + ") : " + cmd;
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, errMsg, MethodBase.GetCurrentMethod(), err);
            }
        }

        public void InsertTable_cmd(string spname, string tabletypename, DataTable tableparam)
        {
            try
            {
                using (SqlCommand sqlCmd = new SqlCommand())
                {

                    sqlCmd.CommandText = spname;
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Connection = conn;
                    sqlCmd.CommandTimeout = int.MaxValue;
                    SqlParameter InputTableParam = sqlCmd.Parameters.AddWithValue(tabletypename, tableparam);
                    InputTableParam.SqlDbType = SqlDbType.Structured;
                    sqlCmd.ExecuteNonQuery();

                }
            }
            catch (Exception err)
            {
                var errMsg = "SQL COMMAND (" + DateTime.Now + ") : " + err.Message;
                errMsg = Environment.NewLine + "SP EXECUTION (" + DateTime.Now + ") : " + spname;
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, errMsg, MethodBase.GetCurrentMethod(), err);
                //WriteLog.CreateLog("SQL COMMAND (" + DateTime.Now + ") : " + err.Message, "SQLServer", "Query");
                //WriteLog.CreateLog("SP EXECUTION (" + DateTime.Now + ") : " + spname, "SQLServer", "Query");
                //error.error_log("SQL COMMAND : " + err.Message);
                //error.error_log("STRING CMD : " + cmd);
            }
        }
        public void InsertTable_cmdScalar(string spname, string tabletypename, DataTable tableparam)
        {
            try
            {
                using (SqlCommand sqlCmd = new SqlCommand())
                {

                    sqlCmd.CommandText = spname;
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Connection = conn;
                    sqlCmd.CommandTimeout = int.MaxValue;
                    SqlParameter InputTableParam = sqlCmd.Parameters.AddWithValue(tabletypename, tableparam);
                    InputTableParam.SqlDbType = SqlDbType.Structured;
                    sqlCmd.ExecuteScalar();

                }
            }
            catch (Exception err)
            {
                var errMsg = "SQL COMMAND (" + DateTime.Now + ") : " + err.Message;
                errMsg = Environment.NewLine + "SP EXECUTION (" + DateTime.Now + ") : " + spname;
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, errMsg, MethodBase.GetCurrentMethod(), err);
                //WriteLog.CreateLog("SQL COMMAND (" + DateTime.Now + ") : " + err.Message, "SQLServer", "Query");
                //WriteLog.CreateLog("SP EXECUTION (" + DateTime.Now + ") : " + spname, "SQLServer", "Query");
                //error.error_log("SQL COMMAND : " + err.Message);
                //error.error_log("STRING CMD : " + cmd);
            }
        }
        public void close_con()
        {
            try
            {
                conn.Close();
                conn.Dispose();
            }
            catch (Exception err)
            {
                //WriteLog.CreateLog("SQL CLOSE (" + DateTime.Now + ") : " + err.Message, "SQLServer", "Query");
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, "SQL CLOSE (" + DateTime.Now + ") : " + err.Message, MethodBase.GetCurrentMethod(), err);
                //error.error_log("SQL CLOSE : " + err.Message);
            }
        }
    }
}
