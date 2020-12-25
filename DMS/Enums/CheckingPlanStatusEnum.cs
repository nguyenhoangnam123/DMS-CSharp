using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Enums
{
    public class CheckingPlanStatusEnum
    {
        public static GenericEnum UNPLANNED = new GenericEnum { Id = 2, Code = "UNPLANNED", Name = "Ngoài tuyến" };
        public static GenericEnum PLANNED = new GenericEnum { Id = 1, Code = "PLANNED", Name = "Trong tuyến" };
        public static GenericEnum ALL = new GenericEnum { Id = 0, Code = "ALL", Name = "Tất cả" };
        public static List<GenericEnum> CheckingPlanStatusEnumList = new List<GenericEnum>()
        {
            ALL, PLANNED, UNPLANNED
        };
    }
}
