using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Repair.Web.Site.Utilities;
using LZY.BX.Service.Mb;
using Repair.Web.Site.Areas.Sp.Models;
using LZY.BX.Model.Enum;

namespace Repair.Web.Site.Areas.Sp.Controllers
{
    public class MngController : ControllerBase<MngController>
    {
        //
        // GET: /Sp/Msg/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CopMngList(ConMngListQueryModel query)
        {
            ViewData["RoleKey"] = CurrentUser.User.RoleKey;
            using (var db = new MbContext())
            {
                //DOTO 根据用户单位名称查询
                var useComapny = db.UseCompany
                    .Where(query);

                var model = db.JoinCompany
                    .Where(t => t.ServiceCompanyId == CurrentUser.User.ServiceCompanyId);

                if (!string.IsNullOrEmpty(query.NameLike))
                {
                    //DOTO 如果存在单位查询，则使用查询结果作为查询签约合作表条件
                    var tts = useComapny.Select(t => t.UseCompanyId).ToList();

                    if (tts.Count == 0)
                    {
                        return View(query);
                    }

                    model.Where(t => tts.Contains(t.UseCompanyId));
                }

                query.Data = model
                 .OrderByDescending(x => x.CreateTime)
                 .Skip(query.PageInfo.RecIndex)
                 .Take(query.PageInfo.PageSize)
                 .ToList();

                var uids = query.Data
                            .Select(t => t.UseCompanyId).ToList();
                //DOTO  查询签约的业主单位
                query.UseCompanyDic = useComapny
                    .Where(t => uids.Contains(t.UseCompanyId))
                    .ToLookup(t => t.UseCompanyId)
                    .ToDictionary(t => t.Key, t => t.First());


                //DOTO数量
                query.PageInfo.TotalCount = model.Count();
            }
            return View(query);
        }

        /// <summary>
        /// 通过
        /// </summary>
        /// <returns></returns>
        public ActionResult PassedConMng(string uid)
        {
            using (var db = new MbContext())
            {
                var model = db.JoinCompany.FirstOrDefault(t => t.UseCompanyId == uid && t.ServiceCompanyId == CurrentUser.User.ServiceCompanyId);
                model.State = CompanySginState.Passed.ToString();

                db.SaveChanges();
            }


            var dlgInf = new DlgConfirmInfo();
            //todo 报修单位向维修单位发起签约申请
            dlgInf.Title = "通过";
            dlgInf.Content = "操作成功";
            dlgInf.Type = InfoType.Success;
            return View("dlg_confirm_info", dlgInf);
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <returns></returns>
        public ActionResult CloseConMng(string uid)
        {
            using (var db = new MbContext())
            {
                var model = db.JoinCompany.FirstOrDefault(t => t.UseCompanyId == uid && t.ServiceCompanyId == CurrentUser.User.ServiceCompanyId);
                model.State = CompanySginState.Close.ToString();

                db.SaveChanges();
            }


            var dlgInf = new DlgConfirmInfo();
            //todo 报修单位向维修单位发起签约申请

            dlgInf.Title = "关闭";
            dlgInf.Content = "操作成功";
            dlgInf.Type = InfoType.Success;
            return View("dlg_confirm_info", dlgInf);
        }
    }
}
