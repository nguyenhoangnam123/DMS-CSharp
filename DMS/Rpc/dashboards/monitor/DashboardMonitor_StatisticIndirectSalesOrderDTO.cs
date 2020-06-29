using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.dashboards.monitor
{
    public class DashboardMonitor_StatisticIndirectSalesOrderDTO : DataDTO
    {
        public long Sum => StatisticIndirectSalesOrderHours.Sum(x => x.Counter);
        public List<Dashboard_StatisticIndirectSalesOrderHourDTO> StatisticIndirectSalesOrderHours { get; set; }
    }

    public class Dashboard_StatisticIndirectSalesOrderHourDTO : DataDTO
    {
        public string Hour { get; set; }
        public long Counter { get; set; }
    }
}
