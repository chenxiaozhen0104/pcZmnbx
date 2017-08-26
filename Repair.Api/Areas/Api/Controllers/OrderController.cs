using GetuiServerApiSDK;
using log4net;
using LZY.BX.Model;
using LZY.BX.Model.Enum;
using LZY.BX.Service;
using LZY.BX.Service.Mb;
using LZY.BX.SMSManager;
using Repair.Api.Areas.Api.Utilities;
using Repair.Api.Areas.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Transactions;
using System.Web.Mvc;
using WeiXin;
using ZR;

namespace Repair.Api.Areas.Api.Controllers
{
    public class OrderController : ControllerApiBase
    {
        OrderService os = new OrderService();
        CompanyService cs = new CompanyService();
        DeviceService ds = new DeviceService();

        //
        // GET: /Api/Order/

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <returns></returns>
        public ActionResult Search(OrderState state, AppType atype, int pageIndex = 1, int pageSize = 20, long? AreaId = null)
        {
            try
            {
                List<long> AreaArray = new List<long>();
                if (!string.IsNullOrEmpty(ApiUser.Current.AreaName))
                    AreaArray = new List<string>((ApiUser.Current.AreaName).Split(',').Where(s => !String.IsNullOrEmpty(s)).ToList()).ConvertAll(i => long.Parse(i));
                var page = os.List(
                    ApiUser.Current.UserId,
                    state,
                    atype,
                    ApiUser.Current.ServiceCompanyId,
                    ApiUser.Current.UseCompanyId,
                    ApiUser.Current.RoleKey, AreaArray, pageIndex, pageSize, AreaId);

                return Json(new
                {
                    data = page.Data.Select(t => new
                    {
                        id = t.MainOrderId.ToString(),
                        deviceName = t.Device != null ? t.Device.Name : "",
                        deviceModel = t.Device != null ? t.Device.Model : "",
                        devicePosition = t.Device != null ? t.Device.Position : "",
                        level = t.Level,
                        type = t.Type,
                        state = t.State,
                        serviceUserId = t.ServiceUserId,
                        createTime = t.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        areaName = t.Area != null ? t.Area.Name : "",
                    }),
                    count = page.Count,
                    index = page.Index,
                    size = page.Size
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Logger.Error("查询工单信息错误", e);
                return Json(new { error = "服务器繁忙，请稍后再试试" });
            }
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Get(string id)
        {
            try
            {
                var order = os.Get(id);

                if (order == null)
                    return Json(new { error = "服务器繁忙，请稍后再试试" });

                //string imgPath = "";
                ////获取图片前缀
                //if (ConfigurationManager.AppSettings["IsLocal"].ToString() == "1")
                //{
                //    imgPath = ConfigurationManager.AppSettings["ImgUrl"].ToString();
                //}
                //else
                //{
                //    imgPath = ConfigurationManager.AppSettings["ImgUrlProduce"].ToString();
                //}
                return Json(new
                {
                    Id = order.MainOrderId.ToString(),
                    State = order.State,
                    Level = order.Level,
                    Type = order.Type,
                    Describe = string.IsNullOrWhiteSpace(order.Describe) ? "" : order.Describe,
                    CreateTime = order.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    ServiceUserId = order.ServiceUserId,
                    ServiceCompany = order.ServiceUser?.ServiceCompany?.Name,
                    ServiceUserName = order.ServiceUser?.RealName,
                    ServiceUserPhone = order.ServiceUser?.Phone ,
                    UserName = order.User?.RealName,
                    UserPhone = order.User?.Phone,
                    UserCompany = order.User?.UseCompany?.Name,
                    CategoryName = order.Device?.Category?.Name,
                    QRCode = order.Device?.QRCode,
                    AreaName = order.Area?.Name,
                    DevicePosition = order.Device?.Position,
                    DeviceId = order.Device?.DeviceId,
                    DeviceName = order.Device?.Name,
                    DeviceBrand = order.Device?.Brand?.Name,
                    companyname= order.Device?.UseCompanyName,
                    warrantytime = order.Device != null ? (order.Device.WarrantyTime!=null?order.Device.WarrantyTime.Value.ToString("yyyy-MM-dd"): ""):"",
                    DeviceManufacturer =
                    order.Device != null ?
                    (order.Device.Brand != null ?
                    (order.Device.Brand.Manufacturer != null ?
                    order.Device.Brand.Manufacturer.Name : "") : "") : "",
                    imgUrl=order.ImgUrlList.Select(t=>new { t.Url,t.PictureId,t.OuterId}).ToArray()
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Logger.Error("查询工单信息错误", e);
                return Json(new { error = "服务器繁忙，请稍后再试试" },
                    JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 创建工单
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public ActionResult Create(MainOrder order)
        {
            try
            {
                var areaId = ApiUser.Current.AreaId;

                if (order.DeviceId.HasValue && order.DeviceId > 0)
                {
                    var device = ds.Get(order.DeviceId.Value);
                    areaId = device != null ? device.AreaId : ApiUser.Current.AreaId;
                }

                order.UseCompanyId = ApiUser.Current.UseCompanyId;
                order.ServiceCompanyId = order.ServiceCompanyId ?? ApiUser.Current.ServiceCompanyId;
                order.UserId = ApiUser.Current.UserId;

                order.AreaId = areaId;
                order.MainOrderId = SequNo.NewId;
                order.LastEditTime = DateTime.Now;
                //判断RepairImages 是否为空 不为空要更新图片
                if (!string.IsNullOrEmpty(order.RepairImages))
                    new PictureService().UpdatePicOuterId(order.RepairImages, order.MainOrderId);


                var serviceCompany = cs.GetServiceCompany(order.ServiceCompanyId);

                var serviceUsers = new UserService().ServiceCompanyUserList(order.ServiceCompanyId);

                var flag = os.Create(order, ApiUser.Current, serviceCompany);

                if (flag)
                {
                    new Task(new Action(() =>
                    {
                        if (serviceUsers != null)
                        {
                            foreach (var item in serviceUsers)
                            {
                                if (item.RoleKey == UserType.SvcCompanyUserAdmin || item.RoleKey == (UserType.UseCompanyUserAdmin | UserType.SvcCompanyUserAdmin))
                                    SMSManager.Instance.SendNotify(item.Phone, order.MainOrderId.ToString());
                            }
                        }

                        //消息推送
                        var obj = new { title = "新订单提醒", content = string.Format("您有一个新的服务工单:{0}", order.MainOrderId), orderId = order.MainOrderId, page = string.Format("/orderDetail/{0}", order.MainOrderId) };

                        var tagList = new List<string> { serviceCompany.ServiceCompanyId.ToString(), "1" };
                        var resultMsg = PushMassageHelper.Push(Newtonsoft.Json.JsonConvert.SerializeObject(obj), tagList, PushMassageHelper.AppType.WX);

                        Logger.InfoFormat("个推推送结果{0}===={1}", serviceCompany.ServiceCompanyId, resultMsg);
                    })).Start();
                    //发送短信
                    //判断报修终端
                    if (order.RepairType == RepairType.Wx)
                    {
                        //发送公共号消息
                        new Task(new Action(() =>
                        {
                            var resultMsg = os.SendWxTmpMsg(order.UserId, order.MainOrderId, OrderState.Created);
                            Logger.InfoFormat("微信消息推送{0}===={1}", order.UserId, resultMsg);
                        })).Start();

                    }
                    return Json(new { success = "创建成功", id = order.MainOrderId.ToString() }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { error = "服务器繁忙，请稍后再试试" }
                        , JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                Logger.Error("创建工单异常", e);
                return Json(new { error = "服务器繁忙，请稍后再试试" }
                    , JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 创建工单
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>

        public ActionResult Dispatch(string orderId, long serviceUserId, string serviceUserName, string serviceUserPhone)
        {
            try
            {
                var userId = ApiUser.Current.UserId;

                string serviceCompanyId = "";
                string repairUserId ="";
                var repairType = RepairType.App;
                var flag = os.Dispatch(orderId, userId, serviceUserId, serviceUserName, serviceUserPhone, ref serviceCompanyId, ref repairUserId, ref repairType);

                if (flag)
                {

                    new Task(new Action(() =>
                    {
                        //发送短信
                        SMSManager.Instance.SendNotify(serviceUserPhone, orderId.ToString());

                        //消息推送
                        var obj = new { title = "新订单提醒", content = string.Format("您有一个新的服务工单:{0}", orderId), orderId = orderId, page = string.Format("/orderDetail/{0}", orderId) };

                        var tagList = new List<string> { serviceCompanyId.ToString(), serviceUserId.ToString() };
                        var resultMsg = PushMassageHelper.Push(Newtonsoft.Json.JsonConvert.SerializeObject(obj), tagList, PushMassageHelper.AppType.WX);

                        Logger.InfoFormat("个推推送结果{0}===={1}", serviceCompanyId, resultMsg);
                    })).Start();
                    //判断报修终端
                    if (repairType == RepairType.Wx)
                    {
                        //发送公共号消息
                        new Task(new Action(() =>
                        {
                            var resultMsg = os.SendWxTmpMsg(repairUserId, orderId, OrderState.Sended);
                            Logger.InfoFormat("微信消息推送{0}===={1}", orderId, resultMsg);
                        })).Start();

                    }
                    return Json(new { success = "派单成功" }
                        , JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { error = "服务器繁忙，请稍后再试试" }
                        , JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                Logger.Error("派单异常", e);
                return Json(new { error = "服务器繁忙，请稍后再试试" }
                    , JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 开始工作
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult Working(string orderId)
        {
            try
            {
                var userId = ApiUser.Current.UserId;
                string repairUserId ="";
                RepairType repairType = RepairType.App;
                var flag = os.Working(orderId, userId, ref repairUserId, ref repairType);
                //更新坐标轨迹
                new UserService().PositionAction(orderId, userId, PositionAction.Work);
                if (flag)
                {
                    //判断报修终端
                    if (repairType == RepairType.Wx)
                    {
                        //发送公共号消息
                        new Task(new Action(() =>
                        {
                            var resultMsg = os.SendWxTmpMsg(repairUserId, orderId, OrderState.Working);
                            Logger.InfoFormat("微信消息推送{0}===={1}", orderId, resultMsg);
                        })).Start();

                    }

                    return Json(new { success = "工作开始" }
                        , JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { error = "服务器繁忙，请稍后再试试" }
                        , JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                Logger.Error("开始工作异常", e);
                return Json(new { error = "服务器繁忙，请稍后再试试" }
                    , JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 提交工单状态操作
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="description"></param>
        /// <param name="state"></param>
        /// <param name="utype"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult SubmitState(string orderId, string description, OrderState state, decimal amount=0, int level = 5, long toUserId = 0, AppType? appType = null)
        {
            try
            {
                var flag = false;

                RepairType repairType = RepairType.App;
                string repairUserId = "";
                var userId = ApiUser.Current.UserId;
                var utype = ApiUser.Current.RoleKey;
                var userPhone = "";
                switch (state)
                {
                    case OrderState.Worked:
                        flag = os.Worked(orderId, userId, amount, utype, description, ref repairUserId, ref repairType,ref userPhone, appType);
                        //发短信
                        SMSManager.Instance.SendNotify(userPhone, orderId);
                        if (appType == AppType.Service)
                        {
                            //更新坐标轨迹
                            new UserService().PositionAction(orderId, userId, PositionAction.Worked);
                        }
                        break;
                    case OrderState.Cancel:
                        flag = os.Cancel(orderId, userId, utype, description, ref repairUserId, ref repairType, appType);
                        break;
                    case OrderState.Close:
                        flag = os.Close(orderId, userId, utype, description, appType);
                        break;
                    case OrderState.Unsolved:
                        if (appType == AppType.Use) {
                            flag = os.ResetWorking(orderId, userId, description, ref repairType,ref userPhone);
                            //SMSManager.Instance.SendNotify(userPhone, orderId);
                        }
                        else { 
                            flag = os.Unsolved(orderId, userId, utype, description, appType);
                        }
                        break;
                    case OrderState.Confirm:
                        flag = os.Confirm(orderId, userId, utype, description, appType);
                        break;
                    case OrderState.UseComment:
                        flag = os.UseComment(orderId, userId, utype, description, appType);

                        flag = new CommentService().Comments(userId, toUserId, orderId, description, level);
                        break;
                }

                if (flag)
                {
                    //判断报修终端
                    if (repairType == RepairType.Wx && appType == AppType.Service)
                    {
                        //发送公共号消息
                        new Task(new Action(() =>
                        {
                            var resultMsg = os.SendWxTmpMsg(repairUserId, orderId, state);
                            Logger.InfoFormat("微信消息推送{0}===={1}", orderId, resultMsg);
                        })).Start();

                    }

                    return Json(new { success = "操作成功" }
                        , JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { error = "服务器繁忙，请稍后再试试" }
                        , JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                Logger.Error("操作异常", e);
                return Json(new { error = "服务器繁忙，请稍后再试试" }
                    , JsonRequestBehavior.AllowGet);
            }
        }

        //转单
        public ActionResult Forward(string orderId, string serviceCompanyId)
        {
            try
            {
                var newOrderId = os.Forward(ApiUser.Current, orderId, serviceCompanyId);

                return Json(new { orderId = newOrderId.ToString() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Logger.Error("转单异常", e);
                return Json(new { error = "服务器繁忙，请稍后再试试" }
                    , JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 时间抽
        /// </summary>
        /// <returns></returns>
        public ActionResult Timeline(string orderId)
        {
            try
            {
                var model = os.Timeline(orderId);

                var last = model.First();

                return Json(model.Select(t => new
                {
                    CreateTime = t.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    Content = t.Content,
                    First = t.OrderLogId == last.OrderLogId ? "first" : "",
                }), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Logger.Error("查询时间抽信息错误", e);
                return Json(new { error = "服务器繁忙，请稍后再试试" });
            }
        }
        /// <summary>
        /// Author:Gavin
        /// Create Date:2017-04-12
        /// Description:获取单子状态的总数量
        /// </summary>
        /// AppType app类型
        /// <returns></returns>
        public ActionResult GetOrderStateNum(int? appType, long? AreaId)
        {
            try
            {
                var result = os.GetOrderStateNum(appType, ApiUser.Current, AreaId);
                return Json(result.Select(x => new { State = x.Key, Num = x.Value }), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.Error("查询时间抽信息错误", ex);
                return Json(new { error = ex.Message });
            }
        }


        #region 工单页面按区域搜索

        public ActionResult GetAreas()
        {
            var area = os.GetAreaList(ApiUser.Current);
            var areaArr = new AreaService().GetAreaList(area);
            return Json(areaArr.Select(t => new { t.AreaId, t.Name }), JsonRequestBehavior.AllowGet);
        }
        #endregion

        /// <summary>
        /// 微信创建工单
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public ActionResult WxCreate(MainOrder order)
        {
            try
            {
                //var areaId = ApiUser.Current.AreaId;
                var wxUser = Session["WxUser"] as WeiXinUser;

                var user = new AuthAccountServer().Get(wxUser.openid);

                Device Info = new Device();
                if (order.DeviceId.HasValue && order.DeviceId > 0)
                {
                    Info = ds.Get(order.DeviceId.Value);
                    if (Info.Position != order.Address)
                    {
                        Info.Position = order.Address;
                        DeviceService.Instance.Edit(Info);
                    }
                }
                else {
                    //创建设备
                    long deviceId = DeviceService.Instance.WxDeviceWrite(order);
                }
                order.UserId = user.User.UserId;
                order.AreaId = Info.AreaId;
                order.MainOrderId = SequNo.NewId;

                //判断RepairImages 是否为空 不为空要更新图片
                if (!string.IsNullOrEmpty(order.RepairImages))
                    new PictureService().UpdatePicOuterId(order.RepairImages, order.MainOrderId);

                var serviceCompany = cs.GetServiceCompany(order.ServiceCompanyId);

                var serviceUsers = new UserService().ServiceCompanyUserList(order.ServiceCompanyId);

                var flag = os.Create(order, user.User, serviceCompany);

                if (flag)
                {
                    //判断包修人员信息有没有更改
                    if (order.ContactsPhone != user.User.Phone || order.Contacts != user.User.RealName || order.UseCompanyName != user.User.CompanyName)
                    {
                        new Task(new Action(() =>
                        {
                            user.User.Phone = order.ContactsPhone;
                            user.User.RealName = order.Contacts;
                            user.User.CompanyName = order.UseCompanyName;
                            UserService.Instance.Edit(user.User);
                        })).Start();
                    }

                    new Task(new Action(() =>
                    {
                        if (serviceUsers != null)
                        {
                            foreach (var item in serviceUsers)
                            {
                                if (item.RoleKey == UserType.SvcCompanyUserAdmin || item.RoleKey == (UserType.UseCompanyUserAdmin | UserType.SvcCompanyUserAdmin))
                                    SMSManager.Instance.SendNotify(item.Phone, order.MainOrderId.ToString());
                            }
                        }

                        //消息推送
                        var obj = new { title = "新订单提醒", content = string.Format("您有一个新的服务工单:{0}", order.MainOrderId), orderId = order.MainOrderId, page = string.Format("/orderDetail/{0}", order.MainOrderId) };

                        var tagList = new List<string> { serviceCompany.ServiceCompanyId.ToString(), "1" };
                        var resultMsg = PushMassageHelper.Push(Newtonsoft.Json.JsonConvert.SerializeObject(obj), tagList, PushMassageHelper.AppType.WX);

                        Logger.InfoFormat("个推推送结果{0}===={1}", serviceCompany.ServiceCompanyId, resultMsg);
                    })).Start();
                    //发送短信
                    //判断报修终端
                    if (order.RepairType == RepairType.Wx)
                    {
                        //发送公共号消息
                        new Task(new Action(() =>
                        {
                            var resultMsg = os.SendWxTmpMsg(order.UserId, order.MainOrderId, OrderState.Created);
                            Logger.InfoFormat("微信消息推送{0}===={1}", order.UserId, resultMsg);
                        })).Start();

                    }
                    return Json(new { success = "创建成功", id = order.MainOrderId.ToString() }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { error = "服务器繁忙，请稍后再试试" }
                        , JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                Logger.Error("创建工单异常", e);
                return Json(new { error = "服务器繁忙，请稍后再试试" }
                    , JsonRequestBehavior.AllowGet);
            }



        }

        /// <summary>
        /// 微信查询工单
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public ActionResult WxSearch(OrderState state, AppType atype, int pageIndex = 1, int pageSize = 20, long? AreaId = null)
        {
            try
            {
                List<long> AreaArray = new List<long>();
                var WxInfo = AuthAccountServer.Instance.Get((Session["WxUser"] as WeiXinUser).openid);
                var page = os.UserOrderList(WxInfo.User, state, pageIndex, pageSize);

                return Json(new
                {
                    data = page.Data.Select(t => new
                    {
                        id = t.MainOrderId.ToString(),
                        deviceName = t.Device != null ? t.Device.Name : "",
                        deviceModel = t.Device != null ? t.Device.Model : "",
                        devicePosition = t.Device != null ? t.Device.Position : "",
                        level = t.Level,
                        type = t.Type,
                        state = t.State,
                        serviceUserId = t.ServiceUserId,
                        createTime = t.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        areaName = t.Area != null ? t.Area.Name : "",
                    }),
                    count = page.Count,
                    index = page.Index,
                    size = page.Size
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Logger.Error("查询工单信息错误", e);
                return Json(new { error = "服务器繁忙，请稍后再试试" });
            }
        }

        /// <summary>
        /// Author:Gavin
        /// Create Date:2017-05-24
        /// Description:微信获取单子状态的总数量
        /// </summary>
        /// AppType app类型
        /// <returns></returns>
        public ActionResult WxGetOrderStateNum(int? appType, long? AreaId)
        {
            try
            {
                var WxInfo = AuthAccountServer.Instance.Get((Session["WxUser"] as WeiXinUser).openid);

                var result = os.GetOrderStateNum(appType, WxInfo.User, AreaId);
                return Json(result.Select(x => new { State = x.Key, Num = x.Value }), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.Error("查询时间抽信息错误", ex);
                return Json(new { error = ex.Message });
            }
        }

        /// <summary>
        /// 微信手工单
        /// 2017-05-26
        /// </summary>
        public ActionResult WxHandWorkOrder(MainOrder mainOrder)
        {
            long deviceId = DeviceService.Instance.WxDeviceWrite(mainOrder);
            if (deviceId > 0)
            {
                //创建新工单
                var wxUser = Session["WxUser"] as WeiXinUser;
                var user = new AuthAccountServer().Get(wxUser.openid);
                mainOrder.MainOrderId = SequNo.NewId;
                mainOrder.DeviceId = deviceId;
                mainOrder.UserId = user.User.UserId;
                ServiceCompany serviceCompany = CompanyService.Instance.GetServiceCompany(mainOrder.ServiceCompanyId);
                var serviceUsers = UserService.Instance.ServiceCompanyUserList(mainOrder.ServiceCompanyId);
                //判断RepairImages 是否为空 不为空要更新图片
                if (!string.IsNullOrEmpty(mainOrder.RepairImages))
                    new PictureService().UpdatePicOuterId(mainOrder.RepairImages, mainOrder.MainOrderId);

                var flag = OrderService.Instance.Create(mainOrder, serviceCompany);
                if (flag)
                {
                    //判断包修人员信息有没有更改
                    if (mainOrder.ContactsPhone != user.User.Phone || mainOrder.Contacts != user.User.RealName || mainOrder.UseCompanyName != user.User.CompanyName)
                    {
                        new Task(new Action(() =>
                        {
                            user.User.Phone = mainOrder.ContactsPhone;
                            user.User.RealName = mainOrder.Contacts;
                            user.User.CompanyName = mainOrder.UseCompanyName;
                            UserService.Instance.Edit(user.User);
                        })).Start();
                    }

                    new Task(new Action(() =>
                    {
                        if (serviceUsers != null)
                        {
                            foreach (var item in serviceUsers)
                            {
                                if (item.RoleKey == UserType.SvcCompanyUserAdmin || item.RoleKey == (UserType.UseCompanyUserAdmin | UserType.SvcCompanyUserAdmin))
                                    SMSManager.Instance.SendNotify(item.Phone, mainOrder.MainOrderId.ToString());
                            }
                        }

                        //消息推送
                        var obj = new { title = "新订单提醒", content = string.Format("您有一个新的服务工单:{0}", mainOrder.MainOrderId), orderId = mainOrder.MainOrderId, page = string.Format("/orderDetail/{0}", mainOrder.MainOrderId) };

                        var tagList = new List<string> { serviceCompany.ServiceCompanyId.ToString(), "1" };
                        var resultMsg = PushMassageHelper.Push(Newtonsoft.Json.JsonConvert.SerializeObject(obj), tagList, PushMassageHelper.AppType.WX);

                        Logger.InfoFormat("个推推送结果{0}===={1}", serviceCompany.ServiceCompanyId, resultMsg);
                    })).Start();
                    //发送短信
                    //判断报修终端
                    if (mainOrder.RepairType == RepairType.Wx)
                    {
                        //发送公共号消息
                        new Task(new Action(() =>
                        {
                            var resultMsg = os.SendWxTmpMsg(mainOrder.UserId, mainOrder.MainOrderId, OrderState.Created);
                            Logger.InfoFormat("微信消息推送{0}===={1}", mainOrder.UserId, resultMsg);
                        })).Start();

                    }
                    return Json(new { success = "创建成功", id = mainOrder.MainOrderId.ToString() }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { error = "服务器繁忙，请稍后再试试" }
                        , JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { error = "服务器繁忙，请稍后再试试" }
                       , JsonRequestBehavior.AllowGet);
            }
        }



        #region 工作中状态超过48时的短信提示


        public void DoWorkOverTimeTips()
        {
            SendWoekingOverTimeInfo();
            AutoWorkedTask();
            Timer t = new Timer(3600000);//1000=1S
            t.Elapsed += new ElapsedEventHandler(t_Elapsed);
            t.AutoReset = true;
            t.Enabled = true;
        }



        private static void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            using (var mb = new MbContext())
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    new Task(new Action(() =>
                        SendWoekingOverTimeInfo()
                    )).Start();
                    new Task(new Action(() =>
                      AutoWorkedTask()
                  )).Start();
                }
            }
        }
        //超过48小时的自动确认
        private static void AutoWorkedTask()
        {
            ILog log = LogManager.GetLogger("AutoWorkedTask");
            try
            {
                using (var mb = new MbContext())
                    {
                        using (TransactionScope scope = new TransactionScope())
                        {
                            //获取订单数量
                            var workedList = mb.Database.SqlQuery<MainOrder>(@"SELECT mo.* from mainorder  mo
                            left join orderlog  ol
                            on mo.MainOrderId=ol.MainOrderId and mo.State=ol.State
                            where mo.State=64 and datediff(NOW(),ol.CreateTime)>=2;").ToList();

                            if (workedList.Count > 0)
                            {
                                //超时的订单编号
                                var mainOrderIdArr = "(\"" + String.Join("\",\"", workedList.Select(m => m.MainOrderId).ToArray()) + "\")";
                                //要更新的数据
                                mb.Database.ExecuteSqlCommand(string.Format(@"update mainorder  set State=1024 where MainOrderId in {0}", mainOrderIdArr));
                                //往OrderLog里面更新数据
                                List<OrderLog> infos = new List<OrderLog>();
                                foreach (var item in workedList)
                                {
                                    infos.Add(new OrderLog
                                    {
                                        MainOrderId = item.MainOrderId,
                                        UserId = item.UserId,
                                        State = OrderState.Confirm,
                                        Content = "[用户]已确认工单,完成",
                                        CreateTime = DateTime.Now
                                    });
                                }
                                mb.OrderLog.AddRange(infos);
                                mb.SaveChanges();
                                scope.Complete();
                                log.Info("AutoWorkedTask:--" + DateTime.Now + "--已经更新未确认的订单数量:" + workedList.Count.ToString() + "条");
                            }
                            else
                            {
                                log.Info("AutoWorkedTask:--" + DateTime.Now + "--没有超过48小时的订单");
                            }
                        }
                    }
            }
            catch (Exception ex)
            {
                log.Info("AutoWorkedTask:--" + DateTime.Now + "--" + ex.Message);
            }

        }
        //messageTip 超过48小时短信提示
        private static void SendWoekingOverTimeInfo()
        {
            ILog log = LogManager.GetLogger("SendWoekingOverTimeInfo");
            try
            {
                if (DateTime.Now.Hour >= 7 && DateTime.Now.Hour <= 21)
                {
                    using (var mb = new MbContext())
                    {
                        using (TransactionScope scope = new TransactionScope())
                        {
                            //获取订单数量
                            var workArr = mb.Database.SqlQuery<MainOrder>(@"SELECT mo.* from mainorder  mo
                            left join orderlog  ol on mo.MainOrderId = ol.MainOrderId and mo.State = ol.State  
                            where(mo.State = 16 or mo.State = 32) and mo.SMSNotification=0 and datediff(NOW(), ol.CreateTime) >= 2;").ToList();
                            var userIdArr = workArr.Select(t => t.ServiceUserId).ToArray();
                            var userPhoneArr = mb.User.Where(t => userIdArr.Contains(t.UserId)).GroupBy(t => t.Phone).Select(t => t.Key).ToArray();
                            if (workArr.Count > 0)
                            {
                                //发短信
                                new Task(new Action(() =>
                                {
                                    foreach (var item in userPhoneArr)
                                    {
                                        SMSManager.Instance.WorkingNotify(item, "维修人员");
                                    }
                                })).Start();
                                //发送之后更新SMSNotification
                                var mainOrderIdArr = "(\"" + String.Join("\",\"", workArr.Select(t => t.MainOrderId).ToArray()) + "\")";
                                mb.Database.ExecuteSqlCommand(string.Format(@"update mainorder  set SMSNotification=1 where MainOrderId in {0}", mainOrderIdArr));
                                mb.SaveChanges();
                                scope.Complete();
                                log.Info("SendWoekingOverTimeInfo:" + DateTime.Now + "---更新了" + workArr.Count.ToString() + "条提示信息工单");
                            }
                            else
                            {
                                log.Info("SendWoekingOverTimeInfo:" + DateTime.Now + "---更新了0条提示信息工单");
                            }
                        }
                    }
                }
                else {
                    log.Info("SendWoekingOverTimeInfo:维修人员已经休息,不发送短信提示");
                }
            }
            catch (Exception ex)
            {
                log.Info("SendWoekingOverTimeInfo:" + DateTime.Now + "---" + ex.Message);
            }

        }
        #endregion
    }
}
