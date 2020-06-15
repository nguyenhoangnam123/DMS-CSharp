﻿using Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DMS.Rpc.monitor.monitor_store_checker
{
    public class MonitorStoreChecker_StoreCheckingDTO : DataDTO
    {
        public long SaleEmployeeId { get; set; }
        public DateTime Date { get; set; }
        public long PlanCounter { get; set; }
        public long InternalCounter { get { return Internal == null ? 0 : Internal.Count; } }
        public long ExternalCounter { get { return External == null ? 0 : External.Count; } }
        public long ImageCounter { get { return Image == null ? 0 : Image.Count; } }
        public long SalesOrderCounter { get { return SalesOrder == null ? 0 : SalesOrder.Count; } }
        public decimal RevenueCounter { get { return Revenue == null ? 0 : Revenue.Select(r => r.Value).Sum(); } }
        internal HashSet<long> Image { get; set; }
        internal HashSet<long> SalesOrder { get; set; }
        internal HashSet<long> Internal { get; set; }
        internal HashSet<long> External { get; set; }
        internal Dictionary<long, decimal> Revenue { get; set; }
    }
}
