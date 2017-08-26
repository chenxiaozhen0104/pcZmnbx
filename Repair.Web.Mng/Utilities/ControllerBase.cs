using log4net;
using Repair.Web.Mng.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Repair.Web.Mng.Utilities
{
    [Authorize]
    public class ControllerBase<T> : DefaultControllerBase<T>
    {
        public UserCookie CurrentUser
        {
            get
            {
                return User as UserCookie;
            }
        }
    }
}