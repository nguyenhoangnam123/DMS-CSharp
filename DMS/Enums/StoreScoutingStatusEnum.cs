using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Enums
{
    public class StoreScoutingStatusEnum
    {
        public static GenericEnum ACTIVE = new GenericEnum { Id = 1, Code = "ACTIVE", Name = "Đã mở" };
        public static GenericEnum INACTIVE = new GenericEnum { Id = 0, Code = "INACTIVE", Name = "Chưa mở" };
        public static List<GenericEnum> StoreScoutingStatusEnumList = new List<GenericEnum>()
        {
            ACTIVE, INACTIVE
        };
    }
}
