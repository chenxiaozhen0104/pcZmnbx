using Repair.Web.Mng.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Repair.Web.Mng.Menu;
using LZY.BX.Service.Mb;
using System.Data.Entity;
using Repair.Web.Mng.Models;
using LZY.BX.Utilities;
using System.Data;
using LZY.BX.Model;
using System.IO;
using LZY.BX.Service;

namespace Repair.Web.Mng.Controllers
{
    /// <summary>
    /// 设备 控制器
    /// </summary>
    public class DeviceController : ControllerBase<DeviceController>
    {
        //
        // GET: /Device/
        [MenuCurrent("设备列表", PAction = "Index", PArea = "SysAdmin", PController = "CommData")]
        public ActionResult Index()
        {
            var cs = new CompanyService();

            var companys = cs.GetAllCompanyInfo();

            return View(companys);
        }

        public ActionResult DeviceList(DeviceQueryModel query)
        {
            using (var db = new MbContext())
            {
                var model = db.Device.Where(query).OrderBy(t => t.CreateTime);

                query.Data = model
                            .Include(t => t.UseCompany)
                            .Include(t => t.Brand)
                            .Include(t => t.Brand.Manufacturer)
                            .Include(t => t.Area)
                            .Include(t => t.Category)
                            .Skip(query.PageInfo.RecIndex)
                            .Take(query.PageInfo.PageSize)
                            .ToList();

                query.PageInfo.TotalCount = model.Count();

                return View(query);
            }
        }

        /// <summary>
        /// 编辑视图
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return View(new Device());
            }

            using (var db = new MbContext())
            {
                var devId = Convert.ToInt64(id);

                var model = db.Device
                            .Include(t => t.UseCompany)
                            .Include(t => t.Brand)
                            .Include(t => t.Brand.Manufacturer)
                            .Include(t => t.Area)
                            .Include(t => t.Category)
                            .FirstOrDefault(t => t.DeviceId == devId);

                return View(model);
            }
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddOrEdit(Device dev)
        {
            using (var db = new MbContext())
            {
                db.Device.AddOrUpdate(dev);

                return db.SaveChanges() > 0 ? ResultSuccess("操作成功！") : ResultError("操作异常！");
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Remove(string id)
        {
            using (var db = new MbContext())
            {
                var devId = Convert.ToInt64(id);

                var dev = db.Device.FirstOrDefault(t => t.DeviceId == devId);

                db.Device.Remove(dev);

                return db.SaveChanges() > 0 ? ResultSuccess("删除成功！") : ResultError("删除异常！");
            }
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
                var manufacturers = row["厂家"].ToString();
                var brand = row["品牌"].ToString();
                var name = row["设备名称"].ToString();
                var model = row["设备型号"].ToString();
                var postion = row["安装地址"].ToString();
                var areaName = row["区域名称"].ToString();
                var areaCode = row["区域代码"].ToString();
                var assets = row["资产代码"].ToString();
                var category = row["类目"].ToString();
                var buyTimestr = row["采购日期"].ToString();
                var warrantyTimestr = row["保修日期"].ToString();

                if (string.IsNullOrEmpty(name.Trim()) ||
                   string.IsNullOrEmpty(areaName.Trim()) ||
                   string.IsNullOrEmpty(areaCode.Trim()) ||
                   string.IsNullOrEmpty(category.Trim()))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 表格导入
        /// </summary>
        /// <returns></returns>
        public ActionResult ImportExcel(long company)
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
                    var manufacturers = row["厂家"].ToString();
                    var brand = row["品牌"].ToString();
                    var name = row["设备名称"].ToString();
                    var model = row["设备型号"].ToString();
                    var postion = row["安装地址"].ToString();
                    var areaName = row["区域名称"].ToString();
                    var areaCode = row["区域代码"].ToString();
                    var assets = row["资产代码"].ToString();
                    var category = row["类目"].ToString();
                    var buyTimestr = row["采购日期"].ToString();
                    var warrantyTimestr = row["保修日期"].ToString();

                    var buyTime = DateTime.Now;
                    DateTime.TryParse(buyTimestr, out buyTime);

                    var warrantyTime = DateTime.Now;
                    DateTime.TryParse(warrantyTimestr, out warrantyTime);

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
                    if (!string.IsNullOrEmpty(areaCode) || !string.IsNullOrEmpty(areaName))
                    {
                        a = db.Area.FirstOrDefault(t => t.Code == areaCode || t.Name == areaName);
                        if (a == null)
                        {
                            a = db.Area.Add(new Area
                            {
                                LevelDeep = 0,
                                Name = areaName,
                                Code = areaCode,
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

                    db.Device.Add(new Device()
                    {
                        AssetsId = assets,
                        CategoryId = c.CategoryId,
                        AreaId = a.AreaId,
                        BrandId = b.BrandId,
                        UseCompanyId = company,
                        Name = name,
                        Model = model,
                        BuyTime = buyTime,
                        WarrantyTime = warrantyTime,
                        LastUpdateTime = DateTime.Now,
                        CreateTime = DateTime.Now
                    });
                    db.SaveChanges();
                }
            }

            return ResultSuccess("导入成功！");
        }

        /// <summary>
        /// 到处二维码表格
        /// </summary>
        /// <returns></returns>
        public ActionResult ExportQRCode(string companyCode, int count, int recIndex = 1)
        {
            var dt = new DataTable();
            dt.Columns.Add();
            dt.Columns.Add();
            for (int i = recIndex; i < +recIndex + count; i++)
            {
                var tr = dt.NewRow();
                tr[0] = string.Format("http://www.zmnbx.com/equipment/{0}{1}", companyCode, i.ToString("00000"));
                tr[1] = companyCode + i.ToString("00000");
                dt.Rows.Add(tr);
            }

            ExcelHelper.InitializeWorkbook();
            ExcelHelper.GenerateData(dt, "sheet1");

            var ms = new MemoryStream();

            ms.Write(ExcelHelper.WriteToStream().GetBuffer(), 0, ExcelHelper.WriteToStream().GetBuffer().Length);
            ms.Position = 0;

            return new FileStreamResult(ms, "application/ms-excel") { FileDownloadName = DateTime.Now.ToString("yyyyMMddHHmmssfff'.xls'") };
        }

        /// <summary>
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
        public ActionResult select_usecompany_control(string companyId)
        {
            using (var db = new MbContext())
            {
                var model = db.UseCompany
                            .OrderBy(t => t.Name)
                            .ToList();

                ViewBag.companyId = companyId;

                return View(model);
            }
        }
    }
}