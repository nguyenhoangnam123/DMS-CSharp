using Common;
using System.Collections.Generic;

namespace DMS.Enums
{
    public class StoreCheckingStatusEnum
    {
        public static GenericEnum CHECKEDIN = new GenericEnum { Id = 1, Code = "CHECKED_IN", Name = "Đã viếng thăm" };
        public static GenericEnum NOTCHECKED = new GenericEnum { Id = 0, Code = "NOT_CHECKED", Name = "Chưa viếng thăm" };

        public static List<GenericEnum> StoreCheckingStatusEnumList = new List<GenericEnum>()
        {
            CHECKEDIN, NOTCHECKED
        };
    }
}
