using LZY.BX.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Repair.Web.Site2._0.Models
{
    public class UserQueryModel: QueryListModelBase
    {
        public long? UserId { get; set; }

        public string RealNameLike { get; set; }

        public string Phone { get; set; }

        public List<User> Data { get; set; }
    }
}