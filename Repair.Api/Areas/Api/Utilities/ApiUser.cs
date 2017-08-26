using LZY.BX.Model;
using LZY.BX.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;

namespace Repair.Api.Areas.Api.Utilities
{
    public class ApiUser
    {
        public static User Current
        {
            get
            {
                var context = System.Web.HttpContext.Current;

                if (context.Request["token"] == null)
                {
                    return null;
                }

                var token = context.Request["token"];

                if (string.IsNullOrEmpty(token))
                {
                    return null;
                }
                if (context.Cache[token] == null)
                {
                    if (!string.IsNullOrEmpty(token))
                    {
                        var user = UserService.Instance.Get(token);
                        if (user == null)
                        {
                            return null;
                        }
                        context.Cache.Insert(token, user, null, Cache.NoAbsoluteExpiration, TimeSpan.FromHours(2));
                    }
                }
                return context.Cache[token] as User;
            }
        }

        public static void UpdateCurrent(string userId)
        {
            System.Web.HttpContext.Current.Cache.Remove(userId.ToString());
        }
    }
}
