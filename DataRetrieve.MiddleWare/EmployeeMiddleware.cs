using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Capital.Library;
using DataRetrieve.LogHelper;

namespace DataRetrieve.MiddleWare
{
    public static class EmployeeMiddleware
    {
        public static IApplicationBuilder UseRequestCulture(
           this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<EmployeeSession>();
        }
    }

    class EmployeeSession
    {
        ILog _log = ILogHelper.GetLog("Log4Net");
        private static bool FirstRun = true;
        private ILogger _logger;
        private IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        private readonly RequestDelegate _next;

        private IEmployee _emp;
        public EmployeeSession(RequestDelegate next, IHttpContextAccessor httpContextAccessor, IEmployee employee, ILogger<EmployeeSession> logger)
        {
            _next = next;
            _httpContextAccessor = httpContextAccessor;
            _emp = employee;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_emp.EmpNo))
                {
                    _log.Debug("寫入ad帳號");
                    //_session.SetString("EmpNo", "98046");
                    _emp.SetUserSession();
                }
                await _next.Invoke(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "寫入ad帳號發生錯誤");
                throw;
            }
        }
    }
}
