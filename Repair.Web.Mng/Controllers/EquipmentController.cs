using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Repair.Web.Mng.Controllers
{
    public class EquipmentController : Controller
    {
        //
        // GET: /Equipment/

        public ActionResult Index(string id)
        {
            ViewBag.id = id;
            return View();
        }

    }
}
