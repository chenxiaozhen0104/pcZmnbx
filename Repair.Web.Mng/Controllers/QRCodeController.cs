using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using ICSharpCode.SharpZipLib.Zip;
using LZY.BX.Model;
using LZY.BX.Model.PageModel;
using LZY.BX.Service;
using LZY.BX.Service.Mb;

using Repair.Web.Mng.Menu;
using System.Drawing;
using LZY.BX.Utilities;

namespace Repair.Web.Mng.Controllers
{
    public class QRCodeController : BaseController
    {
        //
        // GET: /QRCode/
        [MenuDefault("二维码管理", PArea = "")]
        public void Index1() { }

        /// <summary>
        /// 二维码列表
        /// </summary>
        /// <returns></returns>
        [MenuCurrent("二维码列表", PAction = "Index1")]
        public ActionResult Index()
        {
            var pageIndex = Request["pageIndex"].AsInt(1);
            var pageSize = Request["pageSize"].AsInt(20);
            ViewData["useCompany"] = CompanySvr.Instance.GetList();
            using (var db = new MbContext())
            {
                var qrCode = db.Equipment.OrderByDescending(x => x.CreateTime).ToList();
                var pager = new Page<Equipment>
                {
                    Index = pageIndex,
                    Size = pageSize,
                    Count = qrCode.Count,
                    Data = qrCode.Skip((pageIndex - 1) * pageSize).Take(pageSize)
                };
                return View(pager);
            }
        }
        /// <summary>
        /// 删除二维码
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [MenuHide("删除", PAction = "Index")]
        public ActionResult Delete(long id)
        {
            if (QRCodeSvr.Instance.Delete(id) > 0)
            {
                return Content("<script>alert('操作成功');window.location.href='/QRCode/Index'</script>");
            }
            return Content("<script>alert('操作失败');window.location.href='/QRCode/Index'</script></script>");
        }

        /// <summary>
        /// 服务单位绑定
        /// </summary>
        /// <param name="serviceCompanyId"></param>
        /// <returns></returns>
        [MenuCurrent("服务单位绑定", PAction = "Index1")]
        public ActionResult ServiceCompanyBind(long? serviceCompanyId = 0)
        {
            var pageIndex = Request["pageIndex"].AsInt(1);
            var pageSize = Request["pageSize"].AsInt(20);
            var user = CurrentUser;
            ViewData["serviceCompanyList"] = ServiceUnitSvr.Instance.GetList();
            ViewData["brandList"] = BrandSvr.Instance.GetList();
            using (var db = new MbContext())
            {
                var useCompanyId = 0L;
                var useCompanyUser = db.UseCompanyUser.FirstOrDefault(x => x.UserId == user.UserId);
                if (useCompanyUser != null)
                {
                    useCompanyId = useCompanyUser.UseCompanyId;

                }
                ViewData["useCompanyId"] = useCompanyId;

                var models = (from se in db.ServiceEquipment
                              join sc in db.ServiceCompany on se.ServiceCompanyId equals sc.ServiceCompanyId
                              join e in db.Equipment on se.EquipmentId equals e.EquipmentId
                              select new PageServiceEquipment()
                              {
                                  ServiceEquipmentId = se.ServiceEquipmentId,
                                  EquipmentId = se.EquipmentId,
                                  ServiceCompanyId = se.ServiceCompanyId,
                                  ServiceCompanyName = sc.Name,
                                  EquipmentName = e.Name
                              }).ToList();

                if (serviceCompanyId > 0)
                {
                    models = models.FindAll(x => x.ServiceCompanyId == serviceCompanyId);
                }
                if (user.UserId > 0)
                {
                    var serviceCompany = db.ServiceCompanyUser.FirstOrDefault(x => x.UserId == user.UserId);
                    if (serviceCompany != null)
                    {
                        serviceCompanyId = serviceCompany.ServiceCompanyId;
                        models = models.FindAll(x => x.ServiceCompanyId == serviceCompanyId);
                    }
                }
                var pager = new Page<PageServiceEquipment>()
                {
                    Index = pageIndex,
                    Size = pageSize,
                    Count = models.Count,
                    Data = models.Skip((pageIndex - 1) * pageSize).Take(pageSize)
                };
                return View(pager);
            }
        }

        /// <summary>
        /// 添加服务单位与设备绑定
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [MenuHide("添加", PAction = "ServiceCompanyBind")]
        [HttpPost]
        public ActionResult Add(ServiceEquipment model)
        {
            var user = CurrentUser;
            using (var db = new MbContext())
            {
                var serviceEquipment =
                    db.ServiceEquipment.FirstOrDefault(
                        x => x.EquipmentId == model.EquipmentId && x.ServiceCompanyId == model.ServiceCompanyId);
                if (serviceEquipment == null)
                {
                    model = new ServiceEquipment()
                    {
                        EquipmentId = model.EquipmentId,
                        ServiceCompanyId = model.ServiceCompanyId,
                        SortField = 0,
                        CreateTime = DateTime.Now,
                        CreateUserId = user.UserId
                    };
                }
                if (ServiceEquipmentSvr.Instance.Save(model) > 0)
                {
                    return Content("<script>alert('操作成功');window.location.href='/QRCode/ServiceCompanyBind'</script>");
                }
                return Content("<script>alert('操作失败');window.location.href='/QRCode/ServiceCompanyBind'</script>");
            }

        }

        /// <summary>
        /// 导入基础设备信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Import()
        {
            var user = CurrentUser;
            var useCompanyId = 0L;
            using (var db = new MbContext())
            {
                var useCompanyUser = db.UseCompanyUser.FirstOrDefault(x => x.UserId == user.UserId);
                if (useCompanyUser != null)
                {
                    useCompanyId = useCompanyUser.UseCompanyId;
                }
            }
            var file = Request.Files["dataFile"];
            if (file != null)
            {
                var bytes = new byte[file.ContentLength];
                file.InputStream.Read(bytes, 0, file.ContentLength);
                var text = Encoding.Default.GetString(bytes);
                var lines = text.Split(new[] { '\n' });
                var result = 0;
                for (var i = 1; i < lines.Length; i++)
                {
                    var arr = lines[i].Split('\t');
                    if (arr.Length != 7) continue;

                    var mechName = string.IsNullOrEmpty(arr[4]) ? "其他" : arr[4].Trim();
                    var brandName = string.IsNullOrEmpty(arr[3]) ? "其他" : arr[3].Trim();
                    var prodName = string.IsNullOrEmpty(arr[1]) ? "其他" : arr[1].Trim();
                    var model = string.IsNullOrEmpty(arr[2]) ? "其他" : arr[2].Trim();
                    var catName = string.IsNullOrEmpty(arr[5]) ? "其他" : arr[5].Trim();
                    //导入品牌、厂家 产品等信息
                    var product = ProductSvr.Instance.CreateProduct(mechName, brandName, prodName, model);
                    var categoryModel = CategorySvr.Instance.Save(catName);

                    var productCatlog = new ProductCategory
                    {
                        CategoryId = categoryModel.CategoryId,
                        CreateTime = DateTime.Now,
                        Note = string.Empty,
                        OuterId = product.ProductId,
                        SortField = 0,
                        Type = "Product"
                    };
                    ProductCategorySvr.Instance.Save(productCatlog);
                    //更具产品信息读取产品对象
                    AssertSvr.Instance.TryGetProductInfo(mechName, brandName, prodName, model, out product);

                    //读取产品类目对象
                    var productCat = ProductCategorySvr.Instance.GetItemByOuterId(product.ProductId);
                    if (productCat == null)
                        continue;

                    //生成资产数据
                    var item = new UseCompanyAssert
                    {
                        UseCompanyId = useCompanyId,
                        AssertNumber = arr[0].Trim(),
                        CNName = product.CNName,
                        Model = product.Model,
                        ManufacturerId = product.ManufacturerId,
                        BrandId = product.BrandId,
                        ProductId = product.ProductId,
                        Location = arr[6].Trim(),
                        CategoryId = productCat.CategoryId,
                    };
                    result += UseCompanyAssertSvr.Instance.Save(item);

                }
                if (result > 0)
                {
                    return Content("<script>alert('成功添加" + result + "条数据');window.location.href='/QRCode/ServiceCompanyBind'</script>");
                }
                return Content("<script>alert('操作失败');window.location.href='/QRCode/ServiceCompanyBind'</script>");
            }
            return Content("<script>alert('操作失败');window.location.href='/QRCode/ServiceCompanyBind'</script>");
        }

        /// <summary>
        /// 删除服务单位与设备绑定
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ServiceEquipmentDelete(long id)
        {
            var result = ServiceEquipmentSvr.Instance.Delete(id);
            if (result > 0)
            {
                return Content("<script>alert('操作成功');window.location.href='/QRCode/ServiceCompanyBind'</script>");
            }
            return Content("<script>alert('操作失败');window.location.href='/QRCode/ServiceCompanyBind'</script>");
        }

        /// <summary>
        /// 报修单位绑定
        /// </summary>
        /// <returns></returns>
        [MenuCurrent("报修单位绑定", PAction = "Index1")]
        public ActionResult UseCompanyBind()
        {
            var pageIndex = Request["pageIndex"].AsInt(1);
            var pageSize = Request["pageSize"].AsInt(20);
            var user = CurrentUser;
            ViewData["UserId"] = user.UserId;
            ViewData["useCompanyList"] = CompanySvr.Instance.GetList();
            ViewData["brandList"] = BrandSvr.Instance.GetList();
            using (var db = new MbContext())
            {
                var models = (from e in db.Equipment
                              join uc in db.UseCompany on e.UseCompanyId equals uc.UseCompanyId
                              select new PageEquipment()
                              {
                                  EquipmentId = e.EquipmentId,
                                  UseCompanyId = e.UseCompanyId,
                                  Name = e.Name,
                                  UseCompanyName = uc.Name
                              }).ToList();

                var useCompanyUser = db.UseCompanyUser.FirstOrDefault(x => x.UserId == user.UserId);
                if (useCompanyUser != null)
                {
                    models = models.FindAll(x => x.UseCompanyId == useCompanyUser.UseCompanyId);
                }
                var pager = new Page<PageEquipment>
                {
                    Index = pageIndex,
                    Size = pageSize,
                    Data = models.Skip((pageIndex - 1) * pageSize).Take(pageSize),
                    Count = models.Count
                };
                return View(pager);
            }

        }

        /// <summary>
        /// 添加报修单位与设备绑定
        /// </summary>
        /// <param name="useCompanyId"></param>
        /// <param name="equipmentId"></param>
        /// <returns></returns>
        [MenuHide("添加", PAction = "UseCompanyBind")]
        [HttpPost]
        public ActionResult UseCompanyEquipmentAdd(long useCompanyId, long equipmentId)
        {
            var result = -1;
            using (var db = new MbContext())
            {
                var equipment = db.Equipment.FirstOrDefault(x => x.EquipmentId == equipmentId);
                if (equipment != null)
                {
                    equipment.UseCompanyId = useCompanyId;
                    result = db.SaveChanges();
                }
            }
            if (result > 0)
            {
                return Content("<script>alert('操作成功');window.location.href='/QRCode/UseCompanyBind'</script>");
            }

            return Content("<script>alert('操作失败');window.location.href='/QRCode/UseCompanyBind'</script>");
        }

        /// <summary>
        /// 删除报修单位与设备绑定
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult UseCompanyEquipmentDelete(long id)
        {
            var result = EquipmentSvr.Instance.DeleteUseCompanyEquipment(id);
            if (result > 0)
            {
                return Content("<script>alert('操作成功');window.location.href='/QRCode/UseCompanyBind'</script>");
            }
            return Content("<script>alert('操作失败');window.location.href='/QRCode/UseCompanyBind'</script>");
        }


        /// <summary>
        /// 下载二维码
        /// </summary>
        public ActionResult DownloadQRCodePic()
        {
            var strFileName = "QRCode";
            if (Request["QRCodeUrl"] == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                var picList = Request["QRCodeUrl"].Split(',');
                var logoImg = Image.FromFile(Server.MapPath("/") + "logo/zdLogo.png");
                var fontImg = Image.FromFile(Server.MapPath("/") + "logo/font.png");
                var byteList = new List<byte[]>();
                var fileNameList = new List<string>();
                foreach (var item in picList)
                {
                    byteList.Add(QRCodeHelper.GetCode(item, logoImg, fontImg));
                    fileNameList.Add(item);
                }
                DownZip(byteList, fileNameList, strFileName);
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// 批量生成二维码并下载
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateQRCodeAndDownload(int number, string info)
        {
            var useCompanyId = Convert.ToInt64(info.Split(',')[0]);
            var code = Convert.ToString(info.Split(',')[1]);

            var byteList = new List<byte[]>();
            var fileNameList = new List<string>();
            var codeList = new List<string>();
            var logoImg = Image.FromFile(Server.MapPath("/") + "logo/zdLogo.png");
            var fontImg = Image.FromFile(Server.MapPath("/") + "logo/font.png");
            var count = 0L;
            using (var db = new MbContext())
            {
                var qrCodeCount = db.QrCodeCount.FirstOrDefault(x => x.CompanyId == useCompanyId);
                if (qrCodeCount == null)
                {
                    qrCodeCount = new QrCodeCount();
                    qrCodeCount.Count = 0;
                    qrCodeCount.CompanyId = useCompanyId;
                    qrCodeCount.LastUpdateTime = DateTime.Now;
                    db.QrCodeCount.Add(qrCodeCount);
                }
                qrCodeCount.Count += number;
                qrCodeCount.LastUpdateTime = DateTime.Now;
                count = qrCodeCount.Count;

                db.SaveChanges();
            }

            for (var i = 0; i < number; i++)
            {
                var cd = string.Format("{0}{1}", code, (count - number + i + 1).ToString().PadLeft(6, '0'));

                fileNameList.Add(cd);
                byteList.Add(QRCodeHelper.GetCode(cd, logoImg, fontImg));

                codeList.Add(cd);
            }
            EquipmentSvr.Instance.CreateQRCode(codeList, useCompanyId);
            DownZip(byteList, fileNameList, "NewQRCode");
            return RedirectToAction("Index");
        }

        static ZipOutputStream zos = null;
        /// <summary>
        /// 将文件放出压缩包下载
        /// </summary>
        /// <param name="byteList">文件流列表</param>
        /// <param name="fileNameList">文件名列表</param>
        /// <param name="strFileName">压缩包名称</param>
        public void DownZip(List<byte[]> byteList, List<string> fileNameList, string strFileName)
        {
            Response.ContentType = "application/octet-stream";
            strFileName = HttpUtility.UrlEncode(strFileName).Replace('+', ' ');
            Response.AddHeader("Content-Disposition", "attachment; filename=" + strFileName + ".zip");
            var ms = new MemoryStream();
            zos = new ZipOutputStream(ms);
            //添加文件至压缩包
            for (var i = 0; i < byteList.Count; i++)
            {
                var entry = new ZipEntry(fileNameList[i] + ".jpg");
                zos.PutNextEntry(entry);
                zos.Write(byteList[i], 0, byteList[i].Length);
            }
            zos.Finish();
            zos.Close();
            Response.Clear();
            Response.BinaryWrite(ms.ToArray());
            Response.End();
        }


    }
}
