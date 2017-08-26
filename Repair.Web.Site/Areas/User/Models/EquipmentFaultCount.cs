using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Repair.Web.Site.Areas.User.Models
{
    public class EquipmentFaultCount
    {
        public virtual long EquipmentId { get; set; }

        public virtual int Count { get; set; }
    }
}