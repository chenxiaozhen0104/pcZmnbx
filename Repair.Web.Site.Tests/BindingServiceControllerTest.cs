using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LZY.BX.Service;
using System.Collections.Generic;
using LZY.BX.Model.Enum;
using System.Linq;

namespace Repair.Web.Site.Tests
{
    [TestClass]
    public class BindingServiceControllerTest
    {
        BindingService bs = new BindingService();
        /// <summary>
        /// 类目绑定
        /// </summary>
        [TestMethod]
        public void Can_Binding_Category()
        {
            List<long> categoryArr = new List<long>() { 1, 4 };
            BindingServiceType bindingType = BindingServiceType.Category;
            var serviceCompanyId = "636065437685331833";
            var userCompanyId = "636065437685331833";
            bs.Binding(categoryArr, bindingType, userCompanyId, serviceCompanyId);
        }
        /// <summary>
        /// 设备列表
        /// </summary>
        [TestMethod]
        public void Equipment_Test()
        {
            var UseCompanyId = "636065437685331833";
            var Data = bs.GetBindingServiceList(UseCompanyId).Where(t => t.BindingType == BindingServiceType.Equipment).ToList();

        }
    }
}
