using LZY.BX.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

namespace Repair.Web.Mng.Models
{
    public class UserCookie : AuthUser, IPrincipal
    {
        public UserCookie(AuthUser user)
        {
            this.User = user.User;
        }

        public UserCookie(FormsIdentity identity)
        {
            _Identity = identity;

            SetProperty(identity.Ticket.UserData);
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        private void SetProperty(string userData)
        {
            var authUser = Newtonsoft.Json.JsonConvert.DeserializeObject<AuthUser>(userData);
            this.User = authUser.User;
        }

        #region IPrincipal 成员

        private FormsIdentity _Identity;
        /// <summary>
        /// 获取当前用户的标识
        /// </summary>
        public IIdentity Identity
        {
            get
            {
                if (_Identity == null)
                {
                    var ticket = new FormsAuthenticationTicket(1,
                        User.UserId.ToString(),
                        DateTime.Now,
                        DateTime.Now.AddDays(90),
                        false, //TODO 是否需要保存cookie,需要考虑手机还是电脑
                        TryGetUserData,
                        FormsAuthentication.FormsCookiePath);
                    _Identity = new FormsIdentity(ticket);
                }
                return _Identity;
            }
        }

        /// <summary>
        /// 用户角色判断
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public bool IsInRole(string role)
        {
            return true;
        }

        /// <summary>
        /// 用户数据
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public string TryGetUserData
        {
            get
            {
                var auth = new AuthUser
                {
                    User = this.User
                };

                return Newtonsoft.Json.JsonConvert.SerializeObject(auth);
            }
        }

        #endregion
    }
}