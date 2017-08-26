
using LZY.BX.Model.Enum;
using LZY.BX.Service;
using System;
using System.Web.Mvc;
using System.Linq;
using LZY.BX.Model;
using Repair.Api.Areas.Api.Utilities;
using System.Web;
using Newtonsoft.Json;
using System.IO;
using System.Configuration;
using Repair.Api.Areas.Utilities;
using System.Globalization;

namespace Repair.Api.Areas.Api.Controllers
{
    public class UserController : ControllerApiBase
    {
        public const int TIEM_Out = 5;
        private ActionResult SetUser(User user)
        {
            Response.Cookies.Add(new System.Web.HttpCookie("token", user.UserId));
            //微信登录
            var wx_token = System.Web.HttpContext.Current.Request["wx_token"];
            if (wx_token != null)
            {
                AuthAccountServer.Instance.Update(wx_token, user.UserId);
            }
            var LoginRecord = new LoginRecord
            {
                UserId = user.UserId,
                IP = System.Web.HttpContext.Current.Request.UserHostAddress,
                LoginTime = DateTime.Now,
                Type = (wx_token!=null?"Wx":"App")
            };
            new UserService().AddLoginRecord(LoginRecord);
            Session["User"] = user;
            Session.Timeout = 60 * 1;
            //获取经纬度
            var userPosition = new UserService().GetUserPosition(user.UserId);
            return Json(new
            {
                id = user.UserId,
                name = user.NickName,
                type = (int)user.RoleKey,
                phone = user.Phone,
                sex = user.Sex,
                //使用单位信息是否激活(审核通过)
                status = user.UseCompany != null ? "" + user.UseCompany.State + "" : "NotActive",
                //服务单位信息是否激活(审核通过)
                serviceStatus = user.ServiceCompany != null ? "" + user.ServiceCompany.State + "" : "NotActive",
                useCompany = user.UseCompany?.Name,
                useCompanyId = user.UseCompany?.UseCompanyId,
                serviceCompany = user.ServiceCompany?.Name,
                serviceCompanyId = user.ServiceCompany?.ServiceCompanyId,
                longitude = userPosition?.Longitude ,
                latitude = userPosition?.Latitude,
                workingway = user.ServiceCompany?.WorkingWay,
                workedway = user.ServiceCompany?.WorkedDWay

            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SMSLogin(string mobile, string smsCode)
        {
            try
            {
                var flag = SmsLogService.Instance.CheckVerifyCode(mobile, smsCode, TIEM_Out);

                var user = UserService.Instance.GetByPhone(mobile);

                return SetUser(user);
            }
            catch (ArgumentNullException)
            {
                return Json(new { error = "请输入完整信息" });
            }
            catch (UserNotExisitException)
            {
                return Json(new { error = "该手机号未注册" });
            }
            catch (ArgumentException e)
            {
                return Json(new { error = e.Message });
            }
            catch (VerifyCodeExpireException)
            {
                return Json(new { error = "验证码已过期" });
            }
        }
        public ActionResult PasswordLogin(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                return Json(new { error = "请输入完整信息" });
            }
            try
            {
                var user = UserService.Instance.Login(userName, password);
                return SetUser(user);
            }
            catch (ArgumentException e)
            {
                return Json(new { error = e.Message });
            }
            catch (UserNotExisitException)
            {
                return Json(new { error = "该用户名未注册" });
            }
            catch (Exception e)
            {
                Logger.Error("用户登录错误", e);
                return Json(new { error = "服务器内部错误" });
            }
        }
        public ActionResult Register(string mobile, string smsCode, string password, string realName)
        {
            try
            {
                var user = UserService.Instance.Register(mobile, smsCode, TIEM_Out, password, realName);
                return SetUser(user);
            }
            catch (FormatException e)
            {
                return Json(new { error = e.Message });
            }
            catch (ArgumentException e)
            {
                return Json(new { error = "验证码输入有误" });
            }
            catch (UserExisitException)
            {
                return Json(new { error = "该手机号已注册" });
            }
            catch (VerifyCodeExpireException)
            {
                return Json(new { error = "验证码已过期" });
            }
        }

        CompanyService cService = new CompanyService();

        public ActionResult UseCompanyRegister(UseCompany userCompany)
        {
            try
            {
                cService.Register(userCompany, ApiUser.Current);
                return Json(new { status = 0 });
            }
            catch (DataExistException e)
            {
                return Json(new { error = e.Message });
            }

        }

        public ActionResult ServiceCompanyRegister(ServiceCompany serviceCompany)
        {
            try
            {
                cService.Register(serviceCompany, ApiUser.Current);
                return Json(new { status = 0 });
            }
            catch (DataExistException e)
            {
                return Json(new
                {
                    error = e.Message
                });
            }
        }

        public ActionResult ResetPassword(string newPassword, string oldPassword)
        {
            try
            {
                UserService.Instance.ResetPassword(ApiUser.Current.UserId, oldPassword, newPassword);
                return Json(new { status = 0 });
            }
            catch (FormatException)
            {
                return Json(new { error = "密码格式不正确" });
            }
            catch (ArgumentException e)
            {
                return Json(new { error = e.Message });
            }
        }

        public ActionResult ForgetPassword(string mobile, string smsCode, string password)
        {
            try
            {
                UserService.Instance.ResetPasswordByVerifyCode(mobile, smsCode, TIEM_Out, password);
                return Json(new { status = 0 });
            }
            catch (FormatException)
            {
                return Json(new { error = "密码格式不正确" });
            }
            catch (ArgumentException e)
            {
                return Json(new { error = e.Message });
            }
        }

        public ActionResult ServiceCompanyUserList()
        {
            try
            {
                var users = UserService.Instance.ServiceCompanyUserList(ApiUser.Current.ServiceCompanyId ?? "");
                //返回维修企业员工当前身上的单子数量
                var userOrderList = UserService.Instance.ServerCompanyUserOrderNum(ApiUser.Current.ServiceCompanyId ?? "");
                return Json(users.Select(m => new
                {
                    name = m.RealName,
                    id = m.UserId,
                    phone = m.Phone,
                    num = (userOrderList.ContainsKey(m.UserId) ? userOrderList[m.UserId] : 0)
                }), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { error = e.Message });
            }

        }


        public ActionResult UseCompanyUserList()
        {
            try
            {
                var users = UserService.Instance.UseCompanyUserList(ApiUser.Current.ServiceCompanyId ??"");

                return Json(users.Select(m => new
                {
                    name = m.RealName,
                    id = m.UserId,
                    phone = m.Phone,

                }), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { error = e.Message });
            }


        }

        public ActionResult Info()
        {
            var current = new UserService().Get(ApiUser.Current.UserId);
            //获取经纬度
            var userPosition = new UserService().GetUserPosition(ApiUser.Current.UserId);
            return Json(new
            {
                id = current.UserId,
                name = current.RealName,
                email = current.Email,
                phone = current.Phone,
                sex = current.Sex,
                //使用单位信息是否激活(审核通过)
                status = current.UseCompany?.State.ToString()??"NotActive",
                //服务单位信息是否激活(审核通过)
                serviceStatus = current.ServiceCompany?.State.ToString()??"NotActive",
                useCompany = current.UseCompany?.Name,
                useCompanyId = current.UseCompany?.UseCompanyId,
                serviceCompany = current.ServiceCompany?.Name,
                serviceCompanyId = current.ServiceCompany?.ServiceCompanyId,
                type = current.RoleKey,
                longitude = userPosition?.Longitude,
                latitude = userPosition?.Latitude
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Exit(int appType)
        {
            Session.Remove("User");
            Response.Cookies["token"].Value = string.Empty;

            return Redirect("/Areas/H5/Content/dist/view/login.html?target=_blank_closecurrent&appType=" + appType);
        }
        /// <summary>
        /// Author:Gavin
        /// Create Date:2017-03-14
        /// Description:添加使用单位企业信息
        /// </summary>
        /// <param name="userCompany">使用单位实体</param>
        /// <returns></returns>
        public ActionResult CompanyInfo(UseCompany userCompany)
        {
            try
            {
                cService.AddCompanyInfo(userCompany, ApiUser.Current);
                return Json(new { status = 0 });
            }
            catch (DataExistException e)
            {
                return Json(new { error = e.Message });
            }
        }

        /// <summary>
        /// Author:Gavin
        /// Create Date:2017-03-14
        /// 添加服务单位企业信息
        /// </summary>
        /// <param name="serverCompany">服务单位实体</param>
        /// <returns></returns>
        public ActionResult ServiceCompany()
        {
            try
            {
                ServiceCompany serverCompany = JsonConvert.DeserializeObject<ServiceCompany>(Request.Form["servicecompany"].ToString());

                string fileName = string.Empty;
                HttpFileCollectionBase uploadFile = Request.Files;
                if (uploadFile.Count > 0)
                {
                    HttpPostedFileBase file = uploadFile[0];
                    string path = ConfigurationManager.AppSettings["repairImgPath"].ToString() + "/" + PhotoType.Server.ToString() + "/";

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    fileName = Guid.NewGuid().ToString("N") + "." + file.ContentType.ToString().Split('/')[1];
                    file.SaveAs(path + "/" + fileName);


                }

                serverCompany.ImgUrl = !string.IsNullOrEmpty(fileName) ? PhotoType.Server.ToString() + "/" + fileName : "";
                cService.AddServiceCompanyInfo(serverCompany, new Picture { Type = PhotoType.Server, Url = serverCompany.ImgUrl }, ApiUser.Current);
                return Json(new { status = 0 });
            }
            catch (DataExistException e)
            {
                return Json(new { error = e.Message });
            }
        }

        /// <summary>
        /// Author:Gavin
        /// Create Date:2017-03-16
        /// Description:获取使用(服务)单位信息
        /// </summary>
        /// <param name="appType">App类型</param>
        /// <returns></returns>
        public ActionResult GetCompanyInfo(AppType appType)
        {
            try
            {

                if (appType == AppType.Use)
                {
                    var model = cService.GetCompanyInfo(ApiUser.Current);
                    if (model != null)
                    {
                        return Json(new { model.Name, model.Phone, model.Position, model.Contact, model.Note, Id = model.UseCompanyId, State = model.State }, JsonRequestBehavior.AllowGet);
                    }
                }
                else if (appType == AppType.Service)
                {
                    var model = cService.GetServiceInfo(ApiUser.Current);
                    if (model != null)
                    {
                        return Json(new { model.Name, model.Phone, model.Position, model.Contact, model.Note, Id = model.ServiceCompanyId, State = model.State }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }



        }


        ///// <summary>
        ///// Author:Gavin
        ///// Create Date:2017-03-17
        ///// Description:申请加入服务(使用)单位
        ///// </summary>
        ///// <param name="appType">App类型</param>
        ///// <param name="id">UseCompanyId(ServiceCompanyId)</param>
        ///// <returns></returns>
        //public ActionResult AddCompany(string appType, long id)
        //{

        //    try
        //    {
        //        cService.AddCompany(ApiUser.Current, appType, id);
        //        return Json(new { status = 0 });
        //    }
        //    catch (DataExistException e)
        //    {
        //        return Json(new { error = e.Message });
        //    }
        //}

        ///// <summary>
        ///// Author:Gavin
        ///// Create Date:2017-03-17
        ///// Description:获取所有使用(服务)单位信息
        ///// </summary>
        ///// <param name="appType">App类型</param>
        ///// <returns></returns>
        //public ActionResult GetAllCompanyInfo(string appType) {

        //    try
        //    {
        //        if (appType == "1")
        //        {
        //            var model = cService.GetAllCompanyInfo(ApiUser.Current);
        //            if (model != null)
        //            {
        //                return Json(model, JsonRequestBehavior.AllowGet);
        //            }
        //        }
        //        else if (appType == "2")
        //        {
        //            var model = cService.GettAllServiceInfo(ApiUser.Current);
        //            if (model != null)
        //            {

        //                return Json(model, JsonRequestBehavior.AllowGet);
        //            }
        //        }
        //        return Json(null, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(null, JsonRequestBehavior.AllowGet);
        //    }

        //}

        /// <summary>
        /// 获取类目列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCategoryList()
        {
            try
            {
                return Json(new CategorySvr().GetList(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult AddServiceCompany(ServiceCompany serverCompany)
        {
            try
            {
                cService.AddServiceCompanyInfo(serverCompany, ApiUser.Current);
                var userInfo = new UserService().Get(ApiUser.Current.UserId);
                if (!string.IsNullOrEmpty(serverCompany.ImgUrl))
                    new PictureService().UpdatePicOuterId(serverCompany.ImgUrl,userInfo.ServiceCompanyId);

                return Json(new { status = 0 });
            }
            catch (DataExistException e)
            {
                return Json(new { error = e.Message });
            }
        }

        #region 人员管理
        /// <summary>
        /// 用户经纬度
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UserPosition(UserPosition position)
        {

            if (new UserService().UserPosition(position.UserId, position.Longitude, position.Latitude, position.Creatime) > 0)
                return Json(new { success = "操作成功" });
            else
                return Json(new { error = "操作失败" });
        }
        //定位模块
        //获取公司下面的人员信息
        public ActionResult GetUserList(AppType appType)
        {
            var userList = new UserService().GetUserList(appType, ApiUser.Current.UserId);
            var userIdArr = userList.Select(t => t.UserId).ToArray();
            var userPosition = new UserService().GetUserPositionList(userIdArr);
            var dicUserPosition = userPosition.ToLookup(t => t.UserId).ToDictionary(t => t.Key, t => t.First());
            //获取订单数量
            var mainOrderNum = new OrderService().GetOrderNum(appType, userIdArr);
            return Json(
                userList.Select(t => new
                {
                    UserId = t.UserId,
                    t.RealName,
                    t.NickName,
                    t.Phone,
                    Longitude = (dicUserPosition.ContainsKey(t.UserId) ? dicUserPosition[t.UserId].Longitude : ""),
                    Latitude = (dicUserPosition.ContainsKey(t.UserId) ? dicUserPosition[t.UserId].Latitude : ""),
                    Num = (mainOrderNum.ContainsKey(t.UserId) ? mainOrderNum[t.UserId] : "0"),
                    PositionTime = (dicUserPosition.ContainsKey(t.UserId) ? dicUserPosition[t.UserId].Creatime.ToString() : String.Empty),
                    t.RoleKey,
                    t.UserState
                }), JsonRequestBehavior.AllowGet);
        }

        //更新用户与公司的关系
        [HttpPost]
        public ActionResult UpdateUserState(UserState userState, string userId,AppType? type)
        {
            if (type == null)
                type = AppType.Service;
            if (new UserService().UpdateUserState(userState, userId, type) > 0)
            {
                return Json(new { success = "操作成功!" });
            }
            else {
                return Json(new { error = "操作失败!" });
            }
        }

        /// <summary>
        /// 获取用户坐标集合
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public ActionResult GetUserPositionList(string userId, DateTime beginTime, DateTime endTime)
        {

            var list = new UserService().GetUserPositionList(userId, beginTime, endTime);
            if (list.Count > 0)
            {
                var result = list.GroupBy(t => new { t.Latitude, t.Longitude })
                     .ToLookup(t => t.Key)
                     .ToDictionary(m => m.Key, m => m.Last())
                    .Select(m =>
                    {
                        var inTime = m.Value.Last().Creatime;
                        return new
                        {
                            m.Key.Longitude,
                            m.Key.Latitude,
                            Creatime = inTime.HasValue ? inTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : ""
                        };
                    })
                    .OrderByDescending(t => t.Creatime).ToArray();
                return Json(new { list = result }, JsonRequestBehavior.AllowGet);
            }
            else {
                return Json(list = null, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        #region 微信注册
        /// <summary>
        /// 微信注册
        /// update 2017-06-07 新增用户名和公司名
        /// </summary>
        /// <param name="strName">用户名</param>
        /// <param name="strCompanyName">公司名</param>
        /// <param name="strPhone">手机号码</param>
        /// <param name="strSmsCode">验证码</param>
        /// <returns></returns>
        public ActionResult WxLogin(string strName,string strCompanyName,string strPhone, string strSmsCode)
        {

            try
            {
                var flag = SmsLogService.Instance.CheckVerifyCode(strPhone, strSmsCode, TIEM_Out);
                var user = UserService.Instance.GetByPhone(strPhone);
                user.RealName = user.NickName = strName;
                user.CompanyName = strCompanyName;
                UserService.Instance.Edit(user);
               
                return SetUser(user);
            }
            catch (FormatException e)
            {
                return Json(new { error = e.Message });
            }
            catch (ArgumentException e)
            {
                return Json(new { error = "验证码输入有误" });
            }
            catch (UserNotExisitException)
            {
                //用户不存在,注册
                var user = UserService.Instance.WxRegister(strName,strCompanyName,strPhone, strSmsCode, TIEM_Out);
                return SetUser(user);
            }
            catch (UserExisitException)
            {
                return Json(new { error = "该手机号已注册" });
            }
            catch (VerifyCodeExpireException)
            {
                return Json(new { error = "验证码已过期" });
            }
        }
        #endregion


        /// <summary>
        /// 用户信息的字段
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ActionResult EditField(string userId, string fieldName, string value)
        {
            User user = UserService.Instance.Get(userId);
            var prop = typeof(User).GetProperty(fieldName);
            var type = prop.PropertyType;
            //判断convertsionType是否为nullable泛型类
            if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                //如果type为nullable类，声明一个NullableConverter类，该类提供从Nullable类到基础基元类型的转换
                System.ComponentModel.NullableConverter nullableConverter = new System.ComponentModel.NullableConverter(type);
                //将type转换为nullable对的基础基元类型
                type = nullableConverter.UnderlyingType;
                if (string.IsNullOrEmpty(value))
                {
                    prop.SetValue(user, null);
                }
                else
                {
                    prop.SetValue(user, Convert.ChangeType(value, type), null);
                }
            }
            else
            {
                var val = Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
                prop.SetValue(user, val);
            }
            UserService.Instance.Edit(user);
            return Json(new { status = 0 });
        }

    }
}

