using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.dashboards.monitor
{
    public class DashboardMonitor_StoreCheckingImageMappingDTO : DataDTO
    {
        public long Sum => StoreCheckingImageMappingHours.Sum(x => x.Counter);
        public List<DashboardMonitor_StoreCheckingImageMappingHourDTO> StoreCheckingImageMappingHours { get; set; }
    }

    public class DashboardMonitor_StoreCheckingImageMappingHourDTO : DataDTO
    {
        public string Hour { get; set; }
        public long Counter { get; set; }
    }
}
