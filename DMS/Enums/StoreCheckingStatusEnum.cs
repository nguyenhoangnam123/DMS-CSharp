using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Enums
{
    public class StoreCheckingStatusEnum
    {
        public static GenericEnum CHECKEDIN = new GenericEnum { Id = 1, Code = "CHECKED_IN", Name = "Đã viếng thăm" };
        public static GenericEnum NOTCHECKED = new GenericEnum { Id = 0, Code = "NOT_CHECKED", Name = "Chưa viếng thăm" };
    }
}
