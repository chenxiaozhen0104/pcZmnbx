using LZY.BX.Service.Mb;
using Repair.Web.Site.Areas.User.Models;
using Repair.Web.Site.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using LZY.BX.Model.Enum;

namespace Repair.Web.Site.Areas.User.Controllers
{
    public class ReportController : ControllerBase<OrderController>
    {
        //
        // GET: /User/Report/

        /// <summary>
        /// 派单平均时间
        /// </summary>
        /// <returns></returns>
        public ActionResult Dispatch(Int32? keywords)
        {
            Int32 year = DateTime.Now.Year;
            if (keywords != null)
            {
                year = (Int32)keywords;
            }

            using (var db = new MbContext()) {
                ViewData["Year"] = year;
                //获取订单列表
                var orderList = db.MainOrder
                             .Include(x => x.ServiceCompany)
                             .Where(x => x.UseCompanyId == CurrentUser.User.UseCompanyId && (CurrentUser.User.RoleKey == UserType.UseCompanyUser ? x.UserId == CurrentUser.User.UserId : x.MainOrderId == x.MainOrderId) && x.CreateTime.Year == year).ToList();


                var orderLogList = (from o in db.OrderLog
                                    where db.MainOrder.Where(m => m.UseCompanyId == CurrentUser.User.UseCompanyId).Select(m => m.MainOrderId).ToList().Contains(o.MainOrderId)
                                    where (o.State == OrderState.Sended || o.State == OrderState.Sending)
                                    select o
                           );
                var query = from t in orderLogList
                            group t by t.MainOrderId into m
                            select new MainOrderTime
                            {
                                MainOrderId = m.Key,
                                BeginTime = m.Max(n => (n.State == OrderState.Sending ? n.CreateTime : DateTime.MinValue)),
                                EndTime = m.Max(n => (n.State == OrderState.Sended ? n.CreateTime : DateTime.MinValue))
                            };
                var result = (from o in orderList
                              from q in query
                              where q.MainOrderId == o.MainOrderId
                              select new { o, q.BeginTime, q.EndTime }
                           )
                           .AsEnumerable()
                           .Select(t => new
                           {
                               ServiceCompanyId = t.o.ServiceCompanyId,
                               ServiceCompanyName = t.o.ServiceCompany.Name,
                               OrderId = t.o.MainOrderId,
                               TotalMinutes = t.EndTime > t.BeginTime ? (DateTime.Parse(t.EndTime.ToString()) - DateTime.Parse(t.BeginTime.ToString())).TotalMinutes : 0,
                               CreateTime = t.o.CreateTime,
                           })
                             .GroupBy(t => new
                             {
                                 t.ServiceCompanyName,
                                 Time = t.CreateTime.Month
                             })
                             .Select(t => new DispatchReportModel
                             {
                                 Time = t.Key.Time,
                                 ServiceCompanyName = t.Key.ServiceCompanyName,
                                 AvgTime = t.Average(z => z.TotalMinutes).ToString("f2"),
                                 Count = t.Count()
                             })
                            .ToLookup(t => t.ServiceCompanyName)
                           .ToDictionary(x => x.Key, x => x.ToList());

                return View(result);

            }
            //using (var db = new MbContext())
            //{
            //    var useOrder = db.UseCompanyRepairOrder
            //                    .Where(t => t.UseCompanyId == CurrentUser.UseCompany.UseCompanyId)
            //                    .Where(t => t.CreateTime < DateTime.Now);

            //    var repairOrderIds = useOrder.Select(t => t.RepairOrderId).ToList();

            //    var created = db.RepairOrderLog
            //             .Where(t => repairOrderIds.Contains(t.OrderId))
            //             .Where(t => t.Content == "发起新维修工单");

            //    var dispatch = db.RepairOrderLog
            //             .Where(t => repairOrderIds.Contains(t.OrderId))
            //             .Where(t => t.Content == "管理员派单");

            //    var serviceOrder = from order in db.ServiceCompanyRepairOrder
            //                       from company in db.ServiceCompany
            //                       where order.ServiceCompanyId == company.ServiceCompanyId
            //                       select new
            //                       {
            //                           order.RepairOrderId,
            //                           company.ServiceCompanyId,
            //                           company.Name
            //                       };

            //    var result = (from c in created
            //                  from d in dispatch
            //                  from service in serviceOrder
            //                  from use in useOrder
            //                  where c.OrderId == d.OrderId
            //                  where service.RepairOrderId == c.OrderId
            //                  where use.RepairOrderId == c.OrderId
            //                  select new { BeginTime = c.CreateTime, EndTime = d.CreateTime, OrderId = c.OrderId, ServiceCompanyId = service.ServiceCompanyId, ServiceCompanyName = service.Name, use })
            //              .AsEnumerable()
            //              .Select(t => new
            //               {
            //                   ServiceCompanyId = t.ServiceCompanyId,
            //                   ServiceCompanyName = t.ServiceCompanyName,
            //                   OrderId = t.OrderId,
            //                   TotalMinutes = (t.EndTime - t.BeginTime).TotalMinutes,
            //                   CreateTime = t.use.CreateTime,
            //               })
            //               .GroupBy(t => new
            //               {
            //                   t.ServiceCompanyName,
            //                   Time = type == 1 ? t.CreateTime.Month : t.CreateTime.Day
            //               })
            //               .Select(t => new DispatchReportModel
            //                {
            //                    Time = t.Key.Time,
            //                    ServiceCompanyName = t.Key.ServiceCompanyName,
            //                    AvgTime = t.Average(z => z.TotalMinutes).ToString("f2"),
            //                    Count = t.Count()
            //                })
            //                .ToLookup(t => t.ServiceCompanyName)
            //                .ToDictionary(x => x.Key, x => x.ToList());

            //    return View(result);
            //}
        }

        /// <summary>
        /// 开始工作平均时间
        /// </summary>
        /// <returns></returns>
        public ActionResult StartWork(Int32 ? keywords)
        {
            Int32 year = DateTime.Now.Year;
            if (keywords != null)
            {
                year = (Int32)keywords;
            }

            using (var db = new MbContext())
            {
                ViewData["Year"] = year;
                //获取订单列表
                var orderList = db.MainOrder
                             .Include(x => x.ServiceCompany)
                             .Where(x => x.UseCompanyId == CurrentUser.User.UseCompanyId && (CurrentUser.User.RoleKey == UserType.UseCompanyUser ? x.UserId == CurrentUser.User.UserId : x.MainOrderId == x.MainOrderId) && x.CreateTime.Year == year).ToList();


                var orderLogList = (from o in db.OrderLog
                                    where db.MainOrder.Where(m => m.UseCompanyId == CurrentUser.User.UseCompanyId).Select(m => m.MainOrderId).ToList().Contains(o.MainOrderId)
                                    where (o.State == OrderState.Working || o.State == OrderState.Worked || o.State == OrderState.Unsolved)
                                    select o
                           );
                var query = from t in orderLogList
                            group t by t.MainOrderId into m
                            select new MainOrderTime
                            {
                                MainOrderId = m.Key,
                                BeginTime = m.Max(n => (n.State == OrderState.Working ? n.CreateTime : DateTime.MinValue)),
                                EndTime = m.Max(n => ((n.State == OrderState.Worked || n.State == OrderState.Unsolved) ? n.CreateTime : DateTime.MinValue))
                            };
                var result = (from o in orderList
                              from q in query
                              where q.MainOrderId == o.MainOrderId
                              select new { o, q.BeginTime, q.EndTime }
                           )
                           .AsEnumerable()
                           .Select(t => new
                           {
                               ServiceCompanyId = t.o.ServiceCompanyId,
                               ServiceCompanyName = t.o.ServiceCompany.Name,
                               OrderId = t.o.MainOrderId,
                               TotalMinutes = (t.EndTime > t.BeginTime && DateTime.Parse(t.BeginTime.ToString()).Year != 0) ? (DateTime.Parse(t.EndTime.ToString()) - DateTime.Parse(t.BeginTime.ToString())).TotalMinutes : 0,
                               CreateTime = t.o.CreateTime,
                           })
                             .GroupBy(t => new
                             {
                                 t.ServiceCompanyName,
                                 Time = t.CreateTime.Month
                             })
                             .Select(t => new WorkIngReportModel
                             {
                                 Time = t.Key.Time,
                                 ServiceCompanyName = t.Key.ServiceCompanyName,
                                 AvgTime = t.Average(z => z.TotalMinutes).ToString("f2"),
                                 Count = t.Count()
                             })
                            .ToLookup(t => t.ServiceCompanyName)
                           .ToDictionary(x => x.Key, x => x.ToList());

                return View(result);

            }
           
        }

        /// <summary>
        /// 设备故障数
        /// </summary>
        /// <returns></returns>
        public ActionResult Dervice()
        {
            return View();
        }

        /// <summary>
        /// 设备列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public ActionResult DerviceList(ReportDeviceQueryModel query,Int32? keywords)
        {
           
            Int32 year = DateTime.Now.Year;
            if (keywords != null)
            {
                year = (Int32)keywords;
            }
            using (var db = new MbContext())
            {
                query.deviceData= db.Device
                         .Include(t => t.UseCompany)
                             .Include(t => t.Brand)
                             .Include(t => t.Brand.Manufacturer)
                             .Include(t => t.Area)
                             .Include(t => t.Category)
                     .Where(t => t.UseCompanyId == CurrentUser.User.UseCompanyId).ToList();

                query.deviceCount = db.MainOrder
                          .Where(t => t.UseCompanyId == CurrentUser.User.UseCompanyId&&t.CreateTime.Year == year&&t.DeviceId>0)
                          .GroupBy(t => t.DeviceId)
                          .Select(t => new {
                              deviceId = t.Key,
                              num = t.Count().ToString()
                          }).ToDictionary(x => x.deviceId, x => x.num);

                //设备数量
                query.PageInfo.TotalCount = query.deviceData.Count();
            }

            return View(query);
        }
    }
}
