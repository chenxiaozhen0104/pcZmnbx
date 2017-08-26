using LZY.BX.Model;
using LZY.BX.Service;
using Repair.Web.Site.Areas.User.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Repair.Web.Site.Utilities;
using Repair.Web.Site.Models;
using LZY.BX.Service.Mb;

namespace Repair.Web.Site.Areas.User.Controllers
{
    public class CompanyController : ControllerBase<CompanyController>
    {
        //
        // GET: /User/Company/
        CompanyService cService = new CompanyService();
        public ActionResult Index()
        {
            return View();
        }

        #region 加入公司
        public ActionResult AddCompany()
        {
            return View();
        }
        //搜索使用单位事件
        public ActionResult UserCompanyList(CompanyQueryModel query, string keywords)
        {
            //获取所有使用单位信息
            return Json(cService.GetAllCompanyInfo(keywords), JsonRequestBehavior.AllowGet);          
        }
        /// <summary>
        /// Author:Gavin
        /// Description:加入使用企业
        /// Create Data:2017-03-18
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult SubmitCompany(string id)
        {
            try
            {
                LZY.BX.Model.User user = new UserService().Get(CurrentUser.User.UserId);
                cService.AddCompany(user, id);
                CurrentUser.User = new MbContext().User.FirstOrDefault(t => t.Account == CurrentUser.User.Phone);
                AuthMng.Instance.InitUserCookie(HttpContext, new UserCookie(
                    new AuthUser
                    {
                        User = CurrentUser.User,
                        UseCompany = cService.GetCompanyInfo(user),
                        ServiceCompany = CurrentUser.ServiceCompany
                    }));
                return ResultSuccess("操作成功.", "../Order/Order");
            }
            catch (DataExistException e)
            {
                return Json(new { error = e.Message });
            }
        }
        #endregion



    }
}
