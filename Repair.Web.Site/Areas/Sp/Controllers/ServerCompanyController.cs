using LZY.BX.Service;
using Repair.Web.Site.Areas.User.Models;
using Repair.Web.Site.Models;
using Repair.Web.Site.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Repair.Web.Site.Areas.Sp.Controllers
{
   
        public class ServerCompanyController : ControllerBase<ServerCompanyController>
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
                query.ServiceCompanyList = cService.GettAllServiceInfo(keywords);
                query.PageInfo.TotalCount = query.ServiceCompanyList.Count;
                return View(query);
            }
            /// <summary>
            /// Author:Gavin
            /// Description:加入使用企业
            /// Create Data:2017-03-18
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public ActionResult SubmitServerCompany(string id)
            {
                try
                {
                    LZY.BX.Model.User user = new UserService().Get(CurrentUser.User.UserId);
                    cService.AddServerCompany(user, id);                   
                AuthMng.Instance.InitUserCookie(HttpContext, new UserCookie(
                        new AuthUser
                        {
                            User = CurrentUser.User,
                            UseCompany = CurrentUser.UseCompany,
                            ServiceCompany = cService.GetServiceInfo(user)
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
