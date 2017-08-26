using System;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using LZY.BX.Model;
using LZY.BX.Service;

namespace Repair.Api
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
