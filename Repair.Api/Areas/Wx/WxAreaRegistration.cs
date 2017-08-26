using System.Web.Mvc;

namespace Repair.Api.Areas.Wx
{
    public class WxAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Wx";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "weixindefault",
                "Wx/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                 new string[] { "Repair.Api.Areas.Wx.Controllers" }
            );
        }
    }
}
