using log4net;
using Repair.Web.Mng.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Repair.Web.Mng.Utilities
{
    public class DefaultControllerBase<T> : Controller
    {
        #region 返回信息
        private readonly JsonMessage _jsonMessage = new JsonMessage();

        /// <summary>
        /// 成功
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="url"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public JsonResult ResultSuccess(string msg, string url = "", JsonErrCode errCode = JsonErrCode.Success)
        {
            _jsonMessage.ErrCode = errCode;
            _jsonMessage.ErrMsg = msg;
            _jsonMessage.ReturnUrl = url;

            return Json(_jsonMessage);
        }

        /// <summary>
        /// 失败
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="html"></param>
        /// <param name="errCode"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public JsonResult ResultError(string msg, string html = "", JsonErrCode errCode = JsonErrCode.Error, string url = "")
        {
            _jsonMessage.ErrCode = errCode;
            _jsonMessage.ErrMsg = msg;
            _jsonMessage.ReturnUrl = url;
            _jsonMessage.HtmlId = html;

            return Json(_jsonMessage);
        }
        #endregion

        public ILog Logger = LogManager.GetLogger(typeof(T));

        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
        }
    }
}