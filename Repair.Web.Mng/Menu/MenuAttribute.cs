using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;

namespace Repair.Web.Mng.Menu
{
    /// <summary>
    /// 菜单
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class MenuAttribute : AuthorizeAttribute
    {
        private static readonly string[] PartList = new[] {"area", "controller", "action"};
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var ctx = filterContext.Controller.ControllerContext.RequestContext;
            //未登录，采用默认的方式处理
            if (!ctx.HttpContext.User.Identity.IsAuthenticated)
            {
                base.OnAuthorization(filterContext);
                return;
            }
            var routeData = ctx.RouteData;
            var rout = new RouteValueDictionary();
            var r = (Route)routeData.Route;
            var list = new [] {routeData.Values, r.DataTokens, r.Defaults};

            foreach (var s in PartList)
            {
                foreach (var dic in list)
                {
                    var v = dic[s];
                    if(v == null) continue;

                    rout[s] = v;
                    break;
                }
            }

            var url1 = RouteUtils.Url( rout);
            Roles = MenuMng.GenId(url1);

            base.OnAuthorization(filterContext);
        }


        public MenuAttribute(string title)
        {
            Title = title;
            Icon = "fa-hand-o-right";
        }

        internal MemberInfo Info;

        /// <summary>
        /// 上级菜单区域
        /// 
        /// null : 不设置，表示自动集成 当前动作
        /// string.empty 表示清空
        /// 有值，表示重新设置
        /// </summary>
        public string PArea { get; set; }

        /// <summary>
        /// 上级菜单控制器名称
        /// </summary>
        public string PController { get; set; }

        /// <summary>
        /// 上级菜单响应名称
        /// </summary>
        public string PAction { get; set; }

        /// <summary>
        /// 隐藏菜单
        /// </summary>
        public bool Hide { get; set; }

        /// <summary>
        /// 模块图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 上级菜单模块响应名称
        /// </summary>
        public string PModual { get; set; }

        /// <summary>
        /// 模块
        /// </summary>
        public string Modual { get; set; }


        /// <summary>
        /// 同级菜单显示顺序，
        /// </summary>
        public long Sequ { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 默认的链接
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Remark
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 产生菜单列表
        /// </summary>
        /// <returns></returns>
        public virtual MenuItem[] Generate()
        {
            return new[] { CreateMenu() };
        }

        protected readonly RouteValueDictionary RouteValue = new RouteValueDictionary();

        protected virtual MenuItem CreateMenu()
        {
            //读取当前方法的 action、controller ,  
            var menu = new MenuItem
            {
                Title = Title,
                Icon = Icon,
                Remark = Remark,
                Sequ = Sequ,
                Hide = Hide
            };
            var url = Url;

            if (Info != null)
            {//兼容自动配置的

                menu.ActionName = Info.Name;
                RouteValue["action"] = Info.Name;
                RouteValue["controller"] = Info.ReflectedType.Name.Replace("Controller", string.Empty);

                if (!string.IsNullOrEmpty(Modual))
                    RouteValue["modual"] = Modual;

                //读取当前方法的 areas
                {
                    var arr = new List<string>(Info.ReflectedType.FullName.Split('.'));
                    var i = arr.IndexOf("Areas");
                    if (i >= 0)
                    {
                        RouteValue["area"] = arr[i + 1];
                    }
                }

                url = RouteUtils.Url(RouteValue);
            }

            menu.Url = url;
            menu.Id = MenuMng.GenId(url);

            if (!string.IsNullOrEmpty(PArea))
                RouteValue["area"] = PArea;
            else if (PArea == string.Empty && RouteValue.ContainsKey("area"))
                RouteValue.Remove("area");

            if (!string.IsNullOrEmpty(PController))
                RouteValue["controller"] = PController;
            else if (PController == string.Empty && RouteValue.ContainsKey("controller"))
                RouteValue.Remove("controller");

            if (!string.IsNullOrEmpty(PAction))
                RouteValue["action"] = PAction;
            else if (PAction == string.Empty && RouteValue.ContainsKey("action"))
                RouteValue.Remove("action");

            if (!string.IsNullOrEmpty(PModual))
                RouteValue["modual"] = PModual;
            else if (PModual == string.Empty && RouteValue.ContainsKey("modual"))
                RouteValue.Remove("modual");

            url = RouteUtils.Url(RouteValue);

            menu.ParentId = MenuMng.GenId(url);

            if (menu.Id == menu.ParentId)
                menu.ParentId = string.Empty;

            return menu;
        }


        public override string ToString()
        {
            return string.Format("{0} {1}.{2}", Title, Info.ReflectedType.FullName, Info.Name);
        }

    }
}