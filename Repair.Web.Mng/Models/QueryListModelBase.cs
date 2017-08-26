using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Repair.Web.Mng.Models
{
    public class QueryListModelBase
    {
        public QueryListModelBase()
        {
            PageInfo = new PageInfoByNum
            {
                PageSize = 20
            };
        }

        public PageInfoByNum PageInfo { get; set; }
    }
}