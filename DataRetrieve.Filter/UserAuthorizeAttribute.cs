using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Capital.Library;

namespace DataRetrieve.Filter
{


    //http://www.dotblogs.com.tw/rainmaker/archive/2011/11/09/55539.aspx
    /// <summary>
    /// 驗證User是否有ProgramID的權限，沒有就導到Home/AccessDenied
    /// </summary>
    public class ProgramIDAuthorizeAttribute : ActionFilterAttribute
    {
        public ProgramIDAuthorizeAttribute(int programID) {
            ProgramID = programID;
        }
        /// <summary>
        /// SYSDATA..SMSiteMap裡面的ProgramID
        /// </summary>
        public int ProgramID { get; set; }

        /// <summary>
        /// RoldeCode 可選
        /// </summary>
        public string[] RoleCode { get; set; }

        /// <summary>
        /// PositionCode 可選
        /// </summary>
        public string[] PositionCode { get; set; }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var _emp = context.HttpContext.RequestServices.GetService<IEmployee>();

            if (!AuthorizeCore(_emp, context))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(
              new { action = "AccessDenied", controller = "Error" }));
            }
            else {
                 await next.Invoke();
            }          
        }

        protected bool AuthorizeCore(IEmployee _emp, ActionExecutingContext context)
        {
            if (string.IsNullOrWhiteSpace(context.HttpContext.User.Identity.Name))
                return false;
            else
            {
                if (RoleCode != null && RoleCode.Length > 0)
                {
                    return RoleCode.Contains(_emp.RoleCode);
                }
                if (PositionCode != null && PositionCode.Length > 0)
                {
                    return PositionCode.Contains(_emp.PositionCode);
                }
                return _emp.CheckProgramAuthority(_emp.EmpNo, ProgramID);
            }
        }
    }
}
