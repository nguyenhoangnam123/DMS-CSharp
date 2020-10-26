using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Enums
{
    public class NotificationStatusEnum
    {
        public static GenericEnum UNSEND = new GenericEnum { Id = 0, Code = "UNSEND", Name = "Chưa gửi" };
        public static GenericEnum SENT = new GenericEnum { Id = 1, Code = "SENT", Name = "Đã gửi" };
        public static List<GenericEnum> NotificationStatusEnumList = new List<GenericEnum>()
        {
            UNSEND, SENT
        };
    }
}
