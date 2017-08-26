using Repair.Web.Site.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Repair.Web.Site.Areas.WeiXin.Controller
{
    public class RepairController :ControllerBase<RepairController>
    {
        public ActionResult Repair(string id)
        {
            string path = ConfigurationManager.AppSettings["returnUrl"].ToString();

            string url = path + "/Areas/Wx/Content/zmnbxapp/repair.html?id=" + id;
            string data = HttpUtility.UrlEncode(url, System.Text.Encoding.UTF8);
            //跳转到登录后的首页
            return Redirect(path + "/Wx/Auth/Index?returnUrl=" + data);

        }

    }
}
