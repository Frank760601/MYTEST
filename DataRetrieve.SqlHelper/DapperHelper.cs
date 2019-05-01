using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRetrieve.SqlHelper
{
    public class DapperHelper : ISqlHelper
    {
        private string _constr { get; set; }
        private SqlConnection _con { get; set; }
        private SqlTransaction _trans { get; set; }
        private static DapperHelper uniqueInstance;
        
        public DapperHelper(string constr) {
            _constr = constr;
        }
        public static DapperHelper getInstance(string constr)
        {
            uniqueInstance = new DapperHelper(constr);
            return uniqueInstance;
        }
        public bool Open() {
            try {
                if (_constr == null)
                    return false;

                _con = new SqlConnection(_constr);
                if (_con == null)
                    return false;

                _con.Open();
                return true;
            }
            catch (SqlException ex) {
                return false;
            }
        }
        public bool BeginTransact() {
            try
            {
                if (_con == null)
                    return false;

                _trans = _con.BeginTransaction();
                return true;
            }
            catch (Exception ex) {
                return false;
            }
        }
        /// <summary>
        /// SQL查詢
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SQL"></param>
        /// <param name="type">1:SQL語法; 4:Stored procedure</param>
        /// <param name="param"></param>
        /// <returns></returns>
        public IEnumerable<T> Query<T>(string SQL, int type, dynamic param)
        {
            try
            {
                if (_constr == null)
                    return Enumerable.Empty<T>();

                _con = new SqlConnection(_constr);

                if (_con == null)
                    return Enumerable.Empty<T>();

                _con.Open();

                var Result = _con.Query<T>(SQL, (object)param,commandTimeout:0, commandType: (CommandType)type);
                
                return Result;
                
            }
            catch (Exception ex)
            {
                return Enumerable.Empty<T>();
            }
        }
        /// <summary>
        /// 多合一SQL查詢
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SQL"></param>
        /// <param name="type">1:SQL語法; 4:Stored procedure</param>
        /// <param name="param"></param>
        /// <returns></returns>
        public SqlMapper.GridReader QueryMultiple(string SQL, int type, dynamic param)
        {
            try
            {
                if (_constr == null)
                    return null;

                _con = new SqlConnection(_constr);

                if (_con == null)
                    return null;

                _con.Open();

                var Result = _con.QueryMultiple(SQL, (object)param, commandTimeout: 0, commandType: (CommandType)type);
                
                return Result;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public IDataReader ExecuteReader(string SQL, int type, dynamic param) {
            try
            {
                if (_constr == null)
                    return null;

                _con = new SqlConnection(_constr);

                if (_con == null)
                    return null;

                _con.Open();

                IDataReader Result = _con.ExecuteReader(SQL, (object)param, commandType: (CommandType)type);
                
                return Result;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// 單一SQL執行
        /// </summary>
        /// <param name="SQL"></param>
        /// <param name="type">1:SQL語法; 4:Stored procedure</param>
        /// <param name="param"></param>
        /// <returns></returns>
        public bool ExecuteNonQuery(string SQL, int type, dynamic param)
        {
            try
            {
                SqlTransaction trans = _con.BeginTransaction();
                
                try
                {
                    var Result = _con.Execute(SQL, (object)param, trans, commandType: (CommandType)type);
                    trans.Commit();
                    trans.Dispose();
                    trans = null;
                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    trans.Dispose();
                    trans = null;
                    return false;
                }
                
            }
            catch(Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 配合Transaction執行
        /// </summary>
        /// <param name="SQL"></param>
        /// <param name="type">1:SQL語法; 4:Stored procedure</param>
        /// <param name="param"></param>
        /// <returns></returns>
        public bool Execute(string SQL, int type, dynamic param)
        {
            try
            {
                if (_con == null)
                    return false;
                
                try
                {
                    var Result = _con.Execute(SQL, (object)param, transaction:_trans, commandType: (CommandType)type);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }                
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Commit()
        {
            try
            {
                if (_trans == null)
                    return false;

                _trans.Commit();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool Rollback()
        {
            try
            {
                if (_trans == null)
                    return false;

                _trans.Rollback();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Close() {
            if (_trans != null || _con != null) {
                if (_trans != null) {
                    _trans.Dispose();
                    _trans = null;
                }
                if (_con != null) {
                    _con.Close();
                    _con = null;
                }
                return true;
            }
            return false;
        }
    }
}
