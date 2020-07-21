using Common;
using System.Collections.Generic;

namespace DMS.Enums
{
    public class KpiCriteriaGeneralEnum
    {
        public static GenericEnum TOTAL_INDIRECT_SALES_ORDER = new GenericEnum { Id = 1, Code = "TotalIndirectOrders", Name = "Số đơn hàng gián tiếp" };
        public static GenericEnum TOTAL_INDIRECT_SALES_OUTPUT = new GenericEnum { Id = 2, Code = "TotalIndirectOutput", Name = "Tổng sản lượng đơn hàng gián tiếp" };
        public static GenericEnum TOTAL_INDIRECT_SALES_AMOUNT = new GenericEnum { Id = 3, Code = "TotalIndirectSalesAmount", Name = "Doanh thu đơn hàng gián tiếp" };
        public static GenericEnum SKU_INDIRECT_SALES_ORDER = new GenericEnum { Id = 4, Code = "SKUIndirectOrder", Name = "SKU/ Đơn hàng gián tiếp" };
        public static GenericEnum STORE_VISITED = new GenericEnum { Id = 5, Code = "StoresVisited", Name = "Số đại lý viếng thăm" };
        public static GenericEnum NEW_STORE_CREATED = new GenericEnum { Id = 6, Code = "NewStoresCreated", Name = "Số đại lý tạo mới" };
        public static GenericEnum NUMBER_OF_STORE_VISIT = new GenericEnum { Id = 7, Code = "NumberOfStoreVisits", Name = "Số lần viếng thăm đại lý" };

        public static List<GenericEnum> KpiCriteriaGeneralEnumList = new List<GenericEnum>()
        {
            TOTAL_INDIRECT_SALES_ORDER, TOTAL_INDIRECT_SALES_OUTPUT, TOTAL_INDIRECT_SALES_AMOUNT, SKU_INDIRECT_SALES_ORDER, STORE_VISITED, NEW_STORE_CREATED, NUMBER_OF_STORE_VISIT,
        };
    }
}
