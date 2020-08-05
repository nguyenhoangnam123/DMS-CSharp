using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Enums
{
    public class WorkflowTypeEnum
    {
        public static GenericEnum STORE = new GenericEnum { Id = 1, Code = "STORE", Name = "Đại lý" };
        public static GenericEnum PRODUCT = new GenericEnum { Id = 2, Code = "PRODUCT", Name = "Sản phẩm" };
        public static GenericEnum EROUTE = new GenericEnum { Id = 3, Code = "EROUTE", Name = "Tuyến" };
        public static GenericEnum ORDER = new GenericEnum { Id = 4, Code = "ORDER", Name = "Đơn hàng" };
        public static List<GenericEnum> WorkflowTypeEnumList = new List<GenericEnum>()
        {
            STORE, PRODUCT, EROUTE, ORDER
        };
    }

    
}
