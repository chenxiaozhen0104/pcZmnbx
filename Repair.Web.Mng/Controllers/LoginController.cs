using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using LZY.BX.Model;
using LZY.BX.Service;
using LZY.BX.Service.Mb;

using Repair.Web.Mng.Menu;
using Repair.Web.Mng.Utilities;
using Repair.Web.Mng.Models;
using LZY.BX.Utilities;

namespace Repair.Web.Mng.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/

        public ActionResult Index()
        {
            Session["UserId"] = null;
            return View();
        }

        [HttpPost]
        public ActionResult Index(string username, string password)
        {
            var userService = new LoginSvr();
            if (userService.Login(username, password))
            {

                //登录，写cookie
                using (var db = new MbContext())
                {
                    var user = db.User.FirstOrDefault(m => m.Account == username);
                    var roles = db.UserRoles.FirstOrDefault(x => x.UserId == user.UserId);
                    if (roles == null)
                    {
                        return
                            Content("<script>alert('账户无权限');window.location.href='/Login/Index'</script>");
                    }
                    UserSession.SetUser(user);

                    //AuthMng.Instance.InitUserCookie(HttpContext, new UserCookie(
                    //new AuthUser
                    //{
                    //    User = user
                    //}));
                }

                return Redirect(GenJumpUrl());
            }
            ViewBag.UserName = username;
            ViewBag.ErrorMsg = "用户名或密码有误，请重新输入。";

            return View();
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <returns></returns>
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return Redirect("/");
        }


        public ActionResult ChangePassword(long id)
        {
            using (var db=new MbContext())
            {
                return View(db.User.FirstOrDefault(x => x.UserId == id));
            }
        }


        [HttpPost]
        public ActionResult ChangePassword(User model)
        {
            using (var db = new MbContext())
            {
                var oldPassword = Request["OldPassword"];
                var newPassword = Request["password"];
                var safty = db.SafetyAccount.FirstOrDefault(x => x.UserId == model.UserId);
                if (safty.Content!=oldPassword)
                {
                    return View();
                }
                safty.Content = Md5Helper.MD5Encrption(newPassword);
                db.SafetyAccount.AddOrUpdate(safty);
                db.SaveChanges();
                return Redirect("/Login/Index");
            }
        }

        private string GenJumpUrl()
        {
            var userSession = (UserSession)User;

            var curMenu = MenuMng.MenuTree[MenuMng.RootId];

            while (true)
            {
                if (!curMenu.HasVisiableSubItem)
                {
                    break;
                }

                var tmp = curMenu.SubItems.FirstOrDefault(x => !x.Hide && userSession.Roles.Contains(x.Id));
                if (tmp == null) break;
                curMenu = tmp;
            }

            return curMenu.Url;
        }
    }
}
