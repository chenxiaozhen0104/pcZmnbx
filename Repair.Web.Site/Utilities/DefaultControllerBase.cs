using log4net;
using Repair.Web.Site.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Repair.Web.Site.Utilities
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
            // 当自定义显示错误 mode = On，显示友好错误页面
            Logger.ErrorFormat("啄木鸟报修系统异常，错误：{0}", filterContext.Exception);

            //异常信息直接返回
            //if (!filterContext.ExceptionHandled)
            //{
            //    filterContext.Result = RedirectToAction("Index", "Error");
            //    filterContext.ExceptionHandled = true;
            //}

            base.OnException(filterContext);
        }
    }
}