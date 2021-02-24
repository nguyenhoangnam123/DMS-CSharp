using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Enums
{
    public class EntityTypeEnum
    {
        public static GenericEnum SALES_ORDER = new GenericEnum { Id = 1, Code = "SalesOrder", Name = "Đơn hàng" };
        public static GenericEnum STORE = new GenericEnum { Id = 3, Code = "Store", Name = "Đại lý" };

        public static List<GenericEnum> EntityTypeEnumList = new List<GenericEnum>
        {
            SALES_ORDER, STORE
        };
    }
}
