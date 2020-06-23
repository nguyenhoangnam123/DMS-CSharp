using Common;
using System.Collections.Generic;

namespace DMS.Enums
{
    public class KpiCriteriaItemEnum
    {
        public static GenericEnum INDIRECT_OUTPUT_OF_KEY_ITEM = new GenericEnum { Id = 1, Code = "IndirectOutputOfKeyItem", Name = "Sản lượng theo đơn gián tiếp" };
        public static GenericEnum INDIRECT_SALES_OF_KEY_ITEM = new GenericEnum { Id = 2, Code = "IndirectSalesOfKeyItem", Name = "Doanh số theo đơn gián tiếp" };
        public static GenericEnum INDIRECT_ORDERS_OF_KEY_ITEM = new GenericEnum { Id = 3, Code = "IndirectOrdersOfKeyItem", Name = "Số đơn hàng theo đơn gián tiếp" };
        public static GenericEnum INDIRECT_STORES_OF_KEY_ITEM = new GenericEnum { Id = 4, Code = "IndirectStoresOfKeyItem", Name = "Số khách hàng theo đơn gián tiếp" };
        public static GenericEnum DIRECT_OUTPUT_OF_KEY_ITEM = new GenericEnum { Id = 5, Code = "DirectOutputOfKeyItem", Name = "Sản lượng theo đơn trực tiếp" };
        public static GenericEnum DIRECT_SALES_OF_KEY_ITEM = new GenericEnum { Id = 6, Code = "DirectSalesOfKeyItem", Name = "Doanh số theo đơn trực tiếp" };
        public static GenericEnum DIRECT_ORDERS_OF_KEY_ITEM = new GenericEnum { Id = 7, Code = "DirectOrdersOfKeyItem", Name = "Số đơn hàng theo đơn trực tiếp" };
        public static GenericEnum DIRECT_STORES_OF_KEY_ITEM = new GenericEnum { Id = 8, Code = "DirectStoresOfKeyItem", Name = "Số khách hàng theo đơn trực tiếp" };

        public static List<GenericEnum> KpiCriteriaItemEnumList = new List<GenericEnum>()
        {
            INDIRECT_OUTPUT_OF_KEY_ITEM, INDIRECT_SALES_OF_KEY_ITEM, INDIRECT_ORDERS_OF_KEY_ITEM, INDIRECT_STORES_OF_KEY_ITEM, DIRECT_OUTPUT_OF_KEY_ITEM, DIRECT_SALES_OF_KEY_ITEM,
            DIRECT_ORDERS_OF_KEY_ITEM, DIRECT_STORES_OF_KEY_ITEM
        };
    }
}
