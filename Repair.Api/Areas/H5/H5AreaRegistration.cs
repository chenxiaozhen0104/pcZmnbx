using System.Web.Mvc;

namespace Repair.Web.Api.Areas.H5
{
    public class H5AreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "H5";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "H5_default",
                "H5/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
