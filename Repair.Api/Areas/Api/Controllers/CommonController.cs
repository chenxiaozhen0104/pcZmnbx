using LZY.BX.Model.Enum;
using LZY.BX.Service;
using LZY.BX.SMSManager;
using Repair.Api.Areas.Utilities;
using System;
using System.Web.Mvc;

namespace Repair.Api.Areas.Api.Controllers
{
    public class CommonController : ControllerApiBase
    {
        //
        // GET: /Api/Common/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 发送验证码
        /// </summary>
        /// <returns></returns>
        public ActionResult SendVerifyCode(string mobile)
        {
            try
            {
                if (SMSManager.Instance.SendVerifyCode(mobile))
                {
                    return Json(new { status = 0 }
                        , JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { error = "服务器繁忙，请稍后再试试" }
                        , JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("发送验证码异常" + ex);

                return Json(new { error = "服务器繁忙，请稍后再试试" }
                        , JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 检查升级更新
        /// </summary>
        /// <returns></returns>
        public ActionResult CheckVersion(TerminalType terminalType, AppType appType, int version)
        {
            try
            {
                var ds = new DownloadAppService();

                var model = ds.Max(terminalType, appType);

                if (model != null && model.InVersion > version)
                {
                    return Json(model, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //没有更新信息
                    return Json(new
                    {
                        noupdate = 0
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("检查版本信息异常" + ex);

                return Json(new { error = "服务器繁忙，请稍后再试试" }
                        , JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 更新指定表的某个字段
        /// </summary>
        /// <param name="table">表名称</param>
        /// <param name="fileName">字段名称</param>
        /// <param name="fileValue">字段值</param>
        /// <param name="byField">where字段</param>
        /// <param name="byFieldValue">where字段值</param>
        /// <returns></returns>
        public ActionResult UpdateFieldValue(string table, string fileName, string fileValue, string byField, string byFieldValue) {
            if (new CommonService().UpdateFieldValue(table, fileName, fileValue, byField, byFieldValue) > 0)
            {
                return Json(new { result = "操作成功" });
            }else
            {
                return Json(new { result = "操作失败" });
            }
        }
    }
}