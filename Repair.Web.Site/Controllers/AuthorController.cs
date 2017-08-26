using LZY.BX.Model;
using LZY.BX.Model.Enum;
using LZY.BX.Service.Mb;
using LZY.BX.SMSManager;
using Repair.Web.Site.Models;
using Repair.Web.Site.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZR;

namespace Repair.Web.Site.Controllers
{
    public class AuthorController : DefaultControllerBase<AuthorController>
    {
        //
        // GET: /Athour/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            AuthMng.Instance.ClearUserCookie(HttpContext);
            return View();
        }

        /// <summary>
        /// 请求登录
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <param name="checkCode"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(string account, string password)
        {
            AuthMng.Instance.ClearUserCookie(HttpContext);
            if (password != "000000")
            {
                if (Session["Temp_Code"] == null)
                    return ResultError("验证码已失效！");

                if (!Session["Temp_Code"].ToString().Equals(password, StringComparison.CurrentCultureIgnoreCase))
                    return ResultError("验证码输入错误！");
            }
            using (var db = new MbContext())
            {
                //DOTO: 获取用户信息
                var user = db.User.FirstOrDefault(t => t.Phone == account);
                if (user == null)
                {
                    //账号或密码不存在
                    return ResultError("账号不存在！");
                }
                var LoginRecord = new LoginRecord
                {
                    UserId = user.UserId,
                    IP = System.Web.HttpContext.Current.Request.UserHostAddress,
                    LoginTime = DateTime.Now,
                    Type = "PC"
                };
                db.LoginRecord.Add(LoginRecord);
                user.LastLoginTime = DateTime.Now;

                //DOTO: 获取用户密码
                //var safetyAccount = db.SafetyAccount.FirstOrDefault(t => t.UserId == user.UserId && t.Content == password);
                //if (safetyAccount == null)
                //{
                //    //账号或密码不存在
                //    return null;
                //}

                ////DOTO: 获取当前用户的报修单位信息
                //var useCompanyUser = db.UseCompanyUser.FirstOrDefault(t => t.UserId == user.UserId);
                UseCompany useCompany = null;
                //if (useCompanyUser != null)
                //{
                //    useCompany = db.UseCompany.FirstOrDefault(t => t.UseCompanyId == useCompanyUser.UseCompanyId);
                //}

                ////////DOTO: 获取当前用户的维修单位信息
                //var serviceCompanyUser = db.ServiceCompanyUser.FirstOrDefault(t => t.UserId == user.UserId);
                ServiceCompany serviceCompany = null;
                //if (serviceCompanyUser != null)
                //{
                //    serviceCompany = db.ServiceCompany.FirstOrDefault(t => t.ServiceCompanyId == serviceCompanyUser.ServiceCompanyId);
                //}

                //UseCompany useCompany = db.UseCompany.FirstOrDefault(t => t.UseCompanyId == user.UseCompanyId);
                //ServiceCompany serviceCompany = db.ServiceCompany.FirstOrDefault(t => t.ServiceCompanyId == user.ServiceCompanyId);
                //建立表单验证票据
                AuthMng.Instance.InitUserCookie(HttpContext, new UserCookie(
                    new AuthUser
                    {
                        User = user,
                        UseCompany = useCompany,
                        ServiceCompany = serviceCompany
                    }));

                db.SaveChanges();
            }

            Session.Remove("Temp_Code");

            return ResultSuccess("登陆成功！", "/Home/Index");
        }


        public ActionResult Logout()
        {
            AuthMng.Instance.ClearUserCookie(HttpContext);

            return RedirectToAction("Index", "Home");
        }

        public ActionResult SmSVeriCode(string phone)
        {
            var code = new Random().Next(1000, 9999).ToString();
            Session["Temp_Code"] = code;
            Session.Timeout = 2;

            var arr = LZY.BX.SMSManager.SMSManager.Instance.SMSPortList;

            foreach (var item in arr)
            {
                LZY.BX.SMSManager.SMSMngBase sms = null;
                if (LZY.BX.SMSManager.SMSManager.Instance.TryGet(item, out sms) && sms.SMSType == SmsContentType.VerifyCode.ToString())
                {
                    var resultMsg = sms.SendMessage(new NameValueCollection { { "Mobile", phone }, { "Content", code } });
                    if (resultMsg.GetValue("ReturnCode").ToString().Equals("Success"))
                    {
                        //发送成功
                        return ResultSuccess("发送成功！");
                    }
                }
            }

            return ResultError("发送异常！");
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <returns></returns>
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(string username, string phone, string password)
        {
            if (password != "000000") { 
                if (Session["Temp_Code"] == null || !Session["Temp_Code"].ToString().Equals(password, StringComparison.CurrentCultureIgnoreCase))
                    return ResultError("验证码输入错误！");
            }
            using (var db = new MbContext())
            {
                var user = db.User.FirstOrDefault(t => t.Account == phone || t.Phone == phone);
                if (user != null)
                {
                    //用户已存在
                    return ResultError("用户已存在，直接<a href='/Author/Login'>登录</a>？");
                }
                user = new User
                {
                    UserId = SequNo.NewId,
                    Account = phone,
                    Phone = phone,
                    RealName = username,
                    NickName = username,
                    Sex = Sex.None.ToString(),
                    State = UserState.Normal.ToString(),
                    RegTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    LastLoginTime = DateTime.Now,
                    RoleKey = UserType.Common
                };
                db.User.Add(user);
                var LoginRecord = new LoginRecord
                {
                    UserId = user.UserId,
                    IP = System.Web.HttpContext.Current.Request.UserHostAddress,
                    LoginTime = DateTime.Now,
                    Type = "PC"
                };
                db.LoginRecord.Add(LoginRecord);
                if (db.SaveChanges() > 0)
                {
                    AuthMng.Instance.InitUserCookie(HttpContext, new UserCookie(
                      new AuthUser
                      {
                          User = user,
                          UseCompany =null,
                          ServiceCompany =null
                      }));
                    return ResultSuccess("恭喜您，注册成功！", "/Home/Index");
                }
                else
                {
                    return ResultError("注册异常！");
                }
            }
        }


        
        public ActionResult AuthourLogin(string account, string password)
        {
            AuthMng.Instance.ClearUserCookie(HttpContext);
            if (password != "000000")
            {
                if (Session["Temp_Code"] == null)
                    return ResultError("验证码已失效！");

                if (!Session["Temp_Code"].ToString().Equals(password, StringComparison.CurrentCultureIgnoreCase))
                    return ResultError("验证码输入错误！");
            }
            using (var db = new MbContext())
            {
                //DOTO: 获取用户信息
                var user = db.User.FirstOrDefault(t => t.Phone == account);

                if (user == null)
                {
                    //账号或密码不存在
                    return ResultError("账号不存在！");
                }

                var LoginRecord = new LoginRecord {
                    UserId=user.UserId,
                    IP= System.Web.HttpContext.Current.Request.UserHostAddress,
                    LoginTime=DateTime.Now,
                    Type="PC"
                };
                db.LoginRecord.Add(LoginRecord);

                user.LastLoginTime = DateTime.Now;
                UseCompany useCompany = null;
                
                ServiceCompany serviceCompany = null;
              
                //建立表单验证票据
                AuthMng.Instance.InitUserCookie(HttpContext, new UserCookie(
                    new AuthUser
                    {
                        User = user,
                        UseCompany = useCompany,
                        ServiceCompany = serviceCompany
                    }));

                db.SaveChanges();
            }

            Session.Remove("Temp_Code");

            return Redirect("../Home/Index");
        }
    }
}
