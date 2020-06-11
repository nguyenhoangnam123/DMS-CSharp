using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Entities
{
    public class ERoutePerformance : DataEntity
    {
        public long Id { get; set; }
        public long SaleEmployeeId { get; set; }
        public DateTime Date { get; set; }
        public long PlanCounter { get; set; }
    }
}
