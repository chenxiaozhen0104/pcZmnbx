using LZY.BX.Model;
using LZY.BX.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Repair.Api.Areas.Api.Models
{
    public class QeuryUserAll
    {
        public virtual User User { get; set; }

        public virtual UseCompanyUser UseCompanyUser { get; set; }

        public virtual UseCompany UseCompany { get; set; }

        public virtual ServiceCompanyUser ServiceCompanyUser { get; set; }

        public virtual ServiceCompany ServiceCompany { get; set; }
    }
}