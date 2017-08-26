using LZY.BX.Model;
using LZY.BX.Service.Mb;
using LZY.BX.Utilities;
using Repair.Web.Site.Areas.Sp.Models;
using Repair.Web.Site.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using LZY.BX.Service;

namespace Repair.Web.Site.Areas.Sp.Controllers
{
    public class DeviceController : ControllerBase<DeviceController>
    {
        //
        // GET: /Sp/Device/

        public ActionResult Device()
        {
            return View();
        }

        public ActionResult DeviceList(DeviceQueryModel query, string keywords)
        {
            var eId = 0L;
            if (long.TryParse(keywords, out eId))
            {
                query.DeviceIdLike = eId;
            }
            else
            {
                query.NameLike = keywords;
            }

            using (var db = new MbContext())
            {
                //DOTO 获取当前单位下拥有的设备
                if (CurrentUser.User.RoleKey .HasFlag(LZY.BX.Model.Enum.UserType.SvcCompanyUserAdmin))
                {
                    IQueryable<Device> equipmentModel;
                    if (query.DeviceIdLike != null|| query.NameLike!=null) {
                         equipmentModel = db.Device
                        .Where(query);
                    }
                    else {
                        var deviceArr = db.DeviceContract.Where(x => x.ServiceCompanyId == CurrentUser.User.ServiceCompanyId).Select(t => t.DeviceId).ToArray();
                        equipmentModel = db.Device.Where(x => deviceArr.Contains(x.DeviceId));
                    }
                    //设备列表主数据
                    query.Data = equipmentModel
                        .OrderByDescending(x => x.CreateTime)
                        .Skip(query.PageInfo.RecIndex)
                        .Take(query.PageInfo.PageSize)
                        .ToList();
                    //品牌字典
                    var brandsIds = query.Data.Select(x => x.BrandId).ToList();

                    query.BrandDic = db.Brand
                        .Where(x => brandsIds.Contains(x.BrandId))
                        .ToLookup(x => x.BrandId)
                        .ToDictionary(x => x.Key, x => x.First());

                    ////设备信息
                    var equipmentIds = query.Data.Select(x => x.CategoryId).ToList();

                    query.CategoryDic = db.Category
                          .Where(t => equipmentIds.Contains(t.CategoryId))
                          .ToLookup(x => x.CategoryId)
                          .ToDictionary(x => x.Key, x => x.First());
                    //设备数量
                    query.PageInfo.TotalCount = equipmentModel.Count();
                }
            }
            return View(query);
        }
        /// <summary>
        /// 验证数据合法性
        /// </summary>
        /// <param name="td"></param>
        /// <returns></returns>
        private bool ValidateExcelModel(DataTable dt)
        {
            //DOTO 厂家	品牌	设备名称	设备型号	安装地址	区域名称	区域代码	资产代码	类目	采购日期	保修日期
            foreach (DataRow row in dt.Rows)
            {
                var name = row["设备名称"].ToString();
                var category = row["类目"].ToString();
                var brand = row["品牌"].ToString();
                var model = row["设备型号"].ToString();
                var qrcode = row["二维码"].ToString();
                var usecompanyname = row["客户名称"].ToString();
                var postion = row["安装地址"].ToString();
                var areaName = row["区域名称"].ToString();
                var manufacturers = row["厂家"].ToString();
                var buyTimestr = row["采购日期"].ToString();
                var warrantyTimestr = row["保修日期至"].ToString();

                if (string.IsNullOrEmpty(name.Trim()) ||
                   string.IsNullOrEmpty(category.Trim()))
                {
                    return false;
                }
            }

            return true;
        }
        /// 表格导入
        /// </summary>
        /// <returns></returns>
        public ActionResult ImportExcel()
        {
            var excelFile = Request.Files["excelFile"];

            var oStream = excelFile.InputStream;

            //DOTO 转换DataTable
            var dt = ExcelHelper.ExcelToTable(oStream);

            if (!ValidateExcelModel(dt))
            {
                ///DOTO Excel验证失败
                throw new Exception("文件内容不符合模板要求！");
            }

            using (var db = new MbContext())
            {
                foreach (DataRow row in dt.Rows)
                {
                    var name = row["设备名称"].ToString();
                    var category = row["类目"].ToString();
                    var brand = row["品牌"].ToString();
                    var model = row["设备型号"].ToString();
                    var qrcode = row["二维码"].ToString();
                    var usecompanyname = row["客户名称"].ToString();
                    var postion = row["安装地址"].ToString();
                    var areaName = row["区域名称"].ToString();
                    var manufacturers = row["厂家"].ToString();
                    var buyTimestr = row["采购日期"].ToString();
                    var warrantyTimestr = row["保修日期至"].ToString();
                    var Spec = row["规格"].ToString();
                    var LotNo = row["批次号"].ToString();
                    var buyTime = DateTime.Now;
                    if (!DateTime.TryParse(buyTimestr, out buyTime))
                        DateTime.TryParseExact(buyTimestr, "dd-M月-yyyy", null, System.Globalization.DateTimeStyles.NoCurrentDateDefault, out buyTime);
                    
                    var warrantyTime = DateTime.Now;
                    if (!DateTime.TryParse(warrantyTimestr, out warrantyTime))
                        DateTime.TryParseExact(warrantyTimestr, "dd-M月-yyyy", null, System.Globalization.DateTimeStyles.NoCurrentDateDefault, out warrantyTime);
                    //DOTO 厂家
                    var m = new Manufacturer();
                    if (!string.IsNullOrEmpty(manufacturers))
                    {
                        m = db.Manufacturer.FirstOrDefault(t => t.Name == manufacturers);
                        if (m == null)
                        {
                            m = db.Manufacturer.Add(new Manufacturer
                            {
                                Name = manufacturers,
                                CreateTime = DateTime.Now
                            });
                        }
                    }

                    //DOTO 品牌
                    var b = new Brand();
                    if (!string.IsNullOrEmpty(brand))
                    {
                        b = db.Brand.FirstOrDefault(t => t.Name == brand);
                        if (b == null)
                        {
                            b = db.Brand.Add(new Brand
                            {
                                ManufacturerId = m.ManufacturerId,
                                Name = brand,
                                CreateTime = DateTime.Now
                            });
                        }
                    }

                    //DOTO 区域
                    var a = new Area();
                    if ( !string.IsNullOrEmpty(areaName))
                    {
                        a = db.Area.FirstOrDefault(t => t.Name == areaName);
                        if (a == null)
                        {
                            a = db.Area.Add(new Area
                            {
                                LevelDeep = 0,
                                Name = areaName,
                                Code = "0",
                                CreateTime = DateTime.Now
                            });
                        }
                    }

                    //DOTO 类目
                    var c = new Category();
                    if (!string.IsNullOrEmpty(category))
                    {
                        c = db.Category.FirstOrDefault(t => t.Name == category);
                        if (c == null)
                        {
                            c = db.Category.Add(new Category
                            {
                                LevelDeep = 0,
                                Name = category,
                                SortField = 999,
                                CreateTime = DateTime.Now
                            });
                        }
                    }
                    var dInfo = new Device()
                    {
                        CategoryId = c.CategoryId,
                        AreaId = a.AreaId,
                        BrandId = b.BrandId,
                        Name = name,
                        Model = model,
                        Position = postion,
                        BuyTime = buyTime.Year==1?null:(DateTime?)buyTime,
                        WarrantyTime = warrantyTime,
                        LastUpdateTime = DateTime.Now,
                        UseCompanyName=usecompanyname,
                        CreateTime = DateTime.Now
                    };
                    db.Device.Add(dInfo);

                    db.DeviceContract.Add(new DeviceContract()
                    {
                        DeviceId = dInfo.DeviceId,
                        ServiceCompanyId = CurrentUser.User.ServiceCompanyId,
                        Sort = 0
                    });
                    db.SaveChanges();
                }
            }

            return ResultSuccess("导入成功！");
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Remove(long id)
        {
            using (var db = new MbContext())
            {
                var dev = db.Device.FirstOrDefault(t => t.DeviceId == id);
                var devcon = db.DeviceContract.FirstOrDefault(t => t.DeviceId == id);
                if (devcon != null) {
                    db.DeviceContract.Remove(devcon);
                }
                db.Device.Remove(dev);

                return db.SaveChanges() > 0 ? ResultSuccess("删除成功！") : ResultError("删除异常！");
            }
        }

        public ActionResult AddOrEditDevice(long? id) {
            if (null == id)
            {
                return View(new Device());
            }

            using (var db = new MbContext())
            {
                var model = db.Device
                            .Include(t => t.UseCompany)
                            .Include(t => t.Brand)
                            .Include(t => t.Brand.Manufacturer)
                            .Include(t => t.Area)
                            .Include(t => t.Category)
                            .FirstOrDefault(t => t.DeviceId == id);
              
                return View(model);
            }
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult AddOrEditDevice(Device dev) {
            using (var db = new MbContext())
            {
                DeviceContract infp = new DeviceContract();
                if (dev.DeviceId == 0)
                {
                    infp = new DeviceContract
                    {
                        DeviceId = dev.DeviceId,
                        ServiceCompanyId = CurrentUser.User.ServiceCompanyId,
                        Sort = 0
                    };
                }
                db.Device.AddOrUpdate(dev);
                if(infp!=null)
                    db.DeviceContract.AddOrUpdate(infp);
                return db.SaveChanges() > 0 ? ResultSuccess("操作成功！") : ResultError("操作异常！");
            }
        }

        /// 品牌控件
        /// </summary>
        /// <returns></returns>
        public ActionResult select_brand_control(string brandId)
        {
            using (var db = new MbContext())
            {
                var model = db.Manufacturer
                            .Include(t => t.Brands)
                            .OrderBy(t => t.Name)
                            .ToList();

                ViewBag.brandId = brandId;

                return View(model);
            }
        }

        /// <summary>
        /// 区域控件
        /// </summary>
        /// <returns></returns>
        public ActionResult select_area_control(string areaId)
        {
            using (var db = new MbContext())
            {
                var model = db.Area
                            .OrderBy(t => t.Name)
                            .ToList();

                ViewBag.areaId = areaId;

                return View(model);
            }
        }

        /// <summary>
        /// 类目控件
        /// </summary>
        /// <returns></returns>
        public ActionResult select_category_control(string categoryId)
        {
            using (var db = new MbContext())
            {
                var model = db.Category
                            .OrderBy(t => t.Name)
                            .ToList();

                ViewBag.categoryId = categoryId;

                return View(model);
            }
        }

        /// <summary>
        /// 业主公司控件
        /// </summary>
        /// <returns></returns>
        public ActionResult select_usecompany_control(string UserCompanyName)
        {
                //获取所有添加过的公司名称
                List<string> nameList = new DeviceService().GetCompanyName(CurrentUser.User.ServiceCompanyId);
            ViewBag.UserCompanyName = UserCompanyName;
            return View(nameList);
          
        }
    }

}
