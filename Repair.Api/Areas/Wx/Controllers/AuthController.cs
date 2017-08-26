using LZY.BX.Model.Enum;
using LZY.BX.Service;
using Repair.Api.Areas.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WeiXin;
using WeiXin.JsApi;

namespace Repair.Api.Areas.Wx.Controllers
{
    public class AuthController : ControllerApiBase
    {
        AuthAccountServer aa = new AuthAccountServer();
        UserService us = new UserService();

        //
        // GET: /Api/Weixin/

        public ActionResult ScanQrCode(string url)
        {
            var dic = new Dictionary<string, string>();
            var nnonceStr = Guid.NewGuid().ToString();
            var timestamp = GetTimeStamp();
            
            dic.Add("appId", Config.AppId);
            dic.Add("nnonceStr", nnonceStr);
            dic.Add("timestamp", timestamp);
            dic.Add("signature", Ticket.GetJsSignature(nnonceStr, timestamp, url));

            return Json(dic);
        }

        public ActionResult Index(string returnUrl)
        {
            var user = Session["User"] as LZY.BX.Model.User;          
            if (user != null)
            {
                if (aa.GetByUserId(user.UserId) == null
                    ||
                    us.Get(user.UserId) == null)
                {
                    Session.Remove("User");
                    Session.Remove("WxUser");
                }
            }

            if (Session["User"] != null)
            {
                //跳转到登录后的首页
                //return Redirect("/Areas/Wx/Content/zmnbxapp/weex.html#/homePage");
                return Redirect(returnUrl);
            }

            //授权登录
            var state = "";
            if (!string.IsNullOrEmpty(returnUrl))
            {
                var data = Convert.ToBase64String(Encoding.UTF8.GetBytes(returnUrl));
                state = HttpUtility.UrlEncode(data);
            }
            ////如果数据库找到授权信息
            if (Session["WxUser"] != null)
            {
                //静默授权
                return Redirect(WeiXinAuthor.OauthUrl(HttpUtility.UrlEncode(string.Concat(Config.DoMain, Url.Action("WeixinAuth", "Auth"))), "snsapi_base", state));
            }
            //弹框授权
            return Redirect(WeiXinAuthor.OauthUrl(HttpUtility.UrlEncode(string.Concat(Config.DoMain, Url.Action("WeixinAuth", "Auth"))), "snsapi_userinfo", state));
        }

        /// <summary>
        /// 微信授权回调
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public ActionResult WeixinAuth(string code, string state)
        {
            var returnUrl = "";
            if (!string.IsNullOrEmpty(state))
            {
                var data = Encoding.UTF8.GetString(Convert.FromBase64String(state));
                returnUrl = HttpUtility.UrlDecode(data);
            }

            var wxUser = WeiXinAuthor.TryGetUserInfo(code);
            if (wxUser == null)
                return Content(string.Format("授权失败，请<a href='{0}'>点击此处</>重试", Url.Action("Index", "Auth", new { returnUrl = returnUrl })));
            Session["WxUser"] = wxUser;                  
            //缓存并保存微信授权信息
            SaveAuthAccount(wxUser);
            //是否关注,没关注跳关注页面,0没有关注     
            if (wxUser.subscribe == 0) {
                return Redirect("/Areas/Wx/Content/zmnbxapp/fouceWeChat.html");
            }
            var authAccount = aa.Get(wxUser.openid);
            //上一步已经保存微信的信息，所以本次只需要验证是否存在UserId即可
            if (!string.IsNullOrEmpty(authAccount.UserId))
            {
                Session.Remove("User");
                //return Redirect("/Areas/Wx/Content/zmnbxapp/weex.html#/login");
                var data = QueryString(returnUrl);
                if (data["id"] != null)
                {
                    return Redirect("/Areas/Wx/Content/zmnbxapp/repair.html?id=" + data["id"]);
                }
                else {
                    return Redirect(returnUrl);
                }
            }

            var user = us.Get(authAccount.UserId);
            if (user == null)
            {
                //用户丢失
                Session.Remove("User");
                //return Redirect("/Areas/Wx/Content/zmnbxapp/weex.html#/login");
                var data = QueryString(returnUrl);
                return Redirect("/Areas/Wx/Content/zmnbxapp/login.html?id=" + data["id"]);              
            }

            //跳转对应的页面
            //return Redirect("/Areas/Wx/Content/zmnbxapp/weex.html#/homePage");
            return Redirect(returnUrl);

        }

        //保存授权信息
        public bool SaveAuthAccount(WeiXinUser wxUser)
        {
            var flag = false;

            var sex = Sex.Man;
            switch (wxUser.sex)
            {
                case 1:
                    sex = Sex.Man;
                    break;
                case 2:
                    sex = Sex.Woman;
                    break;
                case 0:
                    sex = Sex.None;
                    break;
            }

            var isSave = false;

            var authAccount = aa.Get(wxUser.openid);

            if (authAccount == null)
            {
                authAccount = new LZY.BX.Model.AuthAccount();
                isSave = true;
            }
            
            authAccount.OpenId = wxUser.openid;
            authAccount.Server = "Wx";
            authAccount.NickName = wxUser.nickname;
            authAccount.Sex = sex.ToString();
            authAccount.Province = wxUser.province;
            authAccount.City = wxUser.city;
            authAccount.Country = wxUser.country;
            authAccount.Headimgurl = wxUser.headimgurl;
            authAccount.Privilege = Newtonsoft.Json.JsonConvert.SerializeObject(wxUser.privilege);
            authAccount.Unionid = wxUser.subscribe.ToString();//subscribe=1 表示已经关注 0还没有关注
            authAccount.AccessToken = wxUser.access_token;
            authAccount.RefreshToken = wxUser.refresh_token;
            authAccount.ExpiresIn = 0;
            authAccount.ExpiresOut = 0;
            authAccount.RefreshExpiresOut = 0;
            authAccount.LastUpdateTime = DateTime.Now;

            if (isSave)
            {
                authAccount.CreateTime = DateTime.Now;
                //保存内容
                flag = aa.Save(authAccount);
            }
            else
            {
                //更新内容
                flag = aa.Update(authAccount);
            }

            //缓存用户基础信息
            Response.Cookies.Add(new System.Web.HttpCookie("wx_token", authAccount.OpenId));
            return flag;
        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
        public static NameValueCollection QueryString(String url)
        {
            Uri uri = new Uri(url);
            string queryString = uri.Query;
            return GetQueryString(queryString, null, true);
        }

        /// <summary>
        /// 将查询字符串解析转换为名值集合.
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public static NameValueCollection GetQueryString(string queryString)
        {
            return GetQueryString(queryString, null, true);
        }

        /// <summary>
        /// 将查询字符串解析转换为名值集合.
        /// </summary>
        /// <param name="queryString"></param>
        /// <param name="encoding"></param>
        /// <param name="isEncoded"></param>
        /// <returns></returns>
        public static NameValueCollection GetQueryString(string queryString, Encoding encoding, bool isEncoded)
        {
            queryString = queryString.Replace("?", "");
            NameValueCollection result = new NameValueCollection(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrEmpty(queryString))
            {
                int count = queryString.Length;
                for (int i = 0; i < count; i++)
                {
                    int startIndex = i;
                    int index = -1;
                    while (i < count)
                    {
                        char item = queryString[i];
                        if (item == '=')
                        {
                            if (index < 0)
                            {
                                index = i;
                            }
                        }
                        else if (item == '&')
                        {
                            break;
                        }
                        i++;
                    }
                    string key = null;
                    string value = null;
                    if (index >= 0)
                    {
                        key = queryString.Substring(startIndex, index - startIndex);
                        value = queryString.Substring(index + 1, (i - index) - 1);
                    }
                    else
                    {
                        key = queryString.Substring(startIndex, i - startIndex);
                    }
                    if (isEncoded)
                    {
                        result[MyUrlDeCode(key, encoding)] = MyUrlDeCode(value, encoding);
                    }
                    else
                    {
                        result[key] = value;
                    }
                    if ((i == (count - 1)) && (queryString[i] == '&'))
                    {
                        result[key] = string.Empty;
                    }
                }
            }
            return result;
        }



        /// <summary>
        /// 解码URL.
        /// </summary>
        /// <param name="encoding">null为自动选择编码</param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string MyUrlDeCode(string str, Encoding encoding)
        {
            if (encoding == null)
            {
                Encoding utf8 = Encoding.UTF8;
                //首先用utf-8进行解码                     
                string code = HttpUtility.UrlDecode(str.ToUpper(), utf8);
                //将已经解码的字符再次进行编码.
                string encode = HttpUtility.UrlEncode(code, utf8).ToUpper();
                if (str == encode)
                    encoding = Encoding.UTF8;
                else
                    encoding = Encoding.GetEncoding("gb2312");
            }
            return HttpUtility.UrlDecode(str, encoding);
        }
    }
}
