using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DataRetrieve.Models;
using Microsoft.Extensions.Logging;
using DataRetrieve.LogHelper;
using DataRetrieve.SqlHelper;
using DataRetrieve.ConfigHelper;
using Dapper;
using Microsoft.Extensions.Options;
using DataRetrieve.DataAccess;
using DataRetrieve.BusinessLogic;
using System.Dynamic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Capital.Library;
using System.Data.SqlClient;

namespace DataRetrieve.Controllers
{
    public class HomeController : IController
    {
        ILog _log = ILogHelper.GetLog("Log4Net");
        public IHostingEnvironment _hostingEnvironment;
        public IEmployee _emp;
        public HomeController(IEmployee employee, IHostingEnvironment hostingEnvironment) : base(employee, hostingEnvironment)
        {
            _emp = employee;
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index()
        {
            //abc();
            //_logger.LogDebug("ad帳號：");
            _log.Debug("HomeController:"+_emp.RoleCode);
            dynamic a = new ExpandoObject();
            return View(a);
        }
        //public async Task abc() {
        //    await Task.Delay(60000);
        //    SqlConnection _con = new SqlConnection(_configurations.DSSConnectionString);
        //    var Result = _con.Execute(@"INSERT INTO SYSDATA.dbo.tblScheduleMail (ProgCode, ExpectSendTime, Recipient, CC, BCC, Sender, Subject,BodyType,Body, URL, SendSpeed,Charset)  
        //    SELECT ProgCode='AutoReply'
        //    , ExpectSendTime= DateAdd( MI ,0, getDate()) 
        //    , Recipient='A97358@capital.com.tw;A98046@capital.com.tw;'
        //    , CC=''
        //    , BCC=''
        //    , Sender='SQLMail@capital.com.tw'
        //    , Subject='收盤後自動轉檔(威旭)'
        //    , BodyType=0
        //    , Body = '123'
        //    , URL=''
        //    , SendSpeed='S',Charset='utf-8'");
        //} 

        [ValidateAntiForgeryToken]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> subIndex() {
            IFormCollection form = await HttpContext.Request.ReadFormAsync();
            string pageStock = form["pageStock"];
            return new EmptyResult();
        }

        public class ModelTest {
            public string I_UNIT { get; set; }
            public string I_ACNO { get; set; }
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
