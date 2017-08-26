using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LZY.BX.Model;
using LZY.BX.Service.Mb;
using ZR;
using Repair.Web.Mng.Menu;

namespace Repair.Web.Mng.Controllers
{
    public class DefaultController : BaseController
    {
        //
        // GET: /Home/
        //[MenuDefault("首页",PArea = "")]
        public void Index1() { }

        [Menu("啄木鸟保修")]
        public ActionResult Index()
        {
           
            return View();

        }
        //[MenuCurrent("shouye", PAction = "Index1")]
        public ActionResult Index2()
        {
            var ds = ExcelDataSource("H:\\excel\\school.xlsx", "Sheet1");
            ExcelToDB(ds);
            return View();

        }

        public DataSet ExcelDataSource(string filepath, string sheetname)
        {
            string strConn;
            strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filepath + ";Extended Properties=Excel 8.0;";
            var conn = new OleDbConnection(strConn);
            var oada = new OleDbDataAdapter("select * from [" + sheetname + "$]", strConn);
            var ds = new DataSet();
            oada.Fill(ds);
            return ds;
        }

        public void ExcelToDB(DataSet ds)
        {
            for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
            {

                var propertyId = ds.Tables[0].Rows[i]["PropertyId"].ToString();
                var propertyNumber = ds.Tables[0].Rows[i]["PropertyNumber"].ToString();
                var propertyName = ds.Tables[0].Rows[i]["PropertyName"].ToString();
                var model = ds.Tables[0].Rows[i]["Model"].ToString();
                var price = ds.Tables[0].Rows[i]["Price"].ToString();
                var state = ds.Tables[0].Rows[i]["State"].ToString();
                var usePersonId = ds.Tables[0].Rows[i]["UsePersonId"].ToString();
                var usePerson = ds.Tables[0].Rows[i]["UsePerson"].ToString();
                var useUnitId = ds.Tables[0].Rows[i]["UseUnitId"].ToString();
                var useUnit = ds.Tables[0].Rows[i]["UseUnit"].ToString();
                var designatedArea = ds.Tables[0].Rows[i]["DesignatedArea"].ToString();
                var buyTime = ds.Tables[0].Rows[i]["BuyTime"].ToString();
                var putInTime = ds.Tables[0].Rows[i]["PutInTime"].ToString();
                var manufacturerName = ds.Tables[0].Rows[i]["Manufacturer"].ToString();
                var supplier = ds.Tables[0].Rows[i]["Supplier"].ToString();
                var note = ds.Tables[0].Rows[i]["Note"].ToString();
                var postingTime = ds.Tables[0].Rows[i]["PostingTime"].ToString();
                var classifyId = ds.Tables[0].Rows[i]["ClassifyId"].ToString();
                var classifyName = ds.Tables[0].Rows[i]["ClassifyName"].ToString();
                var size = ds.Tables[0].Rows[i]["Size"].ToString();
                var country = ds.Tables[0].Rows[i]["Country"].ToString();
                var accessoryNumber = ds.Tables[0].Rows[i]["AccessoryNumber"].ToString();
                var accessoryPrice = ds.Tables[0].Rows[i]["AccessoryPrice"].ToString();
                var fundsSunject = ds.Tables[0].Rows[i]["FundsSunject"].ToString();
                var fundsId = ds.Tables[0].Rows[i]["FundsId"].ToString();
                var fundsName = ds.Tables[0].Rows[i]["FundsName"].ToString();
                var isImport = ds.Tables[0].Rows[i]["IsImport"].ToString();
                var propertyBelong = ds.Tables[0].Rows[i]["PropertyBelong"].ToString();
                var receiptsId = ds.Tables[0].Rows[i]["ReceiptsId"].ToString();
                var contractId = ds.Tables[0].Rows[i]["ContractId"].ToString();
                var handledBy = ds.Tables[0].Rows[i]["HandledBy"].ToString();
                var buyer = ds.Tables[0].Rows[i]["Buyer"].ToString();
                var inspectState = ds.Tables[0].Rows[i]["InspectState"].ToString();
                var factoryId = ds.Tables[0].Rows[i]["FactoryId"].ToString();
                var financeVoucherId = ds.Tables[0].Rows[i]["FinanceVoucherId"].ToString();
                var setAccountsBusinessId = ds.Tables[0].Rows[i]["SetAccountsBusinessId"].ToString();
                if (ds.Tables[0].Rows[i]["id"].ToString() == "")
                {
                    break;
                }
                //using (var db = new MbContext())
                //{
                   
                //    var mId = SequNo.NewId;
                //    var brandId = SequNo.NewId;
                //    var productId = SequNo.NewId;
                //    var useCompanyId = SequNo.NewId;
                //    var equId = SequNo.NewId;
                //    //添加制造商
                //    var manufacturer = db.Manufacturer.FirstOrDefault(x => x.CNName == manufacturerName);
                //    if (manufacturer == null)
                //    {
                //        db.Manufacturer.Add(new Manufacturer()
                //        {
                //            ManufacturerId = mId,
                //            CNName = manufacturerName,
                //            Position = "中国",
                //            Contact = "暂无",
                //            CreateTime = DateTime.Now,
                //            //Label = productName,
                //            Phone = "13800000111"
                //        });
                //    }
                //    else
                //    {
                //        mId = manufacturer.ManufacturerId;
                //    }
                //    //添加品牌
                //    var brand = db.Brand.FirstOrDefault(x => x.CNName == brandName);
                //    if (brand == null)
                //    {
                //        db.Brand.Add(new Brand()
                //        {
                //            BrandId = brandId,
                //            CNName = brandName,
                //            CreateTime = DateTime.Now,
                //            ManufacturerId = mId,
                //            Label = productName,
                //            BirthTime = DateTime.Now

                //        });
                //    }
                //    else
                //    {
                //        brandId = brand.BrandId;
                //    }
                //    //添加产品
                //    var product = db.Product.FirstOrDefault(x => x.CNName == productName);
                //    if (product == null)
                //    {
                //        db.Product.Add(new Product()
                //        {
                //            ProductId = productId,
                //            ManufacturerId = mId,
                //            BrandId = brandId,
                //            Model = model,
                //            CNName = productName,
                //            BirthTime = DateTime.Now,
                //            CreateTime = DateTime.Now
                //        });
                //    }
                //    else
                //    {
                //        productId = product.ProductId;
                //    }
                   
                //    //添加使用单位
                //    var useCompany = db.UseCompany.FirstOrDefault(x => x.Name == "浙江大学");
                //    if (useCompany == null)
                //    {
                //        db.UseCompany.Add(new UseCompany()
                //        {
                //            UseCompanyId = useCompanyId,
                //            Contact = contant,
                //            Phone = "13800001111",
                //            Name = "浙江大学",
                //            Position = "浙江省|杭州市",
                //            Label = "学校|厨房",
                //            CreateTime = DateTime.Now
                //        });
                //    }
                //    else
                //    {
                //        useCompanyId = useCompany.UseCompanyId;
                //    }
                //    //添加空设备（二维码）
                //    var useCompanyEquipnet =
                //       db.Equipment.FirstOrDefault(
                //           x => x.UseCompanyId == useCompanyId && x.EquipmentId == equId);
                //    var useCompanyEqu = new Equipment()
                //    {
                //        EquipmentId = equId,
                //        UseCompanyId = useCompanyId,
                //        CreateTime = DateTime.Now
                //    };
                //    if (useCompanyEquipnet == null)
                //    {
                //        useCompanyEqu.UseCompanyId = useCompanyId;
                //    }
                //    db.Equipment.Add(useCompanyEqu);
                    
                //    var a = db.SaveChanges();
                //}
            }
        }


    }
}
