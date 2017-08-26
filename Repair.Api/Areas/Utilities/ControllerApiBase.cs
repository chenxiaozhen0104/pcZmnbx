using log4net;
using LZY.BX.Model;
using LZY.BX.Model.Enum;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Repair.Api.Areas.Utilities
{
    public class ControllerApiBase : Controller
    {
        public ILog Logger;
        public ControllerApiBase()
        {
            Logger = LogManager.GetLogger(this.GetType());
        }
    }
}