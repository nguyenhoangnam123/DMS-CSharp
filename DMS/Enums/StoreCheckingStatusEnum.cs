using Common;
using System.Collections.Generic;

namespace DMS.Enums
{
    public class StoreCheckingStatusEnum
    {
        public static GenericEnum CHECKED = new GenericEnum { Id = 1, Code = "CHECKED", Name = "Đã viếng thăm" };
        public static GenericEnum NOTCHECKED = new GenericEnum { Id = 0, Code = "NOT_CHECKED", Name = "Chưa viếng thăm" };

        public static List<GenericEnum> StoreCheckingStatusEnumList = new List<GenericEnum>()
        {
            CHECKED, NOTCHECKED
        };
    }
}
