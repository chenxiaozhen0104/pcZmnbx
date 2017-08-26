using System.Web.Mvc;

namespace Repair.Web.Mng.Areas.SysAdmin
{
    public class SysAdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "SysAdmin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "SysAdmin",
                "SysAdmin/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
