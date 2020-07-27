using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.dashboards.user
{
    public class DashboardUser_StoreCheckingDTO : DataDTO
    {
        public long Sum => StoreCheckingHours.Sum(x => x.Counter);
        public List<DashboardUser_StoreCheckingHourDTO> StoreCheckingHours { get; set; }
    }

    public class DashboardUser_StoreCheckingHourDTO : DataDTO
    {
        public string Hour { get; set; }
        public long Counter { get; set; }
    }
}
