using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Repair.Web.Site2._0.Utilities
{
    public class DefaultControllerBase<T> : Controller
    {
        public ILog Logger = LogManager.GetLogger(typeof(T));

        protected override void OnException(ExceptionContext filterContext)
        {
            // 当自定义显示错误 mode = On，显示友好错误页面
            Logger.ErrorFormat("啄木鸟报修系统异常，错误：{0}", filterContext.Exception);
            base.OnException(filterContext);
        }
    }
}