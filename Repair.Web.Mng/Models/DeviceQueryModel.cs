using LZY.BX.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Repair.Web.Mng.Models
{
    /// <summary>
    /// 设备查询类
    /// </summary>
    public class DeviceQueryModel : QueryListModelBase
    {
        public DeviceQueryModel()
        {
            PageInfo = new PageInfoByNum
            {
                PageSize = 50
            };
        }
        /// <summary>
        /// 二维码
        /// </summary>
        public string QRCode { get; set; }

        /// <summary>
        /// 资产
        /// </summary>
        public string AssetsId { get; set; }

        /// <summary>
        /// 报修单位
        /// </summary>
        public long? UseCompanyId { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string NameLike { get; set; }

        /// <summary>
        /// 设备型号
        /// </summary>
        public string ModelLike { get; set; }

        /// <summary>
        /// 隶属品牌
        /// </summary>
        public long? BrandId { get; set; }

        /// <summary>
        /// 隶属区域
        /// </summary>
        public long? AreaId { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public List<Device> Data { get; set; }
    }
}