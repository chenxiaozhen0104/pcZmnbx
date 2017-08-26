using Repair.Web.Site.Models;
using Repair.Web.Site.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Repair.Web.Site.Controllers
{
    public class SystemInfoController : Controller
    {
        //
        // GET: /Other/About/

        public ActionResult Index(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = "帮助中心";
            }

            var arr = id.Split(',');

            var list = new List<string>(arr);

            var baseDir = Server.MapPath("/Views/SystemInfo");

            while (list.Count < 4)
            {
                var subDrs = Directory.GetDirectories(
                    Path.Combine(baseDir, string.Join("\\", list)));

                var firstDir = Path.GetFileName(subDrs[0]);
                list.Add(firstDir);

                if (list.Count == 3)
                {

                    var files = Directory.GetFiles(Path.Combine(baseDir, string.Join("\\", list)));

                    var file = Path.GetFileNameWithoutExtension(files[0]);

                    list.Add(file);
                }
            }

            return View(list.ToArray());
        }
    }
}
