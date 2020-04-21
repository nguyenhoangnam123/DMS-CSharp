using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Enums
{
    public class RequestStateEnum
    {
        public static GenericEnum NEW = new GenericEnum { Id = 1, Code = "NEW", Name = "Mới" };
        public static GenericEnum APPROVING = new GenericEnum { Id = 2, Code = "APPROVING", Name = "Đang duyệt" };
        public static GenericEnum APPROVED = new GenericEnum { Id = 3, Code = "APPROVED", Name = "Đã duyệt" };
        public static GenericEnum REJECTED = new GenericEnum { Id = 4, Code = "REJECTED", Name = "Đã từ chối" };
    }
}
