
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;

namespace WebAPI.Utility
{
    /// <summary>
    /// 数据访问基础类
    /// </summary>
    public class MySqlDbHelper
    {
        /// <summary>
        /// 错误
        /// </summary>
        private List<Exception> _errs = new List<Exception>();

        /// <summary>
        /// 当前执行的参数
        /// </summary>
        private MySqlParameter[] current_params;

        /// <summary>
        /// 当前执行的sql
        /// </summary>
        private string current_sql = string.Empty;

        /// <summary>
        /// 是否启用多租户
        /// </summary>
        private Boolean tenant_enable = false;       //默认：不启用多租户

        /// <summary>
        /// 当前表名
        /// </summary>
        private string tablename { set; get; }

        /// <summary>
        /// db连接字符串
        /// </summary>
        private string ConnectionString = string.Empty;

        #region 构造函数
        public MySqlDbHelper(string ConnectionName = "")
        {
            try
            {
                if (ConnectionName != "")
                {//采用Web.config方式
                    ConnectionString = ConnectionName;
                }
            }
            catch (Exception ex)
            {
                this._errs.Add(ex);
            }
        }

        public MySqlDbHelper(string db_server = "", string db_database = "", string db_uid = "", string db_pwd = "", string db_port = "", string db_charset = "utf8")
        {
            ConnectionString = string.Format("server={0};database={1};uid={2};pwd={3};port={4};charset={5};",db_server, db_database, db_uid, db_pwd, db_port, db_charset);
        }
        #endregion

        #region 公用方法
        /// <summary>
        /// 测试连接
        /// </summary>
        /// <param name="connStr"></param>
        /// <returns>true:成功,false:失败</returns>
        public bool ping()
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                try
                {
                    //测试连接
                    connection.Open();
                }
                catch (MySqlException ex)
                {
                    this._errs.Add(ex);
                    return false;
                }
                finally
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
            return true;
        }
        /// <summary>
        /// 返回受影响的行数
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string strSql, MySqlParameter[] parameters = null)
        {
            using (MySqlConnection sqlConn = new MySqlConnection(ConnectionString))
            {
                ////----------------因为中文插入会被吃掉部分，所以在此转化，但是带来无法插入包含,逗号等符合---------
                //if (parameters != null)
                //{
                //    for (int i = 0; i < parameters.Length; i++)
                //    {
                //        MySqlParameter value = parameters[i];
                //        strSql = strSql.Replace(value.ParameterName, string.Format("'{0}'", EncryptHelper.ConvertToString(value.Value)));
                //    }
                //    parameters = null;
                //}
                ////--------------------------------------------------------------
                MySqlCommand sqlCmd = new MySqlCommand(strSql, sqlConn);
                if (parameters != null)
                {
                    sqlCmd.Parameters.AddRange(parameters);
                }
                sqlConn.Open();
                int objResult = sqlCmd.ExecuteNonQuery();
                sqlConn.Close();
                return objResult;
            }
        }

        /// <summary>
        /// 返回MySqlDataReader
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public MySqlDataReader ExecuteReader(string strSql, MySqlParameter[] parameters = null)
        {
            using (MySqlConnection sqlConn = new MySqlConnection(ConnectionString))
            {
                MySqlCommand sqlCmd = new MySqlCommand(strSql, sqlConn);
                if (parameters != null)
                {
                    sqlCmd.Parameters.AddRange(parameters);
                }
                sqlConn.Open();
                MySqlDataReader sdr = sqlCmd.ExecuteReader(CommandBehavior.CloseConnection);
                return sdr;
            }
        }

        /// <summary>
        /// 返回首行首列
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object ExecuteScalar(string strSql, MySqlParameter[] parameters = null)
        {
            using (MySqlConnection sqlConn = new MySqlConnection(ConnectionString))
            {
                MySqlCommand sqlCmd = new MySqlCommand(strSql, sqlConn);
                if (parameters != null)
                {
                    sqlCmd.Parameters.AddRange(parameters);
                }
                sqlConn.Open();
                object objResult = sqlCmd.ExecuteScalar();
                sqlConn.Close();
                return objResult;
            }
        }

        /// <summary>
        /// 返回DataSet
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(string strSql, MySqlParameter[] parameters = null)
        {
            using (MySqlConnection sqlConn = new MySqlConnection(ConnectionString))
            {
                MySqlCommand sqlCmd = new MySqlCommand(strSql, sqlConn);
                if (parameters != null)
                {
                    sqlCmd.Parameters.AddRange(parameters);
                }

                MySqlDataAdapter adp = new MySqlDataAdapter(sqlCmd);
                DataSet ds = new DataSet();
                adp.Fill(ds);
                return ds;
            }
        }
        /// <summary>
        /// 返回DataTable
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="parameters"></param>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(string strSql, MySqlParameter[] parameters = null)
        {
            DataSet ds = ExecuteDataSet(strSql, parameters);
            return ds.Tables[0];
        }
        /// <summary>
        /// 返回DataRow
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataRow ExecuteDataRow(string strSql, MySqlParameter[] parameters = null)
        {
            DataTable dt = ExecuteDataTable(strSql, parameters);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0];
            }
            else
            {
                return null;
            }
            
        }
        /// <summary>
        /// 批量执行sql
        /// </summary>
        /// <param name="listSql"></param>
        /// <returns></returns>
        public bool ExecuteSqlTran(List<string> listSql)
        {
            using (MySqlConnection sqlConn = new MySqlConnection(ConnectionString))
            {
                MySqlCommand sqlCmd = new MySqlCommand();
                sqlCmd.Connection = sqlConn;
                sqlConn.Open();
                MySqlTransaction sqlTran = sqlConn.BeginTransaction();
                sqlCmd.Transaction = sqlTran;
                bool objResult = false;
                try
                {
                    for (int i = 0; i < listSql.Count; i++)
                    {
                        sqlCmd.CommandText = listSql[i];
                        sqlCmd.ExecuteNonQuery();
                    }
                    sqlTran.Commit();
                    objResult = true;
                }
                catch
                {
                    sqlTran.Rollback();
                }
                finally
                {
                    sqlConn.Close();
                }
                return objResult;
            }
        }

        /// <summary>
        /// 批量执行带参数Sql
        /// </summary>
        /// <param name="listSql"></param>
        /// <returns></returns>
        public bool ExecuteSqlTran(Dictionary<string, MySqlParameter[]> listSql)
        {
            using (MySqlConnection sqlConn = new MySqlConnection(ConnectionString))
            {
                MySqlCommand sqlCmd = new MySqlCommand();
                sqlCmd.Connection = sqlConn;
                sqlConn.Open();
                MySqlTransaction sqlTran = sqlConn.BeginTransaction();
                sqlCmd.Transaction = sqlTran;
                bool objResult = false;

                try
                {
                    foreach (string item in listSql.Keys)
                    {
                        sqlCmd.CommandText = item;
                        sqlCmd.Parameters.AddRange(listSql[item]);
                        sqlCmd.ExecuteNonQuery();
                        sqlCmd.Parameters.Clear();
                    }
                    sqlTran.Commit();
                    objResult = true;
                }
                catch(Exception ex)
                {
                    this._errs.Add(ex);
                    sqlTran.Rollback();
                }
                finally
                {
                    sqlConn.Close();
                }
                return objResult;
            }
        }
        #endregion
         
    }

}


