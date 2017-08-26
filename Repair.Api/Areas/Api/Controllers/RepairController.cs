
using LZY.BX.Model.Enum;
using LZY.BX.Service;
using Repair.Api.Areas.Api.Utilities;

using System;
using System.Web.Mvc;
using System.Data.Entity;
using System.Linq;
using System.Collections;
using LZY.BX.Service.Mb;
using System.Collections.Generic;
using Repair.Api.Areas.Utilities;

namespace Repair.Api.Areas.Api.Controllers
{
    /// <summary>
    /// 报修订单
    /// </summary>
    public class RepairController : ControllerApiBase
    {
        ///// <summary>
        ///// 报修网关
        ///// </summary>
        ///// <resultMsg></resultMsg>
        //[HttpPost]
        //public ActionResult Repair()
        //{
        //    var param =
        //        Request.RequestType == "POST" ? Request.Form : Request.QueryString;

        //    if (param.Count <= 0 && Request.RequestType == "POST")
        //    {
        //        param = Request.QueryString;
        //    }
        //    else if (param.Count <= 0 && Request.RequestType == "GET")
        //    {
        //        param = Request.Form;
        //    }

        //    var resultMsg = GetResult(param);

        //    if (resultMsg.GetValue("ReturnCode").Equals(ResultMsg.ResultCode.Fail.ToString()))
        //    {
        //        Logger.WarnFormat("<---------01--------->接口访问错误：{0}", param);
        //    }
        //    return Json(resultMsg.ToHashtable());
        //}


        /// <summary>
        /// 类目报修统计
        /// </summary>
        /// <param name="appType">appType类型</param>        
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public ActionResult CategoryCount(AppType appType, DateTime beginTime, DateTime endTime)
        {
            var list = new OrderService().GetRepairList(appType, ApiUser.Current, beginTime, endTime);

            //获取设备数量
            var deviceCountDic = list.Where(t => t.DeviceId > 0).AsEnumerable().GroupBy(t => t.DeviceId)
                .Select(t => new { DeviceId = t.Key, Num = t.Count() })
                .ToDictionary(t => t.DeviceId, t => t.Num);
            //设备和类目
            var deviceCateDic = list.Where(t => t.DeviceId > 0).Select(t => new { t.Device.DeviceId, t.Device.CategoryId }).ToLookup(t => t.DeviceId).ToDictionary(t => t.Key, t => t.First());

            //类目
            var cateCountDic = new Dictionary<long, int>();
            foreach (var item in deviceCountDic)
            {
                if (deviceCateDic.ContainsKey((long)item.Key))
                {
                    if (cateCountDic.ContainsKey((long)deviceCateDic[(long)item.Key].CategoryId))
                    {
                        cateCountDic[(long)deviceCateDic[(long)item.Key].CategoryId] = cateCountDic[(long)deviceCateDic[(long)item.Key].CategoryId] + item.Value;
                    }
                    else
                    {
                        cateCountDic.Add((long)deviceCateDic[(long)item.Key].CategoryId, item.Value);
                    }
                }
            }
            //类目编号
            cateCountDic = cateCountDic.OrderBy(t=>t.Key).ToDictionary(t=>t.Key,t=>t.Value);
            var cateIdArr = cateCountDic.Select(t=>t.Key).ToArray();
            //获取类目名称
            using (var mb = new MbContext())
            {
                var cateNameArr = mb.Category.Where(t => cateIdArr.Contains(t.CategoryId)).Select(t => t.Name).ToArray();
                //类目数量
                var cateArr = cateCountDic.Values.ToArray();

                return Json(new { yAxis = cateNameArr, seriesData = cateArr }, JsonRequestBehavior.AllowGet);
            }

        }
        /// <summary>
        /// 区域报修统计
        /// </summary>
        /// <param name="appType"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public ActionResult AreaCount(AppType appType, DateTime beginTime, DateTime endTime) {

            var list = new OrderService().GetRepairList(appType, ApiUser.Current, beginTime, endTime);

            //获取区域数量
            var areaCountDic = list.Where(t => t.AreaId > 0).AsEnumerable().GroupBy(t => t.AreaId)
                .Select(t => new { AreaId = t.Key, Num = t.Count() })
                .ToDictionary(t => t.AreaId, t => t.Num);

            //排序
            areaCountDic = areaCountDic.OrderBy(t => t.Key).ToDictionary(t => t.Key, t => t.Value);
            using (var mb = new MbContext())
            {
                //区域名称
                var areaArr = areaCountDic.Keys.ToArray();
                var areaName = mb.Area.Where(t => areaArr.Contains(t.AreaId)).ToList().Select(t => t.Name).ToArray();
                return Json(new { yAxis = areaName, seriesData = areaCountDic.Values.ToArray() }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 派单响应时间
        /// </summary>
        /// <param name="appType"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public ActionResult RepairdTimeCount(AppType appType, string year) {
            using (var db = new MbContext())
            {
                int nowYear = DateTime.Now.Year;
                int.TryParse(year, out nowYear);
                //获取订单列表
                var orderList = db.MainOrder
                              .Include(x => x.ServiceCompany)
                             .Where(x => x.UseCompanyId == ApiUser.Current.UseCompanyId && (ApiUser.Current.RoleKey == UserType.UseCompanyUser ? x.UserId == ApiUser.Current.UserId : x.MainOrderId == x.MainOrderId) && x.CreateTime.Year == nowYear && x.ServiceCompany != null).ToList();

                var MainOrderIdArr = orderList.Where(t=>!(string.IsNullOrEmpty(t.ServiceCompanyId))).Select(t => t.MainOrderId).ToArray();
                var orderLogList = db.OrderLog.Where(t => MainOrderIdArr.Contains(t.MainOrderId) && (t.State == OrderState.Sended || t.State == OrderState.Sending)).ToList();

                var listTotalMinutes = orderLogList.GroupBy(t => t.MainOrderId)
                                       .Select(m => new
                                       {
                                           MainOrderId = m.Key,
                                           BeginTime = m.Max(t => (t.State == OrderState.Sending ? t.CreateTime : DateTime.MinValue)),
                                           EndTime = m.Max(t => (t.State == OrderState.Sended ? t.CreateTime : DateTime.MinValue))
                                       })
                                       .Select(t => new {
                                           MainOrderId = t.MainOrderId,
                                           TotalMinutes = (t.EndTime > t.BeginTime ? (DateTime.Parse(t.EndTime.ToString()) - DateTime.Parse(t.BeginTime.ToString())).TotalMinutes : 0)
                                       }).ToList();

                var res = (from o in orderList
                           from l in listTotalMinutes
                           where l.MainOrderId == o.MainOrderId 
                         
                           select new { o, l.TotalMinutes }
                         ).AsEnumerable()
                         .Where(t => !string.IsNullOrEmpty(t.o.ServiceCompanyId) && t.o.ServiceCompany!=null)
                         .Select(t => new
                         {
                             ServiceCompanyId = t.o.ServiceCompanyId,
                             ServiceCompanyName = t.o.ServiceCompany.Name,
                             OrderId = t.o.MainOrderId,
                             TotalMinutes = t.TotalMinutes,
                             CreateTime = t.o.CreateTime,
                         })
                          .GroupBy(t => new
                          {
                              t.ServiceCompanyName,
                              t.CreateTime.Month
                          })
                           .Select(m => new
                           {
                               Time = m.Key.Month,
                               ServiceCompanyName = m.Key.ServiceCompanyName,
                               AvgTime = Convert.ToDouble(m.Average(z => z.TotalMinutes).ToString("f2")),
                               Count = m.Count()
                           })
                           .ToLookup(t => t.ServiceCompanyName)
                           .ToDictionary(x => x.Key, x => x.ToList());

                ArrayList re = new ArrayList();
                foreach (var item in res.Keys)
                {
                    var arr = res[item].ToArray();
                    double[] data = new double[12];

                    for (int i = 1; i < 13; i++)
                    {
                        for (int j = 0; j < arr.Length; j++)
                        {
                            if (i == arr[j].Time)
                            {
                                data[i - 1] = arr[j].AvgTime;
                                break;
                            }
                            else
                                data[i - 1] = 0;
                        }
                    }
                    re.Add(new { name = item, type = "bar", data });
                }
                return Json(new { series = re, lengData = res.Keys.ToArray() }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 维修数量按照人员统计(以及状态)
        /// </summary>
        /// <param name="appType"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public ActionResult RepartUserCount(AppType appType, DateTime beginTime, DateTime endTime)
        {

            var list = new OrderService().GetRepairList(appType, ApiUser.Current, beginTime, endTime);
            //获取人员ID,状态以及数量
            var userCountDic = list.Where(t => !string.IsNullOrEmpty(t.ServiceUserId)).OrderBy(t => t.ServiceUserId).GroupBy(t => new { t.ServiceUserId, t.State })
                .Select(t => new { ServiceUserId = t.Key.ServiceUserId, State = t.Key.State, Num = t.Count() }).ToList();

            using (var mb = new MbContext())
            {
                //获取人员ID集合
                var userIdArr = userCountDic.Select(m => m.ServiceUserId).Distinct().ToArray();
                //人员删除,出去不存在的人员
                var user = mb.User.Where(t => userIdArr.Contains(t.UserId)).ToList();
                userIdArr = user.Select(t => t.UserId).Distinct().ToArray();
                var userNmaeArr = user.Select(t => t.NickName).ToArray();
                //获取每个人进行中的，已完成和未完成的个数集合
                //(8 + 16 + 32 + 64 )进行中 128 未解决 1024 已完成 //其他
                //进行中
                var orderIng = userCountDic

                .Where(t => (OrderState.Sending | OrderState.Sended | OrderState.Working | OrderState.Worked).HasFlag(t.State))
                .GroupBy(t => t.ServiceUserId)
                .Select(m => new { ServiceUserId = m.Key, Num = m.Sum(t => t.Num) })
                .ToDictionary(t => t.ServiceUserId, t => t.Num);
            //已完成
            var ordered = userCountDic.Where(t => (OrderState.Confirm.HasFlag(t.State)|| OrderState.UseComment.HasFlag(t.State)))
                 .GroupBy(t => t.ServiceUserId)
                .Select(m => new { ServiceUserId = m.Key, Num = m.Sum(t => t.Num) })
                .ToDictionary(t => t.ServiceUserId, t => t.Num);
            //未解决
            var orderUnComplated = userCountDic.Where(t => OrderState.Unsolved.HasFlag(t.State))
                  .GroupBy(t => t.ServiceUserId)
                .Select(m => new { ServiceUserId = m.Key, Num = m.Sum(t => t.Num) })
                 .ToDictionary(t => t.ServiceUserId, t => t.Num);
            //构造数据源
            var seriesorderIng = new int[userIdArr.Length];
            var seriesordered = new int[userIdArr.Length];
            var seriesorderUnComplated = new int[userIdArr.Length];
            for (int i = 0; i < userIdArr.Length; i++)
            {
                if (orderIng.ContainsKey(userIdArr[i]))
                    seriesorderIng[i] = orderIng[userIdArr[i]];
                else
                    seriesorderIng[i] = 0;

                if (ordered.ContainsKey(userIdArr[i]))
                    seriesordered[i] = ordered[userIdArr[i]];
                else
                    seriesordered[i] = 0;

                if (orderUnComplated.ContainsKey(userIdArr[i]))
                    seriesorderUnComplated[i] = orderUnComplated[userIdArr[i]];
                else
                    seriesorderUnComplated[i] = 0;
            }

           
                var result = new ArrayList();
            
                var Ing = new { name = "进行中", stack = (userNmaeArr.Length != 1 ? "总量" : ""), type = "bar", label = new { normal = new { show = true, position = "insideRight" }}, data = seriesorderIng };
                result.Add(Ing);
                var ed = new { name = "已完成", stack = (userNmaeArr.Length != 1 ? "总量" : ""), type = "bar", label = new { normal = new { show = true, position = "insideRight" }}, data = seriesordered };
                result.Add(ed);
                var UnComplated = new { name = "未解决", stack = (userNmaeArr.Length != 1 ? "总量" : ""), type = "bar", label = new { normal = new { show = true, position = "insideRight" } }, data = seriesorderUnComplated };
                result.Add(UnComplated);
                var legend = new string[3] { "进行中", "已完成", "未解决" };
                return Json(new { result, userNmaeArr, legend }, JsonRequestBehavior.AllowGet);

            }
        }


        /// <summary>
        /// 饼图数据获取,只统计进行中 已完成 和未解决
        /// </summary>
        /// <param name="appType"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public ActionResult PieOrderStateCount(AppType appType, DateTime beginTime, DateTime endTime)
        {

            var list = new OrderService().GetRepairList(appType, ApiUser.Current, beginTime, endTime);


            //获取,状态以及数量
            var userCountDic = list.Where(t => !(string.IsNullOrEmpty(t.ServiceUserId))).OrderBy(t => t.ServiceUserId).GroupBy(t => t.State)
                .Select(t => new { State = t.Key, Num = t.Count() }).ToList();
            //总条数
            var total = userCountDic.Sum(t => t.Num);
            var pieArry = new ArrayList();

            var IngNum = userCountDic
              .Where(t => (OrderState.Sending | OrderState.Sended | OrderState.Working | OrderState.Worked).HasFlag(t.State))
              .Sum(t => t.Num);
            pieArry.Add(new { value = IngNum, name = "进行中" });

            var edNum = userCountDic
                 .Where(t => OrderState.Confirm.HasFlag(t.State)).Sum(t => t.Num);
            pieArry.Add(new { value = edNum, name = "已完成" });

            var unNum = userCountDic
             .Where(t => OrderState.Unsolved.HasFlag(t.State)).Sum(t => t.Num);

            pieArry.Add(new { value = unNum, name = "未解决" });

            pieArry.Add(new { value = (total - IngNum - edNum - unNum), name = "其他" });


            return Json(new { seriesdata = pieArry.ToArray() }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 单子数量按时间统计
        /// </summary>
        /// <param name="appType"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="Type">1按天2按周3按月4按年</param>
        /// <returns></returns>
        public ActionResult ServerRepairTime(AppType appType, DateTime beginTime, DateTime endTime, string Type)
        {
            var list = new OrderService().GetRepairList(appType, ApiUser.Current, beginTime, endTime);

            //获取,状态以及数量
            var userCountDic = list.OrderBy(t => t.ServiceUserId).GroupBy(t => t.State)
                .Select(t => new { State = t.Key, Num = t.Count() }).ToList();
            //总条数
            var total = userCountDic.Sum(t => t.Num);
            var pieArry = new ArrayList();

            var IngNum = userCountDic
              .Where(t => (OrderState.Sending | OrderState.Sended | OrderState.Working | OrderState.Worked).HasFlag(t.State))
              .Sum(t => t.Num);
            pieArry.Add(new { value = IngNum, name = "进行中" });

            var edNum = userCountDic
                 .Where(t => OrderState.Confirm.HasFlag(t.State)).Sum(t => t.Num);
            pieArry.Add(new { value = edNum, name = "已完成" });

            var unNum = userCountDic
             .Where(t => OrderState.Unsolved.HasFlag(t.State)).Sum(t => t.Num);

            pieArry.Add(new { value = unNum, name = "未解决" });

            pieArry.Add(new { value = (total - IngNum - edNum - unNum), name = "其他" });

            if (Type == "1") {
                var resultHour = list.GroupBy(t => t.CreateTime.Hour)
                .Select(m => new { m.Key, Count = m.Count() }).ToDictionary(t => t.Key, t => t.Count);
                var Axis = new string[24];
                var data = new int[24];
                for (int i = 0; i <= 23; i++)
                {
                    Axis[i] = (i + 1) + "时";
                    data[i] = (resultHour.ContainsKey(i) ? resultHour[i] : 0);
                }
                return Json(new { Axis = Axis, data = data, pieArry }, JsonRequestBehavior.AllowGet);

            } else if (Type == "2") {
                //week
                var resultweek = list.GroupBy(t => t.CreateTime.DayOfWeek)
                        .Select(m => new { m.Key, Count = m.Count() }).ToDictionary(t => t.Key, t => t.Count);

                var Axisweek = new string[] { "周一", "周二", "周三", "周四", "周五", "周六", "周日" };
                var dataWeek = new int[7];
                dataWeek[0] = resultweek.Keys.Contains(DayOfWeek.Monday) ? resultweek[DayOfWeek.Monday] : 0;
                dataWeek[1] = resultweek.Keys.Contains(DayOfWeek.Tuesday) ? resultweek[DayOfWeek.Tuesday] : 0;
                dataWeek[2] = resultweek.Keys.Contains(DayOfWeek.Wednesday) ? resultweek[DayOfWeek.Wednesday] : 0;
                dataWeek[3] = resultweek.Keys.Contains(DayOfWeek.Thursday) ? resultweek[DayOfWeek.Thursday] : 0;
                dataWeek[4] = resultweek.Keys.Contains(DayOfWeek.Friday) ? resultweek[DayOfWeek.Friday] : 0;
                dataWeek[5] = resultweek.Keys.Contains(DayOfWeek.Saturday) ? resultweek[DayOfWeek.Saturday] : 0;
                dataWeek[6] = resultweek.Keys.Contains(DayOfWeek.Sunday) ? resultweek[DayOfWeek.Sunday] : 0;

                return Json(new { Axis = Axisweek, data = dataWeek, pieArry }, JsonRequestBehavior.AllowGet);
            }
            
            else if (Type == "3")
            {
                var resultday = list.GroupBy(t => t.CreateTime.Day)
                   .Select(m => new { m.Key, Count = m.Count() }).ToDictionary(t => t.Key, t => t.Count);

                var length = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                var Axis = new string[length];
                var data = new int[length];
                for (int i = 0; i < length; i++)
                {
                    Axis[i] = (i + 1) + "日";
                    data[i] = (resultday.ContainsKey(i) ? resultday[i] : 0);
                }
                return Json(new { Axis = Axis, data = data, pieArry }, JsonRequestBehavior.AllowGet);
            }           
            else {
                //month
                var resultmonth = list.GroupBy(t => t.CreateTime.Month)
                       .Select(m => new { m.Key, Count = m.Count() }).ToDictionary(t => t.Key, t => t.Count);
                var Axismonth = new string[12];
                var datamonth = new int[12];
                for (int i = 1; i < 13; i++)
                {
                    Axismonth[i - 1] = i + "月";
                    datamonth[i - 1] = (resultmonth.ContainsKey(i) ? resultmonth[i] : 0);
                }
                return Json(new { Axis = Axismonth, data = datamonth, pieArry }, JsonRequestBehavior.AllowGet);
            }
        }

        //工作时常测试
        public ActionResult WorkTime(AppType appType, DateTime beginTime, DateTime endTime)
        {
            using (var mb = new MbContext())
            {
              
                var list = new OrderService().GetRepairList(appType, ApiUser.Current, beginTime, endTime);

                //获取维修员列表
                var userArr = list.Where(t => !string.IsNullOrEmpty(t.ServiceCompanyId)).Select(t => t.ServiceUserId).Distinct().ToList();
                //从orderlog当中获取 开始工作日期以及结束日期
                var beginArr = mb.OrderLog.Where(t => userArr.Contains(t.UserId) && t.State == OrderState.Working)
                                 .OrderBy(t => t.CreateTime)
                                 .GroupBy(t => t.MainOrderId)
                                 .ToLookup(t => t.Key).ToDictionary(t => t.Key, t => t.FirstOrDefault().Select(m => new { m.CreateTime, m.UserId }));

                var endArr = mb.OrderLog.Where(t => userArr.Contains(t.UserId) && t.State == OrderState.Worked)
                              .OrderByDescending(t => t.CreateTime)
                              .GroupBy(t => t.MainOrderId)
                              .ToLookup(t => t.Key).ToDictionary(t => t.Key, t => t.FirstOrDefault().Select(m => new { m.CreateTime, m.UserId }));

                var sameArr = (from v in beginArr
                               join e in endArr
                              on v.Key equals e.Key
                               select new
                               {
                                   v.Key,
                                   UserId = v.Value.Select(m => m.UserId).First(),
                                   BeginTime = v.Value.Select(m => m.CreateTime).First(),
                                   EndTime = e.Value.Select(m => m.CreateTime).First()
                               }
                              ).ToList();
                //算每个人的工作时间
                var timeArr = sameArr.GroupBy(t => t.UserId).
                    Select(m => new {
                        m.Key,
                        avgTime = Convert.ToDouble(m.Average(t => (t.EndTime - t.BeginTime).TotalMinutes).ToString("f2"))
                    }).ToDictionary(t => t.Key, t => t.avgTime);
                var listResult = new ArrayList();
                var nameList = mb.User.Where(t => timeArr.Keys.Contains(t.UserId)).Select(t =>t.NickName ).ToArray();
                listResult.Add(new { seriesdata = "平均工作时间", type = "bar", data = timeArr.Values.ToArray() });
                return Json(new { seriesdata = listResult .ToArray(), Axis = nameList });
            }
        }
    }
}