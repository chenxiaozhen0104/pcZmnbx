using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LZY.BX.Model.Enum;
using LZY.BX.Service.Mb;
using Repair.Web.Site.Areas.User.Models;
using Repair.Web.Site.Utilities;
using LZY.BX.Utilities;
using LZY.BX.Model;
using ZR;
using LZY.BX.Service;
using System.Threading.Tasks;

namespace Repair.Web.Site.Areas.User.Controllers
{
    public class OrderController : ControllerBase<OrderController>
    {
        //
        // GET: /User/Order/
        OrderService os = new OrderService();
        DeviceService ds = new DeviceService();
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
                query.ServiceCompanyId = CurrentUser.User.UseCompanyId;
                List<long> AreaArray = new List<string>((CurrentUser.User.AreaName ?? "").Split(',').Where(s => !String.IsNullOrEmpty(s)).ToList()).ConvertAll(i => long.Parse(i));
                //订单列表
                var MainOrderData = os.PcGetAllOrderList(
                    CurrentUser.User.UserId,
                    query.State,
                    AppType.Use,
                    CurrentUser.User.ServiceCompanyId,
                    CurrentUser.User.UseCompanyId,
                    CurrentUser.User.RoleKey, AreaArray);
                if (!string.IsNullOrEmpty(keywords)) { 
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
        /// 催单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Reminder(long id)
        {
            //var dlgInf = new DlgConfirmInfo();

            //var listPushUser = new List<GetuiPushMessage.Client>();

            //using (var db = new MbContext())
            //{
            //    var uIds = db.ServiceCompanyRepairOrder.FirstOrDefault(t => t.RepairOrderId == id);
            //    if (uIds == null)
            //    {
            //        return ResultError("操作失败,未找到服务人员");
            //    }
            //    var pushModel = db.PushAuthAccount.FirstOrDefault(t => uIds.UserId == t.UserId);

            //    if (pushModel != null)
            //    {
            //        listPushUser.Add(new GetuiPushMessage.Client
            //        {
            //            ChannelId = pushModel.ClientId,
            //            DeviceToken = pushModel.DeviceToken,
            //            DeviceType = pushModel.TerminalType
            //        });
            //    }
            //    db.Massage.Add(new Massage()
            //    {
            //        MassageId = SequNo.NewId,
            //        RepairOrderId = id,
            //        Content = "用户催单",
            //        SendUserId = CurrentUser.User.UserId,
            //        SendUserName = CurrentUser.User.RealName,
            //        ReceiveUserId = uIds.UserId,
            //        ReceiveUserName = uIds.Contact,
            //        CreateTime = DateTime.Now
            //    });

            //    db.SaveChanges();
            //}

            //PushMassageHelper.ReminderOrder(id, listPushUser);

            //todo 调用个推接口发送推送消息给维修单位
            return ResultSuccess("操作成功");
        }

        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Cancel(string id)
        {
            var dlgInf = new DlgConfirmInfo();
            using (var db = new MbContext())
            {
                
                return ResultError("操作失败");
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
                        flag = os.Worked(orderId, userId,0, utype, description, ref repairUserId, ref repairType,ref userPhone);
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
                    if (repairType == RepairType.Wx)
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
    }
}
