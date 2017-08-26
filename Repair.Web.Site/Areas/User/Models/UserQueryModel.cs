using LZY.BX.Model.Enum;

namespace Repair.Web.Site.Areas.User.Models
{
    public class UserQueryModel:QueryListModelBase
    {
        public long? UserId { get; set; }
        
        public string RealNameLike { get; set; }

        public string Phone { get; set; }

        public System.Collections.Generic.List<LZY.BX.Model.User> Data { get; set; }

       
    }
}
