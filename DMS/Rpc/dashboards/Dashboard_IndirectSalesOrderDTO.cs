using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.dashboards
{
    public class Dashboard_IndirectSalesOrderDTO : DataDTO
    {
        public long Sum => IndirectSalesOrderHours.Sum(x => x.Counter);
        public List<Dashboard_IndirectSalesOrderHourDTO> IndirectSalesOrderHours { get; set; }
    }

    public class Dashboard_IndirectSalesOrderHourDTO : DataDTO
    {
        public string Hour { get; set; }
        public long Counter { get; set; }
    }
}
