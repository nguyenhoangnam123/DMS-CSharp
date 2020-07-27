using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.dashboards.user
{
    public class DashboardUser_TopSaleEmployeeStoreCheckingDTO : DataDTO
    {
        public long SaleEmployeeId { get; set; }
        public string DisplayName { get; set; }
        public long Counter { get; set; }
    }

    public class DashboardUser_TopSaleEmployeeStoreCheckingFilterDTO : FilterDTO
    {
        public IdFilter Time { get; set; }
    }
}
