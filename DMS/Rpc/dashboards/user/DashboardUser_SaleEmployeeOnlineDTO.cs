using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.dashboards.user
{
    public class DashboardUser_SaleEmployeeOnlineDTO : DataDTO
    {
        public long Sum => SaleEmployeeOnlineHours.Sum(x => x.Counter);
        public List<DashboardUser_SaleEmployeeOnlineHourDTO> SaleEmployeeOnlineHours { get; set; }
    }

    public class DashboardUser_SaleEmployeeOnlineHourDTO : DataDTO
    {
        public string Hour { get; set; }
        public long Counter { get; set; }
    }
}
