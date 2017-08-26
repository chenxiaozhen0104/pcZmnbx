using System;

namespace Repair.Web.Site.Areas.User.Models
{
    public class EquipmentReportQueryModel : QueryListModelBase
    {
        public EquipmentReportQueryModel()
        {
            base.PageInfo.PageSize = 20;
        }
        public System.Collections.Generic.List<EquipmentFull> Data { get; set; }
    }

    public class DeviceReportQueryModel : QueryListModelBase {
       

    }
}
