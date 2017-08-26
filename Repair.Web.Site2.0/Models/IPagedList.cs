using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Repair.Web.Site2._0.Models
{
    /// <summary>
    /// 分页列表
    /// </summary>
    public interface IPagedList
    {
        /// <summary>
        /// 总页数
        /// </summary>
        int TotalCount { get; set; }

        /// <summary>
        /// 当前页
        /// </summary>
        int Page { get; set; }

        /// <summary>
        /// 当前页
        /// </summary>
        int PageSize { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        int TotalPage { get; }

        /// <summary>
        /// 表单名称
        /// </summary>
        string FormName { get; set; }

        /// <summary>
        /// 当前页数
        /// </summary>
        string CurPageName { get; set; }

    }


    /// <summary>
    /// 按条数分页
    /// </summary>
    public class PageInfoByNum : IPagedList
    {
        public PageInfoByNum()
        {
            Page = 1;
            PageSize = 100;
            PageRangeSize = 10;
            FormName = "editFrm";
            CurPageName = "PAGE_CUR";
        }

        /// <summary>
        /// 页尺寸
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 显示翻页的页码数量
        /// </summary>
        public int PageRangeSize { get; set; }

        /// <summary>
        /// 当前页码
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// 总的数据量
        /// </summary>
        public int TotalCount { get; set; }



        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPage
        {
            get
            {
                //根据 TotalCount PageSize算总页数， 第一页为 1
                return (int)Math.Ceiling(((double)TotalCount) / PageSize);
            }
        }

        /// <summary>
        /// 起始记录数
        /// </summary>
        public int RecIndex { get { return (Page - 1) * PageSize; } }

        /// <summary>
        /// 表单名称
        /// </summary>
        public string FormName { get; set; }

        /// <summary>
        /// 当前页名称
        /// </summary>
        public string CurPageName { get; set; }

        /// <summary>
        /// 数据隐藏域输出标记
        /// </summary>
        public bool Mark { get; set; }

        /// <summary>
        /// 输出隐藏字段
        /// </summary>
        public bool OutputField { get; set; }
    }
}