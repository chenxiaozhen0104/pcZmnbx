using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Repair.Web.Site.Areas.Sp.Models
{
    public class RepairCountReportModel
    {
        public virtual int Time { get; set; }

        public virtual int Count { get; set; }
    }
}