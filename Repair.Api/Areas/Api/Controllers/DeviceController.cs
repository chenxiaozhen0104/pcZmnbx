using LZY.BX.Model.Enum;
using LZY.BX.Service;
using System;
using System.Web.Mvc;
using System.Linq;
using Repair.Api.Areas.Api.Utilities;
using LZY.BX.Model;
using System.Collections.Generic;
using LZY.BX.Service.Mb;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using Repair.Api.Areas.Utilities;
using System.Configuration;
using System.Globalization;
using WeiXin;

namespace Repair.Api.Areas.Api.Controllers
{
    public class DeviceController : ControllerApiBase
    {

        DeviceService ds = new DeviceService();
        /// <summary>
        /// 设置二维码
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="qrCode"></param>
        /// <returns></returns>
        public ActionResult SetQRCode(long deviceId, string qrCode, string address)
        {
            if (string.IsNullOrEmpty(qrCode))
            {
                return Json(new
                {
                    error = "请输入二维码信息"
                });
            }
            try
            {
                ds.SetQRCode(deviceId, qrCode, address);
                return Json(new
                {
                    status = 0
                });
            }
            catch (QRCodeExistException)
            {
                return Json(new
                {
                    error = "该二维码已绑定其他设备，请更换其他二维码"
                });
            }
            catch (Exception e)
            {
                Logger.Error("设置二维码错误", e);
                return Json(new
                {
                    error = "服务器繁忙，请稍后再试试"
                });
            }
        }
        /// <summary>
        /// 设置二维码以及默认设备服务商
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="qrCode"></param>
        /// <returns></returns>
        public ActionResult SetQRCodeWithServerCompany(Device device)
        {

            if (string.IsNullOrEmpty(device.QRCode))
            {
                return Json(new
                {
                    error = "请输入二维码信息"
                });
            }
            try
            {
                ds.SetQRCode(device.DeviceId, device.QRCode, device.Position, device.ServiceCompanyId, ApiUser.Current.UseCompanyId);
                return Json(new
                {
                    status = 0
                });
            }
            catch (QRCodeExistException)
            {
                return Json(new
                {
                    error = "该二维码已绑定其他设备，请更换其他二维码"
                });
            }
            catch (Exception e)
            {
                Logger.Error("设置二维码错误", e);
                return Json(new
                {
                    error = "服务器繁忙，请稍后再试试"
                });
            }
        }

        /// <summary>
        /// 搜索设备列表
        /// </summary>
        /// <param name="searchKey"></param>
        /// <param name="areaId"></param>
        /// <param name="brandId"></param>
        /// <param name="categoryId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult Search(string searchKey,
            long? areaId,
            long? brandId,
            long? categoryId,
            int pageIndex = 1,
            int pageSize = 20)
        {
            try
            {
                var page = ds.List(ApiUser.Current.UseCompanyId ?? "", searchKey, areaId, categoryId, brandId, pageIndex, pageSize);
                return Json(new
                {
                    data = page.Data.Select(m => new
                    {
                        id = m.DeviceId,
                        name = m.Name,
                        brand = m.Brand?.Name,
                        category =m.Category?.Name,
                        model = m.Model,
                        area =m.Area?.Name,
                        address = m.Position,
                        qrcode = m.QRCode,
                        assetsid=m.AssetsId
                    }),
                    count = page.Count,
                    index = page.Index,
                    size = page.Size
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Logger.Error("查询设备信息错误", e);
                return Json(new { error = "服务器繁忙，请稍后再试试" }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// 获取设备明细
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public ActionResult Get(long deviceId)
        {
            var device = ds.Get(deviceId);

            if (device != null)
            {
                device.imgList = new PictureService().GetImgLists(deviceId.ToString(), PhotoType.Device);
                if (device.Brand != null&& device.Brand.BrandId > 0) { 
                     device.ManufacturerName = ds.GetManufacturer(device.Brand.BrandId).Manufacturer.Name;
                }
                ////签约公司
                //device.JoinCompanyList = new CompanyService().GetJoinCompanys((long)ApiUser.Current.UseCompanyId).Select(t => new ServiceCompany { Name = t.Name, ServiceCompanyId = t.ServiceCompanyId.ToString() }).ToList();
                //设备签约公司
                var DeviceContractList = ds.GetDeviceContactList(deviceId);
                return Json(new
                {
                    device.DeviceId,
                    device.Name,
                    device.QRCode,
                    device.Position,
                    device.Model,
                    device.Spec,
                    device.LotNo,
                    device.ManufacturerName,
                    brand = device.Brand?.Name,
                    area = device.Area?.Name,
                    category = device.Category?.Name,
                    device.UseCompanyName,
                    device.imgList,
                    device.Note,
                    BuyTime = (device.BuyTime != null && device.BuyTime.Value.Year != 1) ? device.BuyTime.Value.ToString("yyyy-MM-dd") : "",
                    WarrantyTime = (device.WarrantyTime != null && device.WarrantyTime.Value.Year != 1) ? device.WarrantyTime.Value.ToString("yyyy-MM-dd") : "",
                    DeviceContractList= DeviceContractList.ToArray(),
                    device.Category

                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { error = "设备不存在" }, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// 获取设备明细以及签约公司信息
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public ActionResult GetDeviceInfo(long deviceId)
        {
            var device = ds.Get(deviceId);
            //2017-05-22 增加设备图片列表      
            
            device.imgList = new PictureService().GetImgLists(deviceId.ToString(), PhotoType.Device);
            var canRpearid = true;
            //签约公司
            
            var JoinCompanyList = new CompanyService().GetJoinCompanys(ApiUser.Current.UseCompanyId).Select(t => new { Name = t.Name, ServiceCompanyId = t.ServiceCompanyId }).ToList();
            //设备签约公司
            
            var DeviceContractList = ds.GetDeviceContactList(deviceId).Select(t => new { ServiceCompanyId = t.ServiceCompanyId, Name = t.ServiceCompany.Name });
            ////判断数据库该设备的最新一单有没有完成          
            var deviceOrderState = new OrderService().GetOrderState(device.DeviceId);
            if (deviceOrderState != null && !deviceOrderState.State.HasFlag(OrderState.Cancel) && !deviceOrderState.State.HasFlag(OrderState.Close) && !deviceOrderState.State.HasFlag(OrderState.Confirm) && !deviceOrderState.State.HasFlag(OrderState.UseComment))
            {
                canRpearid = false;
            }
            return Json(new
            {
                device = device,
                JoinCompanyList = JoinCompanyList,
                DeviceContractList = DeviceContractList,
                canRpearid = canRpearid
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Exist(string qrCode)
        {
            var device = ds.GetByQrCode(qrCode, ApiUser.Current.UseCompanyId);
            if (device == null)
            {
                return Json(new { error = "该二维码不存在" }, JsonRequestBehavior.AllowGet);
            }
            else if (ApiUser.Current.UseCompanyId != device.UseCompanyId)
            {
                return Json(new { error = "您公司没有该设备,不可以报修" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(device != null, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="qrCode"></param>
        /// <returns></returns>
        public ActionResult GetByQRCode(string qrCode)
        {
            var device = ds.GetByQrCode(qrCode);
            //2017-05-22 增加设备图片列表      
       
            device.imgList = new PictureService().GetImgLists(device.DeviceId.ToString(), PhotoType.Device);
            return Json(device, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取设备明细以及签约公司信息
        /// </summary>
        /// <param name="qrCode"></param>
        /// <returns></returns>
        public ActionResult GetDeviceInfoByQRCode(string qrCode)
        {
            var device = ds.GetByQrCode(qrCode);
            //2017-05-22 增加设备图片列表      
            device.imgList = new PictureService().GetImgLists(device.DeviceId.ToString(), PhotoType.Device);
            if (device.UseCompanyId != null && ApiUser.Current.UseCompanyId == device.UseCompanyId)
            {
                //签约公司
                var JoinCompanyList = new CompanyService().GetJoinCompanys(ApiUser.Current.UseCompanyId).Select(t => new { Name = t.Name, ServiceCompanyId = t.ServiceCompanyId }).ToList();
                //设备签约公司
                var DeviceContractList = ds.GetDeviceContactList(device.DeviceId).Select(t => new { ServiceCompanyId = t.ServiceCompanyId, Name = t.ServiceCompany.Name }).ToList();
                var canRpearid = true;
                ////判断数据库该设备的最新一单有没有完成
                var deviceOrderState = new OrderService().GetOrderState(device.DeviceId);
                if (deviceOrderState != null && !deviceOrderState.State.HasFlag(OrderState.Cancel) && !deviceOrderState.State.HasFlag(OrderState.Close) && !deviceOrderState.State.HasFlag(OrderState.Confirm) && !deviceOrderState.State.HasFlag(OrderState.UseComment))
                {
                    canRpearid = false;
                }
                //2017-05-22 增加设备图片列表           
                var deviceImgList = new PictureService().GetImgLists(device.DeviceId.ToString(), PhotoType.Device);
                return Json(new { device = device, JoinCompanyList = JoinCompanyList, DeviceContractList = DeviceContractList, canRpearid = canRpearid, deviceImgList = deviceImgList }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { error = "您公司没有该设备,不可以报修" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult WxGetDeviceInfoByQRCode(string qrCode)
        {
            var device = ds.GetByQrCode(qrCode);
            var canRpearid = true;
            var authInfo = AuthAccountServer.Instance.Get((Session["WxUser"] as WeiXinUser).openid);
            if (device != null)
            {
                //2017-05-22 增加设备图片列表      
                device.imgList = new PictureService().GetImgLists(device.DeviceId.ToString(), PhotoType.Device);
                //签约公司
                var JoinCompanyList = new CompanyService().GetJoinCompanys(device.UseCompanyId).Select(t => new { Name = t.Name, ServiceCompanyId = t.ServiceCompanyId.ToString() }).ToList();
                //设备签约公司
                var DeviceContract = ds.WxGetDeviceContact(device.DeviceId);
                var ServiceCompany = CompanyService.Instance.GetServiceCompany(DeviceContract.ServiceCompanyId);



                ////判断数据库该设备的最新一单有没有完成
                var deviceOrderState = new OrderService().GetOrderState(device.DeviceId);
                if (deviceOrderState != null && !deviceOrderState.State.HasFlag(OrderState.Cancel) && !deviceOrderState.State.HasFlag(OrderState.Close) && !deviceOrderState.State.HasFlag(OrderState.Confirm) && !deviceOrderState.State.HasFlag(OrderState.UseComment))
                {
                    canRpearid = false;
                }

                return Json(new
                {
                    device = device,
                    JoinCompanyList = JoinCompanyList,
                    DeviceContractList = new
                    {
                        ServiceCompanyId = ServiceCompany.ServiceCompanyId.ToString(),
                        ImgUrl = new PictureService().imgPath + ServiceCompany.ImgUrl,
                        ServiceCompany.Note,
                        ServiceCompany.Name
                    },
                    canRpearid = canRpearid,
                    userName = authInfo.User.NickName,
                    userPhone = authInfo.User.Phone,
                    companyName=authInfo.User.CompanyName
                }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(new { device = "", JoinCompanyList = "", DeviceContractList = "", canRpearid = canRpearid, userName = authInfo.NickName, userPhone = authInfo.User.Phone }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// 微信手工保修获取服务公司信息
        /// </summary>
        public ActionResult WxHandWorkBaseInfo()

        {
            //服务公司列表
            var list = CompanyService.Instance.GettAllServiceInfo()
                                     .Select(t => new { ServiceCompanyId = t.ServiceCompanyId, t.Name }).ToArray();
            var authInfo = AuthAccountServer.Instance.Get((Session["WxUser"] as WeiXinUser).openid);

            return Json(new { serviceCompanyList = list, userName = authInfo.User.RealName, userPhone = authInfo.User.Phone, companyName = authInfo.User.CompanyName, canRpearid = true, }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 搜索列表筛选条件初始化数据
        /// </summary>
        /// <returns></returns>
        public ActionResult InitData(AppType? type)
        {
            string companyId ="";
            if (type == AppType.Service)
                companyId = ApiUser.Current.ServiceCompanyId ?? "";
            else
                companyId = ApiUser.Current.UseCompanyId ??"";
            var category = ds.Categorys(companyId, type);
            var area = ds.Areas(companyId, type);
            var brand = ds.Brands(companyId, type);
            return Json(new
            {
                brand = (brand.Count != 0 ? brand.Where(m => m != null).Select(m => new { id = m.BrandId, name = m.Name, right = 0, left = 0, depth = 0 }) : null),
                category = (category.Count != 0 ? category.Where(m => m != null).Select(m => new { id = m.CategoryId, name = m.Name, right = m.Right, left = m.Left, depth = m.LevelDeep }) : null),
                area = (area.Count != 0 ? area.Where(m => m != null).Select(m => new { name = m.Name, right = m.Right, left = m.Left, depth = m.LevelDeep, id = m.AreaId }) : null)
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取维修记录
        /// </summary>
        /// <param name="qrCodeOrDeviceId">二维码或者设备编号</param>
        /// <returns></returns>
        public ActionResult GetRepairRecord(string qrCodeOrDeviceId)
        {

            long deviceId = 0;
            long.TryParse(qrCodeOrDeviceId, out deviceId);
            if (deviceId == 0)
                deviceId = ds.GetByQrCode(qrCodeOrDeviceId).DeviceId;

            var orderRecord = new OrderService().GetRecordByDeviceId(deviceId);

            return Json(orderRecord.Select(t =>
                    new { MainOrderId = t.MainOrderId, t.User.RealName, t.User.NickName, t.User.Phone, UserId = t.ServiceUserId, ServiceCompanyName = t.ServiceCompany.Name, CreateTime = t.CreateTime.ToString("yyyy-MM-dd HH:mm:ss") }).ToList()
                , JsonRequestBehavior.AllowGet);
        }

        #region 添加设备接口
        /// <summary>
        /// 获取厂家,区域,类目，当前登陆人使用公司信息，以及签约单位列表列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetDeviceSelectInfo()
        {
            try
            {
                using (var db = new MbContext())
                {
                    //厂家 
                    List<Brand> brandArr = db.Brand.Include(t => t.Manufacturer).OrderBy(t => t.Name).ToList();
                    //和品牌
                    List<Manufacturer> manufacturerList = db.Manufacturer
                                .OrderBy(t => t.Name)
                                .ToList();
                    //
                    //区域
                    //List<Area> AreaList = db.Area
                    //       .OrderBy(t => t.Name)
                    //       .ToList();

                    List<Area> areaList = new AreaService().GetAreaList(new OrderService().GetAreaList(ApiUser.Current));
                    //类目
                    List<Category> listCategory = db.Category
                          .OrderBy(t => t.Name)
                          .ToList();
                    //所属公司信息
                    List<UseCompany> listUseCompany = db.UseCompany.Where(t => t.UseCompanyId == ApiUser.Current.UseCompanyId)
                           .OrderBy(t => t.Name)
                           .ToList();
                    //返回签约单位信息
                    var JoinCompany = db.JoinCompany
                                       .Where(t => t.UseCompanyId == ApiUser.Current.UseCompanyId && t.State == "Passed")
                                       .Select(x => x.ServiceCompanyId)
                                       .ToList();

                    List<ServiceCompany> serCompany = db.ServiceCompany
                                                      .Where(t => JoinCompany.Contains(t.ServiceCompanyId))
                                                      .OrderBy(t => t.ServiceCompanyId)
                                                      .ToList();
                    List<string> companyName = ds.GetCompanyName(ApiUser.Current.ServiceCompanyId);
                    return Json(new
                    {
                        Manufacturer = manufacturerList.Select(x => new { x.Name, x.ManufacturerId }),
                        BrandArr= brandArr.Select(t=>new { t.Name,t.BrandId,t.ManufacturerId}),
                        Area = areaList.Select(x => new { x.Name, x.AreaId }),
                        Category = listCategory.Select(x => new { x.CategoryId, x.Name,x.LevelDeep,x.Left,x.Right }),
                        UseCompany = listUseCompany.Select(x => new { x.Name, UseCompanyId = x.UseCompanyId }),
                        ServiceCompany = serCompany.Select(x => new { ServiceCompanyId = x.ServiceCompanyId, x.Name, Checked = false }),
                        CompanyName = companyName.ToArray()
                    }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                Logger.Error("GetBrandWithManufacturer(获取品牌)", ex);
                return Json(new { error = "获取品牌" });

            }
        }

        //根据厂家获取品牌
        public ActionResult GetBrandWithManufacturer(long? ManufacturerId)
        {
            try
            {
                using (var mb = new MbContext())
                {
                    List<Brand> list = new List<Brand>();
                    if (ManufacturerId == 0)
                        list = mb.Brand.ToList();
                    else
                        list = mb.Brand.Where(t => t.ManufacturerId == ManufacturerId).ToList();

                    return Json(new { Brand = list.Select(x => new { x.BrandId, x.Name }) }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("GetDeviceSelectInfo(获取列表失败)", ex);
                return Json(new { error = "获取列表失败" });


            }
        }


        /// <summary>
        /// 添加保存设备信息
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
       
        public ActionResult SaveDeviceInfo(Device device)
        {
            try
            {
                using (var db = new MbContext())
                {


                    #region 判断厂家
                    var m = new Manufacturer();
                    if (!string.IsNullOrEmpty(device.ManufacturerName))
                    {
                        m = db.Manufacturer.FirstOrDefault(t => t.Name == device.ManufacturerName);
                        if (m == null)
                        {
                            m = db.Manufacturer.Add(new Manufacturer
                            {
                                Name = device.ManufacturerName,
                                CreateTime = DateTime.Now
                            });
                        }
                    }
                    #endregion
                    #region 判断品牌
                    var b = new Brand();
                    if (!string.IsNullOrEmpty(device.BrandName))
                    {
                        b = db.Brand.FirstOrDefault(t => t.Name == device.BrandName);
                        if (b == null)
                        {
                            b = db.Brand.Add(new Brand
                            {
                                ManufacturerId = m.ManufacturerId,
                                Name = device.BrandName,
                                CreateTime = DateTime.Now
                            });
                        }
                        device.BrandId = b.BrandId;
                    }
                    #endregion
                    #region 判断区域
                    var a = new Area();
                    if (!string.IsNullOrEmpty(device.AreaName))
                    {
                        a = db.Area.FirstOrDefault(t => t.Name == device.AreaName);
                        if (a == null)
                        {
                            a = db.Area.Add(new Area
                            {
                                LevelDeep = 0,
                                Name = device.AreaName,
                                Code = "0",
                                CreateTime = DateTime.Now
                            });
                        }
                        device.AreaId = a.AreaId;
                    }
                    #endregion
                    #region 类目                   
                    var c = new Category();
                    if (!string.IsNullOrEmpty(device.CategoryName))
                    {
                        c = db.Category.FirstOrDefault(t => t.Name == device.CategoryName);
                        if (c == null)
                        {
                            c = db.Category.Add(new Category
                            {
                                LevelDeep = 0,
                                Name = device.CategoryName,
                                SortField = 999,
                                CreateTime = DateTime.Now
                            });
                        }
                        device.CategoryId = c.CategoryId;
                    }
                    #endregion
                    device.LastUpdateTime = DateTime.Now;

                    device.CreateTime = DateTime.Now;
                    if (device.QRCode != null && device.QRCode != "")
                    {
                        //判断二维码是否已经存在
                        var dInfo = db.Device.Where(t => t.QRCode == device.QRCode).FirstOrDefault();
                        if (dInfo != null && device.DeviceId != dInfo.DeviceId)
                        {
                            return Json(new { success = "该二维码已经存在", deviceId = device.DeviceId });
                        }
                    }

                    db.Device.AddOrUpdate(device);

                  
                    int result = db.SaveChanges();
                    if (device.pictureId != null && device.pictureId.Length > 0)
                        new PictureService().EditPicture(device.pictureId, device.DeviceId.ToString());


                    if (device.ServiceCompanyId != null && device.ServiceCompanyId.Length > 0)
                    {
                        //往ServiceBinding里面插数据

                        for (int i = 0; i < device.ServiceCompanyId.Length; i++)
                        {
                            var category = new ServiceBinding
                            {
                                BindingType = BindingServiceType.Equipment,
                                Content = device.DeviceId.ToString(),
                                OutNote = string.Empty,
                                ServiceCompanyId = device.ServiceCompanyId[i],
                                UseCompanyId = (device.UseCompanyId ??""),
                                CreateTime = DateTime.Now
                            };
                            db.ServiceBinding.AddOrUpdate(category);
                            var deviceContract = new DeviceContract
                            {
                                DeviceId = device.DeviceId,
                                Sort = i,
                                ServiceCompanyId = device.ServiceCompanyId[i]
                            };
                            db.DeviceContract.AddOrUpdate(deviceContract);
                        }
                        result = db.SaveChanges();

                    }
                    if (result > 0)
                        return Json(new { success = "操作成功", deviceId = device.DeviceId }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { error = "操作失败，请稍后重试" }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                Logger.Error("添加设备异常", ex);
                return Json(new { error = "添加设备异常" }, JsonRequestBehavior.AllowGet);
            }

        }


        #endregion
        /// <summary>
        /// 服务商获取设备列表
        /// </summary>
        /// <param name="searchKey"></param>
        /// <param name="areaId"></param>
        /// <param name="brandId"></param>
        /// <param name="categoryId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult ServerCompanyDevices(string searchKey,
            long? areaId,
            long? brandId,
            long? categoryId,
            int pageIndex = 1,
            int pageSize = 20)
        {
            try
            {
               
                var page = ds.ServerCompanyDevicesList(ApiUser.Current.ServiceCompanyId ?? "", searchKey, areaId, categoryId, brandId, pageIndex, pageSize);
                return Json(new
                {
                    data = page.Data.Select(m => new
                    {
                        id = m.DeviceId,
                        name = m.Name,
                        brand = m.Brand?.Name,
                        category = m.Category?.Name,
                        model = m.Model,
                        area =  m.Area?.Name,
                        address = m.Position,
                        qrcode = m.QRCode,
                        serverName =m.UseCompany?.Name,
                        lotNo = m.LotNo,
                        spec = m.Spec,
                        useCompanyName = m.UseCompanyName
                    }),
                    count = page.Count,
                    index = page.Index,
                    size = page.Size
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Logger.Error("查询设备信息错误", e);
                return Json(new { error = "服务器繁忙，请稍后再试试" }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// 编辑设备信息的字段
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ActionResult EditField(long id, string fieldName, string value)
        {
            Device device = DeviceService.Instance.Get(id);

            var prop = typeof(Device).GetProperty(fieldName);
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
                    prop.SetValue(device, null);
                }
                else
                {
                    prop.SetValue(device, Convert.ChangeType(value, type), null);
                }
            }
           
            else
            {
                var val = Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
                prop.SetValue(device, val);
            }
            DeviceService.Instance.Edit(device);
            return Json(new { status = 0 });
        }

       
    }
}
