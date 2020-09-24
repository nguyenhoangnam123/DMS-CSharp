using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Enums
{
    public class PromotionTypeEnum
    {
        public static GenericEnum ALL = new GenericEnum { Id = 1, Code = "ALL", Name = "Tất cả đại lý" };
        public static GenericEnum STORE_GROUPING = new GenericEnum { Id = 2, Code = "STORE_GROUPING", Name = "Theo nhóm" };
        public static GenericEnum STORE_TYPE = new GenericEnum { Id = 3, Code = "STORE_TYPE", Name = "Theo loại" };
        public static GenericEnum STORE = new GenericEnum { Id = 4, Code = "STORE", Name = "Chi tiết" };

        public static List<GenericEnum> PromotionTypeEnumList = new List<GenericEnum>()
        {
            ALL, STORE_GROUPING, STORE_TYPE, STORE
        };
    }
}
