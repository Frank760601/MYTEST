using Dapper;
using DataRetrieve.SqlHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace DataRetrieve.DataAccess
{
    public class InvestmentTrustDA
    {
        private static InvestmentTrustDA uniqueInstance;
        public static InvestmentTrustDA getInstance()
        {
            uniqueInstance = new InvestmentTrustDA();
            return uniqueInstance;
        }
        public IEnumerable<dynamic> GetInvestmentTrust(string NO, string NAME, string IDNO, int index, int PAGESIZE) {
            StringBuilder SQL = new StringBuilder();
            SQL.Append("SELECT 編號=NO, 客戶名稱=NAME, 身份證字號或營利事業統一編號 = IDNO , COUNT(1) OVER() AS TOTALCNT FROM AMD..tblInvestmentTrust WHERE 1=1 ");
            var p = new DynamicParameters();
            if (!string.IsNullOrWhiteSpace(NO))
            {
                SQL.Append(" AND NO = @NO ");
                p.Add("@NO",NO);
            }
            if (!string.IsNullOrWhiteSpace(NAME))
            {
                SQL.Append(" AND NAME like @NAME ");
                p.Add("@NAME", "%"+ NAME + "%");
            }
            if (!string.IsNullOrWhiteSpace(IDNO))
            {
                SQL.Append(" AND IDNO = @IDNO ");
                p.Add("@IDNO", IDNO);
            }
            SQL.Append(" ORDER BY NO ");
            if (PAGESIZE != 0)
            {
                SQL.Append(" OFFSET @index ROWS FETCH NEXT @PAGESIZE ROWS ONLY ");
                p.Add("@index", index);
                p.Add("@PAGESIZE", PAGESIZE);
            }
            ISqlHelper sqlHelper = SQLHelper.GetInstance("Dapper", Configurations.AMDConnectionString);
            
            IEnumerable<dynamic> result = sqlHelper.Query<dynamic>(SQL.ToString(), 1, (dynamic)p);
            sqlHelper.Close();
            return result;
        }
        public bool UpdateInvestmentTrust(string NO, string NAME, string IDNO)
        {
            string SQL = "UPDATE AMD..tblInvestmentTrust SET NAME = @NAME, IDNO = @IDNO WHERE NO = @NO";
            var p = new DynamicParameters();
            p.Add("@NO", NO);
            p.Add("@NAME", NAME);
            p.Add("@IDNO", IDNO);

            ISqlHelper sqlHelper = SQLHelper.GetInstance("Dapper", Configurations.AMDConnectionString);

            sqlHelper.Open();
            bool result = sqlHelper.Execute(SQL, 1, (dynamic)p);
            sqlHelper.Close();

            return result;
        }
        public (bool MsgCode, string MsgName) InsertInvestmentTrust(string NO, string NAME, string IDNO)
        {
            string SQLQuery = " SELECT * FROM AMD..tblInvestmentTrust WHERE NO = @NO ";
            var p = new DynamicParameters();
            p.Add("@NO", NO);
            ISqlHelper sqlHelper = SQLHelper.GetInstance("Dapper", Configurations.AMDConnectionString);
            dynamic result = sqlHelper.Query<dynamic>(SQLQuery.ToString(), 1, (dynamic)p);
            sqlHelper.Close();
            if (((IEnumerable<dynamic>)result).AsList().Count > 0)
            {
                return (false, "失敗(己存在重覆編號資料)");
            }

            string SQL = "INSERT INTO AMD..tblInvestmentTrust VALUES (@NO, @NAME, @IDNO)";
            p = new DynamicParameters();
            p.Add("@NO", NO);
            p.Add("@NAME", NAME);
            p.Add("@IDNO", IDNO);

            sqlHelper = SQLHelper.GetInstance("Dapper", Configurations.AMDConnectionString);

            sqlHelper.Open();
            result = sqlHelper.ExecuteNonQuery(SQL, 1, (dynamic)p);
            sqlHelper.Close();

            return ((bool)result) ? (true, "成功") : (false, "失敗");
        }
        public bool DeleteInvestmentTrust(string NO)
        {
            string SQL = "DELETE FROM AMD..tblInvestmentTrust WHERE NO = @NO";
            var p = new DynamicParameters();
            p.Add("@NO", NO);

            ISqlHelper sqlHelper = SQLHelper.GetInstance("Dapper", Configurations.AMDConnectionString);

            sqlHelper.Open();
            bool result = sqlHelper.ExecuteNonQuery(SQL, 1, (dynamic)p);
            sqlHelper.Close();

            return result;
        }
    }
}
