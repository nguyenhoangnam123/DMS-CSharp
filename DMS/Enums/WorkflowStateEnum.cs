using Common;
using System.Collections.Generic;

namespace DMS.Enums
{
    public class WorkflowStateEnum
    {
        public static GenericEnum NEW = new GenericEnum { Id = 1, Code = "NEW", Name = "Mới" };
        public static GenericEnum PENDING = new GenericEnum { Id = 2, Code = "PENDING", Name = "Chờ duyệt" };
        public static GenericEnum APPROVED = new GenericEnum { Id = 3, Code = "APPROVED", Name = "Đã duyệt" };
        public static GenericEnum REJECTED = new GenericEnum { Id = 4, Code = "REJECTED", Name = "Đã từ chối" };
        public static List<GenericEnum> WorkflowStateEnumList = new List<GenericEnum>()
        {
            NEW, PENDING, APPROVED, REJECTED
        };
    }
}
