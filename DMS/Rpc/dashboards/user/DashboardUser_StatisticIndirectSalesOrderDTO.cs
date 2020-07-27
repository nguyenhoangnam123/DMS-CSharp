using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.dashboards.user
{
    public class DashboardUser_StatisticIndirectSalesOrderDTO : DataDTO
    {
        public long Sum => StatisticIndirectSalesOrderHours.Sum(x => x.Counter);
        public List<DashboardUser_StatisticIndirectSalesOrderHourDTO> StatisticIndirectSalesOrderHours { get; set; }
    }

    public class DashboardUser_StatisticIndirectSalesOrderHourDTO : DataDTO
    {
        public string Hour { get; set; }
        public long Counter { get; set; }
    }
}
