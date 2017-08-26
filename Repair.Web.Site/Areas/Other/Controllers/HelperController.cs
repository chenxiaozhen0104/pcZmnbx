using Repair.Web.Site.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Repair.Web.Site.Areas.Other.Controllers
{
    public class HelperController : Controller
    {
        //
        // GET: /Other/Helper/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ConttactService()
        {
            return View();
        }
        /**--------------------     维修机构     ---------------------**/
        /**  帮助中心-维修机构-产品 **/
        public ActionResult Service_product()
        {
            return View();
        }
        /**  帮助中心-维修机构-加入 **/
        public ActionResult Service_add()
        {
            return View();
        }
        /**  帮助中心-维修机构-交易评价 **/
        public ActionResult Service_comment()
        {
            return View();
        }
        /**  帮助中心-维修机构-交易中 **/
        public ActionResult Service_transaction()
        {
            return View();
        }
       /**  帮助中心-维修机构-交易前 **/
        public ActionResult Service_beforeTransaction()
        {
            return View();
        }
        /**  帮助中心-维修机构-账号注册 **/
        public ActionResult Service_register()
        {
            return View();
        }
        /**  帮助中心-维修机构-账号登录 **/
        public ActionResult Service_login()
        {
            return View();
        }
        /**  帮助中心-维修机构-账号安全 **/
        public ActionResult Service_safe()
        {
            return View();
        }
        /**  帮助中心-维修机构-账号绑定 **/
        public ActionResult Service_binding()
        {
            return View();
        }
        /**--------------------     用户     ---------------------**/
        /**  帮助中心-用户-账号注册 **/
        public ActionResult User_register()
        {
            return View();
        }
        /**  帮助中心-用户-账号登录 **/
        public ActionResult User_login()
        {
            return View();
        }
        /**  帮助中心-用户-账号安全 **/
        public ActionResult User_safe()
        {
            return View();
        }
        /**  帮助中心-用户-账号安全 **/
        public ActionResult User_binding()
        {
            return View();
        }
        /**  帮助中心-用户-交易订单 **/
        public ActionResult User_transactionOrder()
        {
            return View();
        }
        /**  帮助中心-用户-交易评价 **/
        public ActionResult User_comment()
        {
            return View();
        }

    }
}
