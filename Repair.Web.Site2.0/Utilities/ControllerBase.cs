
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Repair.Web.Site2._0.Utilities
{
    //[Authorize]
    public class ControllerBase<T> : Controller
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