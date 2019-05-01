using Dapper;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capital.Library;
using DataRetrieve.SqlHelper;
using DataRetrieve.ConfigHelper;

namespace DataRetrieve.ViewComponents
{
    [ViewComponent(Name = "LayoutView")]
    public class LayoutViewComponent:ViewComponent
    {
        private IEmployee _emp;
        public static Configurations _configurations;
        public LayoutViewComponent(IEmployee employee, Configurations configurations) {
            _emp = employee;
            _configurations = configurations;
        }
        public async Task<HtmlString> InvokeAsync()
        {
            var sb = new StringBuilder();
            int BaseCode = 4034;
            //var data = GetMenu(BaseCode);
            //sb = GenMenuMap(sb, data, BaseCode, BaseCode);
            return new HtmlString(@"<li class='nav-item'>
            <a href='javascript:;' class='nav-link nav-toggle'>
            <i class='fa fa-university'></i>
            <span class='title'>證券市場</span><span class='arrow'></span>
            </a>
            <ul class='sub-menu'>
            <li class='nav-item'><a href='/InvestmentTrust/Index' class='nav-link'> 證券團體戶折讓</a></li></ul>
            </li>"); //Return Default.cshtml
        }

        private IQueryable<MenuMapModel> GetMenu(int BaseCode) {
            var result = new StringBuilder();

            string query = @"
                            SELECT A.SiteMapName, A.SiteMapURL,A.SiteMapID,A.ParentSiteMapID,A.ProgramID
                            FROM    ( SELECT    *
                                      FROM      SYSDATA..SMSiteMap WITH ( NOLOCK )
                                      WHERE     (ISNULL(DisplayMode, 0) = 0 OR DisplayMode = @DisplayMode)
                                                AND LevelTree LIKE ';'+CAST(@BaseCode AS VARCHAR)+';%'
					                            AND LEN(LevelOrder) >= 8
                                    ) A
                            INNER JOIN SYSDATA..SMPermissionCache B WITH ( NOLOCK ) ON A.ProgramID = B.ProgramID
                            AND B.EmpNo = @EmpNo
                            ORDER BY A.LevelOrder";
            var p = new DynamicParameters();
            p.Add("@EmpNo", _emp.EmpNo);
            p.Add("@DisplayMode", (Environment.MachineName.Contains("SRVCRMWEBP")) ? 1 : 2);
            p.Add("@BaseCode", BaseCode);
            ISqlHelper sqlHelper  = SQLHelper.GetInstance("Dapper", _configurations.SYSDATAConnectionString);
            var data = sqlHelper.Query<MenuMapModel>(query, 1, p).AsQueryable();
            sqlHelper.Close();
            return data;
        }

        /// <summary>
        /// 產生Menu的HTML結構
        /// </summary>
        /// <param name="result">HTML結果</param>
        /// <param name="data">選單資料</param>
        /// <param name="SiteMapID">功能代號</param>
        /// <param name="FirstSiteMapID">第一層功能代號</param>
        /// <returns></returns>
        private StringBuilder GenMenuMap(StringBuilder result, IQueryable<MenuMapModel> data, int SiteMapID, int FirstSiteMapID)
        {
            if (data.Where(o => o.ParentSiteMapID == SiteMapID).Count() <= 0)//如果沒權限則不動態產生Menu
                return result;
            else
            {
                foreach (var d in data.Where(o => o.ParentSiteMapID == SiteMapID))
                {
                    if (d.ParentSiteMapID == FirstSiteMapID && data.Where(o => o.ParentSiteMapID == d.SiteMapID).Count() > 0)//第一層的HTML寫法比較特別，故先判斷
                    {
                        result.AppendLine($@"
                                             <li class='nav-item'>
                                                 <a href='javascript:;' class='nav-link nav-toggle'>
                                                    <i class='{d.SiteMapURL}'></i>
                                                    <span class='title'>{d.SiteMapName}</span>
                                                    <span class='arrow'></span>
                                                 </a>
                                                 <ul class='sub-menu'>
                                                     {GenMenuMap(new StringBuilder(), data, d.SiteMapID, FirstSiteMapID)}
                                                 </ul>
                                             </li>
                                        ");
                    }
                    else if (d.ParentSiteMapID == FirstSiteMapID)
                    {
                        result.AppendLine($@"<li class='nav-item'><a href='javascript:;' class='nav-link'><i class='{d.SiteMapURL}'></i><span class='title'>{d.SiteMapName}</span></a></li>");
                    }
                    else if (d.ParentSiteMapID == SiteMapID && data.Where(o => o.ParentSiteMapID == d.SiteMapID).Count() > 0)//只要有子項就跑此HTML
                    {
                        result.AppendLine($@"
                                         <li class='nav-item'>
                                            <a href='javascript:;' class='nav-link nav-toggle'>
                                                <span class='title'>{d.SiteMapName}</span>
                                                <span class='arrow'></span>
                                            </a>
                                            <ul class='sub-menu'>
                                                    {GenMenuMap(new StringBuilder(), data, d.SiteMapID, FirstSiteMapID)}
                                            </ul>
                                        </li>
                                        ");
                    }
                    else if (d.ParentSiteMapID == SiteMapID && data.Where(o => o.ParentSiteMapID == d.SiteMapID).Count() <= 0)//沒有子項則直接提供Link
                        result.AppendLine($@"<li class='nav-item'><a href='{d.SiteMapURL}' class='nav-link'> {d.SiteMapName}</a></li>");
                }
                return result;
            }
        }

        /// <summary>
        /// 動態產生Menu專用Model
        /// </summary>
        private class MenuMapModel
        {
            public string SiteMapName { get; set; }
            public string SiteMapURL { get; set; }
            public int SiteMapID { get; set; }
            public int ParentSiteMapID { get; set; }
            public int ProgramID { get; set; }
        }
    }

    [ViewComponent(Name = "LayoutViewMap")]
    public class LayoutViewMapComponent : ViewComponent
    {
        private IEmployee _emp;
        public static Configurations _configurations;
        public LayoutViewMapComponent(IEmployee employee, Configurations configurations)
        {
            _emp = employee;
            _configurations = configurations;
        }
        public async Task<HtmlString> InvokeAsync(string Url)
        {
            var sb = new StringBuilder();
            int BaseCode = 4034;
            var data = GetMenu(Url);
            sb = GenMenuMap(sb, data, BaseCode, BaseCode);
            return new HtmlString(sb.ToString()); //Return Default.cshtml
        }

        private List<MenuMapModel> GetMenu(string Url)
        {
            var MainUrl = Url.Substring(0, Url.LastIndexOf("/"));
            var result = new StringBuilder();
            var ListSiteMap = new List<MenuMapModel>();
            string query = @"
                            IF EXISTS(SELECT LevelTree FROM SYSDATA..SMSiteMap WITH(NOLOCK) WHERE LevelTree like '%4034%' AND SiteMapURL like @SiteMapURL) 
                            BEGIN 
	                            SELECT LevelTree FROM SYSDATA..SMSiteMap WITH(NOLOCK) WHERE LevelTree like '%4034%' AND SiteMapURL like @SiteMapURL
                            END 
                            ELSE 
                            BEGIN 
	                            SELECT LevelTree FROM SYSDATA..SMSiteMap WITH(NOLOCK) WHERE LevelTree like '%4034%' AND SiteMapURL like @SiteMapMainUrl
                            END";
            var p = new DynamicParameters();
            p.Add("@SiteMapURL", "%"+Url+"%");
            p.Add("@SiteMapMainUrl", "%" + MainUrl + "%");
            ISqlHelper sqlHelper = SQLHelper.GetInstance("Dapper", _configurations.SYSDATAConnectionString);
            var data = sqlHelper.Query<string>(query, 1, p).FirstOrDefault();

            var ListNode = data.Trim(';').Split(";");
            ListNode = ListNode.Skip<string>(1).ToArray();
            foreach (var SiteMapID in ListNode) {
                query = @"
                        SELECT SiteMapName,SiteMapURL,SiteMapID
                        FROM SYSDATA..SMSiteMap WITH(NOLOCK)
                        WHERE SiteMapID = @SiteMapID";
                p = new DynamicParameters();
                p.Add("@SiteMapID", SiteMapID);
                var model = sqlHelper.Query<MenuMapModel>(query, 1, p).FirstOrDefault();

                ListSiteMap.Add(model);
            }
            sqlHelper.Close();
            return ListSiteMap;
        }

        /// <summary>
        /// 產生Menu的HTML結構
        /// </summary>
        /// <param name="result">HTML結果</param>
        /// <param name="data">選單資料</param>
        /// <param name="SiteMapID">功能代號</param>
        /// <param name="FirstSiteMapID">第一層功能代號</param>
        /// <returns></returns>
        private StringBuilder GenMenuMap(StringBuilder result, List<MenuMapModel> data, int SiteMapID, int FirstSiteMapID)
        {
            for(int i = 0; i < data.Count; i++) {
                if (i == data.Count-1) {
                    if (data[i].SiteMapURL.IndexOf("/") >= 0)
                        result.AppendLine($@"<li><a href='{data[i].SiteMapURL}'>{data[i].SiteMapName}</a></li>");
                    else
                        result.AppendLine($@"<li><span>{data[i].SiteMapName}</span></li>");
                }
                else {
                    if (data[i].SiteMapURL.IndexOf("/") >=0)
                        result.AppendLine($@"<li><a href='{data[i].SiteMapURL}'>{data[i].SiteMapName}</a><i class='fa fa-angle-double-right'></i></li>");
                    else
                        result.AppendLine($@"<li><span>{data[i].SiteMapName}</span><i class='fa fa-angle-double-right'></i></li>");
                }
               
            }

            return result;
        }

        /// <summary>
        /// 動態產生Menu專用Model
        /// </summary>
        private class MenuMapModel
        {
            public string SiteMapName { get; set; }
            public string SiteMapURL { get; set; }
            public int SiteMapID { get; set; }
        }
    }
}

