using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Repair.Web.Site2._0.Utilities
{
    public class ControllerWebBase : Controller
    {
        public ILog Logger;
        public ControllerWebBase()
        {
            Logger = LogManager.GetLogger(this.GetType());
        }


    }
}