using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Capital.Library;

namespace DataRetrieve.Controllers
{
    public class ErrorController : IController
    {
        private IHostingEnvironment _hostingEnvironment;
        private IEmployee _emp;
        public ErrorController(IEmployee employee, IHostingEnvironment hostingEnvironment) : base(employee, hostingEnvironment)
        {
            _emp = employee;
            _hostingEnvironment = hostingEnvironment;
        }
        [Route("/Error")]
        public ActionResult AccessDenied(string id)
        {
            return View();
        }

        public ActionResult AuthenticateFail() {
            return View();
        }
        public ActionResult ErrorPage()
        {
            var n = HttpContext.Features.Get<IExceptionHandlerFeature>();
            ViewBag.Error = n.Error;
            return View();
        }
    }
}
