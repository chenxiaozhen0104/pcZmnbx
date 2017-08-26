using System;
using System.Collections.Generic;
using LZY.BX.Model;
using LZY.BX.Model.Enum;

namespace Repair.Web.Site.Areas.Sp.Models
{
    public class OrderQueryModel : QueryListModelBase
    {
        public string ServiceCompanyId { get; set; }

        public string RepairOrderId { get; set; }

        public OrderState State { get; set; }

        public DateTime CreateTimeLess { get; set; }

        public DateTime CreateTimeGreaterEqual { get; set; }

        //public List<ServiceCompanyRepairOrder> Data { get; set; }

        //public Dictionary<long, RepairOrder> RepairOrderDic { get; set; }

        public Dictionary<long, Device> DeviceDic { get; set; }
        /// <summary>
        /// 使用单位信息
        /// </summary>
        public Dictionary<string, UseCompany> UseCompanyDic { get; set; }
        //public Dictionary<long, RepairBooking> RepairBookingDic { get; set; }
        /// <summary>
        /// 单子列表
        /// </summary>
        public List<MainOrder> MainOrderData { get; set; }
        /// <summary>
        /// 单子中使用单位人员
        /// </summary>
        public Dictionary<string,LZY.BX.Model.User> UserDic { get; set; }
        /// <summary>
        /// 单子中维修单位人员
        /// </summary>
        public Dictionary<string, LZY.BX.Model.User> ServeUserDic { get; set; }
        //当前登陆人的权限
        public UserType RoleKey { get; set; }
    }
}
