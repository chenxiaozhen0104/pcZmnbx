using LZY.BX.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Repair.Web.Site.Areas.Sp.Models
{
    public class DispatchOrderQueryModel
    {
        public long RepairOrderId { get; set; }

        public List<LZY.BX.Model.User> UserList { get; set; }
    }
}