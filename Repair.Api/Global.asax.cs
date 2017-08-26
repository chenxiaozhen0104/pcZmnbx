
using LZY.BX.Service;
using Repair.Api.Areas.Api.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Repair.Api
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(Server.MapPath("log4net.config")));

            Thread WorkOverimeThread = new Thread(new OrderController().DoWorkOverTimeTips);
            WorkOverimeThread.Name = "DoWorkOverTimeTips";
            WorkOverimeThread.IsBackground = true;
            WorkOverimeThread.Start();

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

          
        }

        protected void Application_PostAuthenticateRequest(object sender, EventArgs arg)
        {
            var ctx = HttpContext.Current;
            ctx.Response.AddHeader("p3p",
                @"CP=\""IDC DSP COR ADM DEVi TAIi PSA PSD IVAi IVDi CONi HIS OUR IND CNT\""");
            ctx.Response.AddHeader("Access-Control-Allow-Origin", @"*");

          
        }
    }
}
