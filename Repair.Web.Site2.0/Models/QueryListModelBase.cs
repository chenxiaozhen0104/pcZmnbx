using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Repair.Web.Site2._0.Models
{
    public class QueryListModelBase
    {
        public QueryListModelBase()
        {
            PageInfo = new PageInfoByNum
            {
                PageSize = 10
            };
        }

        public PageInfoByNum PageInfo { get; set; }

    }
}