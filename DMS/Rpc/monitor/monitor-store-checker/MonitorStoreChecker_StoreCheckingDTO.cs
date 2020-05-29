using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.monitor.monitor_store_checker
{
    public class MonitorStoreChecker_StoreCheckingDTO : DataDTO
    {
        public DateTime Date { get; set; }
        public long PlanCounter { get { return Plan == null ? 0 : Plan.Count; } }
        public long InternalCounter { get { return Internal == null ? 0 : Internal.Count; } }
        public long ExternalCounter { get { return External == null ? 0 : External.Count; } }
        public long ImageCounter { get { return Image == null ? 0 : Image.Count; } }
        public long SalesOrderCounter { get { return SalesOrder == null ? 0 : SalesOrder.Count; } }
        public long RevenueCounter { get { return Revenue == null ? 0 : Revenue.Select(r => r.Value).Sum(); } }
        internal HashSet<long> Image { get; set; }
        internal HashSet<long> SalesOrder { get; set; }
        internal HashSet<long> Plan { get; set; }
        internal HashSet<long> Internal { get; set; }
        internal HashSet<long> External { get; set; }
        internal Dictionary<long, long> Revenue { get; set; }
    }
}
