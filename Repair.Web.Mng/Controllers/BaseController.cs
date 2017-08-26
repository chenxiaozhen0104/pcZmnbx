using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LZY.BX.Model;
using LZY.BX.Service.Mb;

using Repair.Web.Mng.Utilities;

namespace Repair.Web.Mng.Controllers
{
    public class BaseController : DefaultControllerBase<BaseController>
    {
        //
        // GET: /Base/
        //protected override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    if (Session["UserId"] == null)
        //    {
        //        if (this.RouteData.Values["Controller"].ToString() != "Login")
        //        {
        //            filterContext.Result = new RedirectResult("/Login/Index");
        //        }
        //    }
        //    else
        //    {
        //        //TODO 加载用户配置的菜单，角色，赋值给用户
        //    }
        //    base.OnActionExecuting(filterContext);


        //}




        #region 权限分级

        public static Dictionary<string, User> App_Users { get; set; }

        static BaseController()
        {
            App_Users = new Dictionary<string, User>();
        }


        public static User CurrentUser
        {
            get
            {
                UserSession.InitUser();

                var user = System.Web.HttpContext.Current.User as UserSession;
                return user.User;

            }
        }
        #endregion




    }
}
