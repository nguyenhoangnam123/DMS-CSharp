using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.dashboards.director
{
    public class DashboardDirector_StatisticDailyDTO : DataDTO
    {
        public decimal Revenue { get; set; }
        public long IndirectSalesOrderCounter { get; set; }
        public long StoreHasCheckedCounter { get; set; }
        public long StoreCheckingCounter { get; set; }
    }
}
