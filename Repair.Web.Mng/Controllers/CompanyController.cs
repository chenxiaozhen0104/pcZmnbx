using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using LZY.BX.Model;
using LZY.BX.Service;
using LZY.BX.Service.Mb;
using Repair.Web.Mng.Menu;
using Repair.Web.Mng.Models;
using LZY.BX.Model.Enum;
using ZR;

namespace Repair.Web.Mng.Controllers
{
    public class CompanyController : BaseController
    {
        [MenuDefault("企业管理", PArea = "")]
        public void Index() { }
        readonly CompanySysSvr _companySysSvr = new CompanySysSvr();

        [MenuCurrent("审核使用单位申请", PAction = "Index")]
        public ActionResult AuditCompany()
        {
            return View(Session["AuditCompanyQueryModel"]);
        }
        /// <summary>
        /// 审核使用企业列表
        /// </summary>
        /// <returns></returns>
        public ActionResult AuditCompanyList(AuditCompanyQueryModel query)
        {
           Session["AuditCompanyQueryModel"] = query;

            using (var db = new MbContext())
            {
                var model = db.UseCompany.Where(query);

                query.UserCompanyList = model.OrderByDescending(t => t.CreateTime)
                    .Skip(query.PageInfo.RecIndex)
                    .Take(query.PageInfo.PageSize)
                    .ToList();

                var uids = query.UserCompanyList.Select(t => t.UseCompanyId).ToList();
                //对应的用户
                query.UserDic = db.User
                  .Where(x => uids.Contains((long)x.UseCompanyId))
                  .ToLookup(t => t.UseCompanyId.GetValueOrDefault(0))
                  .ToDictionary(x => x.Key, x => x.First());

                query.PageInfo.TotalCount = model.Count();
               
                return View(query);
            }
        }

        /// <summary>
        /// 通过审核
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PassedAudit(long id, string reason)
        {
            using (var db = new MbContext())
            {
                UseCompany model = db.UseCompany.FirstOrDefault(t => t.UseCompanyId == id && t.State == CompanyState.NotActive);

                if (model == null)
                {
                    return ResultError("审核异常.");
                }

               
                //reason为空，则审核通过
                if (string.IsNullOrEmpty(reason))
                    model.State = CompanyState.Normal;               
                else{
                    model.State = CompanyState.Locked;
                    model.Note=reason;
                 }
             
                if (model.State == CompanyState.Normal)
                {
                    var cId = SequNo.NewId;
                    //更新用户的UserId的RoleKey，以及
                    User UserInfo = db.User.FirstOrDefault(x => x.UseCompanyId == model.UseCompanyId);
                    UserInfo.RoleKey = UserInfo.RoleKey | UserType.UseCompanyUserAdmin;
                    UserInfo.UseCompanyUserId = cId;
                    UserInfo.UserState = UserState.Normal;
                    db.User.AddOrUpdate(UserInfo);
                    //db.UseCompanyUser.Add(new UseCompanyUser
                    //{
                    //    UseCompanyUserId = cId,
                    //    UserId = UserInfo.UserId,
                    //    UseCompanyId = model.UseCompanyId,
                    //    Type = UseCompanyUserType.Admin.ToString(),
                    //    Title = string.Empty,
                    //    CreateTime = DateTime.Now
                    //});
                }
                

                if ((model.State == CompanyState.Normal && db.SaveChanges() == 3) || (model.State == CompanyState.Locked && db.SaveChanges() == 1))
                {
                    return ResultSuccess("操作成功.");
                }
                else
                {
                    return ResultError("操作失败.");
                }
                ////DOTO 业主单位
                //if (model.CompanyType == CompanyType.Use.ToString())
                //{
                //    db.UseCompany.Add(new UseCompany
                //    {
                //        UseCompanyId = cId,
                //        Code = string.Empty,
                //        Contact = model.Contact,
                //        Phone = model.Phone,
                //        Name = model.Name,
                //        Position = model.Position,
                //        Longitude = 0,
                //        Dimension = 0,
                //        CreateTime = DateTime.Now
                //    });

                //    db.UseCompanyUser.Add(new UseCompanyUser
                //    {
                //        UseCompanyUserId = SequNo.NewId,
                //        UserId = model.ApplyUserId,
                //        UseCompanyId = cId,
                //        Type = UseCompanyUserType.Ordinary.ToString(),
                //        Title = string.Empty,
                //        CreateTime = DateTime.Now
                //    });
                //}
                ////DOTO 服务商
                //else if (model.CompanyType == CompanyType.Service.ToString())
                //{
                //    db.ServiceCompany.Add(new ServiceCompany
                //    {
                //        ServiceCompanyId = cId,
                //        Type = ServiceCompanyType.In.ToString(),
                //        PId = 0,
                //        Contact = model.Contact,
                //        Phone = model.Phone,
                //        Name = model.Name,
                //        Position = model.Position,
                //        Longitude = 0,
                //        Dimension = 0,
                //        CreateTime = DateTime.Now
                //    });

                //    db.ServiceCompanyUser.Add(new ServiceCompanyUser
                //    {
                //        ServiceCompanyUserId = SequNo.NewId,
                //        UserId = model.ApplyUserId,
                //        ServiceCompanyId = cId,
                //        Title = string.Empty,
                //        WorkOn = DateTime.Now,
                //        CreateTime = DateTime.Now
                //    });
                //}

                //model.State = CompanySginState.Passed.ToString();

                //if (db.SaveChanges() == 3)
                //{
                //    return ResultSuccess("审核通过.");
                //}
                //else
                //{
                    //return ResultError("审核失败.");
                //}
            }
        }

        #region 审核服务企业部分

        [MenuCurrent("审核服务单位申请", PAction = "Index")]
        public ActionResult AuditServerCompany()
        {
            return View(Session["AuditServerCompanyQueryModel"]);
        }

        /// <summary>
        /// 审核服务企业列表
        /// </summary>
        /// <returns></returns>
        public ActionResult AuditServerCompanyList(AuditCompanyQueryModel query)
        {
            Session["AuditServerCompanyQueryModel"] = query;

            using (var db = new MbContext())
            {
                var model = db.ServiceCompany.Where(query);

                query.ServiceCompanyList = model.OrderByDescending(t => t.CreateTime)
                    .Skip(query.PageInfo.RecIndex)
                    .Take(query.PageInfo.PageSize)
                    .ToList();

                var uids = query.ServiceCompanyList.Select(t => t.ServiceCompanyId).ToList();
                //对应的用户
                query.UserDic = db.User
                  .Where(x => uids.Contains((long)x.ServiceCompanyId))
                  .ToLookup(t => t.ServiceCompanyId.GetValueOrDefault(0))
                  .ToDictionary(x => x.Key, x => x.First());

                query.PageInfo.TotalCount = model.Count();

                return View(query);
            }
        }
        /// <summary>
        /// 服务企业审核结果
        /// </summary>
        /// <param name="id"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ServerPassedAudit(long id, string reason)
        {
            using (var db = new MbContext())
            {
                ServiceCompany model = db.ServiceCompany.FirstOrDefault(t => t.ServiceCompanyId == id && t.State == CompanyState.NotActive);

                if (model == null)
                {
                    return ResultError("审核异常.");
                }


                //reason为空，则审核通过
                if (string.IsNullOrEmpty(reason))
                    model.State = CompanyState.Normal;
                else
                {
                    model.State = CompanyState.Locked;
                    model.Note = reason;
                }

                db.ServiceCompany.AddOrUpdate(model);
             
                if (model.State == CompanyState.Normal)
                {
                    //更新用户的UserId的RoleKey，以及
                    var cId = SequNo.NewId;
                    User UserInfo = db.User.FirstOrDefault(x => x.ServiceCompanyId == model.ServiceCompanyId);
                    UserInfo.RoleKey = UserInfo.RoleKey | UserType.SvcCompanyUserAdmin;
                    UserInfo.ServiceCompanyUserId = cId;
                    UserInfo.UserState = UserState.Normal;
                    db.User.AddOrUpdate(UserInfo);
                    //db.ServiceCompanyUser.Add(new ServiceCompanyUser
                    //{
                    //    ServiceCompanyUserId = cId,
                    //    UserId = UserInfo.UserId,
                    //    ServiceCompanyId = model.ServiceCompanyId,
                    //    Title = string.Empty,
                    //    State = UserState.Normal,
                    //    CreateTime = DateTime.Now
                    //});
                }
               
                if ((model.State == CompanyState.Normal && db.SaveChanges() == 2) || (model.State == CompanyState.Locked && db.SaveChanges() == 1))
                {
                    return ResultSuccess("操作成功.");
                }
                else
                {
                    return ResultError("操作失败.");
                }
            }
        }
        #endregion

        //
        // GET: /Company/
        #region 用户企业管理
        [MenuCurrent("用户企业管理", PAction = "Index")]
        public ActionResult Company(string key = null)
        {
            var pageIndex = Request["pageIndex"].AsInt(1);
            var pageSize = Request["pageSize"].AsInt(20);
            var user = CurrentUser;
            ViewData["UserId"] = user.UserId;
            using (var db = new MbContext())
            {
                var companies = db.UseCompany.Where(t=>t.State==CompanyState.Normal).ToList();
                if (key != null)
                {
                    companies = companies.FindAll(x => x.Name.Contains(key));
                }
                if (user.UserId > 0)
                {
                    var useCompanyUser = db.UseCompanyUser.FirstOrDefault(x => x.UserId == user.UserId);
                    if (useCompanyUser != null)
                    {
                        companies = companies.FindAll(x => x.UseCompanyId == useCompanyUser.UseCompanyId);
                    }
                }
                var pager = new Page<UseCompany>
                {
                    Index = pageIndex,
                    Size = pageSize,
                    Count = companies.Count,
                    Data = companies.OrderByDescending(m => m.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize)
                };

                return View(pager);
            }

        }
        [MenuHide("编辑", PAction = "Company")]
        public ActionResult CompanyEdit(long id = 0)
        {
            using (var db = new MbContext())
            {
                return View(db.UseCompany.FirstOrDefault(x => x.UseCompanyId == id));
            }

        }
        [MenuHide("编辑", PAction = "Company")]
        [HttpPost]
        public ActionResult CompanyEdit(UseCompany model)
        {
            if (CompanySvr.Instance.Update(model) > 0)
            {
                return Content("<script>alert('操作成功');window.location.href='/Company/Company'</script>");
            }
            return Content("<script>alert('操作失败');window.location.href='/Company/Company'</script>");

        }
        [MenuHide("删除", PAction = "Company")]
        public ActionResult CompanyDelete(long id = 0)
        {
            if (CompanySvr.Instance.Delete(id) > 0)
            {
                return Content("<script>alert('操作成功');window.location.href='/Company/Company'</script>");
            }
            return Content("<script>alert('操作失败');window.location.href='/Company/Company'</script>");

        }
        #endregion



        #region 维修单位管理
        [MenuCurrent("维修单位管理", PAction = "Index")]
        public ActionResult ServiceUnit(string key = null)
        {
            var pageIndex = Request["pageIndex"].AsInt(1);
            var pageSize = Request["pageSize"].AsInt(20);
            var user = CurrentUser;
            ViewData["UserId"] = user.UserId;

            using (var db = new MbContext())
            {
                var serviceUnits = db.ServiceCompany.Where(t => t.State == CompanyState.Normal).ToList();
                if (key != null)
                {
                    serviceUnits = serviceUnits.FindAll(x => x.Name.Contains(key));
                }
                if (user.UserId > 0)
                {
                    var serviceUser = db.ServiceCompanyUser.FirstOrDefault(x => x.UserId == user.UserId);
                    if (serviceUser != null)
                    {
                        serviceUnits = serviceUnits.FindAll(x => x.ServiceCompanyId == serviceUser.ServiceCompanyId);
                    }
                }
                var pager = new Page<ServiceCompany>
                {
                    Index = pageIndex,
                    Size = pageSize,
                    Count = serviceUnits.Count,
                    Data = serviceUnits.OrderByDescending(m => m.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize)
                };

                return View(pager);
            }
        }

        [MenuHide("编辑", PAction = "ServiceUnit")]
        public ActionResult ServiceUnitEdit(long id = 0)
        {
            using (var db = new MbContext())
            {
                return View(db.ServiceCompany.FirstOrDefault(x => x.ServiceCompanyId == id));
            }

        }

        [MenuHide("编辑", PAction = "ServiceUnit")]
        [HttpPost]
        public ActionResult ServiceUnitEdit(ServiceCompany model)
        {
            if (ServiceUnitSvr.Instance.Update(model) > 0)
            {
                return Content("<script>alert('操作成功');window.location.href='/Company/ServiceUnit'</script>");
            }
            return Content("<script>alert('操作失败');window.location.href='/Company/ServiceUnit'</script>");

        }
        [MenuHide("删除", PAction = "ServiceUnit")]
        public ActionResult ServiceUnitDelete(long id = 0)
        {
            if (ServiceUnitSvr.Instance.Delete(id) > 0)
            {
                return Content("<script>alert('操作成功');window.location.href='/Company/ServiceUnit'</script>");
            }
            return Content("<script>alert('操作失败');window.location.href='/Company/ServiceUnit'</script>");
        }
        #endregion

    }
}
