using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Repair.Web.Site2._0.Utilities
{
    /// <summary>
    /// 系统安全控制器
    /// </summary>
    public class AuthMng
    {
        #region 单件实现

        private static volatile AuthMng _instance = null;
        private static readonly object LockObj = new object();

        private AuthMng()
        {
        }

        /// <summary>
        /// 获取单件
        /// 获取单件的唯一途径
        /// </summary>
        public static AuthMng Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (LockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new AuthMng();
                        }
                    }
                }
                return _instance;
            }
        }

        #endregion

        /// <summary>
        /// 授权处理
        /// </summary>
        /// <param name="ctx"></param>
        public void OnPostAuthenticateRequest(HttpContext ctx)
        {
            var context = new HttpContextWrapper(ctx);

            var identity = context.User.Identity;
            //已经登录，并已经授权完成
            if (identity.IsAuthenticated && identity.AuthenticationType == "Forms")
            {
                var user = new UserCookie(identity as FormsIdentity);
                context.User = user;
            }
        }

        public void OnRequestEnd(HttpContext ctx)
        {
            //页面是否被重定向
            if (!ctx.Response.IsRequestBeingRedirected)
                return;

            //检查被重定向到登录登录页面
            if (!ctx.Response.RedirectLocation.ToLower().StartsWith(FormsAuthentication.LoginUrl.ToLower()))
                return;

            var uri = new Uri(ctx.Response.RedirectLocation);
            var val = HttpUtility.ParseQueryString(uri.Query);

            val["returnUrl"] = ctx.Request.Url.ToString();

            string url = FormsAuthentication.LoginUrl;
            if (url.Contains("?"))
            {
                url += "&" + val;
            }
            else
            {
                url += "?" + val;
            }
            ctx.Response.RedirectLocation = url;
        }

        public void ClearUserCookie(HttpContextBase ctx)
        {
            HttpCookie cok = ctx.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cok == null)
                return;
            var ts = new TimeSpan(-1, 0, 0, 0);
            //cok.Domain = FormsAuthentication.CookieDomain;
            cok.Expires = DateTime.Now.Add(ts); //删除整个Cookie，只要把过期时间设置为现在
            ctx.Response.AppendCookie(cok);
        }

        /// <summary>
        /// 设置用户登录cookie
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="userCookie"></param>
        public void InitUserCookie(HttpContextBase ctx, UserCookie userCookie)
        {
            //初始化当前用户
            ctx.User = userCookie;

            var identity = ctx.User.Identity as FormsIdentity;
            FormsAuthenticationTicket ticket = identity.Ticket;
            string encTicket = FormsAuthentication.Encrypt(ticket);

            var uc = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket)
            {
                Path = ticket.CookiePath,
                HttpOnly = true,
                //Domain = FormsAuthentication.CookieDomain
            };
            //保存cookie
            ctx.Response.Cookies.Add(uc);
        }

        /// <summary>
        /// 重置用户登录cookie
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="userCookie"></param>
        public void ResetUserCookie(HttpContextBase ctx, UserCookie userCookie)
        {
            //清除用户登录cookie
            ClearUserCookie(ctx);
            //初始化当前用户
            InitUserCookie(ctx, userCookie);
        }
    }
}