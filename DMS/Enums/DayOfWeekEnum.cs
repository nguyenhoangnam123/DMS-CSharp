using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Enums
{
    public class DayOfWeekEnum
    {
        public static GenericEnum MONDAY = new GenericEnum { Id = 1, Code = "Monday", Name = "Thứ 2" };
        public static GenericEnum TUESDAY = new GenericEnum { Id = 2, Code = "Tuesday", Name = "Thứ 2" };
        public static GenericEnum WEDNESDAY = new GenericEnum { Id = 3, Code = "Uwednesday", Name = "Thứ 4" };
        public static GenericEnum THURSDAY = new GenericEnum { Id = 4, Code = "Thursday", Name = "Thứ 5" };
        public static GenericEnum FRIDAY = new GenericEnum { Id = 5, Code = "Friday", Name = "Thứ 6" };
        public static GenericEnum SATURDAY = new GenericEnum { Id = 6, Code = "Saturday", Name = "Thứ 7" };
        public static GenericEnum SUNDAY = new GenericEnum { Id = 7, Code = "Sunday", Name = "Chủ nhật" };
        public static List<GenericEnum> DayOfWeekEnumList = new List<GenericEnum>
        {
            MONDAY,TUESDAY,WEDNESDAY,THURSDAY,FRIDAY,SATURDAY,SUNDAY
        };
    }
}
