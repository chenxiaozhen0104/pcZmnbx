using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using LZY.BX.Model;
using LZY.BX.Service.Mb;
using Repair.Web.Site.Areas.User.Models;
using ZR;
using Repair.Web.Site.Utilities;
using System.Text.RegularExpressions;
using LZY.BX.Utilities;

namespace Repair.Web.Site.Areas.User.Controllers
{
    public class UserController : ControllerBase<UserController>
    {
        //
        // GET: /User/User/

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
        public ActionResult UserList(UserQueryModel query, string keywords)
        {
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
            if (CurrentUser.User.RoleKey == LZY.BX.Model.Enum.UserType.UseCompanyUserAdmin)
            {
                //DOTO 根据当前用户的单位查找该单位下的所有人员
                using (var db = new MbContext())
                {
                    //DOTO获取当前报修单位下的用户Id
                    var userIds =
                        db.UseCompanyUser
                        .Where(x => x.UseCompanyId == CurrentUser.User.UseCompanyId)
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
            else {
                return View(query);
            }
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

            using (var db = new MbContext())
            {
                var model = db.UseCompanyUser.FirstOrDefault(x => x.UserId == id && x.UseCompanyId == CurrentUser.User.UseCompanyId);
                if (model != null)
                {
                    //获取用户信息,更新单位信息
                    var user = db.User.FirstOrDefault(x => x.UserId == id);
                    user.UseCompanyId = null;
                    user.UseCompanyUserId = null;

                    db.UseCompanyUser.Remove(model);
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
            else if (!string.IsNullOrEmpty(keywords))
            {
                query.RealNameLike = keywords;
            }
            else
            {
                //为输入正确条件 直接返回null
                return View();
            }

            using (var db = new MbContext())
            {
                var models = db.User.Where(query).ToList();
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
                //DOTO是否存在服务单位
                var serviceCompanyUser =
                    db.ServiceCompanyUser
                    .FirstOrDefault(x => x.UserId == userId);
                //DOTO是否存在报修单位
                var useCompanyUser =
                    db.UseCompanyUser
                    .FirstOrDefault(x => x.UserId == userId);
                if (useCompanyUser != null)
                {
                    dlgInf.Title = "失败";
                    dlgInf.Content = "该用户已被绑定";
                    dlgInf.Type = InfoType.Error;
                    return View("dlg_confirm_info", dlgInf);
                }
                if (serviceCompanyUser != null)
                {
                    dlgInf.Title = "失败";
                    dlgInf.Content = "该用户已绑定其他单位";
                    dlgInf.Type = InfoType.Error;
                    return View("dlg_confirm_info", dlgInf);
                }
                string UseCompanyUserId = SequNo.NewId;
                db.UseCompanyUser.Add(new UseCompanyUser()
                {
                    UseCompanyUserId = UseCompanyUserId,
                    UseCompanyId = CurrentUser.User.UseCompanyId,
                    UserId = userId,
                    CreateTime = DateTime.Now,
                    Type = "Ordinary"
                });
                //获取用户信息,更新单位信息
                var user = db.User.FirstOrDefault(x => x.UserId == userId);
                user.UseCompanyId = CurrentUser.User.ServiceCompanyId;
                user.UseCompanyUserId = UseCompanyUserId;

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
    }
}
