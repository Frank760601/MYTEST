using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace Capital.Library
{

    public class DBTool
    {
        private static IConfigurationRoot Configuration { get; set; }

        public DBTool()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
        }

        /// <summary>
        /// 執行SQL QUERY 傳回單一值
        /// </summary>
        /// <param name="query"></param>
        /// <param name="_param"></param>
        /// <returns></returns>
        public object GetScalar(string query, string ConStrName = FieldName.connectionString_DSS, params string[] _param)
        {
            
            string ConnectionString = !string.IsNullOrWhiteSpace(ConStrName)? Configuration[$"connectionStrings:{ConStrName}"]: Configuration["connectionStrings:DSS"];
            int timeout = 3 * 60;
            string _query = "";
            if (_param == null || _param.Length == 0)
                _query = query;
            else
                _query = string.Format(query, _param);
            try
            {
                object _return = null;
                using (SqlConnection sqlcon = new SqlConnection(ConnectionString))
                {
                    sqlcon.Open();
                    SqlCommand sqlcmd = new SqlCommand(_query, sqlcon);
                    sqlcmd.CommandTimeout = timeout;
                    _return = sqlcmd.ExecuteScalar();
                    sqlcon.Close();
                }
                return _return;
            }
            catch (System.Exception ex)
            {
                //Clog.Writer.Error(ex, "USER:{0} {1}", Employee.EmpNo, _query);
                throw new ArgumentNullException(ex.ToString());
            }
        }

        /// <summary>
        /// 執行SQL QUERY 傳回單一值
        /// </summary>
        /// <param name="query"></param>
        /// <param name="_param">查詢參數</param>
        /// <returns></returns>
        public object GetScalar(string query, SqlParameter[] parameter, string ConStrName = FieldName.connectionString_DSS)
        {
            string ConnectionString = !string.IsNullOrWhiteSpace(ConStrName) ? Configuration[$"connectionStrings:{ConStrName}"] : Configuration["connectionStrings:DSS"];
            int timeout = 3 * 60;

            object _return = null;
            try
            {
                
                using (SqlConnection sqlcon = new SqlConnection(ConnectionString))
                {
                    sqlcon.Open();
                    SqlCommand sqlcmd = new SqlCommand(query, sqlcon);
                    sqlcmd.Parameters.AddRange(parameter);
                    sqlcmd.CommandTimeout = timeout;
                    _return = sqlcmd.ExecuteScalar();
                    sqlcmd.Parameters.Clear();
                    sqlcon.Close();
                }

            }
            catch (System.Exception ex)
            {
                //Clog.Writer.Error(ex, "USER:{0} {1} {2}", Employee.EmpNo, GetSqlParameterContent(parameter), query);
            }
            return _return;
        }

        /// <summary>
        /// 由SQL QUERY 取得DataTable
        /// </summary>
        /// <param name="query">SQL Query</param>
        /// <param name="parameter">查詢參數</param>
        /// <returns></returns>
        public List<T> GetDataTable<T>(string query, dynamic param, string ConStrName = FieldName.connectionString_DSS)
        {
            string ConnectionString = !string.IsNullOrWhiteSpace(ConStrName) ? Configuration[$"connectionStrings:{ConStrName}"] : Configuration["connectionStrings:DSS"];
            int timeout = 3 * 60;
            try
            {
                using (var cn = new SqlConnection(ConnectionString))
                {
                    cn.Open();
                    var Result = cn.Query<T>(query, (object)param);
                    return Result.ToList();
                };
            }
            catch (System.Exception ex)
            {
                //Clog.Writer.Error(ex, "USER:{0} {1} {2}", Employee.EmpNo, GetSqlParameterContent(parameter), query);
                throw new ArgumentNullException(ex.ToString());
            }
        }
    }
}
