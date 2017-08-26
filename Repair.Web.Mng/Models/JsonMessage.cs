using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Repair.Web.Mng.Models
{
    /// <summary>
    /// ajax返回的json串
    /// </summary>
    [Serializable]
    public class JsonMessage
    {
        public JsonMessage()
        {
            _errCode = JsonErrCode.Success;
            _errMsg = "";
            _returnUrl = "";
            _htmlId = "";
        }

        private JsonErrCode _errCode;

        private string _errMsg;

        private string _returnUrl;

        private string _htmlId;

        /// <summary>
        /// 返回码，0为请求成功,-1 出现错误或者无权限，-2 验证失败，-3 登录超时
        /// </summary>
        public JsonErrCode ErrCode
        {
            get { return _errCode; }
            set { _errCode = value; }
        }

        /// <summary>
        /// 返回信息说明
        /// </summary>
        public string ErrMsg
        {
            get { return _errMsg; }
            set { _errMsg = value; }
        }

        /// <summary>
        /// 跳转链接，当ErrCode=0时有跳转链接则在此
        /// </summary>
        public string ReturnUrl
        {
            get { return _returnUrl; }
            set { _returnUrl = value; }
        }

        /// <summary>
        /// 对于验证型，返回html控件Id
        /// </summary>
        public string HtmlId
        {
            get { return _htmlId; }
            set { _htmlId = value; }
        }
    }

    public enum JsonErrCode
    {
        Success200 = 200,
        Success = 0,
        Error = -1,
        HtmlError = -2,
        LoginError = -3
    }
}