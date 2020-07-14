using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.reports.report_store.report_store_general
{
    public class ReportStoreGeneral_StoreDetailDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public long CheckingPlannedCounter => StoreCheckingPlannedIds.Count();
        public long CheckingUnPlannedCounter => StoreCheckingUnPlannedIds.Count();
        public string TotalCheckingTime { get; set; }
        public DateTime FirstChecking { get; set; }
        public DateTime LastChecking { get; set; }
        public long IndirectSalesOrderCounter => IndirectSalesOrderIds.Count();
        public long SKUCounter => SKUItemIds.Count();
        public decimal TotalRevenue { get; set; }
        public DateTime LastOrder { get; set; }
        internal HashSet<long> StoreCheckingPlannedIds { get; set; }
        internal HashSet<long> StoreCheckingUnPlannedIds { get; set; }
        internal HashSet<long> SKUItemIds { get; set; }
        internal HashSet<long> IndirectSalesOrderIds { get; set; }
    }
}
