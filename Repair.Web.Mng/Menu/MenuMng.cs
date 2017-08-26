using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ZR.CommSign;

namespace Repair.Web.Mng.Menu
{
    /// <summary>
    /// �˵�������
    /// </summary>
    public static class MenuMng
    {
        public static void ReBuild()
        {
            _menuTree = MenuBuilder.BuildMenuTree();
        }

        /// <summary>
        /// ��ȡ��ǰ�˵�·��
        /// </summary>
        /// <param name="routeData"></param>
        /// <returns></returns>
        public static MenuItem GetCurMenu(RouteData routeData)
        {
            var route = new RouteValueDictionary(routeData.Values);
            if (route.ContainsKey("id"))
            {
                route.Remove("id");
            }

            var url1 = "/" + RouteUtils.Url(routeData.Route, route);
            var id = GenId(url1);

            MenuItem item;
            MenuTree.TryGetValue(id, out item);
            return item;
        }

        public static string GenId(string url)
        {
            return Md5SignChannel.Md5(url.ToLower(), Encoding.UTF8);
        }

        private static string _rootId;

        /// <summary>
        /// ϵͳ���˵�
        /// </summary>
        public static string RootId
        {
            get
            {
                if (string.IsNullOrEmpty(_rootId))
                {
                    foreach (var menuItem in MenuTree.Values)
                    {
                        if (!string.IsNullOrEmpty(menuItem.ParentId))
                            continue;
                        _rootId = menuItem.Id;
                        break;
                    }
                }
                return _rootId;
            }
        }

        /// <summary>
        /// ��ȡָ���˵����������б�
        /// </summary>
        /// <param name="id"></param>
        /// <param name="curMenu"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool GetMenuGroup(string id, out MenuItem curMenu, out List<MenuItem> list)
        {
            list = null;

            if (!MenuTree.TryGetValue(id, out curMenu))
                return false;

            MenuItem p;
            if (!MenuTree.TryGetValue(curMenu.ParentId, out p))
                return false;

            list = p.SubItems;
            return true;
        }

        /// <summary>
        /// ��ȡ��ǰλ�õĲ˵�
        /// </summary>
        /// <param name="viewContext"></param>
        /// <param name="curMenu"></param>
        /// <param name="menuList"></param>
        /// <returns></returns>
        public static bool GetMenuGroup(ViewContext viewContext, out MenuItem curMenu, out List<MenuItem> menuList)
        {
            var controlName = viewContext.Controller.GetType().FullName;
            var action = (string)viewContext.RouteData.Values["action"];
            var modaul = (string)viewContext.RouteData.Values["modual"];

            if (GetMenuGroup(String.Join(".", controlName, action, modaul), out curMenu, out menuList))
                return true;

            if (GetMenuGroup(String.Join(".", controlName, action), out curMenu, out menuList))
                return true;

            if (GetMenuGroup(controlName, out curMenu, out menuList))
                return true;

            return false;
        }

        private static Dictionary<string, MenuItem> _menuTree;

        public static Dictionary<string, MenuItem> MenuTree
        {
            get
            {
                if (_menuTree == null)
                {
                    lock (typeof(MenuMng))
                    {
                        if (_menuTree == null)
                        {
                            ReBuild();
                        }
                    }
                }
                return _menuTree;
            }
        }

        /// <summary>
        /// ��ǰ�����ϼ��˵���
        /// </summary>
        /// <returns></returns>
        public static List<string> ParentIds(MenuItem curMenu)
        {
            var activeMenu = new List<string>();

            while (curMenu != null)
            {
                activeMenu.Add(curMenu.Id);

                MenuItem p;
                if (!MenuTree.TryGetValue(curMenu.ParentId, out p))
                    break;

                curMenu = p;
            }
            activeMenu.Reverse();
            return activeMenu;
        }

        /// <summary>
        /// �����һ��ĩ���ɼ��˵�
        /// </summary>
        /// <param name="curMenu"></param>
        /// <returns></returns>
        public static MenuItem GetLeaf(MenuItem curMenu)
        {
            MenuItem leaf;
            FirtLeafItem(curMenu, out leaf);
            return leaf;
        }

        /// <summary>
        /// �����һ��ĩ���ɼ��˵�
        /// </summary>
        /// <param name="curMenu"></param>
        /// <param name="leafItem"></param>
        /// <returns></returns>
        static bool FirtLeafItem(MenuItem curMenu, out MenuItem leafItem)
        {
            leafItem = null;

            var user = HttpContext.Current.User;

            {//�жϽڵ��Ƿ�Ϊĩ���ɼ��ڵ�
                if (curMenu.Hide || !user.IsInRole(curMenu.Id))
                    return false;

                if (!curMenu.HasVisiableSubItem)
                {
                    leafItem = curMenu;
                    return true;
                }
            }
            
            foreach (var menuItem in curMenu.SubItems)
            {
                if (FirtLeafItem(menuItem, out leafItem))
                    return true;
            }

            return false;
        }

    }
}