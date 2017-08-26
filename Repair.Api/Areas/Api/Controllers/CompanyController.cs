using LZY.BX.Model;
using LZY.BX.Model.Enum;
using LZY.BX.Service;
using Repair.Api.Areas.Api.Utilities;
using Repair.Api.Areas.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using WeiXin;

namespace Repair.Api.Areas.Api.Controllers
{
    public class CompanyController : ControllerApiBase
    {
        CompanyService cService = new CompanyService();

        public ActionResult ListServiceCompany()
        {
            try
            {
                var list = cService.GettAllServiceInfo();

                return Json(list.Select(t => new
                {
                    ServiceCompanyName = t.Name,
                    ServiceCompanyId = t.ServiceCompanyId
                }));
            }
            catch (Exception e)
            {
                Logger.Error("查询查询服务单位异常", e);
                return Json(new { error = "服务器繁忙，请稍后再试试" });
            }
        }

        /// <summary>
        /// Author:Gavin
        /// Description:查询符合的企业信息
        /// Create Data:2017-03-18
        /// </summary>
        /// <param name="appType">app类型</param>
        /// <param name="name">公司名</param>
        /// <returns></returns>
        public ActionResult GetAllCompanyInfo(AppType appType, string name)
        {

            try
            {
                if (appType == AppType.Use)
                {
                    var model = cService.GetAllCompanyInfo(name);
                    if (model != null)
                    {
                        return Json(model.Select(m => new { m.Name, m.Phone, m.Position, m.Contact, Id = m.UseCompanyId }), JsonRequestBehavior.AllowGet);
                    }
                }
                else if (appType == AppType.Service)
                {
                    var model = cService.GettAllServiceInfo(name);
                    if (model != null)
                    {

                        return Json(model.Select(m => new { m.Name, m.Phone, m.Position, m.Contact, Id = m.ServiceCompanyId }), JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Author:Gavin
        /// Description:加入使用(服务)企业
        /// Create Data:2017-03-18
        /// </summary>
        /// <param name="appType"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AddCompany(AppType appType, string id)
        {
            try
            {
                if (AppType.Use == appType)
                {
                    cService.AddCompany(ApiUser.Current, id);
                }
                else if (AppType.Service == appType)
                {
                    cService.AddServerCompany(ApiUser.Current, id);
                }
                //更新缓存
                ApiUser.UpdateCurrent(ApiUser.Current.UserId);
                return Json(new { status = 0 });
            }
            catch (DataExistException e)
            {
                return Json(new { error = e.Message });
            }
        }

        /// <summary>
        /// Author:Gavin
        /// Create Date:2017-03-20
        /// Description:使用单位管理员获取 人员列表
        /// </summary>       
        /// <returns></returns>
        public ActionResult GetCompanyPersonList()
        {
            try
            {
                var model = cService.GetCompanyPersonList(ApiUser.Current);
                if (model != null)
                {
                    return Json(model.Select(m => new { m.NickName, m.Sex, m.Phone, m.Email, UserId = m.UserId, m.AreaName }), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }

            }
            catch (DataExistException e)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// Author:Gavin
        /// Create Data:2017-03-20
        /// Descriptio:设置使用单位人员的区域
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="areas"></param>
        /// <returns></returns>
        public ActionResult SetCompanyPersonArea(string userId, string areas)
        {
            try
            {
                cService.SetCompanyPersonArea(userId, areas);
                return Json(new { status = 0 });
            }
            catch (DataExistException e)
            {
                return Json(new { error = e.Message });
            }
        }


        /// <summary>
        /// Description:获取签约单位列表
        /// Author:Gavin
        /// Create Date:2017/3/31
        /// </summary>
        /// <returns></returns>
        public ActionResult GetJoinCompanys()
        {
            try
            {
                var list = cService.GetJoinCompanys(ApiUser.Current.UseCompanyId);
                return Json(list.Select(x => new { Name = x.Name, ServiceCompanyId = x.ServiceCompanyId }).ToList(), JsonRequestBehavior.AllowGet);
            }
            catch (DataExistException e)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 维修公司编辑
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ActionResult EditField(string id, string fieldName, string value)
        {

            ServiceCompany info = CompanyService.Instance.GetServiceCompany(id);
            if (fieldName == "Name")
            {
                //擦好看公司名存不存在
                if (CompanyService.Instance.GetServiceCompanyInfoByName(value) != null)
                {
                    return Json(new { status = 1, msg = "公司名已经存在" });
                }
            }
            var prop = typeof(ServiceCompany).GetProperty(fieldName);
            var type = prop.PropertyType;
            //判断convertsionType是否为nullable泛型类
            if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                //如果type为nullable类，声明一个NullableConverter类，该类提供从Nullable类到基础基元类型的转换
                System.ComponentModel.NullableConverter nullableConverter = new System.ComponentModel.NullableConverter(type);
                //将type转换为nullable对的基础基元类型
                type = nullableConverter.UnderlyingType;
                if (string.IsNullOrEmpty(value))
                {
                    prop.SetValue(info, null);
                }
                else
                {
                    prop.SetValue(info, Convert.ChangeType(value, type), null);
                }
            }
            else
            {
                var val = Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
                prop.SetValue(info, val);
            }
            CompanyService.Instance.Edit(info);
            return Json(new { status = 0 });
        }

        /// <summary>
        /// 获取工作方式
        /// </summary>
        /// <param name="serverCompanyId"></param>
        /// <returns></returns>
        public ActionResult GetWorkWay(string serviceCompanyId)
        {

            ServiceCompany info = CompanyService.Instance.GetServiceCompany(serviceCompanyId);
            return Json(new { workingway = info.WorkingWay, workedway = info.WorkedDWay }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取技能列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCategorys()
        {

            List<Category> listCategory = CompanyService.Instance.GetCategorys();

            return Json(listCategory.Select(t => new { t.CategoryId, t.Name }).ToArray(), JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// 根据技能筛选服务公司
        /// </summary>
        /// <param name="CategoryId"></param>
        /// <returns></returns>
        public ActionResult GetServerListByCategory(long CategoryId)
        {
            List<ServiceCompany> list = CompanyService.Instance.GetServerListByCategory(CategoryId.ToString());
            return Json(list.Select(t => new { ServiceCompanyId = t.ServiceCompanyId.ToString(), t.Name, t.Note, ImgUrl = new PictureService().imgPath + t.ImgUrl }).ToArray(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取公司信息
        /// </summary>
        /// <param name="serverCompanyId"></param>
        /// <returns></returns>
        public ActionResult GetGetServiceCompanyInfo(long serviceCompanyId)
        {

            ServiceCompany info = CompanyService.Instance.GetServiceCompany(serviceCompanyId.ToString());
            return Json(new { ServiceCompanyId = info.ServiceCompanyId.ToString(), info.Categorys, ImgUrl = new PictureService().imgPath + info.ImgUrl, info.Note, info.WorkedDWay, info.WorkingWay }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 微信推荐服务公司
        /// </summary>
        /// <returns></returns>
        public ActionResult WxAddIntroductCompany(IntroducCompany info)
        {
            //本地测试,没有Session["WxUser"]
            if (Session["WxUser"] != null)
            {
                var wxUser = Session["WxUser"] as WeiXinUser;
                var user = new AuthAccountServer().Get(wxUser.openid);
                info.UserId = user.UserId;
            }
            if (CompanyService.Instance.WxAddIntroductCompany(info)>0)
            {
                //发送短信
                return Json(new { success = "推荐成功" });
            }
            else
            {
                return Json(new { error = "推荐失败" });
            }
        }

    }
}
