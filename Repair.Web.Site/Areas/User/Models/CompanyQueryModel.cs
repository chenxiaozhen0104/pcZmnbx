using LZY.BX.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Repair.Web.Site.Areas.User.Models
{
    public class CompanyQueryModel: QueryListModelBase
    {
        //公司名称(使用和服务)
        public string Name { get; set; }
        /// <summary>
        /// 使用单位列表
        /// </summary>
        public List<UseCompany> UserCompanyList{get;set;}
        /// <summary>
        /// 服务单位列表
        /// </summary>
        public List<ServiceCompany> ServiceCompanyList { get; set; }

        
    }
}