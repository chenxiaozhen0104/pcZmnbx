using LZY.BX.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Repair.Web.Site.Areas.User.Models
{
    public class ReportDeviceQueryModel : QueryListModelBase
    {
        public ReportDeviceQueryModel()
        {
            base.PageInfo.PageSize = 20;
        }

        public List<Device> deviceData { get; set; }

        public Dictionary<long?,string> deviceCount { get; set; }
    }

   
}