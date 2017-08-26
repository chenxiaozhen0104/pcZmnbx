using System;
using System.Collections.Generic;
using System.Linq;
using LZY.BX.Service.Mb;
using Repair.Web.Site.Utilities;
using System.Web.Mvc;
using Repair.Web.Site.Areas.User.Models;
using LZY.BX.Model.Enum;
using LZY.BX.Model;

namespace Repair.Web.Site.Areas.User.Controllers
{
    public class MngController : ControllerBase<MngController>
    {
        //
        // GET: /User/Mng/

        public ActionResult Index()
        {
           
            return View();
        }


        public ActionResult CopMng()
        {
            ViewData["RoleKey"] = CurrentUser.User.RoleKey;
            return View();
        }

        /// <summary>
        /// 合作单位列表
        /// </summary>
        /// <returns></returns>
        public ActionResult CopMngList(CopMngListQueryModel query)
        {
            ViewData["RoleKey"] = CurrentUser.User.RoleKey;
            using (var db = new MbContext())
            {
                //DOTO 根据服务单位名称查询
                var serviceComapny = db.ServiceCompany
                    .Where(query);

                var model = db.JoinCompany
                    .Where(t => t.UseCompanyId == CurrentUser.User.UseCompanyId);

                if (!string.IsNullOrEmpty(query.NameLike))
                {
                    //DOTO 如果存在单位查询，则使用查询结果作为查询签约合作表条件
                    var tts = serviceComapny.Select(t => t.ServiceCompanyId).ToList();

                    if (tts.Count == 0)
                    {
                        return View(query);
                    }

                    model.Where(t => tts.Contains(t.ServiceCompanyId));
                }

                query.Data = model
                 .OrderByDescending(x => x.CreateTime)
                 .Skip(query.PageInfo.RecIndex)
                 .Take(query.PageInfo.PageSize)
                 .ToList();

                var sids = query.Data
                            .Select(t => t.ServiceCompanyId).ToList();
                //DOTO  查询签约的服务单位
                query.ServiceCompanyDic = serviceComapny
                    .Where(t => sids.Contains(t.ServiceCompanyId))
                    .ToLookup(t => t.ServiceCompanyId)
                    .ToDictionary(t => t.Key, t => t.First());


                //DOTO数量
                query.PageInfo.TotalCount = model.Count();
            }
            return View(query);
        }

        /// <summary>
        /// 绑定服务单位页面
        /// </summary>
        /// <returns></returns>
        public ActionResult AddConMng()
        {
            return View();
        }

        /// <summary>
        /// 绑定服务单位页面
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddConMng(string keywords)
        {
            if (string.IsNullOrEmpty(keywords))
            {
                return View();
            }

            using (var db = new MbContext())
            {
                var models = db.ServiceCompany.Where(x=>x.Name.Contains(keywords)&&x.State==CompanyState.Normal).ToList();
                return View(models);
            }
        }

        /// <summary>
        /// 申请记录
        /// </summary>
        /// <returns></returns>
        public ActionResult ApplyConMngList()
        {
            using (var db = new MbContext())
            {
                var model = db.JoinCompany
                    .Where(t => t.State == CompanySginState.Waitting.ToString())
                    .Where(t => t.State == CompanySginState.Close.ToString())
                    .Where(t => t.UseCompanyId == CurrentUser.User.UseCompanyId).ToList();

                var sIds = model.Select(t => t.ServiceCompanyId).ToList();

                ViewBag.joinCompanyDic = model
                   .ToLookup(t => t.ServiceCompanyId)
                   .ToDictionary(x => x.Key, x => x.First());

                var models = db.ServiceCompany.Where(t => sIds.Contains(t.ServiceCompanyId)).ToList();

                return View(models);
            }
        }

        /// <summary>
        /// 申请
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public ActionResult ApplyConMng(string sid)
        {
            var dlgInf = new DlgConfirmInfo();

            using (var db = new MbContext())
            {
                var model = db.JoinCompany.FirstOrDefault(t => t.ServiceCompanyId == sid && t.UseCompanyId == CurrentUser.User.UseCompanyId);

                if (model != null)
                {
                    //DOTO  已经存在 不允许签约
                    dlgInf.Title = "签约";
                    dlgInf.Content = "此单位已签约";
                    dlgInf.Type = InfoType.Error;
                    return View("dlg_confirm_info", dlgInf);
                }

                db.JoinCompany.Add(new JoinCompany
                {
                    UseCompanyId = CurrentUser.User.UseCompanyId,
                    ServiceCompanyId = sid,
                    State = CompanySginState.Waitting.ToString(),
                    CreateTime = DateTime.Now
                });

                db.SaveChanges();
            }


            //todo 报修单位向维修单位发起签约申请

            dlgInf.Title = "签约";
            dlgInf.Content = "签约申请已发送";
            dlgInf.Type = InfoType.Success;
            return View("dlg_confirm_info", dlgInf);
        }

        /// <summary>
        /// 合作单位解绑
        /// </summary>
        /// <returns></returns>
        public ActionResult DelConMng(string sid)
        {
            using (var db = new MbContext())
            {
                var model = db.JoinCompany.FirstOrDefault(t => t.ServiceCompanyId == sid && t.UseCompanyId == CurrentUser.User.UseCompanyId);

                db.JoinCompany.Remove(model);

                db.SaveChanges();
            }


            var dlgInf = new DlgConfirmInfo();
            //todo 报修单位向维修单位发起签约申请

            dlgInf.Title = "解绑";
            dlgInf.Content = "解绑成功";
            dlgInf.Type = InfoType.Success;
            return View("dlg_confirm_info", dlgInf);
        }

    }
}
