using LZY.BX.Model.Enum;
using System.Collections.Generic;

namespace Repair.Web.Site.Areas.Sp.Models
{
    public class UserQueryModel : QueryListModelBase
    {
        public long? UserId { get; set; }

        public string RealNameLike { get; set; }

        public string Phone { get; set; }

        public System.Collections.Generic.List<LZY.BX.Model.User> Data { get; set; }
       
    }
}