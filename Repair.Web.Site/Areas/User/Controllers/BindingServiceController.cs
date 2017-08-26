using LZY.BX.Model;
using LZY.BX.Model.Enum;
using LZY.BX.Service;
using LZY.BX.Service.Mb;
using Repair.Web.Site.Areas.User.Models;
using Repair.Web.Site.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Repair.Web.Site.Areas.User.Controllers
{
    public class BindingServiceController : ControllerBase<BindingServiceController>
    {
        //
        // GET: /User/BindingService/
        BindingService bs = new BindingService();
        public ActionResult Index()
        {         
            return View();
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        public ActionResult BindingServiceList(BindingServiceQueryModel query)
        {
            try
            {
                query.Data = bs.GetBindingServiceList(CurrentUser.User.UseCompanyId);

                var deviceList = new DeviceService().GetDevice(CurrentUser.User.UseCompanyId).ToList();
              
                //所拥有的类目
                query.CategoryData = deviceList.Where(t => t.CategoryId >= 0).Select(t => t.Category).Where(m => m != null).ToLookup(t => t.CategoryId).ToDictionary(t => t.Key, t => t.First());
                query.ListDeviceDic = deviceList.ToLookup(t => t.DeviceId).ToDictionary(t => t.Key, t => t.First());
                query.AreaList = deviceList.Where(t => t.AreaId >= 0).Select(t => t.Area).Where(m => m != null).ToLookup(t => t.AreaId).ToDictionary(t => t.Key, t => t.First());
                query.BrandList = deviceList.Where(t => t.BrandId >= 0).Select(t => t.Brand).Where(m => m != null).ToList().ToLookup(t => t.BrandId).ToDictionary(t => t.Key, t => t.First());
                var ManufacturerDic = deviceList.Where(t => t.BrandId > 0).Select(t => t.Brand).ToList().Select(m => m.ManufacturerId).Where(m => m != null).ToArray();
                query.ManufacturerList = new ManufacturersSvr().GetManufatuer().Where(t => ManufacturerDic.Contains(t.ManufacturerId)).ToList().ToLookup(t => t.ManufacturerId).ToDictionary(t => t.Key, t => t.First());
                return View(query);
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("BindingServiceList:{0}", ex.Message);
                return View(query);
            }
           
        }
        BindingServiceQueryModel query = new BindingServiceQueryModel();

        /// <summary>
        /// 类目绑定列表
        /// </summary>
        /// <returns></returns>
        public ActionResult Category(BindingServiceQueryModel query)
        {
            query.Data = bs.GetBindingServiceList(CurrentUser.User.UseCompanyId).Where(t=>t.BindingType==BindingServiceType.Category).ToList();
            query.JoinCompanyDic=QueryJoinCompany();
            //所拥有的类目            
            query.CategoryData = new DeviceService().GetDevice(CurrentUser.User.UseCompanyId).Where(t=>t.CategoryId>=0).Select(t => t.Category).Where(m=>m!=null).ToList().ToLookup(t => t.CategoryId).ToDictionary(t => t.Key, t => t.First());

            return View(query);
        }

        /// <summary>
        /// 设备绑定列表
        /// </summary>
        /// <returns></returns>
        public ActionResult Equipment()
        {
            ViewBag.JoinCompany = QueryJoinCompany();
            ViewBag.Data = bs.GetBindingServiceList(CurrentUser.User.UseCompanyId).Where(t => t.BindingType == BindingServiceType.Equipment).ToList();
            using (var db = new MbContext())
            {
                //DOTO 获取当前单位下拥有的设备

                var equipmentModel = db.Device
                    .Where(x => x.UseCompanyId == CurrentUser.UseCompany.UseCompanyId)                    
                    .ToList();
                ViewBag.equipmentDic = equipmentModel.ToLookup(t => t.DeviceId).ToDictionary(t => t.Key, t => t.First());
                //品牌字典
                var brandsIds = equipmentModel.Where(x=>x.BrandId>=0).Select(x => x.BrandId).ToList();

                ViewBag.brandDic = db.Brand
                    .Where(x => brandsIds.Contains(x.BrandId))
                    .ToLookup(x => x.BrandId)
                    .ToDictionary(x => x.Key, x => x.First());

                //制造商字典
                var manufactorerIds = db.Brand.Where(x => brandsIds.Contains(x.BrandId)).Select(t=>t.ManufacturerId).ToList();

                ViewBag.manufactorerDic = db.Manufacturer
                    .Where(x => manufactorerIds.Contains(x.ManufacturerId))
                    .ToLookup(x => x.ManufacturerId)
                    .ToDictionary(x => x.Key, x => x.First());

               

                return View(equipmentModel);
            }
        }

        /// <summary>
        /// 区域绑定列表
        /// </summary>
        /// <returns></returns>
        public ActionResult Area()
        {

            query.Data = bs.GetBindingServiceList(CurrentUser.User.UseCompanyId).Where(t => t.BindingType == BindingServiceType.Area).ToList();
            query.JoinCompanyDic = QueryJoinCompany();
            //当前所拥有的区域            
            query.AreaList = new DeviceService().GetDevice(CurrentUser.User.UseCompanyId).Where(t => t.AreaId >= 0).Select(t=>t.Area).ToList().Where(m => m != null).ToLookup(t => t.AreaId).ToDictionary(t => t.Key, t => t.First());

            return View(query);
        }

        /// <summary>
        /// 厂商列表
        /// </summary>
        /// <returns></returns>
        public ActionResult Manufacturer()
        {
            query.Data = bs.GetBindingServiceList(CurrentUser.User.UseCompanyId).Where(t => t.BindingType == BindingServiceType.Manufacturer).ToList();
            query.JoinCompanyDic = QueryJoinCompany();
            var deviceList = new DeviceService().GetDevice(CurrentUser.User.UseCompanyId);
          
            var BrandList = deviceList.Where(t => t.BrandId >= 0).Select(t => t.Brand).Where(m => m != null).ToList();
            query.BrandList = deviceList.Where(t => t.BrandId >= 0).Select(t => t.Brand).Where(m => m != null).ToList().ToLookup(t => t.BrandId).ToDictionary(t => t.Key, t => t.First());
            var ManufacturerDic = BrandList.Select(m=>m.ManufacturerId).ToArray();
            query.ManufacturerList =new ManufacturersSvr().GetManufatuer().Where(t=> ManufacturerDic.Contains(t.ManufacturerId)).ToList().ToLookup(t => t.ManufacturerId).ToDictionary(t => t.Key, t => t.First()); 
            return View(query);
        }

        /// <summary>
        /// 绑定类目
        /// Author:Gavin 
        /// Create Data:2017-04-17
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BindingCategory(List<long> categoryArr, BindingServiceType bindingType, string serviceCompanyId)
        {
            try
            {
                bs.Binding(categoryArr, bindingType, CurrentUser.User.UseCompanyId, serviceCompanyId);
                return ResultSuccess("操作成功");
            }
            catch (Exception ex)
            {
                Logger.Error("绑定类目失败" + ex);
                return ResultError("操作失败");
            };
        }

        /// <summary>
        /// 获取合作单位
        /// </summary>
        public List<ServiceCompany> QueryJoinCompany()
        {
            using (var db = new MbContext())
            {
                var join = db.JoinCompany
                    .Where(t => t.UseCompanyId == CurrentUser.UseCompany.UseCompanyId)
                    .Where(t => t.State == CompanySginState.Passed.ToString())
                    .ToList();

                var jIds = join.Select(t => t.ServiceCompanyId).ToList();

                return db.ServiceCompany
                      .Where(t => jIds.Contains(t.ServiceCompanyId)).ToList();
            }
        }


        /// <summary>
        /// 绑定设备
        /// Author:Gavin 
        /// Create Data:2017-04-17
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BindingDevice(List<long> deviceArr, BindingServiceType bindingType, string[] serviceCompanyArr)
        {
            try
            {
                bs.BindingDevice(deviceArr, bindingType, CurrentUser.User.UseCompanyId, serviceCompanyArr);
                return ResultSuccess("操作成功");
            }
            catch (Exception ex)
            {
                Logger.Error("绑定设备失败" + ex);
                return ResultError("操作失败");
            };
        }

        /// <summary>
        /// 区域绑定
        /// </summary>
        /// <param name="areaArr">区域编号</param>
        /// <param name="bindingType">绑定类型</param>
        /// <param name="serviceCompanyId">服务公司编号</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BindingArea(List<long> areaArr, BindingServiceType bindingType, string serviceCompanyId) {
            try
            {
                bs.Binding(areaArr, bindingType, CurrentUser.User.UseCompanyId, serviceCompanyId);
                return ResultSuccess("操作成功");
            }
            catch (Exception ex)
            {
                Logger.Error("区域绑定" + ex);
                return ResultError("操作失败");
            };
        }

        [HttpPost]
        public ActionResult BindingManufacturer(List<long> manufacturerArr, BindingServiceType bindingType, string serviceCompanyId) {
            try
            {
                bs.Binding(manufacturerArr, bindingType, CurrentUser.User.UseCompanyId, serviceCompanyId);
                return ResultSuccess("操作成功");
            }
            catch (Exception ex)
            {
                Logger.Error("厂家绑定失败" + ex);
                return ResultError("操作失败");
            };
        }
    }
}
