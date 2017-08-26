using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Repair.Web.Site
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Equipment",
                url: "Equipment/{id}",
                defaults: new { controller = "Equipment", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "Repair.Web.Site.Controllers" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "Repair.Web.Site.Controllers" }
            );
        }
    }
}