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
        public long PlanCounter { get { return Plan.Count; } }
        public long InternalCounter => Internal.Count;
        public long ExternalCounter => External.Count;
        public long ImageCounter => Image.Count;
        public long SalesOrderCounter { get; set; }
        internal HashSet<long> Image { get; set; }
        internal HashSet<long> SalesOrder { get; set; }
        internal HashSet<long> Plan { get; set; }
        internal HashSet<long> Internal { get; set; }
        internal HashSet<long> External { get; set; }
    }
}
