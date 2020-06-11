using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class ERoutePerformanceDAO
    {
        public long Id { get; set; }
        public long SaleEmployeeId { get; set; }
        public DateTime Date { get; set; }
        public long PlanCounter { get; set; }

        public virtual AppUserDAO SaleEmployee { get; set; }
    }
}
