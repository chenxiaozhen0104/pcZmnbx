using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.WebPages;
using LZY.BX.Model;
using LZY.BX.Model.Enum;
using LZY.BX.Service.Mb;
using Repair.Web.Site.Areas.Sp.Models;
using Repair.Web.Site.Utilities;
using LZY.BX.Service;
using LZY.BX.SMSManager;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Web;
using System.Configuration;
using System.IO;
using ZR;

namespace Repair.Web.Site.Areas.Sp.Controllers
{
    public class OrderController : ControllerBase<OrderController>
    {
        //
        // GET: /Sp/Order/
        OrderService os = new OrderService();
        DeviceService ds = new DeviceService();
        [HttpPost]
        public ActionResult RealtTimeOrderCount()
        {
            //using (var db = new MbContext())
            //{
            //    var query = new OrderQueryModel();
            //    query.ServiceCompanyId = CurrentUser.ServiceCompany.ServiceCompanyId;
            //    query.State = RepairState.Sending.ToString();
            //    query.CreateTimeLess = DateTime.Now;
            //    query.CreateTimeGreaterEqual = DateTime.Now.AddMonths(-2);

            //    //DOTO 服务工单
            //    var serviceOrderCount = db.ServiceCompanyRepairOrder
            //        .Where(query)
            //        .Count();

            //    return Content(serviceOrderCount.ToString("000"));
            //}
            return Content("");
        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Order()
        {
            return View();
        }

        /// <summary>
        /// 订单列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public ActionResult OrderList(OrderQueryModel query, string keywords)
        {
            query.RoleKey = CurrentUser.User.RoleKey;
            //TODO 获取当前用户单位ID
            using (var db = new MbContext())
            {
                query.ServiceCompanyId = CurrentUser.User.ServiceCompanyId;
                List<long> AreaArray = new List<string>((CurrentUser.User.AreaName ?? "").Split(',').Where(s => !String.IsNullOrEmpty(s)).ToList()).ConvertAll(i => long.Parse(i));
                //订单列表
                var MainOrderData = os.PcGetAllOrderList(
                    CurrentUser.User.UserId,
                    query.State,
                    AppType.Service,
                    CurrentUser.User.ServiceCompanyId,
                    CurrentUser.User.UseCompanyId,
                    CurrentUser.User.RoleKey, AreaArray);

                if (!string.IsNullOrEmpty(keywords))
                {
                    MainOrderData = MainOrderData.Where(x => x.MainOrderId.ToString().Contains(keywords)).ToList();
                }
                if (MainOrderData.Count > 0)
                {
                    //获取报修人员信息
                    var userCid = MainOrderData.Select(t => t.UserId).ToList();
                    query.UserDic = db.User.Where(x => userCid.Contains(x.UserId))
                        .ToLookup(t => t.UserId)
                        .ToDictionary(x => x.Key, x => x.First());

                    //获取报修单位信息
                    var UseCompanyDic = MainOrderData.Select(t => t.UseCompanyId).ToList();
                    query.UseCompanyDic = db.UseCompany.Where(x => UseCompanyDic.Contains(x.UseCompanyId))
                        .ToLookup(t => t.UseCompanyId)
                        .ToDictionary(x => x.Key, x => x.First());

                    //获取维修人员信息
                    var ServeUserDic = MainOrderData.Select(t => t.ServiceUserId).ToList();
                    query.ServeUserDic = db.User.Where(x => ServeUserDic.Contains(x.UserId))
                        .ToLookup(t => t.UserId)
                        .ToDictionary(x => x.Key, x => x.First());
                    //设备信息
                    var DeviceDic = MainOrderData.Select(t => t.DeviceId).ToList();
                    query.DeviceDic = db.Device.Where(x => DeviceDic.Contains(x.DeviceId))
                        .ToLookup(t => t.DeviceId)
                        .ToDictionary(x => x.Key, x => x.First());
                    query.MainOrderData = MainOrderData;
                    //报修订单数量
                    query.PageInfo.TotalCount = query.MainOrderData.Count;
                }
            }

            return View(query);
        }

        /// <summary>
        /// 派单页面
        /// </summary>
        /// <param name="id">订单编号</param>
        /// <returns></returns>
        [HttpGet]
        public ViewResult DispatchOrder(long id)
        {
            //todo 获取当前用户单位ID
            //TODO　当前用户所在单位下的用户列表
            using (var db = new MbContext())
            {
                var userIds = db.ServiceCompanyUser
                    .Where(x => x.ServiceCompanyId == CurrentUser.User.ServiceCompanyId)
                    .Select(x => x.UserId)
                    .ToArray();
                var query = new DispatchOrderQueryModel
                {
                    RepairOrderId = id,
                    UserList = db.User.Where(x => userIds.Contains(x.UserId)).ToList()
                };
                return View(query);
            }


        }
        /// <summary>
        /// 派单
        /// </summary>
        /// <param name="RepairOrderId">订单编号</param>
        /// <param name="ServiceUserId">指派人员Id</param>
        /// <param name="UserName">指派人员姓名</param>
        /// <param name="UserPhone">指派人员手机号码</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DispatchOrderToUser(string RepairOrderId, long ServiceUserId, string UserName, string UserPhone)
        {
            try
            {
                string repairUserId = "";
                var repairType = RepairType.App;

                string sCompanyId = "";
                var dlgInf = new DlgConfirmInfo();
                var flag = os.Dispatch(RepairOrderId, CurrentUser.User.UserId, ServiceUserId, UserName, UserPhone, ref sCompanyId, ref repairUserId, ref repairType);
                if (flag)
                {
                    dlgInf.Title = "成功";
                    dlgInf.Content = "派单成功";
                    dlgInf.Type = InfoType.Success;
                    //发送短信

                    SMSManager.Instance.SendNotify(UserPhone, RepairOrderId.ToString());
                    //判断报修终端
                    if (repairType == RepairType.Wx)
                    {
                        //发送公共号消息

                        new Task(new Action(() =>
                        {
                            var resultMsg = os.SendWxTmpMsg(repairUserId, RepairOrderId, OrderState.Created);
                            Logger.InfoFormat("微信消息推送{0}===={1}", RepairOrderId, resultMsg);
                        })).Start();

                    }
                }
                else
                {
                    dlgInf.Title = "错误";
                    dlgInf.Content = "服务器繁忙，请稍后再试试.";
                    dlgInf.Type = InfoType.Error;
                }
                return View("dlg_confirm_info", dlgInf);
            }
            catch (Exception e)
            {
                Logger.Error("派单异常", e);

                return ResultSuccess("服务器繁忙，请稍后再试试.");
            }
        }
        /// <summary>
        /// 开始工作
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns></returns>
        public ActionResult Working(string orderId)
        {
            try
            {

                var userId = CurrentUser.User.UserId;
                RepairType repairType = RepairType.App;
                string repairUserId = "";
                var flag = os.Working(orderId, userId, ref repairUserId, ref repairType);

                if (flag)
                {
                    //判断报修终端
                    if (repairType == RepairType.Wx)
                    {
                        //发送公共号消息
                        new Task(new Action(() => {
                            var resultMsg = os.SendWxTmpMsg(repairUserId, orderId, OrderState.Working);
                            Logger.InfoFormat("微信消息推送{0}===={1}", orderId, resultMsg);
                        })).Start();

                    }


                    return ResultSuccess("操作成功.");
                }
                else
                {
                    return ResultSuccess("操作失败.");
                }

            }
            catch (Exception e)
            {
                Logger.Error("派单异常", e);

                return ResultSuccess("服务器繁忙，请稍后再试试.");
            }
        }

        /// <summary>
        /// 结束工作
        /// </summary>
        /// <returns></returns>
        public ActionResult SubmitState(string orderId, string description, OrderState state)
        {
            try
            {
                var flag = false;
                RepairType repairType = RepairType.App;
                string repairUserId = "";
                var userId = CurrentUser.User.UserId;
                var utype = CurrentUser.User.RoleKey;
                var userPhone = "";
                switch (state)
                {
                    case OrderState.Worked:
                        flag = os.Worked(orderId, userId,0, utype, description,ref repairUserId, ref repairType, ref userPhone);
                        //发短信
                        //SMSManager.Instance.SendNotify(userPhone, orderId);
                        break;
                    case OrderState.Cancel:
                        flag = os.Cancel(orderId, userId, utype, description, ref repairUserId, ref repairType);
                        break;
                    case OrderState.Close:
                        flag = os.Close(orderId, userId, utype, description);
                        break;
                    case OrderState.Unsolved:
                        flag = os.Unsolved(orderId, userId, utype, description);
                        break;
                    case OrderState.Confirm:
                        flag = os.Confirm(orderId, userId, utype, description);
                        break;
                }

                if (flag)
                {
                    //判断报修终端
                    if (repairType == RepairType.Wx )
                    {
                        //发送公共号消息
                        new Task(new Action(() => {
                            var resultMsg = os.SendWxTmpMsg(repairUserId, orderId, state);
                            Logger.InfoFormat("微信消息推送{0}===={1}", orderId, resultMsg);
                        })).Start();

                    }

                    return ResultSuccess("操作成功.");
                }
                else
                {
                    return ResultSuccess("服务器繁忙，请稍后再试试");
                }
            }
            catch (Exception e)
            {
                Logger.Error("操作异常", e);
                return ResultSuccess("服务器繁忙，请稍后再试试");
            }
        }


        public ActionResult OrderWrite() {
            return View();
        }
        /// <summary>
        /// 区域控件
        /// </summary>
        /// <returns></returns>
        public ActionResult select_area_control()
        {
            using (var db = new MbContext())
            {
                var model = db.Area
                            .OrderBy(t => t.Name)
                            .ToList();

             

                return View(model);
            }
        }

        /// <summary>
        /// 类目控件
        /// </summary>
        /// <returns></returns>
        public ActionResult select_category_control()
        {
            using (var db = new MbContext())
            {
                var model = db.Category
                            .OrderBy(t => t.Name)
                            .ToList();
                return View(model);
            }
        }

        /// 品牌控件
        /// </summary>
        /// <returns></returns>
        public ActionResult select_brand_control()
        {
            using (var db = new MbContext())
            {
                var model = db.Brand
                            .OrderBy(t => t.Name)
                            .ToList();

             

                return View(model);
            }
        }

        /// <summary>
        /// 申请成为服务商用户
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OrderWrite(MainOrder mainOrder)
        {
            try
            {
                    HttpFileCollectionBase uploadFile = Request.Files;
                    List<Picture> pInfo = new List<Picture>();
                    if (uploadFile.Count > 0)
                    {
                        for (int i = 0; i < uploadFile.Count; i++)
                        {
                            HttpPostedFileBase file = uploadFile[i];
                            string path = ConfigurationManager.AppSettings["repairImgPath"].ToString() + "/" + PhotoType.Device.ToString() + "/";

                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }
                            string fileName = Guid.NewGuid().ToString("N") + "." + file.ContentType.ToString().Split('/')[1];
                            file.SaveAs(path + "/" + fileName);

                            pInfo.Add(new Picture
                            {
                                Type = PhotoType.Device,
                                Url = PhotoType.Device.ToString() + "/" + fileName,
                                CreateTime = DateTime.Now
                            });
                        }
                    }
                mainOrder.ServiceCompanyId = CurrentUser.User.ServiceCompanyId;
                long deviceId = new DeviceService().DeviceWrite(mainOrder);
                if (deviceId > 0)
                {
                    //上传图片
                    if (pInfo.Count != 0)
                    {
                        using (var db = new LZY.BX.Service.Mb.MbContext())
                        {
                            foreach (Picture item in pInfo)
                            {
                                item.OuterId = deviceId.ToString();
                                item.CreateTime = DateTime.Now;
                                db.Picture.Add(item);
                            }
                            db.SaveChanges();
                        }
                    }
                    //创建新工单
                    mainOrder.MainOrderId = SequNo.NewId;
                    mainOrder.ServiceCompanyId = CurrentUser.User.ServiceCompanyId;
                    mainOrder.DeviceId = deviceId;
                    if (new OrderService().Create(mainOrder, CurrentUser.User.ServiceCompany))
                        return ResultSuccess("工单已经录入成功", Url.Action("Order", "Order"));
                    else
                        return ResultSuccess("工单录入失败", Url.Action("Order", "OrderWrite"));

                }
                else
                {
                    return ResultSuccess("工单录入失败", Url.Action("Order", "OrderWrite"));
                }
                
                
            }
            catch (DataExistException ex)
            {

                return ResultError(ex.Message);
            }

        }
    }
}
