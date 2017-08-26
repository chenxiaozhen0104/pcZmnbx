using Repair.Web.Site.Utilities;
using System.Web.Mvc;

namespace Repair.Web.Site.Areas.User.Controllers
{
    public class DefaultController : ControllerBase<DefaultController>
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

    }
}
