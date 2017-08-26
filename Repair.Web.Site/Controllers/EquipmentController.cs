using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Repair.Web.Site.Controllers
{
    public class EquipmentController : Controller
    {
        //
        // GET: /Equipment/

        public ActionResult Index(string id)
        {
            //string userAgent = Request.UserAgent;
            //if (userAgent.ToLower().Contains("micromessenger"))
            //{
            //    return Redirect("~/WeiXin/Repair/Repair?id=" + id);
            //}

            //return Content("未实现");
            string path = ConfigurationManager.AppSettings["returnUrl"].ToString();

            string url = path + "/Areas/Wx/Content/zmnbxapp/repair.html?id=" + id;
            string data = HttpUtility.UrlEncode(url, System.Text.Encoding.UTF8);
            //跳转到登录后的首页
            return Redirect(path + "/Wx/Auth/Index?returnUrl=" + data);
        }
    }
}
