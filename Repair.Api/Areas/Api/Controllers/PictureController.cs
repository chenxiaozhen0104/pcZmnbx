using LZY.BX.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Repair.Api.Areas.Api.Controllers
{
    public class PictureController : Controller
    {
        // GET: Api/Pictrue
        /// <summary>
        /// 更新设备图片
        /// </summary>
        /// <param name="pictureId">pictrue数组</param>
        /// <param name="outId">outId</param>
        /// <returns></returns>
        public ActionResult EditPicture(long?[] pictureId, string outId)
        {
            if (new PictureService().EditPicture(pictureId, outId) > 0)
            {
                return Json(new { result = "图片编辑成功" });
            }
            else
            {
                return Json(new { result = "图片编辑失败" });
            }
        }
    }
}