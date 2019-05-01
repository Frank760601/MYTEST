using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Principal;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using Dapper;
using System.Web;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http.Features;
using DataRetrieve.LogHelper;

namespace Capital.Library
{
    public interface IEmployee
    {
        string EmpNo { get; }
        string EmpName { get; }
        string DeptCode { get; }
        string DeptName { get; }
        string JobCode { get; }
        string JobName { get; }
        string RoleCode { get; }
        string RoleName { get; }
        string Unit { get; }
        string UnitOut { get; }
        string UnitName { get; }
        string PositionCode { get; }
        string PositionName { get; }
        string ClientIP { get; }
        bool IsSimulation { get; }
        List<string> ISRegion { get; }
        string ISRegionJSON { get; }
        string SellerNo { get; }
        int MsgCount { get; }
        void SetUserSession(string SimulationEmpNo = null);
        //void SetUserSession(string SimulationEmpNo = null);
        bool IsSimulationEmpNo(string EmpNo);
        bool CheckEmpNoExists(string EmpNo);
        bool IsInRegion(string unit);
        bool IsRole(string RoleGUID);
        bool CheckProgramAuthority(int ProgramID);
        bool CheckProgramAuthority(string EmpNo, int ProgramID);
    }
    

    /// <summary>
    /// 登入者資料和權限相關類別
    /// 相關Session由Global.asax初始化
    /// </summary>
    public class Employee: IEmployee
    {
        private ILogger _logger;
        private static  IConfigurationRoot Configuration { get; set; }

        private IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        ILog _log = ILogHelper.GetLog("Log4Net");

        DBTool _DBTool = new DBTool();

        public Employee(IHttpContextAccessor httpContextAccessor, ILogger<IEmployee> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            var a = 1;
        }

        /// <summary>
        /// 員編
        /// </summary>
        public string EmpNo { get { return string.IsNullOrWhiteSpace(_session.GetString("EmpNo")) ? "" : _session.GetString("EmpNo").ToString(); } }

        /// <summary>
        /// 員工姓名
        /// </summary>
        public string EmpName { get { return string.IsNullOrWhiteSpace(_session.GetString("EmpName")) ? "" : _session.GetString("EmpName").ToString(); } }

        /// <summary>
        /// 部門代碼
        /// </summary>
        public string DeptCode { get { return string.IsNullOrWhiteSpace(_session.GetString("DeptCode")) ? "" : _session.GetString("DeptCode").ToString(); } }

        /// <summary>
        /// 部門名稱
        /// </summary>
        public string DeptName { get { return string.IsNullOrWhiteSpace(_session.GetString("DeptName")) ? "" : _session.GetString("DeptName").ToString();  } }

        /// <summary>
        /// 部門代碼
        /// </summary>
        public string JobCode { get { return string.IsNullOrWhiteSpace(_session.GetString("JobCode")) ? "" : _session.GetString("JobCode").ToString();  } }

        /// <summary>
        /// 部門名稱
        /// </summary>
        public string JobName { get { return string.IsNullOrWhiteSpace(_session.GetString("JobName")) ? "" : _session.GetString("JobName").ToString();} }

        /// <summary>
        /// 角色代碼(舊版CRM)
        /// http://crm.capital.com.tw/CRM/SMSiteMap/SMRole/List.aspx
        /// </summary>
        public string RoleCode { get { return string.IsNullOrWhiteSpace(_session.GetString("RoleCode")) ? "" : _session.GetString("RoleCode").ToString(); } }

        /// <summary>
        /// 角色名稱
        /// </summary>
        public string RoleName { get { return string.IsNullOrWhiteSpace(_session.GetString("RoleName")) ? "" : _session.GetString("RoleName").ToString(); } }

        /// <summary>
        /// 單位(內)
        /// </summary>
        public string Unit { get { return string.IsNullOrWhiteSpace(_session.GetString("Unit")) ? "" : _session.GetString("Unit").ToString(); } }

        /// <summary>
        /// 單位(外)
        /// </summary>
        public string UnitOut { get { return string.IsNullOrWhiteSpace(_session.GetString("UnitOut")) ? "" : _session.GetString("UnitOut").ToString();} }
        /// <summary>
        /// 單位名稱
        /// </summary>
        public string UnitName { get { return string.IsNullOrWhiteSpace(_session.GetString("UnitName")) ? "" : _session.GetString("UnitName").ToString(); } }

        /// <summary>
        /// 職位代碼(三管是11、12)
        /// </summary>
        public string PositionCode { get { return string.IsNullOrWhiteSpace(_session.GetString("PositionCode")) ? "" : _session.GetString("PositionCode").ToString(); } }

        /// <summary>
        /// 職位名稱
        /// </summary>
        public string PositionName { get { return string.IsNullOrWhiteSpace(_session.GetString("PositionName")) ? "" : _session.GetString("PositionName").ToString(); } }

        /// <summary>
        /// 使用者IP
        /// </summary>
        public string ClientIP { get { return string.IsNullOrWhiteSpace(_session.GetString("ClientIP")) ?"":_session.GetString("ClientIP").ToString(); } }

        /// <summary>
        /// 是否為模擬登入
        /// </summary>
        public bool IsSimulation { get { return BitConverter.ToBoolean(_session.Get("IsSimulation"),0); } }

        /// <summary>
        /// 可視單位
        /// </summary>
        public List<string> ISRegion { get { return JsonConvert.DeserializeObject<List<string>>(_session.GetString("ISRegion").ToString()); } }

        public string ISRegionJSON { get { return  _session.GetString("ISRegion").ToString(); } }

        /// <summary>
        /// 營業員編號
        /// </summary>
        public string SellerNo { get { return _session.GetString("SellerNo").ToString(); } }

        /// <summary>
        /// 設定User資料Session
        /// </summary>
        /// <param name="SimulationEmpNo ">員編，模擬登入時傳入</param>
        public void SetUserSession(string SimulationEmpNo = null)
        {
            try
            {

                var LoginAccount = _httpContextAccessor.HttpContext.User.Identity.Name;
                //string LoginAccount = WindowsIdentity.GetCurrent().Name.ToUpper();
                if (LoginAccount.IndexOf(@"\A") > 0)
                {
                    string EmpNo = !string.IsNullOrWhiteSpace(SimulationEmpNo) ? SimulationEmpNo : LoginAccount.Substring(LoginAccount.IndexOf(@"\A") + 2, 5); // CAPITAL\A96973
                    //_logger.LogError("ad帳號："+LoginAccount);
                    var p = new DynamicParameters();
                    p.Add("@EmpNo", EmpNo);

                    List<EmployeeModel> list = _DBTool.GetDataTable<EmployeeModel>(@"SELECT A.EmpNo,A.EmpName,A.DeptCode,A.DeptName,A.JobCode,A.JobName,B.RoleCode,C.RoleName,B.Unit,D.UnitName ,A.PositionCode,A.PositionName,UnitOut = ISNULL(E.UnitOut,D.UnitID)
                FROM SYSDATA..tblEmployee A WITH(NOLOCK)
                LEFT JOIN SYSDATA..tblRoleMember B WITH(NOLOCK) ON A.EmpNo = B.EmpNo
                LEFT JOIN SYSDATA..tblRole C WITH(NOLOCK) ON B.RoleCode = C.RoleCode
                LEFT JOIN AMD..tblCRMUnit D WITH(NOLOCK) ON B.Unit=D.UnitID
                LEFT JOIN AMD..tblUnitOut2UnitIn E WITH(NOLOCK) ON D.UnitID = E.UnitIn
                WHERE A.EmpNo = @EmpNo ", p);
                    //_logger.LogError("筆數："+list.Count.ToString());
                    if (list.Count > 0)
                    {
                        //_logger.LogError("area1");
                        _session.SetString("EmpNo", EmpNo);
                        _session.SetString("EmpName", !string.IsNullOrWhiteSpace(list[0].EmpName) ? list[0].EmpName.ToString():string.Empty);

                        var a = _session.GetString("EmpNo");
                        _session.SetString("DeptCode", !string.IsNullOrWhiteSpace(list[0].DeptCode) ? list[0].DeptCode.ToString():string.Empty);
                        _session.SetString("DeptName", !string.IsNullOrWhiteSpace(list[0].DeptName) ? list[0].DeptName.ToString():string.Empty);
                        _session.SetString("JobCode", !string.IsNullOrWhiteSpace(list[0].JobCode) ? list[0].JobCode.ToString():string.Empty);
                        _session.SetString("JobName", !string.IsNullOrWhiteSpace(list[0].JobName) ? list[0].JobName.ToString():string.Empty);
                        _session.SetString("RoleCode", !string.IsNullOrWhiteSpace(list[0].RoleCode) ? list[0].RoleCode.ToString():string.Empty);
                        _session.SetString("RoleName", !string.IsNullOrWhiteSpace(list[0].RoleName) ? list[0].RoleName.ToString():string.Empty);
                        if (/*Settings.IsDev && */IsSimulationEmpNo(EmpNo))//20161215 97748 驗證是否有測試環境模擬登入名單
                            _session.SetString("RoleCode", "F");
                        _session.SetString("Unit", list[0].Unit.ToString());
                        _session.SetString("UnitOut", list[0].UnitOut.ToString());
                        _session.SetString("UnitName", !string.IsNullOrWhiteSpace(list[0].UnitName) ? list[0].UnitName.ToString():string.Empty);
                        _session.SetString("PositionCode", !string.IsNullOrWhiteSpace(list[0].PositionCode)?list[0].PositionCode.ToString():string.Empty);
                        _session.SetString("PositionName", !string.IsNullOrWhiteSpace(list[0].PositionName)?list[0].PositionName.ToString():string.Empty);
                        _session.SetString("ClientIP", _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString() == "::1" ? "127.0.0.1" : _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString());
                        _session.Set("IsSimulation", (!string.IsNullOrWhiteSpace(SimulationEmpNo) ? BitConverter.GetBytes(true) : BitConverter.GetBytes(false)));//是否為模擬登入
                        p = new DynamicParameters();
                        p.Add("@EmpNo", EmpNo);
                        List<UNITModel> List_ISRegion = _DBTool.GetDataTable<UNITModel>(@"EXEC STOCK.dbo.uspGetISRegion @EmpNo,'',3,0,'001'", p);
                        List<string> list_ISRegion = new List<string>();
                        for (int i = 0; i < List_ISRegion.Count; i++)
                        {
                            list_ISRegion.Add(Convert.ToString(List_ISRegion[i].UNIT));
                        }
                        _session.SetString("ISRegion", JsonConvert.SerializeObject(list_ISRegion));//可視單位
                        if (RoleCode == "S" || RoleCode == "M" || RoleCode == "B")//如果是營業員、分公司經理人、櫃管 取得營業員編號
                        {
                            var sellerNo = _DBTool.GetScalar(@"SELECT SE_SELLER_NO, MAX(SE_D_YMD) FROM EMR.dbo.SELLER WITH(NOLOCK) WHERE SE_UNIT = @Unit AND SE_EMP_NO = @EmpNo GROUP BY SE_SELLER_NO  ", new SqlParameter[] { new SqlParameter("Unit", Unit), new SqlParameter("EmpNo", EmpNo) });
                            _session.SetString("SellerNo", sellerNo == null ? "" : Convert.ToString(sellerNo));
                        }
                        else
                        {
                            _session.SetString("SellerNo", "");
                        }
                        _log.Debug("Employee："+EmpName);
                    }
                    else
                    {
                        //_logger.LogError("area2");
                        _session.SetString("EmpNo", LoginAccount);
                        _session.SetString("EmpName", "");
                        _session.SetString("DeptCode", "");
                        _session.SetString("DeptName", "");
                        _session.SetString("RoleCode", "");
                        _session.SetString("RoleName", "");
                        _session.SetString("Unit", "");
                        _session.SetString("UnitOut", "");
                        _session.SetString("UnitName", "");
                        _session.SetString("PositionCode", "");
                        _session.SetString("PositionName", "");
                        //_session.SetString("ClientIP", EmpNo);
                        _session.Set("IsSimulation", BitConverter.GetBytes(false));
                        _session.SetString("ISRegion", "");
                        _session.SetString("SellerNo", "");
                    }
                }
                else
                {
                    //_logger.LogError("area3");
                    _session.SetString("EmpNo", LoginAccount);
                    _session.SetString("EmpName", "");
                    _session.SetString("DeptCode", "");
                    _session.SetString("DeptName", "");
                    _session.SetString("JobCode", "");
                    _session.SetString("JobName", "");
                    _session.SetString("RoleCode", "");
                    _session.SetString("RoleName", "");
                    _session.SetString("Unit", "");
                    _session.SetString("UnitOut", "");
                    _session.SetString("UnitName", "");
                    _session.SetString("PositionCode", "");
                    _session.SetString("PositionName", "");
                    //_session.SetString("ClientIP", EmpNo);
                    _session.Set("IsSimulation", BitConverter.GetBytes(false));
                    _session.SetString("ISRegion", "");
                    _session.SetString("SellerNo", "");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace, "寫入ad帳號發生錯誤StackTrace");
                _logger.LogError(ex.Message, "寫入ad帳號發生錯誤Message");
                //Clog.Writer.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 檢查使用者是否有這個功能的權限
        /// http://crm-dev.capital.com.tw/CRM/SMSiteMap/list.aspx?ParentID=3148
        /// </summary>
        /// <param name="EmpNo">員編</param>
        /// <param name="ProgramID">功能編號</param>
        /// <returns></returns>
        public bool CheckProgramAuthority(string EmpNo, int ProgramID)
        {
            string query = @"IF EXISTS (SELECT * FROM SYSDATA..SMPermissionCache WITH(NOLOCK) WHERE EmpNo= @EmpNo AND  ProgramID = @ProgramID ) SELECT 'True' ELSE SELECT 'False'";
            bool result = Convert.ToBoolean(_DBTool.GetScalar(query, new SqlParameter[] { new SqlParameter("EmpNo", EmpNo), new SqlParameter("ProgramID", ProgramID) }));
            return result;
        }

        /// <summary>
        /// 檢查目前登入者是否有這個功能的權限
        /// </summary>
        /// <param name="ProgramID"></param>
        /// <returns></returns>
        public bool CheckProgramAuthority(int ProgramID)
        {
            string query = @"IF EXISTS (SELECT * FROM SYSDATA..SMPermissionCache WITH(NOLOCK) WHERE EmpNo= @EmpNo AND  ProgramID = @ProgramID ) SELECT 'True' ELSE SELECT 'False'";
            bool result = Convert.ToBoolean(_DBTool.GetScalar(query, new SqlParameter[] { new SqlParameter("EmpNo", EmpNo), new SqlParameter("ProgramID", ProgramID) }));
            return result;
        }

        /// <summary>
        /// 取得登入者是否用新版CRM權限(RoleGUID)
        /// 
        /// 參考:
        /// http://crm.capital.com.tw/CRM/SMSiteMap/SMRole/List.aspx</summary>
        /// 
        /// SELECT A.*,B.RoleName FROM dbo.SMEmployeeRoleRel A
        /// LEFT JOIN dbo.[SMRole] B ON A.RoleGUID = B.RoleGUID
        /// WHERE A.EmpNo = 96973
        /// 
        /// <param name="RoleGUID"></param>
        /// <returns></returns>
        public bool IsRole(string RoleGUID)
        {
            return Convert.ToBoolean(
                _DBTool.GetScalar(@"IF EXISTS(SELECT 1 FROM SYSDATA.dbo.SMEmployeeRoleRel WITH(NOLOCK) WHERE EmpNo = @EmpNo AND RoleGUID = @RoleGUID) SELECT 'True' ELSE SELECT 'False'",
                new SqlParameter[] {
                    new SqlParameter("EmpNo", EmpNo),
                    new SqlParameter("RoleGUID",new Guid(RoleGUID))
                }));
        }

        /// <summary>
        /// 檢查使用者是否為主管(所屬)單位
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public bool IsInRegion(string unit)
        {
            return ISRegion.Contains(unit);
        }

        /// <summary>
        /// 訊息數量，畫面頂端列用(Views/Shared/_Header.cshtml)
        /// 目前只回傳本日應互動客戶數量，未來應為共用Table...
        /// </summary>
        public int MsgCount
        {
            get
            {
                int count = (int)_DBTool.GetScalar(@"SELECT (SELECT COUNT(DISTINCT SELLER_NO) AS cnt FROM [DSS].[dbo].[tblCRM_ActCust] WITH(NOLOCK) WHERE UNIT_IN=@UNIT AND SELLER_NO = @SELLER AND IsAssign = 1 AND IsClose=0 AND IsKYC =0) + 
(SELECT COUNT(1) FROM tblCRM_JobCalendar WITH(NOLOCK) WHERE IsDel='N' AND GETDATE() BETWEEN DATEADD(MINUTE,-30,startdate) AND enddate  AND AssistEmp LIKE '%'+@EmpNo+'%') ",
                    new SqlParameter[] {
                    new SqlParameter("UNIT",Unit),
                    new SqlParameter("SELLER",SellerNo),
                    new SqlParameter("EmpNo",EmpNo),
                    });
                return count;
            }
        }

        /// <summary>
        /// 檢查員編是否存在
        /// </summary>
        /// <param name="EmpNo"></param>
        /// <returns></returns>
        public bool CheckEmpNoExists(string EmpNo)
        {
            return Convert.ToBoolean(_DBTool.GetScalar(@"IF EXISTS (SELECT 1 FROM SYSDATA.dbo.tblEmployee WITH(NOLOCK) WHERE EmpNo = @EmpNo) SELECT 'True' ELSE SELECT 'False'", new SqlParameter[] { new SqlParameter("EmpNo", EmpNo) }));
        }



        /// <summary>
        /// 當登入人員非資訊人員時，查詢是否有模擬登入權限
        /// </summary>
        /// <param name="EmpNo">要驗證的員工編號</param>
        /// <returns></returns>
        public bool IsSimulationEmpNo(string EmpNo)
        {

            List<string> lstEmpNo = new List<string>();
            lstEmpNo.AddRange(new string[] { "95444", "91465", "95270", "97093", "97591" });//經管測試人員
            lstEmpNo.Add("97143");//20161215 97748 新增吳東霖、測試靜止戶活化作業
            return lstEmpNo.Contains(EmpNo); ;
        }

    }

    public class EmployeeModel
    {
        public String EmpNo { get; set; }
        public String EmpName{ get; set; }
        public String DeptCode { get; set; }
        public String DeptName { get; set; }
        public String JobCode { get; set; }
        public String JobName { get; set; }
        public String RoleCode { get; set; }
        public String RoleName { get; set; }
        public int Unit{ get; set; }
        public String UnitName { get; set; }
        public String PositionCode { get; set; }
        public String PositionName { get; set; }
        public int UnitOut{ get; set; }
    }

    public class UNITModel
    {
        public int UNIT { get; set; }
    }
}