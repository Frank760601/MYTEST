using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Dapper;
using System.Net;
using System.Collections.Specialized;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Capital.Library;
using Microsoft.AspNetCore.Hosting;

namespace DataRetrieve.Controllers
{
    public class SimulateController : IController
    {
        private IHostingEnvironment _hostingEnvironment;
        private IEmployee _emp;
        public SimulateController(IEmployee employee, IHostingEnvironment hostingEnvironment) : base(employee, hostingEnvironment)
        {
            _emp = employee;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        public JsonResult SimulationLogin(string EmpNo)
        {
            if (_emp.RoleCode == "F" || _emp.IsSimulation == true)
            {
                //Clog.Writer.Info("SimulationLogin:" + EmpNo);
                bool exists = _emp.CheckEmpNoExists(EmpNo);
                if (!exists)
                {
                    return Json(new { ERROR = "員編不存在!" });
                }
                else
                {
                    _emp.SetUserSession(EmpNo);
                    return Json("OK");
                }
            }
            else
            {
                return Json(new { ERROR = "非法操作" });
            }
        }

        [HttpPost]
        public JsonResult SimulationLogout()
        {
            try
            {
                _emp.SetUserSession();
                return Json("OK");
            }
            catch (Exception ex)
            {
                return Json(new { ERROR = ex.ToString() });
            }

        }
    }
}