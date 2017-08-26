using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ZR;

namespace Repair.Web.Mng.Menu
{
    /// <summary>
    /// �˵�������
    /// </summary>
    public class MenuBuilder
    {
        /// <summary>
        /// ȫ�´����˵���
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, MenuItem> BuildMenuTree()
        {
            var dic = new Dictionary<string, MenuItem>(1000, StringComparer.CurrentCultureIgnoreCase);

            var list = BuildMenu().ToList();

            foreach (var item in list)
            {
                foreach (var m in item.Generate())
                {
                    if (dic.ContainsKey(m.Id))
                    {
                        continue;
                    }
                    dic.Add(m.Id, m);
                }
            }
            //���ò˵����¼���ϵ
            foreach (var item in dic.Values)
            {
                MenuItem x;
                if (!dic.TryGetValue(item.ParentId, out x))
                    continue;

                x.SubItems.Add(item);
            }
            //��������
            foreach (var item in dic.Values)
            {
                item.SubItems.Sort((x, y) => x.Sequ.CompareTo((y.Sequ)));
            }


            SetLevel(dic.Values.Where(x => x.ParentId == string.Empty).ToList(), 1);
            return dic;
        }



        private static IEnumerable<MenuAttribute> BuildMenu()
        {
            var list = new List<MenuAttribute>();
            ReflectionHelper.FindAttributes<Controller, MenuAttribute>(AppDomain.CurrentDomain.GetAssemblies(),
                (m, x) =>
                {
                    list.Add(x);
                    x.Info = m;
                });

            return list;
        }

        private static void SetLevel(IEnumerable<MenuItem> items, int level)
        {
            foreach (var menuItem in items)
            {
                menuItem.Level = level;

                SetLevel(menuItem.SubItems, level + 1);
            }
        }
    }
}