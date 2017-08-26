using System;
using System.Collections.Generic;
using LZY.BX.Model;

namespace Repair.Web.Site.Areas.Sp.Models
{
    public class ConMngListQueryModel : QueryListModelBase
    {
        public string NameLike { get; set; }

        public List<JoinCompany> Data { get; set; }

        public Dictionary<string, UseCompany> UseCompanyDic { get; set; }
    }
}
