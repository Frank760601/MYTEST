using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace DataRetrieve.SqlHelper
{
    public interface ISqlHelper
    {
        bool Open();
        bool BeginTransact();
        IEnumerable<T> Query<T>(string strSql, int type, dynamic param);
        SqlMapper.GridReader QueryMultiple(string strSql, int type, dynamic param);
        IDataReader ExecuteReader(string strSql, int type, dynamic param);
        bool ExecuteNonQuery(string strSql, int type, dynamic param);
        bool Execute(string strSql, int type, dynamic param);
        bool Commit();
        bool Rollback();
        bool Close();
    }

    public static class SQLHelper
    {
        public static ISqlHelper GetInstance(string SqlHelper, string Constr)
        {
            switch (SqlHelper)
            {
                case "Dapper":
                    return DapperHelper.getInstance(Constr);
            }
            return DapperHelper.getInstance(Constr);
        }
    }
}
