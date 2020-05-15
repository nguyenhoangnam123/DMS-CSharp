using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.store_checker_monitor
{
    public class StoreCheckerMonitor_StoreCheckingDTO : DataDTO
    {
        public DateTime Date { get; set; }
        public long PlanCounter { get; set; }
        public long InternalCounter { get; set; }
        public long ExternalCounter { get; set; }
        public long ImageCounter { get; set; }
        public long SalesOrderCounter { get; set; }

    }
}
