using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;

namespace Repair.Web.Site
{
    public static class Component
    {
        

        /// <summary>
        /// 时间显示
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="span"></param>
        /// <returns></returns>
        public static MvcHtmlString TimeSpanTran(this HtmlHelper helper, TimeSpan span)
        {
            string timeStr;

            if (span.TotalDays > 1) //1天
            {
                timeStr = string.Format("{0}天{1}小时{2}分", span.Days, span.Hours, span.Minutes);
            }
            else if (span.TotalMinutes > 60) //1小时
            {
                timeStr = string.Format("{0}小时{1}分", span.Hours, span.Minutes);
            }
            else
            {
                timeStr = string.Format("{0}分", span.Minutes);
            }
            return new MvcHtmlString(timeStr);
        }

        public static IEnumerable<SelectListItem> ToSelectList(this Enum enumValue, IList values)
        {
            return from Enum e in Enum.GetValues(enumValue.GetType())
                select new SelectListItem
                {
                    Selected = values == null ? false : values.Contains(e),
                    Text = e.ToDescription(),
                    Value = e.ToString()
                };
        }


        public static IEnumerable<SelectListItem> ToSelectList(this Enum enumValue)
        {
            return from Enum e in Enum.GetValues(enumValue.GetType())
                select new SelectListItem
                {
                    Selected = e.Equals(enumValue),
                    Text = e.ToDescription(),
                    Value = e.ToString()
                };
        }

        public static string ToDescription(this Enum value)
        {
            var attributes = (DescriptionAttribute[])value.GetType().GetField(
                value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }


    }
}