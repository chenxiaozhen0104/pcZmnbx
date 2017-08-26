using System;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using LZY.BX.Model;
using LZY.BX.Service;

namespace Repair.Web.Mng
{
    /// <summary>
    /// 用户会话
    /// </summary>
    public sealed class UserSession : IPrincipal
    {
        public User User { get; private set; }

        public string[] Roles { get; set; }
        public FormsIdentity Identity { get; private set; }


        /// <summary>
        /// 初始化当前用户会话
        /// </summary>
        public static void InitUser()
        {
            var ctx = HttpContext.Current;
            var user = ctx.User;

            //已经授权了，重新设置用户对象
            if (!user.Identity.IsAuthenticated)
                return;

            FormsAuthenticationTicket titck;

            {
                var infoCookie = FormsAuthentication.FormsCookieName;

                var authCookie = ctx.Request.Cookies[infoCookie];
                if (authCookie != null)
                {
                    titck = FormsAuthentication.Decrypt(authCookie.Value); //解密 
                }
                else
                {
                    var  uid = long.Parse(user.Identity.Name);

                    var list = PermissionSvr.Instance.GetRoles(uid);

                    titck = new FormsAuthenticationTicket(1, user.Identity.Name,
                        DateTime.Now, DateTime.Now.AddYears(1), false, string.Join(",", list));
                }
            }
            ctx.User = new UserSession(titck);
        }

        /// <summary>
        /// 设置用户会话
        /// </summary>
        /// <param name="user"></param>
        public static void SetUser(User user)
        {
            var list = PermissionSvr.Instance.GetRoles(user.UserId);
            var productList = string.Join(";", list);
            var ticket = new FormsAuthenticationTicket(1, user.UserId.ToString(CultureInfo.InvariantCulture),
                DateTime.Now, DateTime.Now.AddYears(1), false, productList);

            var ctx = HttpContext.Current;

            var authTicket = FormsAuthentication.Encrypt(ticket);
            //将加密后的票据保存为cookie 
            var coo = new HttpCookie(FormsAuthentication.FormsCookieName, authTicket);
            //使用加入了userdata的新cookie 
            ctx.Response.Cookies.Add(coo);

            ctx.User = new UserSession(ticket);
        }

        UserSession(FormsAuthenticationTicket ticket)
        {
            Identity = new FormsIdentity(ticket);

            var productList = ticket.UserData.Split(';')
                .Select(x=>long.Parse(x)).ToArray();

            var str = string.Join(";", PermissionSvr.Instance.GetPermission(productList));
            Roles = str.Split(';');

            User = UserSvr.Instance.GetItem(long.Parse(ticket.Name));
        }

        /// <summary>
        /// 匹配用户权限
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public bool IsInRole(string role)
        {
            return Roles.Any(ro => ro.Equals(role));
        }

        public bool AddUserToRoles(string[] roles) { return false; }
        public bool RemoveUserRole(string role) { return false; }

        IIdentity IPrincipal.Identity { get { return Identity; } }


        

    }
}
