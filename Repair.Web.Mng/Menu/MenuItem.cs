using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;

namespace Repair.Web.Mng.Menu
{
    /// <summary>
    /// 授权菜单
    /// </summary>
    public class MenuItem
    {
        public MenuItem()
        {
            ParentId = string.Empty;
            SubItems = new List<MenuItem>();
            RouteValue = new RouteValueDictionary();
            ActionName = string.Empty;
        }

        /// <summary>
        /// 菜单Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 上级菜单Id
        /// 最顶菜单，ParentId为string.empty
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 同级菜单显示顺序，
        /// </summary>
        public long Sequ { get; set; }

        /// <summary>
        /// 菜单层级
        /// </summary>
        public long Level { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 隐藏菜单
        /// </summary>
        public bool Hide { get; set; }

        /// <summary>
        /// 菜单说明
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 响应地址名称
        /// </summary>
        public string ActionName { get; set; }


        /// <summary>
        /// 固定路由数据
        /// </summary>
        public RouteValueDictionary RouteValue { get; set; }
        
        /// <summary>
        /// 模块图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 下级菜单
        /// </summary>
        public List<MenuItem> SubItems { get; set; }

        /// <summary>
        /// 检查是否有可见子菜单
        /// </summary>
        public bool HasVisiableSubItem
        {
            get
            {
                return SubItems.Count(x => !x.Hide) > 0;
            }
        }

        /// <summary>
        /// 菜单分组Id
        /// </summary>
        public long GroupId { get { return Sequ / 100; } }

        /// <summary>
        /// 固定菜单
        /// </summary>
        public string Url { get; set; }


        internal static string[] SplitString(string original)
        {
            if (string.IsNullOrEmpty(original))
            {
                return new string[0];
            }

            var split = from piece in original.Split(',')
                        let trimmed = piece.Trim()
                        where !string.IsNullOrEmpty(trimmed)
                        select trimmed;
            return split.ToArray();
        }
    }

}
