﻿using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LZY.BX.Model;
using LZY.BX.Service.Mb;
using Repair.Web.Site.Utilities;
using ZR;
using LZY.BX.Utilities;
using Repair.Web.Site.Areas.Sp.Models;
using LZY.BX.Model.Enum;

namespace Repair.Web.Site.Areas.Sp.Controllers
{
    public class UserController : ControllerBase<UserController>
    {
        //
        // GET: /Sp/User/
      
    
        public ActionResult Index()
        {
            ViewData["RoleKey"] = CurrentUser.User.RoleKey;
            return View();
        }
        /// <summary>
        /// 用户列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public ActionResult UserList(string keywords)
        {
            var query = new UserQueryModel();
            //DOTO 条件
            if (CurrentUser.User.RoleKey == LZY.BX.Model.Enum.UserType.SvcCompanyUserAdmin)
            {
              
                var userId = 0L;
                if (ValidatorHelper.IsMobilePhoneNumber(keywords))
                {
                    query.Phone = keywords;
                }
                else if (long.TryParse(keywords, out userId))
                {
                    query.UserId = userId;
                }
                else
                {
                    query.RealNameLike = keywords;
                }

                //DOTO 根据当前用户的单位查找该单位下的所有人员
                using (var db = new MbContext())
                {
                    //DOTO获取当前报修单位下的用户Id
                    var userIds =
                        db.ServiceCompanyUser
                        .Where(x => x.ServiceCompanyId == CurrentUser.User.ServiceCompanyId)
                        .Select(x => x.UserId)
                        .ToArray();

                    //DOTO人员数量
                    var countModel = db.User.Where(x => userIds.Contains(x.UserId)).Where(query);

                    //DOTO人员列表
                    query.Data = countModel
                        .OrderByDescending(x => x.RegTime)
                        .Skip(query.PageInfo.RecIndex)
                        .Take(query.PageInfo.PageSize)
                        .ToList();

                    //DOTO人员数量
                    query.PageInfo.TotalCount = countModel.Count();
                }

                return View(query);
            }
                return View(query);
          
        }

        /// <summary>
        /// 用户解除绑定
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult UnBindUser(string id)
        {
            //TODO 获取当前登录用户的报修单位ID
            var dlgInf = new DlgConfirmInfo();

            if (id == CurrentUser.User.UserId)
            {
                dlgInf.Title = "失败";
                dlgInf.Content = "不允许解绑";
                dlgInf.Type = InfoType.Error;
                return View("dlg_confirm_info", dlgInf);
            }

            using (var db = new MbContext())
            {
                var model = db.ServiceCompanyUser.FirstOrDefault(x => x.UserId == id && x.ServiceCompanyId == CurrentUser.User.ServiceCompanyId);
                if (model != null)
                {
                    //获取用户信息,更新单位信息
                    var user = db.User.FirstOrDefault(x => x.UserId == id);
                    user.ServiceCompanyId =null;
                    user.ServiceCompanyUserId = null;

                    db.ServiceCompanyUser.Remove(model);
                }
                if (db.SaveChanges() > 0)
                {
                    dlgInf.Title = "成功";
                    dlgInf.Content = "解除绑定成功";
                    dlgInf.Type = InfoType.Success;

                    return View("dlg_confirm_info", dlgInf);
                }
                dlgInf.Title = "失败";
                dlgInf.Content = "解除绑定失败";
                dlgInf.Type = InfoType.Error;
                return View("dlg_confirm_info", dlgInf);
            }
        }

        /// <summary>
        /// 绑定用户页面
        /// </summary>
        /// <returns></returns>
        public ActionResult AddUser()
        {
          
            return View();
        }

        /// <summary>
        /// 绑定用户页面
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddUser(string keywords)
        {
            var query = new UserQueryModel();
            var userId = 0L;

            if (ValidatorHelper.IsMobilePhoneNumber(keywords))
            {
                query.Phone = keywords;
            }
            else if (long.TryParse(keywords, out userId))
            {
                query.UserId = userId;
            }
            else
            {
                query.RealNameLike = keywords;
            }

            using (var db = new MbContext())
            {
                var models = db.User.Where(query).Take(query.PageInfo.PageSize).ToList();
                return View(models);
            }
        }

        /// <summary>
        /// 绑定用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult BindUser(string userId)
        {
            var dlgInf = new DlgConfirmInfo();

            //TODO 获取当前登录用户的报修单位ID
            using (var db = new MbContext())
            {
                //DOTO是否存在报修单位
                var useCompanyUser =
                    db.UseCompanyUser
                    .FirstOrDefault(x => x.UserId == userId);

                //DOTO是否存在服务单位
                var serviceCompanyUser =
                    db.ServiceCompanyUser
                    .FirstOrDefault(x => x.UserId == userId);

                if (serviceCompanyUser != null)
                {
                    dlgInf.Title = "失败";
                    dlgInf.Content = "该用户已被绑定";
                    dlgInf.Type = InfoType.Error;
                    return View("dlg_confirm_info", dlgInf);
                }

                if (useCompanyUser != null)
                {
                    dlgInf.Title = "失败";
                    dlgInf.Content = "该用户已绑定其他单位";
                    dlgInf.Type = InfoType.Error;
                    return View("dlg_confirm_info", dlgInf);
                }
                string ServiceCompanyUserId = SequNo.NewId;
                db.ServiceCompanyUser.Add(new ServiceCompanyUser()
                {
                    ServiceCompanyUserId = ServiceCompanyUserId,
                    ServiceCompanyId = CurrentUser.User.ServiceCompanyId,
                    UserId = userId,
                    CreateTime = DateTime.Now,
                });
                //获取用户信息,更新单位信息
                var user = db.User.FirstOrDefault(x => x.UserId == userId);
                user.ServiceCompanyId = CurrentUser.User.ServiceCompanyId;
                user.ServiceCompanyUserId = ServiceCompanyUserId;
                if (db.SaveChanges() > 0)
                {
                    dlgInf.Title = "成功";
                    dlgInf.Content = "绑定成功";
                    dlgInf.Type = InfoType.Success;

                    return View("dlg_confirm_info", dlgInf);
                }
                dlgInf.Title = "失败";
                dlgInf.Content = "绑定失败";
                dlgInf.Type = InfoType.Error;
                return View("dlg_confirm_info", dlgInf);
            }
        }

        public ActionResult Check() {
            return View();
        }
        /// <summary>
        /// Author;Gavin
        /// Create Date:2017-04-02
        /// Description:用户申请列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public ActionResult UserCheckList(string keywords)
        {
            var query = new UserQueryModel();
            //DOTO 条件
            
                var userId = 0L;
                if (ValidatorHelper.IsMobilePhoneNumber(keywords))
                {
                    query.Phone = keywords;
                }
                else if (long.TryParse(keywords, out userId))
                {
                    query.UserId = userId;
                }
                else
                {
                    query.RealNameLike = keywords;
                }

                //DOTO 根据当前用户的单位查找该单位下的所有人员
                using (var db = new MbContext())
                {
                    //DOTO获取当前报修单位下的用户Id
                    var userIds =
                        db.ServiceCompanyUser
                        .Where(x => x.ServiceCompanyId == CurrentUser.User.ServiceCompanyId&&x.State==LZY.BX.Model.Enum.UserState.NotActive)
                        .Select(x => x.UserId)
                        .ToArray();

                    //DOTO人员数量
                    var countModel = db.User.Where(x => userIds.Contains(x.UserId)).Where(query);

                    //DOTO人员列表
                    query.Data = countModel
                        .OrderByDescending(x => x.RegTime)
                        .Skip(query.PageInfo.RecIndex)
                        .Take(query.PageInfo.PageSize)
                        .ToList();

                    //DOTO人员数量
                    query.PageInfo.TotalCount = countModel.Count();
                }
                return View(query);
           
         

        }

        /// <summary>
        /// Author;Gavin
        /// Create Date:2017-04-02
        /// Description:用户审核结果
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="Reason"></param>
        /// <returns></returns>
        public ActionResult CheckUser(string id, UserState state, string Reason) {

            using (var db = new MbContext()) {
                var userServiceCompany = db.ServiceCompanyUser.Where(x => x.UserId == id).FirstOrDefault();
                userServiceCompany.State = state;
                var useInfo= db.User.Where(x => x.UserId == id).FirstOrDefault();
                if (state == UserState.Normal) {
                    useInfo.RoleKey = useInfo.RoleKey | UserType.SvcCompanyUser;
                }            
                if (db.SaveChanges() > 0)
                {
                    return ResultSuccess("审核成功");
                }
                else {
                    return ResultError("服务器繁忙,请您稍后再试");
                }
            }
                
        }
    }
}
