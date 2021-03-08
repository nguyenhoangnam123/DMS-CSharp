using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Enums
{
    public class StoreApprovalStateEnum
    {
        public static GenericEnum PENDING = new GenericEnum { Id = 1, Code = "PENDING", Name = "Chờ duyệt" };
        public static GenericEnum APPROVED = new GenericEnum { Id = 1, Code = "APPROVED", Name = "Hoàn thành" };
        public static GenericEnum REJECTED = new GenericEnum { Id = 1, Code = "REJECTED", Name = "Từ chối" };


        public static List<GenericEnum> StoreApprovalStateEnumList = new List<GenericEnum> {
            PENDING, APPROVED, REJECTED
        };
    }
}
