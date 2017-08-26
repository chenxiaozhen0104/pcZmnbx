using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Repair.Web.Site.Areas.User.Models
{
    /// <summary>
    /// 工作响应时间
    /// </summary>
    public class WorkIngReportModel
    {
        /// <summary>
        /// 时间
        /// </summary>
        public int Time { get; set; }
        
        /// <summary>
        /// 服务单位
        /// </summary>
        public string ServiceCompanyName { get; set; }

        /// <summary>
        /// 平均时间
        /// </summary>
        public string AvgTime { get; set; }

        /// <summary>
        /// 总数
        /// </summary>
        public int Count { get; set; }
    }
}