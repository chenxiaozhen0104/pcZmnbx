using LZY.BX.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LZY.BX.Model.Enum;

namespace Repair.Web.Mng.Models
{
    public class AuditCompanyQueryModel : QueryListModelBase
    {
        public List<CompanySginLog> Data { get; set; }

        public Dictionary<long, User> UserDic { get; set; }

        public CompanyState State { get; set; }

        public DateTime CreateTimeLess { get; set; }

        public DateTime CreateTimeGreaterEqual { get; set; }

        //使用申请单位集合
        public List<UseCompany> UserCompanyList { get; set; }

        //服务申请单位集合
        public List<ServiceCompany> ServiceCompanyList { get; set; }
    }
}