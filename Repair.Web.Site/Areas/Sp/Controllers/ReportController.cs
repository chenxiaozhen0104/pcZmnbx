using LZY.BX.Model.Enum;
using LZY.BX.Service.Mb;
using Repair.Web.Site.Areas.Sp.Models;
using Repair.Web.Site.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Repair.Web.Site.Areas.Sp.Controllers
{
    public class ReportController : ControllerBase<OrderController>
    {
        //
        // GET: /Sp/Report/

        /// <summary>
        /// 评价
        /// </summary>
        /// <returns></returns>
        public ActionResult Evaluate()
        {
            using (var db = new MbContext())
            {
                var sUser = db.ServiceCompanyUser
                     .Where(t => t.ServiceCompanyId == CurrentUser.User.ServiceCompanyId);

                var suIds = sUser.Select(t => t.UserId).ToList();

                var sOrder = from order in db.ServiceCompanyRepairOrder
                             join commentsTemp in db.Comments on order.ServiceCompanyRepairOrderId equals commentsTemp.MainOrderId into commentsTemp
                             from comments in commentsTemp.DefaultIfEmpty()
                             where suIds.Contains(order.UserId)
                             select new
                             {
                                 order,
                                 comments
                             };

                var result = (from user in db.User
                              join orderTemp in sOrder on user.UserId equals orderTemp.order.UserId into orderTemp
                              from order in orderTemp.DefaultIfEmpty()
                              where suIds.Contains(user.UserId)
                              select new
                              {
                                  RealName = user.RealName,
                                  RepairOrderId = order == null ? "": order.order.RepairOrderId,
                                  Level = order == null ? 5 : order.comments == null ? 5 : order.comments.Level
                              })
                             .AsEnumerable()
                             .GroupBy(t => t.RealName)
                             .Select(t => new EvaluteReportModel
                             {
                                 RealName = t.Key,
                                 Level = t.Average(m => m.Level).ToString("f2"),
                                 Count = t.Count()
                             }).ToList();
                return View(result);
            }
        }

        /// <summary>
        /// 维修量
        /// </summary>
        /// <returns></returns>
        public ActionResult RepairCount(Int32? keywords)
        {
            Int32 year = DateTime.Now.Year;
            if (keywords!=null) {
                year = (Int32)keywords;
            }
            using (var db = new MbContext())
            {
                ViewData["Year"] = year;
             
                if (CurrentUser.User.RoleKey.HasFlag(UserType.SvcCompanyUserAdmin))
                {
                    var result = db.MainOrder
                            .Where(t => t.ServiceCompanyId == CurrentUser.User.ServiceCompanyId && t.CreateTime.Year == year)
                            .AsEnumerable()
                            .GroupBy(t => t.CreateTime.Month)
                            .Select(t => new RepairCountReportModel
                            {
                                Time = t.Key,
                                Count = t.Count()
                            }).ToList();
                    return View(result);
                }
                else {
                    var result = db.MainOrder
                           .Where(t => t.ServiceCompanyId == CurrentUser.User.ServiceCompanyId && t.CreateTime.Year == year&&t.ServiceUserId==CurrentUser.User.ServiceCompanyUserId)
                           .AsEnumerable()
                           .GroupBy(t => t.CreateTime.Month)
                           .Select(t => new RepairCountReportModel
                           {
                               Time = t.Key,
                               Count = t.Count()
                           }).ToList();
                    return View(result);
                }
               
              
            }
        }
    }
}
