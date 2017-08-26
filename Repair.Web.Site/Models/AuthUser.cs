using LZY.BX.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Repair.Web.Site.Models
{
    public class AuthUser
    {
        /// <summary>
        /// 当前用户
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// 报修单位信息
        /// </summary>
        public UseCompany UseCompany { get; set; }

        /// <summary>
        /// 维系单位信息
        /// </summary>
        public ServiceCompany ServiceCompany { get; set; }
    }
}