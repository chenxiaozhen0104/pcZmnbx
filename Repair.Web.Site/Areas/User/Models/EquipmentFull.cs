using LZY.BX.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Repair.Web.Site.Areas.User.Models
{
    public class EquipmentFull
    {
        public virtual int Count { get; set; }

        public virtual Manufacturer Manufacturer { get; set; }

        public virtual Brand Brand { get; set; }
    }
}