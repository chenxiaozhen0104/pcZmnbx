using System.Web.Mvc;

namespace Repair.Web.Site.Areas.Sp
{
    public class SpAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Sp";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Sp_default",
                "Sp/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
