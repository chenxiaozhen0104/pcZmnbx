using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Repair.Web.Site.Controllers
{
    public class BaikeController : Controller
    {
        //
        // GET: /Baike/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult baikeInfo()
        {
            return View();
        }

        public ActionResult baikeTips()
        {
            return View();
        }
    }
}
