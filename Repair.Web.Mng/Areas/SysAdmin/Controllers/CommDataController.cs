using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using LZY.BX.Model;
using LZY.BX.Model.PageModel;
using LZY.BX.Service;
using LZY.BX.Service.Mb;
using Repair.Web.Mng.Controllers;
using Repair.Web.Mng.Menu;
using ZR;
using LZY.BX.Model.QueryModel;
using Repair.Web.Areas.SysAdmin.Models;

namespace Repair.Web.Mng.Areas.SysAdmin.Controllers
{
    public class CommDataController : BaseController
    {
        //
        // GET: /SysAdmin/CommData/

        [MenuDefault("常用数据", PArea = "", Icon = "fa-database")]
        public void Index() { }




        #region  产品管理
        /// <summary>
        /// 产品管理
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="brandId"></param>
        /// <param name="manufacturerId"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [MenuCurrent("产品管理", PAction = "Index", Icon = "fa-tachometer")]
        public ActionResult ProductSys(long? productId, long? brandId, long? manufacturerId, string key)
        {
            var user = CurrentUser;
            var pageIndex = Request["pageIndex"].AsInt(1);
            var pageSize = Request["pageSize"].AsInt(20);
            ViewData["UserId"] = user.UserId;
            ViewData["manugacturer"] = ManufacturersSvr.Instance.GetManufatuer();
            var queryModel = new ProductQueryModel
            {
                ProductId = productId,
                BrandId = brandId,
                ManufacturerId = manufacturerId,
                CNNameLike = key ?? null,
                Index = pageIndex,
                Size = pageSize
            };
            using (var db = new MbContext())
            {
                var models = db.Product
                    .Where(queryModel)
                    .ToList();
                queryModel.Data = models.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                var manufacturerIds = models.Select(x => x.ManufacturerId).ToArray();
                queryModel.ManufacturerDic = db.Manufacturer
                    .Where(x => manufacturerIds.Contains(x.ManufacturerId))
                    .ToLookup(x => x.ManufacturerId)
                    .ToDictionary(x => x.Key, x => x.First());
                var brandIds = models.Select(x => x.BrandId).ToArray();
                queryModel.BrandDic = db.Brand
                    .Where(x => brandIds.Contains(x.BrandId))
                    .ToLookup(x => x.BrandId)
                    .ToDictionary(x => x.Key, x => x.First());
                queryModel.Count = models.Count;
                return View(queryModel);

            }

        }
        [MenuHide("编辑", PAction = "ProductSys")]
        public ActionResult ProductEdit(long id = 0)
        {
            ViewData["manufaturers"] = ManufacturersSvr.Instance.GetManufatuer();
            ViewData["brands"] = BrandSvr.Instance.GetBrandById(0);
            using (var db = new MbContext())
            {
                return View(db.Product.FirstOrDefault(x => x.ProductId == id));
            }
        }
        [MenuHide("删除", PAction = "ProductSys")]
        public ActionResult ProductDelete(long id)
        {
            using (var db = new MbContext())
            {
                var model = db.Product.FirstOrDefault(x => x.ProductId == id);
                if (model != null)
                {
                    db.Product.Remove(model);
                }
                if (db.SaveChanges() > 0)
                {
                    return Content("<script>alert('操作成功');window.location.href='/SysAdmin/CommData/ProductSys'</script>");
                }
                return Content("<script>alert('操作失败');window.location.href='/SysAdmin/CommData/ProductSys'</script>");
            }
        }
        [MenuHide("编辑", PAction = "ProductSys")]
        [HttpPost]
        public ActionResult ProductEdit(Product models)
        {
            using (var db = new MbContext())
            {
                if (models.ProductId <= 0)
                {
                    models.ProductId = SequNo.NewId;
                    models.CreateTime = DateTime.Now;
                }
                else
                {
                    var product = db.Product.FirstOrDefault(x => x.ProductId == models.ProductId);
                    if (product != null)
                    {
                        models.Note = product.Note;
                        models.BirthTime = product.BirthTime;
                        models.CreateTime = product.CreateTime;
                    }
                }
                db.Product.AddOrUpdate(models);
                if (db.SaveChanges() > 0)
                {
                    return Content("<script>alert('操作成功');window.location.href='/SysAdmin/CommData/ProductSys'</script>");
                }
                return Content("<script>alert('操作失败');window.location.href='/SysAdmin/CommData/ProductSys'</script>");
            }
        }

        #endregion

        #region 设备管理


        [MenuCurrent("设备管理", PAction = "Index")]
        public ActionResult EquipmentSys()
        {
            return View(Session["EquipmentSysQueryModel"]);
        }

        public ActionResult EquipmentSysList(EquipmentSysQueryModel query)
        {
            Session["EquipmentSysQueryModel"] = query;

            using (var db = new MbContext())
            {
                var model = db.Equipment
                    .Where(query)
                    .Where(x => x.ProductId > 0);

                query.Data = model
                    .OrderByDescending(x => x.CreateTime)
                    .Skip(query.PageInfo.RecIndex)
                    .Take(query.PageInfo.PageSize)
                    .ToList();

                //品牌字典
                var bIds = query.Data
                    .Select(x => x.BrandId)
                    .ToList();

                query.BrandDic = db.Brand
                    .Where(x => bIds.Contains(x.BrandId))
                    .ToLookup(x => x.BrandId)
                    .ToDictionary(x => x.Key, x => x.First());

                //产品字典
                var pIds = query.Data
                    .Select(x => x.ProductId)
                    .ToList();

                query.ProductDic = db.Product
                    .Where(x => pIds.Contains(x.ProductId))
                    .ToLookup(x => x.ProductId)
                    .ToDictionary(x => x.Key, x => x.First());

                //报修单位字典
                var ucIds = query.Data
                    .Select(x => x.UseCompanyId)
                    .ToList();

                query.UseCompanyDic = db.UseCompany
                    .Where(x => ucIds.Contains(x.UseCompanyId))
                    .ToLookup(x => x.UseCompanyId)
                    .ToDictionary(x => x.Key, x => x.First());

                //服务单位设备字典
                var eIds = query.Data
                    .Select(x => x.EquipmentId)
                    .ToList();

                query.ServiceEquipmentDic = db.ServiceEquipment
                    .Where(x => eIds.Contains(x.EquipmentId))
                    .ToLookup(x => x.EquipmentId)
                    .ToDictionary(x => x.Key, x => x.First());

                //服务单位字典
                var scIds = db.ServiceEquipment
                    .Where(x => eIds.Contains(x.EquipmentId))
                    .Select(x => x.ServiceCompanyId)
                    .ToList();

                query.ServiceCompanyDic = db.ServiceCompany
                    .Where(x => scIds.Contains(x.ServiceCompanyId))
                    .ToLookup(x => x.ServiceCompanyId)
                    .ToDictionary(x => x.Key, x => x.First());

                query.PageInfo.TotalCount = model.Count();

                return View(query);
            }
        }

        [MenuHide("编辑", PAction = "EquipmentSys")]
        public ActionResult EquipmentEdit(long id = 0)
        {
            using (var db = new MbContext())
            {
                var model = db.Equipment.FirstOrDefault(x => x.EquipmentId == id);
                ViewData["manufacturer"] = ManufacturersSvr.Instance.GetManufatuer();
                ViewData["serviceCompany"] = ServiceUnitSvr.Instance.GetListByUserId(CurrentUser.UserId);
                ViewData["useCompany"] = UseCompanySvr.Instance.GetUseCompany();

                ViewData["userType"] =
                    db.Roles.FirstOrDefault(
                        m => m.RolesId == db.UserRoles.FirstOrDefault(x => x.UserId == CurrentUser.UserId).RolesId)
                        .CompanyType;

                if (model != null)
                {
                    ViewData["brand"] = BrandSvr.Instance.GetBrandById(model.ManufacturerId);
                    var serviceEquipment = db.ServiceEquipment.FirstOrDefault(x => x.EquipmentId == model.EquipmentId);
                    if (serviceEquipment != null)
                    {
                        ViewData["ServiceCompanyId"] = serviceEquipment.ServiceCompanyId;
                    }
                }
                return View(model);
            }
        }

        [MenuHide("删除", PAction = "EquipmentSys")]
        public ActionResult EquipmentDelete(long id)
        {
            using (var db = new MbContext())
            {
                var model = db.Equipment.FirstOrDefault(x => x.EquipmentId == id);
                if (model != null)
                {
                    db.Equipment.Remove(model);
                }
                if (db.SaveChanges() > 0)
                {
                    return
                        Content("<script>alert('操作成功');window.location.href='/SysAdmin/CommData/EquipmentSys'</script>");
                }
                return Content("<script>alert('操作失败');window.location.href='/SysAdmin/CommData/EquipmentSys'</script>");
            }
        }

        [MenuHide("编辑", PAction = "EquipmentSys")]
        [HttpPost]
        public ActionResult EquipmentEdit(Equipment models, long serviceCompanyId)
        {
            var user = CurrentUser;

            var result = EquipmentSvr.Instance.Save(models, user.UserId, serviceCompanyId);
            if (result > 0)
            {
                return Content("<script>alert('操作成功');window.location.href='/SysAdmin/CommData/EquipmentSys'</script>");
            }
            return Content("<script>alert('操作失败');window.location.href='/SysAdmin/CommData/EquipmentSys'</script>");
        }
        #endregion

        #region 品牌管理
        [MenuCurrent("品牌管理", PAction = "Index", Icon = "fa-apple")]
        public ActionResult BrandSys(long? manufacturerId, string key)
        {
            var user = CurrentUser;
            var pageIndex = Request["pageIndex"].AsInt(1);
            var pageSize = Request["pageSize"].AsInt(20);

            ViewData["UserId"] = user.UserId;

            using (var db = new MbContext())
            {
                var queryModel = new BrandQueryModel()
                {
                    Index = pageIndex,
                    Size = pageSize,
                    ManufacturerId = manufacturerId,
                    CNNameLike = key ?? null
                };
                var models = db.Brand
                    .Where(queryModel)
                    .ToList();
                var manufacturerIds = models.Select(x => x.ManufacturerId).ToArray();
                queryModel.ManufacturerDic = db.Manufacturer
                    .Where(x => manufacturerIds.Contains(x.ManufacturerId))
                    .ToLookup(x => x.ManufacturerId)
                    .ToDictionary(x => x.Key, x => x.First());
                queryModel.Data = models.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                queryModel.Count = models.Count;

                return View(queryModel);
            }

        }
        [MenuHide("编辑", PAction = "BrandSys")]
        public ActionResult BrandEdit(long id = 0)
        {

            using (var db = new MbContext())
            {
                ViewData["manufatuer"] = db.Manufacturer.ToList();
                return View(db.Brand.FirstOrDefault(x => x.BrandId == id));
            }


        }
        [MenuHide("删除", PAction = "BrandSys")]
        public ActionResult BrandDelete(long id)
        {
            using (var db = new MbContext())
            {
                var model = db.Brand.FirstOrDefault(x => x.BrandId == id);
                if (model != null)
                {
                    db.Brand.Remove(model);
                }
                if (db.SaveChanges() > 0)
                {
                    return Content("<script>alert('删除成功');window.location.href='/SysAdmin/CommData/BrandSys'</script>");
                }
                return Content("<script>alert('删除失败');window.location.href='/SysAdmin/CommData/BrandSys'</script>");
            }
        }
        [MenuHide("编辑", PAction = "BrandSys")]
        [HttpPost]
        public ActionResult BrandEdit(Brand model)
        {
            using (var db = new MbContext())
            {
                if (model.BrandId <= 0)
                {
                    model.BrandId = SequNo.NewId;
                    model.CreateTime = DateTime.Now;
                }
                else
                {
                    var brand = db.Brand.FirstOrDefault(x => x.BrandId == model.BrandId);
                    if (brand != null)
                    {
                        model.Note = brand.Note;
                        model.CreateTime = brand.CreateTime;
                    }
                }
                db.Brand.AddOrUpdate(model);
                if (db.SaveChanges() > 0)
                {
                    return Content("<script>alert('操作成功');window.location.href='/SysAdmin/CommData/BrandSys'</script>");
                }
                return Content("<script>alert('操作失败');window.location.href='/SysAdmin/CommData/BrandSys'</script>");
            }
        }
        #endregion

        #region  制造商管理
        [MenuCurrent("制造商管理", PAction = "Index", Icon = "fa-home")]
        public ActionResult ManufacturerSys(string key = null)
        {
            var user = CurrentUser;
            var pageIndex = Request["pageIndex"].AsInt(1);
            var pageSize = Request["pageSize"].AsInt(20);

            ViewData["UserId"] = user.UserId;
            using (var db = new MbContext())
            {
                var manufacturers = db.Manufacturer.ToList();
                if (key != null)
                {
                    //manufacturers = manufacturers.FindAll(x => x.CNName.Contains(key));
                }
                var pager = new Page<Manufacturer>
                {
                    Index = pageIndex,
                    Size = pageSize,
                    Count = manufacturers.Count,
                    Data = manufacturers.OrderByDescending(m => m.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize)
                };

                return View(pager);
            }
        }
        [MenuHide("编辑", PAction = "ManufacturerSys")]
        public ActionResult ManufacturerEdit(long id = 0)
        {
            using (var db = new MbContext())
            {
                return View(db.Manufacturer.FirstOrDefault(x => x.ManufacturerId == id));
            }


        }
        [MenuHide("删除", PAction = "ManufacturerSys")]
        public ActionResult ManufacturerDelete(long id)
        {
            if (ManufacturersSvr.Instance.Delete(id) > 0)
            {
                return Content("<script>alert('操作成功');window.location.href='/SysAdmin/CommData/ManufacturerSys'</script>");
            }
            return Content("<script>alert('操作失败');window.location.href='/SysAdmin/CommData/ManufacturerSys'</script>");
        }
        [MenuHide("编辑", PAction = "ManufacturerSys")]
        [HttpPost]
        public ActionResult ManufacturerEdit(Manufacturer model)
        {
            using (var db = new MbContext())
            {
                if (!(model.ManufacturerId > 0))
                {
                    model.ManufacturerId = SequNo.NewId;
                    model.CreateTime = DateTime.Now;
                }
                else
                {
                    var manufacturer = db.Manufacturer.FirstOrDefault(x => x.ManufacturerId == model.ManufacturerId);
                    if (manufacturer != null)
                    {
                        model.CreateTime = manufacturer.CreateTime;
                        model.Longitude = manufacturer.Longitude;
                        model.Dimension = manufacturer.Dimension;
                        model.Note = manufacturer.Note;
                    }
                }
                db.Manufacturer.AddOrUpdate(model);
                if (db.SaveChanges() > 0)
                {
                    return Content("<script>alert('操作成功');window.location.href='/SysAdmin/CommData/ManufacturerSys'</script>");
                }
                return Content("<script>alert('操作失败');window.location.href='/SysAdmin/CommData/ManufacturerSys'</script>");
            }
        }
        #endregion


        #region 故障管理
        [MenuCurrent("故障描述", PAction = "Index")]
        public ActionResult DiagnosisSys()
        {
            var pageIndex = Request["pageIndex"].AsInt(1);
            var pageSize = Request["pageSize"].AsInt(20);
            using (var db = new MbContext())
            {
                var models = (from d in db.Diagnosis
                              join m in db.Manufacturer on d.ManufacturerId equals m.ManufacturerId into t1
                              from mm in t1.DefaultIfEmpty()
                              join b in db.Brand on d.BrandId equals b.BrandId into t2
                              from bb in t2.DefaultIfEmpty()
                              join p in db.Product on d.ProductId equals p.ProductId into t3
                              from pp in t3.DefaultIfEmpty()
                              select new PageDiagnosis()
                              {
                                  DiagnosisId = d.DiagnosisId,
                                  //ManufacturersName = mm.CNName,
                                  //BrandName = bb.CNName,
                                  ProductName = pp.CNName,
                                  Describe = d.Describe
                              }).ToList();

                var pager = new Page<PageDiagnosis>()
            {
                Index = pageIndex,
                Size = pageSize,
                Count = models.Count,
                Data = models.Skip(pageIndex - 1).Take(pageSize).ToList()
            };
                return View(pager);
            }

        }

        public ActionResult DiagnosisEdit(long id = 0)
        {
            ViewData["manufacturer"] = ManufacturersSvr.Instance.GetManufatuer();
            var model = DiagnosisSvr.Instance.GetItem(id);
            if (model != null)
            {
                ViewData["brand"] = BrandSvr.Instance.GetBrandById(model.ManufacturerId);
                ViewData["product"] = ProductSvr.Instance.GetProductListByBrandId(model.BrandId);
            }
            else
            {
                ViewData["brand"] = BrandSvr.Instance.GetBrandById(0);
                ViewData["product"] = ProductSvr.Instance.GetProductListByBrandId(0);
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult DiagnosisEdit(Diagnosis model)
        {
            var result = DiagnosisSvr.Instance.Update(model);
            if (result > 0)
            {
                return Content("<script>alert('操作成功');window.location.href='/SysAdmin/CommData/DiagnosisSys'</script>");
            }
            return Content("<script>alert('操作失败');window.location.href='/SysAdmin/CommData/DiagnosisSys'</script>");
        }

        public ActionResult DiagnosisDelete(long id = 0)
        {
            var result = DiagnosisSvr.Instance.Delete(id);
            if (result > 0)
            {
                return Content("<script>alert('操作成功');window.location.href='/SysAdmin/CommData/DiagnosisSys'</script>");
            }
            return Content("<script>alert('操作失败');window.location.href='/SysAdmin/CommData/DiagnosisSys'</script>");
        }
        #endregion


        #region 资产管理
        [MenuCurrent("资产管理", PAction = "Index")]
        public ActionResult PropertySys()
        {
            var user = CurrentUser;
            var pageIndex = Request["pageIndex"].AsInt(1);
            var pageSize = Request["pageSize"].AsInt(20);
            var queryModel = new AssertQueryModel()
            {
                Index = pageIndex,
                Size = pageSize
            };
            using (var db = new MbContext())
            {
                var models = db.UseCompanyAssert
                    .Where(queryModel)
                    .ToList();
                var useCompanyUser = db.UseCompanyUser.FirstOrDefault(x => x.UserId == user.UserId);
                if (useCompanyUser != null)
                {
                    models = models.FindAll(x => x.UseCompanyId == useCompanyUser.UseCompanyId);
                }
                //todo 添加制造商字典
                var manufacturerIds = models.Select(x => x.ManufacturerId).ToArray();
                queryModel.ManufacturerDic = db.Manufacturer
                    .Where(x => manufacturerIds.Contains(x.ManufacturerId))
                    .ToLookup(x => x.ManufacturerId)
                    .ToDictionary(x => x.Key, x => x.First());
                //todo 添加品牌字典
                var brandIds = models.Select(x => x.BrandId).ToArray();
                queryModel.BrandDic = db.Brand
                    .Where(x => brandIds.Contains(x.BrandId))
                    .ToLookup(x => x.BrandId)
                    .ToDictionary(x => x.Key, x => x.First());
                //todo 添加产品字典
                var productIds = models.Select(x => x.ProductId).ToArray();
                queryModel.ProductDic = db.Product
                    .Where(x => productIds.Contains(x.ProductId))
                    .ToLookup(x => x.ProductId)
                    .ToDictionary(x => x.Key, x => x.First());
                //添加类目字典
                var categoryIds = models.Select(x => x.CategoryId).ToArray();
                queryModel.CategoryDic = db.Category
                    .Where(x => categoryIds.Contains(x.CategoryId))
                    .ToLookup(x => x.CategoryId)
                    .ToDictionary(x => x.Key, x => x.First());
                //添加主数据
                queryModel.Data = models.Skip((pageIndex - 1) * pageSize).Take(pageSize);
                queryModel.Count = models.Count;

                return View(queryModel);
            }
        }
        [MenuHide("编辑", PAction = "PropertySys")]
        public ActionResult PropertyEdit(long id = 0)
        {
            using (var db = new MbContext())
            {

                return View(db.Property.FirstOrDefault(x => x.AssetsId == id));
            }
        }
        [MenuHide("删除", PAction = "PropertySys")]
        public ActionResult PropertyDelete(long id)
        {
            using (var db = new MbContext())
            {
                var model = db.Property.FirstOrDefault(x => x.AssetsId == id);
                if (model != null)
                {
                    db.Property.Remove(model);
                    if (db.SaveChanges() > 0)
                    {
                        return Content("<script>alert('操作成功');window.location.href='/SysAdmin/CommData/PropertySys'</script>");
                    }

                }
                return Content("<script>alert('操作失败');window.location.href='/SysAdmin/CommData/PropertySys'</script>");
            }

        }
        [MenuHide("编辑", PAction = "PropertySys")]
        [HttpPost]
        public ActionResult PropertyEdit(Assets models)
        {
            using (var db = new MbContext())
            {
                if (models.AssetsId > 0)
                {
                    db.Property.AddOrUpdate(models);
                }
                else
                {
                    models.AssetsId = SequNo.NewId;
                    db.Property.AddOrUpdate(models);
                }
                if (db.SaveChanges() > 0)
                {
                    return Content("<script>alert('操作成功');window.location.href='/SysAdmin/CommData/PropertySys'</script>");
                }
                return Content("<script>alert('操作失败');window.location.href='/SysAdmin/CommData/PropertySys'</script>");
            }

        }
        #endregion
    }
}
