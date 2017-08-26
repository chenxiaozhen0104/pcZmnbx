using LZY.BX.Model;
using LZY.BX.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Repair.Web.Site2._0.Models
{
    public class OrderQueryModel : QueryListModelBase
    {
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
        public Dictionary<string, User> UserDic { get; set; }
        /// <summary>
        /// 单子中维修单位人员
        /// </summary>
        public Dictionary<string, User> ServeUserDic { get; set; }

        public string keywords { get; set; }

        public OrderState State { get; set; }
        //设备图片
        public List<Picture> PicList { get; set; }
    }
}