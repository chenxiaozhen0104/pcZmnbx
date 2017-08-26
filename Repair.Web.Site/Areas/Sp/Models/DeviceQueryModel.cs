using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Repair.Web.Site.Areas.Sp.Models
{
    public class DeviceQueryModel: QueryListModelBase
    {
        public long? DeviceIdLike { get; set; }

        public string NameLike { get; set; }

        public System.Collections.Generic.List<LZY.BX.Model.Device> Data { get; set; }
        

        public System.Collections.Generic.List<System.Web.Mvc.SelectListItem> ManufactorerList { get; set; }

        public System.Collections.Generic.List<System.Web.Mvc.SelectListItem> BrandList { get; set; }

        public System.Collections.Generic.List<System.Web.Mvc.SelectListItem> ProductList { get; set; }

        public System.Collections.Generic.Dictionary<long, LZY.BX.Model.Manufacturer> ManufactorerDic { get; set; }

        public System.Collections.Generic.Dictionary<long, LZY.BX.Model.Brand> BrandDic { get; set; }

        public System.Collections.Generic.Dictionary<long, LZY.BX.Model.Category> CategoryDic { get; set; }
    }
}