using System;
using System.Collections.Generic;

namespace DMS.DWModels
{
    public partial class Dim_DateDAO
    {
        public long DateKey { get; set; }
        public DateTime Date { get; set; }
        public long Day { get; set; }
        public long Weekday { get; set; }
        public string WeekDayName { get; set; }
        public long DayOfYear { get; set; }
        public long Month { get; set; }
        public string MonthName { get; set; }
        public long Quarter { get; set; }
        public string QuarterName { get; set; }
        public long Year { get; set; }
        public long YYYYMM { get; set; }
    }
}
