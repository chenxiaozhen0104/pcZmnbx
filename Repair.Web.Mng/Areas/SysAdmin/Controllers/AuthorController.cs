using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Runtime.Remoting;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Web.WebPages;
using LZY.BX.Model;
using LZY.BX.Model.Enum;
using LZY.BX.Model.PageModel;
using LZY.BX.Model.QueryModel;
using LZY.BX.Service;
using LZY.BX.Service.Mb;

using Repair.Web.Mng.Controllers;
using Repair.Web.Mng.Menu;
using ZR;
using LZY.BX.Utilities;

namespace Repair.Web.Mng.Areas.SysAdmin.Controllers
{
    public class AuthorController : BaseController
    {
        //
        // GET: /SysAdmin/CommData/

        [MenuDefault("权限管理", PArea = "", Icon = "fa-tasks")]
        public void Index() { }
        #region 角色管理
        /// <summary>
        /// 角色列表
        /// </summary>
        /// <returns></returns>
        [MenuCurrent("角色管理", PAction = "Index", Icon = "fa-male")]
        public ActionResult Roles(string key = null)
        {
            var pageIndex = Request["pageIndex"].AsInt(1);
            var pageSize = Request["pageSize"].AsInt(20);
            using (var db = new MbContext())
            {
                var roles = db.Roles.ToList();
                if (key != null)
                {
                    roles = roles.FindAll(x => x.Name.Contains(key));
                }
                var pager = new Page<Roles>
                {
                    Index = pageIndex,
                    Size = pageSize,
                    Count = roles.Count,
                    Data = roles.OrderByDescending(m => m.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize)
                };

                return View(pager);
            }
        }

        /// <summary>
        /// 角色修改，添加
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [MenuHide("角色编辑", PAction = "Roles")]
        public ActionResult Edit(long id = 0)
        {
            Roles model = null;

            if (id > 0)
            {
                using (var db = new MbContext())
                {
                    model = db.Roles.FirstOrDefault(x => x.RolesId == id);
                }
            }

            if (model == null)
            {
                model = new Roles
                {
                    RolesId = 0,
                    Name = "新角色",
                    Permissions = string.Empty
                };
            }

            return View(model);
        }

        /// <summary>
        /// 角色修改，添加（POST）
        /// </summary>
        /// <param name="model"></param>
        /// <param name="menus"></param>
        /// <returns></returns>
        [MenuHide("角色编辑", PAction = "Roles")]
        [HttpPost]
        public ActionResult Edit(Roles model, string[] menus)
        {

            model.Permissions = string.Join(";", menus);
            if (model.RolesId > 0)
            {
                using (var db = new MbContext())
                {
                    var roles = db.Roles.FirstOrDefault(x => x.Name == model.Name);
                    if (roles != null)
                    {

                    }
                    db.Roles.AddOrUpdate(model);
                    if (db.SaveChanges() > 0)
                    {
                        return Content("<script>alert('操作成功');window.location.href='/SysAdmin/Author/Roles'</script>");
                    }
                    return Content("<script>alert('操作失败');window.location.href='/SysAdmin/Author/Roles'</script>");
                }

            }
            else
            {
                model.RolesId = SequNo.NewId;
                using (var db = new MbContext())
                {
                    var roles = db.Roles.FirstOrDefault(x => x.Name == model.Name);
                    if (roles != null)
                    {
                        return Content("<script>alert('改角色已存在');window.location.href='/SysAdmin/Author/Roles'</script>");
                    }
                    db.Roles.Add(model);
                    if (db.SaveChanges() > 0)
                    {
                        return Content("<script>alert('操作成功');window.location.href='/SysAdmin/Author/Roles'</script>");
                    }
                    return Content("<script>alert('操作失败');window.location.href='/SysAdmin/Author/Roles'</script>");
                }

            }

        }

        /// <summary>
        /// 角色删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [MenuHide("角色删除", PAction = "Roles")]
        public ActionResult Delete(long id)
        {
            using (var db = new MbContext())
            {
                var roles = db.Roles.FirstOrDefault(x => x.RolesId == id);
                if (roles != null)
                {
                    db.Roles.Remove(roles);

                }
                var userRoles = db.UserRoles.FirstOrDefault(x => x.RolesId == id);
                if (userRoles != null)
                {
                    db.UserRoles.Remove(userRoles);
                }
                if (db.SaveChanges() > 0)
                {
                    return Content("<script>alert('操作成功');window.location.href='/SysAdmin/Author/Roles'</script>");
                }
                return Content("<script>alert('操作失败');window.location.href='/SysAdmin/Author/Roles'</script>");
            }

        }
        #endregion


        #region 用户角色管理
        /// <summary>
        /// 用户角色分配
        /// </summary>
        /// <returns></returns>
        [MenuCurrent("用户角色管理", PAction = "Index", Icon = "fa-group")]
        public ActionResult UserRoles(long userId = 0, long rolesId = 0)
        {
            var pageIndex = Request["pageIndex"].AsInt(1);
            var pageSize = Request["pageSize"].AsInt(20);
            var queryModel = new UserRoleQueryModel()
            {
                Index = pageIndex,
                Size = pageSize
            };
            using (var db = new MbContext())
            {
                var models = db.UserRoles.ToList();
                var userIds = models.Select(x => x.UserId).ToArray();
                queryModel.UserDic = db.User
                    .Where(x => userIds.Contains(x.UserId))
                    .ToLookup(x => x.UserId)
                    .ToDictionary(x => x.Key, x => x.First());
                var rolesIds = models.Select(x => x.RolesId).ToArray();
                queryModel.RolesDic = db.Roles
                    .Where(x => rolesIds.Contains(x.RolesId))
                    .ToLookup(x => x.RolesId)
                    .ToDictionary(x => x.Key, x => x.First());
                queryModel.Data = models.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                queryModel.Count = models.Count;
                return View(queryModel);
            }
        }
        /// <summary>
        /// 用户角色编辑（添加，修改）
        /// </summary>
        /// <returns></returns>
        [MenuHide("用户角色编辑", PAction = "UserRoles")]
        public ActionResult UserRolesEdit(long id = 0)
        {
            using (var db = new MbContext())
            {
                ViewData["users"] = db.User.ToList();
                ViewData["roles"] = db.Roles.ToList();
                if (id > 0)
                {
                    return View(db.UserRoles.FirstOrDefault(x => x.Id == id));
                }
                return View();
            }
        }
        /// <summary>
        /// 用户角色编辑（添加，修改）  POST
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [MenuHide("用户角色编辑", PAction = "UserRoles")]
        [HttpPost]
        public ActionResult UserRolesEdit(UserRoles model)
        {
            using (var db = new MbContext())
            {

                var userRoles = db.UserRoles.FirstOrDefault(x => x.UserId == model.UserId && x.RolesId == model.RolesId);
                if (userRoles != null)
                {
                    return Content("<script>alert('该用户角色已存在');window.location.href='/SysAdmin/Author/UserRoles'</script>");
                }
                if (model.UserId <= 0)
                {
                    model.Id = SequNo.NewId;
                }
                db.UserRoles.AddOrUpdate(model);
                if (db.SaveChanges() > 0)
                {
                    return Content("<script>alert('操作成功');window.location.href='/SysAdmin/Author/UserRoles'</script>");
                }
                return Content("<script>alert('操作失败');window.location.href='/SysAdmin/Author/UserRoles'</script>");
            }
        }

        /// <summary>
        /// 用户角色删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [MenuHide("用户角色删除", PAction = "UserRoles")]
        public ActionResult UserRolesDelete(long id)
        {
            using (var db = new MbContext())
            {
                var userRoles = db.UserRoles.FirstOrDefault(x => x.Id == id);
                if (userRoles != null)
                {
                    db.UserRoles.Remove(userRoles);
                    if (db.SaveChanges() > 0)
                    {
                        return Content("<script>alert('操作成功');window.location.href='/SysAdmin/Author/UserRoles'</script>");
                    }
                }
                return Content("<script>alert('操作失败');window.location.href='/SysAdmin/Author/UserRoles'</script>");
            }

        }
        #endregion



        #region 用户管理
        /// <summary>
        /// 用户列表
        /// </summary>
        /// <returns></returns>
        [MenuCurrent("用户管理", PAction = "Index", Icon = "fa-user")]
        public ActionResult User(string key)
        {
            var user1 = CurrentUser;
            var pageIndex = Request["pageIndex"].AsInt(1);
            var pageSize = Request["pageSize"].AsInt(20);
            var queryModel = new UserQueryModel()
            {
                Index = pageIndex,
                Size = pageSize,
                RealNameLike = key ?? null
            };
            using (var db = new MbContext())
            {
                var models = db.User
                    .Where(queryModel)
                    .ToList();
                var useCompanyUser = db.UseCompanyUser.FirstOrDefault(x => x.UserId == user1.UserId);
                var serviceCompanyUser = db.ServiceCompanyUser.FirstOrDefault(x => x.UserId == user1.UserId);
                //当前用户为报修公司的用户
                if (useCompanyUser != null)
                {
                    var userIds =
                        db.UseCompanyUser.Where(x => x.UseCompanyId == useCompanyUser.UseCompanyId)
                            .Select(x => x.UserId)
                            .ToArray();
                    models = models.FindAll(x => userIds.Contains(x.UserId));
                }
                //当前用户为报修公司的用户
                if (serviceCompanyUser != null)
                {
                    var userIds =
                        db.ServiceCompanyUser.Where(x => x.ServiceCompanyId == serviceCompanyUser.ServiceCompanyId)
                            .Select(x => x.UserId)
                            .ToArray();
                    models = models.FindAll(x => userIds.Contains(x.UserId));
                }
                queryModel.Count = models.Count;
                queryModel.Data = models.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

                var userId = queryModel.Data.Select(x => x.UserId).ToArray();
                queryModel.UseCompanyUserDic = db.UseCompanyUser.Where(x => userId.Contains(x.UserId))
                    .ToLookup(x => x.UserId)
                    .ToDictionary(x => x.Key, x => x.First());
                queryModel.UseCompanyDic = db.UseCompany
                    .ToLookup(x => x.UseCompanyId)
                    .ToDictionary(x => x.Key, x => x.First());

                queryModel.serviceCompanyUserDic = db.ServiceCompanyUser
                    .Where(x => userId.Contains(x.UserId))
                    .ToLookup(x => x.UserId)
                    .ToDictionary(x => x.Key, x => x.First());
                queryModel.serviceCompanyDic = db.ServiceCompany
                    .ToLookup(x => x.ServiceCompanyId)
                    .ToDictionary(x => x.Key, x => x.First());
                return View(queryModel);
            }

        }

        /// <summary>
        /// 用户编辑（添加，修改）
        /// </summary>
        /// <returns></returns>
        [MenuHide("用户编辑", PAction = "User")]
        public ActionResult UserEdit(long id = 0)
        {
            using (var db = new MbContext())
            {
                ViewData["serviceCompanyUser"] = db.ServiceCompanyUser.FirstOrDefault(x => x.UserId == id);
                ViewData["serviceCompany"] = db.ServiceCompany.ToList();
                ViewData["useCompanyUser"] = db.UseCompanyUser.FirstOrDefault(x => x.UserId == id);
                ViewData["useCompany"] = db.UseCompany.ToList();

                ViewData["userType"] =
                    db.Roles.FirstOrDefault(
                        m => m.RolesId == db.UserRoles.FirstOrDefault(x => x.UserId == CurrentUser.UserId).RolesId)
                        .CompanyType;
                if (id > 0)
                {
                    return View(db.User.FirstOrDefault(x => x.UserId == id));
                }
                return View();
            }
        }

        /// <summary>
        /// 用户编辑（添加，修改）  POST
        /// </summary>
        /// <param name="user"></param>
        /// <param name="useCompanyId"></param>
        /// <param name="serviceCompanyId"></param>
        /// <returns></returns>
        [MenuHide("用户编辑", PAction = "User")]
        [HttpPost]
        public ActionResult UserEdit(User user, long useCompanyId = 0, long serviceCompanyId = 0)
        {
            using (var db = new MbContext())
            {
                if (user.UserId > 0)
                {
                    var model = db.User.FirstOrDefault(x => x.UserId == user.UserId);
                    if (model != null)
                    {
                        model.Phone = user.Phone;
                        model.NickName = user.NickName;
                        model.RealName = user.RealName;
                        model.Sex = user.Sex;
                        model.Age = user.Age;
                        model.Email = user.Email;
                    }
                    db.User.AddOrUpdate(model);
                }
                else
                {
                    var users = db.User.FirstOrDefault(x => x.Account == user.Account);
                    if (users != null)
                    {
                        return Content("<script>alert('该账户名已存在');window.location.href='/SysAdmin/Author/UserEdit'</script>");
                    }

                    user.CreateTime = DateTime.Now;
                    user.RegTime = DateTime.Now;
                    user.LastLoginTime = DateTime.Now;
                    user.State = UserState.Normal.ToString();
                    user.UserId = SequNo.NewId;
                    db.User.AddOrUpdate(user);
                    //添加安全账户信息
                    var safty1 = new SafetyAccount()
                    {
                        Type = "LoginPassword",
                        UserId = user.UserId,
                        CreateTime = DateTime.Now,
                        Content = Md5Helper.MD5Encrption("qwert111")
                    };
                    db.SafetyAccount.Add(safty1);
                }


                //添加或修改报修单位用户
                var useCompanyUser = db.UseCompanyUser.FirstOrDefault(x => x.UserId == user.UserId);
                if (useCompanyUser != null)
                {

                    if (useCompanyId == 0)
                    {
                        db.UseCompanyUser.Remove(useCompanyUser);
                    }
                    else
                    {
                        useCompanyUser.UseCompanyId = useCompanyId;
                        db.UseCompanyUser.AddOrUpdate(useCompanyUser);
                    }
                }
                else
                {
                    if (useCompanyId > 0)
                    {
                        useCompanyUser = new UseCompanyUser()
                    {
                        UseCompanyUserId = SequNo.NewId,
                        UserId = user.UserId,
                        UseCompanyId = useCompanyId,
                        Type = "Ordinary",
                        CreateTime = DateTime.Now
                    };

                        db.UseCompanyUser.AddOrUpdate(useCompanyUser);
                    }

                }

                //添加或修改服务单位用户
                var serviceCompanyUser = db.ServiceCompanyUser.FirstOrDefault(x => x.UserId == user.UserId);
                if (serviceCompanyUser != null)
                {

                    if (serviceCompanyId == 0)
                    {
                        db.ServiceCompanyUser.Remove(serviceCompanyUser);
                    }
                    else
                    {
                        serviceCompanyUser.ServiceCompanyId = serviceCompanyId;
                        db.ServiceCompanyUser.AddOrUpdate(serviceCompanyUser);
                    }
                }
                else
                {
                    if (serviceCompanyId > 0)
                    {
                        serviceCompanyUser = new ServiceCompanyUser()
                    {
                        ServiceCompanyUserId = SequNo.NewId,
                        UserId = user.UserId,
                        ServiceCompanyId = serviceCompanyId,
                        Title = "高级技工",
                        WorkOn = DateTime.Now,
                        CreateTime = DateTime.Now
                    };
                        db.ServiceCompanyUser.AddOrUpdate(serviceCompanyUser);
                    }

                }
                if (db.SaveChanges() > 0)
                {
                    return Content("<script>alert('操作成功');window.location.href='/SysAdmin/Author/User'</script>");
                }
                return Content("<script>alert('操作失败');window.location.href='/SysAdmin/Author/User'</script>");
            }



        }

        /// <summary>
        /// 用户删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [MenuHide("用户删除", PAction = "User")]
        public ActionResult UserDelete(long id)
        {
            using (var db = new MbContext())
            {
                var model = db.User.FirstOrDefault(x => x.UserId == id);
                if (model != null)
                {
                    db.User.Remove(model);
                }
                if (db.SaveChanges() > 0)
                {
                    return Content("<script>alert('操作成功');window.location.href='/SysAdmin/Author/User'</script>");
                }
                return Content("<script>alert('操作失败');window.location.href='/SysAdmin/Author/User'</script>");
            }
        }
        #endregion

    }
}
