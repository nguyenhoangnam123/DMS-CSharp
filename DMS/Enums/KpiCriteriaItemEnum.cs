using Common;
using System.Collections.Generic;

namespace DMS.Enums
{
    public class KpiCriteriaItemEnum
    {
        public static GenericEnum INDIRECT_QUANTITY = new GenericEnum { Id = 1, Code = "IndirectQuantity", Name = "Sản lượng theo đơn hàng gián tiếp" };
        public static GenericEnum INDIRECT_REVENUE = new GenericEnum { Id = 2, Code = "IndirectRevenue", Name = "Doanh thu theo đơn hàng gián tiếp" };
        public static GenericEnum INDIRECT_AMOUNT = new GenericEnum { Id = 3, Code = "IndirectAmount", Name = "Số đơn hàng gián tiếp" };
        public static GenericEnum INDIRECT_STORE = new GenericEnum { Id = 4, Code = "IndirectStore", Name = "Số đại lý theo đơn gián tiếp" };

        public static List<GenericEnum> KpiCriteriaItemEnumList = new List<GenericEnum>()
        {
            INDIRECT_QUANTITY, INDIRECT_REVENUE, INDIRECT_AMOUNT, INDIRECT_STORE
        };
    }
}
