using LZY.BX.Model;
using LZY.BX.Model.Enum;
using LZY.BX.Service;
using LZY.BX.Service.Mb;
using Repair.Web.Site.Models;
using Repair.Web.Site.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Repair.Web.Site.Areas.User.Controllers
{
    public class AuthController : ControllerBase<AuthController>
    {
        //
        // GET: /Home/

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            CurrentUser.User = new MbContext().User.FirstOrDefault(t => t.Phone == CurrentUser.User.Phone);          
            //TODO:判断 如果当前用户不是业主用户，则进入当前页，提示申请成为业主用户
            if (CurrentUser.User.UseCompanyId != null)
            {
                CurrentUser.UseCompany = new CompanyService().GetCompanyInfo(CurrentUser.User);
                if (CurrentUser.UseCompany != null)
                {
                    AuthMng.Instance.InitUserCookie(HttpContext, new UserCookie(
                           new AuthUser
                           {
                               User = CurrentUser.User,
                               UseCompany = CurrentUser.UseCompany,
                               ServiceCompany = CurrentUser.ServiceCompany
                           }));
                    if (CurrentUser.UseCompany.State !=CompanyState.Locked)
                    {
                        return RedirectToAction("Order", "Order");
                    }
                    else {
                        ViewData["UseCompany"] = CurrentUser.UseCompany;
                        return View();
                    }
                }
                else {
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        /// <summary>
        /// 申请成为业主用户
        /// </summary>
        /// <returns></returns>
        public ActionResult Register()
        {
            //using (var db = new MbContext())
            //{
            //    var companySginLogModel = db.CompanySginLog.FirstOrDefault(t => t.ApplyUserId == CurrentUser.User.UserId && t.CompanyType == CompanyType.Use.ToString());
            //    if (companySginLogModel != null)
            //    {
            //        return RedirectToAction("DisplayApply");
            //    }
            //}

            return View();
        }



        /// <summary>
        /// 申请成为业主用户
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Register(UseCompany useCompany)
        {            
            try
            {
                CompanyService cService = new CompanyService();
                if (CurrentUser.UseCompany== null|| CurrentUser.UseCompany.State==CompanyState.Locked)
                {
                    cService.AddCompanyInfo(useCompany, CurrentUser.User);
                  
                    return ResultSuccess("操作成功.");
                }
                else {
                    return ResultError("操作失败.");
                }
               
            }
            catch (Exception ex)
            {
                return ResultError(ex.Message);
            }
           
          
        }


        //搜索使用单位事件
        public ActionResult UserCompanyList(string keywords)
        {   
            //获取所有使用单位信息
            var model = new CompanyService().GetAllCompanyInfo(keywords);
            if (model != null)
            {
                return Json(model.Select(m => new { m.Name, m.Phone, m.Contact, m.Position, id = m.UseCompanyId.ToString() }), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            
        }

        //加入公司
        public ActionResult AddUserCompany(string id)
        {
            try
            {
                LZY.BX.Model.User user = new UserService().Get(CurrentUser.User.UserId);
                new CompanyService().AddCompany(user, id);
                CurrentUser.User = new UserService().Get(CurrentUser.User.UserId);
                AuthMng.Instance.InitUserCookie(HttpContext, new UserCookie(
                        new AuthUser
                        {
                            User = CurrentUser.User,
                            UseCompany = CurrentUser.UseCompany,
                            ServiceCompany = CurrentUser.ServiceCompany
                        }));
                return ResultSuccess("操作成功.");
            }
            catch (DataExistException e)
            {
                return ResultError(e.Message);
            }
           
        }

    }
}
