using System;

namespace Repair.Web.Mng.Models
{
    public class CondTimeRange
    {
        public CondTimeRange(string name)
            : this(name, 7)
        {
            Range = 365;
        }

        public CondTimeRange(string name, int range)
        {
            Name = name;
            TimeEnd = DateTime.Now.Date;
            TimeStart = TimeEnd.AddDays(-range);
        }

        public string Name { get; set; }

        public DateTime TimeStart { get; set; }

        public DateTime TimeEnd { get; set; }

        public int Range { get; set; }
    }
}