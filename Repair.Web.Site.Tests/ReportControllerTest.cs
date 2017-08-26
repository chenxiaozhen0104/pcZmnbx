
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using System.Linq;
using LZY.BX.Service.Mb;
using System.Collections.Generic;
using LZY.BX.Model.Enum;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System;
using System.Data.Entity.Infrastructure;
using LZY.BX.Service;
using Repair.Web.Site.Areas.User.Models;

namespace Repair.Web.Site.Tests
{
    [TestClass]
    public class ReportControllerTest
    {
        [TestMethod]
        public void DerviceListTest()
        {
            ReportDeviceQueryModel query = new ReportDeviceQueryModel();
            string UseCompanyId = "636065437685331832";

            using (var db = new MbContext())
            {
                var device = db.Device
                        .Include(t => t.UseCompany)
                            .Include(t => t.Brand)
                            .Include(t => t.Brand.Manufacturer)
                            .Include(t => t.Area)
                            .Include(t => t.Category)
                    .Where(t => t.UseCompanyId == UseCompanyId).ToList();

                var result = db.MainOrder
                          .Where(t => t.UseCompanyId == UseCompanyId)
                          .GroupBy(t => t.DeviceId)
                          .Select(t => new
                          {
                              deviceId = t.Key,
                              num = t.Count().ToString()
                          }).ToList().ToDictionary(x => x.deviceId, x => x.num); ;




            }
        }

        [TestMethod]
        public void DispatchTest()
        {
            string UseCompanyId = "636065437685331832";
            string UseId = "636094466835819764";
            UserType roleKey = UserType.UseCompanyUserAdmin;
            using (var db = new MbContext())
            {
                //获取订单列表
                var orderList = db.MainOrder
                                .Include(x => x.ServiceCompany)
                                .Where(x => x.UseCompanyId == UseCompanyId && (roleKey == UserType.UseCompanyUser ? x.UserId == UseId : x.MainOrderId == x.MainOrderId)).ToList();
                ////服务商
                //var serverCompany = orderList.Select(t => t.ServiceCompanyId).ToList();
                //var dicServerCompany = db.ServiceCompany.Where(t => serverCompany.Contains(t.ServiceCompanyId)).ToLookup(t => t.ServiceCompanyId)
                //    .ToDictionary(x => x.Key, x => x.First());
                var orderLogList = (from o in db.OrderLog
                                    where db.MainOrder.Where(m => m.UseCompanyId == UseCompanyId).Select(m => m.MainOrderId).ToList().Contains(o.MainOrderId)
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
            }
        }

        [TestMethod]
        public void StartWork()
        {
            string UseCompanyId = "636065437685331832";
            string UseId = "636094466835819764";
            UserType roleKey = UserType.UseCompanyUserAdmin;
            using (var db = new MbContext())
            {
                //获取订单列表
                var orderList = db.MainOrder
                                .Include(x => x.ServiceCompany)
                                .Where(x => x.UseCompanyId == UseCompanyId && (roleKey == UserType.UseCompanyUser ? x.UserId == UseId : x.MainOrderId == x.MainOrderId)).ToList();
                var count = orderList.Count();
                ////服务商
                //var serverCompany = orderList.Select(t => t.ServiceCompanyId).ToList();
                //var dicServerCompany = db.ServiceCompany.Where(t => serverCompany.Contains(t.ServiceCompanyId)).ToLookup(t => t.ServiceCompanyId)
                //    .ToDictionary(x => x.Key, x => x.First());
                var orderLogList = (from o in db.OrderLog
                                    where db.MainOrder.Where(m => m.UseCompanyId == UseCompanyId).Select(m => m.MainOrderId).ToList().Contains(o.MainOrderId)
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
                count = query.Count();
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
                                TotalMinutes = t.EndTime > t.BeginTime && DateTime.Parse(t.BeginTime.ToString()).Year != 0 ? (DateTime.Parse(t.EndTime.ToString()) - DateTime.Parse(t.BeginTime.ToString())).TotalMinutes : 0,
                                CreateTime = t.o.CreateTime,
                            }).GroupBy(t => new
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
            }
        }


        [TestMethod]
        public void CompositeDataTest()
        {
            using (var mb = new MbContext())
            {
                var user = UserService.Instance.Get("636234887666745830");
                var list = new OrderService().GetRepairList(AppType.Service, user, Convert.ToDateTime("2016-02-13"), Convert.ToDateTime("2017-06-13"));
                //获取人员ID,状态以及数量
                var userCountDic = list.Where(t => !string.IsNullOrEmpty(t.ServiceUserId)).OrderBy(t => t.ServiceUserId).GroupBy(t => new { t.ServiceUserId, t.State })
                    .Select(t => new { ServiceUserId = t.Key.ServiceUserId, State = t.Key.State, Num = t.Count() }).ToList();

                //获取人员ID集合
                var userIdArr = userCountDic.Where(m => !String.IsNullOrEmpty(m.ServiceUserId)).Select(m => m.ServiceUserId).Distinct().ToArray();
                var userInfoArr = mb.User.Where(t => userIdArr.Contains(t.UserId)).OrderBy(t => t.CreateTime).ToList();
                userIdArr = userInfoArr.Select(t => t.UserId).ToArray();
                var userNmaeArr = userInfoArr.Select(t => t.NickName).ToArray();
                //获取每个人进行中的，已完成和未完成的个数集合
                //(8 + 16 + 32 + 64 )进行中 128 未解决 1024 已完成 //其他
                //进行中
                var orderIng = userCountDic
                    .Where(t => (OrderState.Sending | OrderState.Sended | OrderState.Working | OrderState.Worked).HasFlag(t.State))
                    .GroupBy(t => t.ServiceUserId).Select(m => new { ServiceUserId = m.Key, Num = m.Sum(t => t.Num) })
                    .ToDictionary(t => t.ServiceUserId, t => t.Num);
                //已完成
                var ordered = userCountDic.Where(t => (OrderState.Confirm.HasFlag(t.State) || OrderState.UseComment.HasFlag(t.State)))
                     .GroupBy(t => t.ServiceUserId).Select(m => new { ServiceUserId = m.Key, Num = m.Sum(t => t.Num) })
                    .ToDictionary(t => t.ServiceUserId, t => t.Num);
                //未解决
                var orderUnComplated = userCountDic.Where(t => OrderState.Unsolved.HasFlag(t.State))
                      .GroupBy(t => t.ServiceUserId).Select(m => new { ServiceUserId = m.Key, Num = m.Sum(t => t.Num) })
                     .ToDictionary(t => t.ServiceUserId, t => t.Num);


                //从orderlog当中获取 开始工作日期以及结束日期
                var beginArr = mb.OrderLog.Where(t => userIdArr.Contains(t.UserId) && t.State == OrderState.Working)
                                 .OrderBy(t => t.CreateTime).GroupBy(t => t.MainOrderId).ToLookup(t => t.Key)
                                 .ToDictionary(t => t.Key, t => t.FirstOrDefault().Select(m => new { m.CreateTime, m.UserId }));

                var endArr = mb.OrderLog.Where(t => userIdArr.Contains(t.UserId) && t.State == OrderState.Worked)
                              .OrderByDescending(t => t.CreateTime).GroupBy(t => t.MainOrderId).ToLookup(t => t.Key)
                              .ToDictionary(t => t.Key, t => t.FirstOrDefault().Select(m => new { m.CreateTime, m.UserId }));

                var workTimeArr = (from v in beginArr
                               join e in endArr
                              on v.Key equals e.Key
                               select new
                               {
                                   v.Key,
                                   UserId = v.Value.Select(m => m.UserId).First(),
                                   workTime = (e.Value.Select(m => m.CreateTime).First() - v.Value.Select(m => m.CreateTime).First()).TotalMinutes
                               }
                              ).ToList();

                var timeArr = workTimeArr.GroupBy(t => t.UserId).
                      Select(m => new
                      {
                          m.Key,
                          totalTime =m.Sum(t =>t.workTime)
                      }).ToDictionary(t => t.Key, t => t.totalTime);
            }
        }

        [TestMethod]
        public void UpdateCategoryTest() {
            var arr = new long[] { 1918, 1917 };
            using (var mb = new MbContext())
            {

                var list = mb.Device.Where(t => arr.Contains(t.DeviceId)).ToList();
                list.ForEach(item => item.CategoryId = 1);
                 mb.SaveChanges();

            }
        }
    }

    public class MainOrderTime
    {

        public string MainOrderId { get; set; }
        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
