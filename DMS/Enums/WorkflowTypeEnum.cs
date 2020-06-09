using Common;
using System.Collections.Generic;

namespace DMS.Enums
{
    public class WorkflowTypeEnum
    {
        public static GenericEnum STORE = new GenericEnum { Id = 1, Code = "STORE", Name = "Cửa hàng" };
        public static GenericEnum PRODUCT = new GenericEnum { Id = 2, Code = "PRODUCT", Name = "Sản phẩm" };
        public static GenericEnum ROUTE = new GenericEnum { Id = 3, Code = "ROUTE", Name = "Tuyến" };
        public static GenericEnum ORDER = new GenericEnum { Id = 4, Code = "ORDER", Name = "Đơn hàng" };
        public static List<GenericEnum> WorkflowTypeEnumList = new List<GenericEnum>()
        {
            STORE, PRODUCT, ROUTE, ORDER
        };
    }
}
