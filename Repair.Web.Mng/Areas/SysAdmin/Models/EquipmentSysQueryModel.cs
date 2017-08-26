using LZY.BX.Model;
using Repair.Web.Mng.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Repair.Web.Areas.SysAdmin.Models
{
    public class EquipmentSysQueryModel : QueryListModelBase
    {
        public List<Equipment> Data { get; set; }

        public Dictionary<long, Brand> BrandDic { get; set; }

        public Dictionary<long, Product> ProductDic { get; set; }

        public Dictionary<long, UseCompany> UseCompanyDic { get; set; }

        public Dictionary<long, ServiceEquipment> ServiceEquipmentDic { get; set; }

        public Dictionary<long, ServiceCompany> ServiceCompanyDic { get; set; }

        public string NameLike { get; set; }
    }
}