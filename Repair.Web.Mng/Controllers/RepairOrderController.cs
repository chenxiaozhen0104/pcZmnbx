using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using System.Data.Entity;
using LZY.BX.Model;
using LZY.BX.Model.Enum;
using LZY.BX.Model.PageModel;
using LZY.BX.Service;
using LZY.BX.Service.Mb;
using Repair.Web.Mng.Menu;
using Repair.Web.Mng.Models;

namespace Repair.Web.Mng.Controllers
{
    public class RepairOrderController : BaseController
    {
        [MenuDefault("订单管理", PArea = "", Icon = "fa-tasks")]
        public void Index1() { }

        // GET: /RepairOrder/

        /// <summary>
        /// 订单列表
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [MenuCurrent("订单列表", PAction = "Index1", Icon = "fa-tasks")]
        public ActionResult Index(string key = null)
        {
            
            var pageIndex = Request["pageIndex"].AsInt(1);
            var pageSize = Request["pageSize"].AsInt(10);
            var user = CurrentUser;
            ViewData["serviceUser"] = TechnicalUserSvr.Instance.GetList(user.UserId);

            using (var db = new MbContext())
            {

                var repairOrders = db.MainOrder.Include(m => m.Device)
                    .Include(m => m.ServiceCompany)
                    .Include(m=>m.User)
                    .Include(m => m.UseCompany).ToList();
                    ;
                
                var pager = new Page<MainOrder>
                {
                    Index = pageIndex,
                    Size = pageSize,
                    Count = repairOrders.Count,
                    Data = repairOrders.OrderByDescending(m => m.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList()
                };

                return View(pager);
            }


        }

        /// <summary>
        /// 管理员派单
        /// </summary>
        /// <param name="repairOrderId"></param>
        /// <param name="serviceCompanyUserId"></param>
        /// <returns></returns>
        [MenuHide("派单", PAction = "Index", Icon = "fa-tasks")]
        [HttpPost]
        public ActionResult SendOrder(long repairOrderId = 0, long serviceCompanyUserId = 0)
        {
            var user = CurrentUser;
            var result = -1;
            using (var db = new MbContext())
            {
                var repairOrder = db.RepairOrder.FirstOrDefault(x => x.RepairOrderId == repairOrderId);
                #region 写入日志
                var serviceUser = db.User.FirstOrDefault(x => x.UserId == serviceCompanyUserId);
                var log = new RepairOrderLog()
                {
                    UserId = serviceCompanyUserId,
                    RepairOrderId = repairOrderId,
                    UserType = UserType.SvcCompanyUser.ToString(),
                    Content = string.Format("管理员派单"),
                    CreateTime = DateTime.Now
                };
                db.RepairOrderLog.AddOrUpdate(log);
                db.SaveChanges();
                #endregion

                //指派维修工
                if (repairOrder != null)
                {
                    repairOrder.State = RepairState.Sended.ToString();
                    repairOrder.LastStateUpdateTime = DateTime.Now;
                    db.RepairOrder.AddOrUpdate(repairOrder);
                    db.SaveChanges();
                }
                var serviceOrder = db.ServiceCompanyRepairOrder.FirstOrDefault(x => x.RepairOrderId == repairOrderId);
                if (serviceOrder != null)
                {
                    serviceOrder.State = RepairState.Sended.ToString();
                    serviceOrder.UserId = serviceCompanyUserId;
                    serviceOrder.Phone = serviceUser.Phone;
                    serviceOrder.Contact = serviceUser.RealName;
                    serviceOrder.LastStateUpdateTime = DateTime.Now;
                    db.ServiceCompanyRepairOrder.AddOrUpdate(serviceOrder);
                    result = db.SaveChanges();
                }
                var useCompanyOrder = db.UseCompanyRepairOrder.FirstOrDefault(x => x.RepairOrderId == repairOrderId);
                if (useCompanyOrder != null)
                {
                    useCompanyOrder.State = RepairState.Sended.ToString();
                    useCompanyOrder.LastStateUpdateTime = DateTime.Now;
                    db.UseCompanyRepairOrder.AddOrUpdate(useCompanyOrder);
                    db.SaveChanges();
                }
                var content = "管理员派单";
                RepairSvr.Instance.CreateMassage(repairOrderId, content, serviceUser.UserId, serviceUser.RealName, serviceCompanyUserId, serviceUser.RealName, false);


            }

            if (result > 0)
            {
                return Content("<script>alert('操作成功');window.location.href='/RepairOrder/Index'</script>");
            }
            return Content("<script>alert('操作失败');window.location.href='/RepairOrder/Index'</script>");
        }

        /// <summary>
        /// 订单详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [MenuHide("订单详情", PAction = "Index1", Icon = "fa-tasks")]
        public ActionResult OrderInfo(long id)
        {
            using (var db = new MbContext())
            {
                var user = CurrentUser;
                var model = db.RepairOrder.FirstOrDefault(x => x.RepairOrderId == id);
                ViewData["UserId"] = user.UserId;
                if (model != null)
                {
                    ViewData["repairBooking"] = RepairSvr.Instance.GetRepairBooking(model.RepairOrderId);
                    ViewData["useCompanyUser"] = RepairSvr.Instance.GetUseCompanyUser(model.RepairOrderId);
                    ViewData["useCompany"] = RepairSvr.Instance.GetUseCompany(model.RepairOrderId);
                    ViewData["repairOrderLog"] = RepairOrderLogSvr.Instance.GetList(model.RepairOrderId);
                    ViewData["serviceCompany"] = RepairSvr.Instance.GetServiceCompany(model.RepairOrderId);
                    ViewData["serviceCompanyUser"] = RepairSvr.Instance.GetServiceCompanyUser(model.RepairOrderId);
                    ViewData["equipment"] = EquipmentSvr.Instance.GetItem(model.EquipmentId);
                }
                return View(model);
            }
        }

        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="repairOrderId"></param>
        /// <param name="userId"></param>
        /// <param name="cause"></param>
        /// <returns></returns>
        [MenuHide("取消订单", PAction = "OrderInfo", Icon = "fa-tasks")]
        [HttpPost]
        public ActionResult CancelOrder(long repairOrderId, long userId, string cause)
        {
            var note = string.Empty;
            var msg = string.Empty;
            var state = string.Empty;
            if (RepairSvr.Instance.CancelRepairOrder(repairOrderId, 0, userId, cause, out state, out msg))
            {
                return Content("<script>alert('操作成功');window.location.href='/RepairOrder/OrderInfo'</script>");
            }
            return Content("<script>alert('操作失败');window.location.href='/RepairOrder/OrderInfo'</script>");
        }


    }
}
