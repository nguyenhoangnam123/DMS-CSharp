using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.dashboards.monitor
{
    public class DashboardMonitor_TopSaleEmployeeStoreCheckingDTO : DataDTO
    {
        public long SaleEmployeeId { get; set; }
        public string DisplayName { get; set; }
        public long Counter { get; set; }
    }

    public class DashboardMonitor_TopSaleEmployeeStoreCheckingFilterDTO : FilterDTO
    {
        public IdFilter Time { get; set; }
    }
}
