using LZY.BX.Model;
using System;
using System.Collections.Generic;

namespace Repair.Web.Site.Areas.User.Models
{
    public class CopMngListQueryModel : QueryListModelBase
    {
        public string NameLike { get; set; }

      
        public System.Collections.Generic.List<JoinCompany> Data { get; set; }

        public Dictionary<string, ServiceCompany> ServiceCompanyDic { get; set; }
    }
}
