using LZY.BX.Model;
using Repair.Web.Site.Utilities;
using System;
using System.Collections.Generic;

namespace Repair.Web.Site.Areas.User.Models
{
    public class BindingServiceQueryModel : QueryListModelBase
    {
        public List<ServiceBinding> Data { get; set; }

        //获取签约单位
        public List<ServiceCompany> JoinCompanyDic { get; set; }

       //获取类目列表
       public List<Category> ListCategory { get; set; }
        //获取区域列表
        public Dictionary<long,Area> AreaList { get; set; }
        //获取已经绑定设备列表
        public Dictionary<long,Device> ListDeviceDic { get; set; }
        //获取已经绑定厂商列表
        public Dictionary<long,Brand> BrandList { get; set; }
        public Dictionary<long, Manufacturer> ManufacturerList { get; set; }
        
        //获取已经绑定的类目列表
        public Dictionary<long, Category> CategoryData { get; set; }

    }
}
