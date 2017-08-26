using System;

namespace Repair.Web.Mng.Menu
{
    /// <summary>
    /// 二级菜单，联到一级菜单，默认到 当前域中 Default->Index 动作
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class MenuCurrentAttribute : MenuAttribute
    {
        public MenuCurrentAttribute(string title)
            : base(title)
        {
            PAction = "Index";
        }
    }


    /// <summary>
    /// 二级菜单，联到一级菜单，默认到 当前域中 Default->Index 动作
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class MenuHideAttribute : MenuAttribute
    {
        public MenuHideAttribute(string title)
            : base(title)
        {
            PAction = "Index";
            Hide = true;
        }
    }
}