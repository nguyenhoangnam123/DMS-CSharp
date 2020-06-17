using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.dashboards
{
    public class Dashboard_SaleEmployeeOnlineDTO : DataDTO
    {
        public long Sum => SaleEmployeeOnlineHours.Sum(x => x.Counter);
        public List<Dashboard_SaleEmployeeOnlineHourDTO> SaleEmployeeOnlineHours { get; set; }
    }

    public class Dashboard_SaleEmployeeOnlineHourDTO : DataDTO
    {
        public string Hour { get; set; }
        public long Counter { get; set; }
    }
}
