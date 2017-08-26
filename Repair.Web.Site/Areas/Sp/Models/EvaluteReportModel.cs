using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Repair.Web.Site.Areas.Sp.Models
{
    public class EvaluteReportModel
    {
        public virtual string RealName { get; set; }

        public virtual string Level { get; set; }

        public virtual int Count { get; set; }
    }
}