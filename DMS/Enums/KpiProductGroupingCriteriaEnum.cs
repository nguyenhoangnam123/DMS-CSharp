using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Enums
{
    public class KpiProductGroupingCriteriaEnum
    {
        public static GenericEnum INDIRECT_REVENUE = new GenericEnum { Id = 2, Code = "IndirectRevenue", Name = "Doanh thu theo đơn hàng gián tiếp" };
        public static GenericEnum INDIRECT_STORE = new GenericEnum { Id = 4, Code = "IndirectStore", Name = "Số đại lý theo đơn gián tiếp" };
        public static List<GenericEnum> KpiProductGroupingCriteriaEnumList = new List<GenericEnum>
        {
            INDIRECT_REVENUE, INDIRECT_STORE
        };
    }
}
