using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.dashboards.monitor
{
    public class DashboardMonitor_StoreCheckingDTO : DataDTO
    {
        public long Sum => StoreCheckingHours.Sum(x => x.Counter);
        public List<DashboardMonitor_StoreCheckingHourDTO> StoreCheckingHours { get; set; }
    }

    public class DashboardMonitor_StoreCheckingHourDTO : DataDTO
    {
        public string Hour { get; set; }
        public long Counter { get; set; }
    }
}
