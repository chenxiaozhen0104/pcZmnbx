using System.Web.Mvc;
using Repair.Web.Site.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using LZY.BX.Model.Enum;
using LZY.BX.Service.Mb;
using LZY.BX.Model;
using System.Web;
using Newtonsoft.Json;
using System.IO;
using System.Configuration;
using LZY.BX.Service;
using Repair.Web.Site.Models;

namespace Repair.Web.Site.Areas.Sp.Controllers
{
    public class AuthController : ControllerBase<AuthController>
    {
        //
        // GET: /Home/
         CompanyService cService = new CompanyService();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            CurrentUser.User = new MbContext().User.FirstOrDefault(t => t.Phone == CurrentUser.User.Phone);
            //TODO:判断 如果当前用户不是服务商，则进入当前页，提示申请成为服务商
            if (CurrentUser.User.ServiceCompanyId != null)
            {
                CurrentUser.ServiceCompany = new CompanyService().GetServiceInfo(CurrentUser.User);
                if (CurrentUser.ServiceCompany != null)
                {
                    
                    AuthMng.Instance.InitUserCookie(HttpContext, new UserCookie(
                           new AuthUser
                           {
                               User = CurrentUser.User,
                               UseCompany = CurrentUser.UseCompany,
                               ServiceCompany = CurrentUser.ServiceCompany
                           }));
                    //还是要判断servicecompanyuser的State值才行
                    // CompanyState.Locked 不等于锁住 要么是正在审核中,要么是审核通过
                    if (CurrentUser.ServiceCompany.State != CompanyState.Locked)
                    {
                        if (CurrentUser.User.ServiceCompanyId != null) {

                            ServiceCompanyUser info=new CompanyService().GetServiceCompanyUser(CurrentUser.User.UserId);
                            if (info!=null&&info.State == UserState.Locked)
                            {
                                ViewData["CheckResult"] = "审核没有通过";
                                return View();
                            }
                            else {
                                return RedirectToAction("Order", "Order");
                            }
                        }
                        else { 
                        return RedirectToAction("Order", "Order");
                        }
                    }
                    else
                    {
                        ViewData["ServiceCompany"] = CurrentUser.ServiceCompany;
                        return View();
                    }
                }
                else
                {
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
            //    var companySginLogModel = db.CompanySginLog.FirstOrDefault(t => t.ApplyUserId == CurrentUser.User.UserId && t.CompanyType == CompanyType.Service.ToString());
            //    if (companySginLogModel != null)
            //    {
            //        return RedirectToAction("DisplayApply");
            //    }
            //}

            return View();
        }
     
        /// <summary>
        /// 申请成为服务商用户
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Registers(ServiceCompany serverCompany)
        {

            try
            {
                if (CurrentUser.ServiceCompany == null || CurrentUser.ServiceCompany.State == CompanyState.Locked)
                {
                    HttpFileCollectionBase uploadFile = Request.Files;
                    List<Picture> pInfo = new List<Picture>();
                    if (uploadFile.Count > 0)
                    {
                        for (int i = 0; i < uploadFile.Count; i++)
                        {
                            HttpPostedFileBase file = uploadFile[i];
                            string path = ConfigurationManager.AppSettings["repairImgPath"].ToString() + "/" + PhotoType.Server.ToString() + "/";

                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }
                            string fileName = Guid.NewGuid().ToString("N") + "." + file.ContentType.ToString().Split('/')[1];
                            file.SaveAs(path + "/" + fileName);

                            pInfo.Add(new Picture
                            {
                                Type = PhotoType.Server,
                                Url = PhotoType.Server.ToString() + "/" + fileName,                               
                                CreateTime = DateTime.Now
                            });
                        }
                    }
                    cService.AddServiceCompanyInfo(serverCompany, CurrentUser.User);
                    CurrentUser.User = new MbContext().User.FirstOrDefault(t => t.Account == CurrentUser.User.Phone);

                    //上传图片
                    if (pInfo.Count != 0)
                    {
                        using (var db = new LZY.BX.Service.Mb.MbContext())
                        {
                            foreach (Picture item in pInfo)
                            {
                                item.OuterId = CurrentUser.User.ServiceCompanyId;
                                item.CreateTime = DateTime.Now;
                                db.Picture.Add(item);
                            }
                            db.SaveChanges();
                        }
                        
                    }
                    AuthMng.Instance.ResetUserCookie(HttpContext, new UserCookie(
                         new AuthUser
                         {
                             User = CurrentUser.User,
                             UseCompany = CurrentUser.UseCompany,
                             ServiceCompany = cService.GetServiceInfo(CurrentUser.User)
                         }));
                    return ResultSuccess("提交成功.请稍后留意审核状态",Url.Action("Order", "Order"));

                }
                else
                {
                    return ResultError("重复申请，您有一个单位等待审核中.");
                }

            }
            catch (DataExistException ex)
            {

                return ResultError(ex.Message);
            }
          
        }

        /// <summary>
        /// 注册获取标签列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetCategoryList()
        {
            try
            {
                return Json(new CategorySvr().GetList(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        /// Author:Gavin
        /// Description:搜索使用单位事件
        /// Create Data:2017-03-29
        public ActionResult UserCompanyList(string keywords)
        {
            //获取所有使用单位信息
            var List = new CompanyService().GettAllServiceInfo(keywords);
            if (List != null) { 
                return Json(List.Select(m => new { m.Name, m.Phone, m.Contact, m.Position, id = m.ServiceCompanyId.ToString() }), JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Author:Gavin
        /// Description:加入服务企业
        /// Create Data:2017-03-29
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult SubmitServerCompany(string id)
        {
            try
            {
                LZY.BX.Model.User user = new UserService().Get(CurrentUser.User.UserId);
                cService.AddServerCompany(user, id);
                CurrentUser.User = new UserService().Get(CurrentUser.User.UserId);

                AuthMng.Instance.InitUserCookie(HttpContext, new UserCookie(
                        new AuthUser
                        {
                            User = CurrentUser.User,
                            UseCompany = CurrentUser.UseCompany,
                            ServiceCompany = cService.GetServiceInfo(user)
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
