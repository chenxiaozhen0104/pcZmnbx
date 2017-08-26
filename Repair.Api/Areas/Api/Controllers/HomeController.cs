
using LZY.BX.Model.Enum;
using System.Web.Mvc;
using Repair.Api.Areas.Api.Utilities;
using Repair.Api.Areas.Utilities;

namespace Repair.Api.Areas.Api.Controllers
{
    public class HomeController : ControllerApiBase
    {
        public ActionResult Index(AppType appType = AppType.Use)
        {
            var context = System.Web.HttpContext.Current;

            Logger.DebugFormat("{0}===={1}", context.Request.Headers["version"], context.Request.UserAgent.Contains("Android"));
            if ((string.IsNullOrWhiteSpace(context.Request.Headers["version"]) || context.Request.Headers["version"] == "1.0" || context.Request.Headers["version"] == "1.0.0.0") && context.Request.UserAgent.Contains("Android"))
            {
                return Redirect("/Areas/H5/Content/dist/view/updateapp.html?appType=" + (int)appType);
            }

            if (ApiUser.Current == null)
            {
                return Redirect("/Areas/H5/Content/dist/view/login.html?appType=" + (int)appType);
            }
            else
            {
                if (appType == AppType.Use)
                {
                    return Redirect("/Areas/H5/Content/dist/view/repair.html?appType=" + (int)appType);
                }
                else
                {
                    return Redirect("/Areas/H5/Content/dist/view/orderlist.html?appType=" + (int)appType);
                }
            }
        }
    }
}
