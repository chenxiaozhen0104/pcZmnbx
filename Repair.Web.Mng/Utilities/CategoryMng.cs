using LZY.BX.Model;
using LZY.BX.Service;
using LZY.BX.Service.Mb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using ZR;

namespace Repair.Web.Mng.Utilities
{
    public class CategoryMng
    {
        private static string _categoryTreeCacheKey = "_categoryTreeCacheKey2016";

        //private static List<Category> _categoryTree;
        //类目树形
        public static List<Category> CategoryTree
        {
            get
            {
                var _categoryTree = new List<Category>();

                if (HttpRuntime.Cache.Get(_categoryTreeCacheKey) == null)
                {
                    lock (typeof(CategoryMng))
                    {
                        if (HttpRuntime.Cache.Get(_categoryTreeCacheKey) == null)
                        { 
                            var templist = new List<Category>();
                            //数据库查询
                            using (var db = new MbContext())
                            {
                                templist = db.Category.ToList();
                            }

                            //foreach (var item in templist.Where(t => t.Pid == 0))
                            //{
                            //    Rebuild(item, templist);

                            //    _categoryTree.Add(item);
                            //}

                            _categoryTree.Sort((x, y) => x.SortField.CompareTo((y.SortField)));
                        }
                    }
                    //DOTO滑动过期， 当3分钟内无人操作即过期
                    HttpRuntime.Cache.Insert(_categoryTreeCacheKey, _categoryTree, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromSeconds(60 * 3));
                }

                _categoryTree = HttpRuntime.Cache.Get(_categoryTreeCacheKey) as List<Category>;

                return _categoryTree;


                //if (_categoryTree == null)
                //{
                //    lock (typeof(CategoryMng))
                //    {
                //        if (_categoryTree == null)
                //        {
                //            _categoryTree = new List<Category>();

                //            var templist = new List<Category>();
                //            //数据库查询
                //            using (var db = new MbContext())
                //            {
                //                templist = db.Category.ToList();
                //            }

                //            foreach (var item in templist.Where(t => t.Pid == 0))
                //            {
                //                Rebuild(item, templist);

                //                _categoryTree.Add(item);
                //            }

                //            _categoryTree.Sort((x, y) => x.SortField.CompareTo((y.SortField)));
                //        }
                //    }
                //}
                //return _categoryTree;
            }
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Category Select(long id)
        {
            var newList = ToList();

            return newList.Find(t => t.CategoryId == id);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="pCategory">父节点</param>
        /// <param name="category"></param>
        /// <returns></returns>
        public static bool Insert(Category category)
        {
            //var newList = ToList();

            //var model = newList.Find(t => t.CategoryId == category.Pid);

            //if (model == null)
            //{
            //    return false;
            //}

            //using (var db = new MbContext())
            //{
            //    category.CategoryId = SequNo.NewId;
            //    category.CreateTime = DateTime.Now;

            //    db.Category.Add(category);

            //    if (db.SaveChanges() > 0)
            //    {
            //        //指定父节点
            //        category.ParentItem = model;
            //        //添加子节点
            //        model.SubItems.Add(category);
            //        //排序
            //        model.SubItems.Sort((x, y) => x.SortField.CompareTo((y.SortField)));

            //        return true;
            //    }

            //    return false;
            //}

            using (var db = new MbContext())
            {
                //var model = db.Category.FirstOrDefault(t => t.CategoryId == category.Pid);
                //if (model == null)
                //{
                //    //DOTO 没有存在父节点，不允许添加
                //    return false;
                //}

                category.CategoryId = SequNo.NewId;
                category.CreateTime = DateTime.Now;

                db.Category.Add(category);

                if (db.SaveChanges() > 0)
                {
                    //DOTO 删除缓存
                    HttpRuntime.Cache.Remove(_categoryTreeCacheKey);
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public static bool Update(Category category)
        {
            //var newList = ToList();

            //var model = newList.Find(t => t.CategoryId == category.CategoryId);
            //if (model == null)
            //{
            //    return false;
            //}

            //using (var db = new MbContext())
            //{
            //    //DOTO 数据库操作
            //    var dCategory = db.Category.FirstOrDefault(t => t.CategoryId == category.CategoryId);

            //    dCategory.Name = category.Name;
            //    dCategory.SortField = category.SortField;

            //    if (db.SaveChanges() > 0)
            //    {
            //        //DOTO 修改
            //        model.Name = category.Name;
            //        model.SortField = category.SortField;

            //        if (model.ParentItem != null)
            //        {
            //            model.ParentItem.SubItems.Sort((x, y) => x.SortField.CompareTo((y.SortField)));
            //        }
            //        return true;
            //    }
            //    return false;
            //}

            using (var db = new MbContext())
            {
                //DOTO 数据库操作
                var dCategory = db.Category.FirstOrDefault(t => t.CategoryId == category.CategoryId);
                if (dCategory == null)
                {
                    //DOTO 找不到对应的节点
                    return false;
                }
                dCategory.Name = category.Name;
                dCategory.SortField = category.SortField;

                if (db.SaveChanges() > 0)
                {
                    //DOTO 删除缓存
                    HttpRuntime.Cache.Remove(_categoryTreeCacheKey);
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool Remove(long id)
        {
            //var newList = ToList();

            //var category = newList.Find(t => t.CategoryId == id);
            //if (category == null)
            //{
            //    return false;
            //}
            //var pCategory = newList.Find(t => t.CategoryId == category.Pid);
            //if (pCategory == null)
            //{
            //    return false;
            //}

            //using (var db = new MbContext())
            //{
            //    //DOTO 数据库删除
            //    var dCategory = db.Category.FirstOrDefault(t => t.CategoryId == id);
            //    db.Category.Remove(dCategory);

            //    if (db.SaveChanges() > 0)
            //    {
            //        //DOTO 删除
            //        pCategory.SubItems.Remove(category);
            //        return true;
            //    }
            //    return false;
            //}

            using (var db = new MbContext())
            {
                //DOTO 数据库删除
                var dCategory = db.Category.FirstOrDefault(t => t.CategoryId == id);
                db.Category.Remove(dCategory);

                if (db.SaveChanges() > 0)
                {
                    //DOTO 删除缓存
                    HttpRuntime.Cache.Remove(_categoryTreeCacheKey);
                    return true;
                }
                return false;
            }
        }

        public static List<Category> ToList()
        {
            var newList = new List<Category>();

            ToList(CategoryTree, newList);

            return newList;
        }

        /// <summary>
        /// 获取父节点
        /// </summary>
        /// <param name="cur"></param>
        /// <returns></returns>
        public static List<long> ParentIds(Category cur)
        {
            var activeMenu = new List<long>();

            while (cur != null)
            {
                activeMenu.Add(cur.CategoryId);

                //cur = cur.ParentItem;
            }
            return activeMenu;
        }

        private static void ToList(List<Category> list, List<Category> newList)
        {
            foreach (var item in list)
            {
                newList.Add(item);

                //ToList(item.SubItems, newList);
            }
        }

        private static void Rebuild(Category root, List<Category> list)
        {
            //var subItems = list.Where(t => t.Pid == root.CategoryId);
            ////添加
            //root.SubItems.AddRange(subItems);
            ////排序
            //root.SubItems.Sort((x, y) => x.SortField.CompareTo((y.SortField)));

            //foreach (var item in root.SubItems)
            //{
            //    item.ParentItem = root;

            //    Rebuild(item, list);
            //}
        }
    }
}