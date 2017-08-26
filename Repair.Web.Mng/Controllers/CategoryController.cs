using LZY.BX.Model;
using LZY.BX.Service.Mb;
using Repair.Web.Mng.Menu;
using Repair.Web.Mng.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Repair.Web.Mng.Controllers
{
    public class CategoryController : ControllerBase<CategoryController>
    {
        //
        // GET: /Category/

        [MenuCurrent("类目管理", PAction = "Index", PArea = "SysAdmin", PController = "CommData")]
        public ActionResult Index()
        {
            return View(CategoryMng.CategoryTree);
        }

        /// <summary>
        /// 增加
        /// </summary>
        /// <returns></returns>
        public ActionResult Add(long pid = 0)
        {
            return View(CategoryMng.Select(pid));
        }

        /// <summary>
        /// 增加节点，Ajax调用
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Add(Category category)
        {
            using (var db = new MbContext())
            {
                var model = db.Category.FirstOrDefault(t => t.Name == category.Name);
                if (model != null)
                {
                    return ResultError("不得与现存的类目名称相同");
                }
            }

            if (CategoryMng.Insert(category))
            {
                return ResultSuccess("操作成功", "/Category/Index");
            }
            else
            {
                return ResultError("操作失败", "/Category/Index");
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit(long id = 0)
        {
            return View(CategoryMng.Select(id));
        }

        /// <summary>
        /// 修改节点，Ajax调用
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(Category category)
        {
            using (var db = new MbContext())
            {
                var model = db.Category.FirstOrDefault(t => t.Name == category.Name && t.CategoryId != category.CategoryId);
                if (model != null)
                {
                    return ResultError("不得与现存的类目名称相同");
                }
            }

            if (CategoryMng.Update(category))
            {
                return ResultSuccess("操作成功", "/Category/Index");
            }
            else
            {
                return ResultError("操作失败", "/Category/Index");
            }
        }

        /// <summary>
        /// 删除节点，Ajax调用
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Remove(long id)
        {
            if (CategoryMng.Remove(id))
            {
                return ResultSuccess("操作成功");
            }
            else
            {
                return ResultError("操作失败");
            }
        }
    }
}
