﻿using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.reports.report_store.report_store_general
{
    public class ReportStoreGeneral_StoreDetailDTO : DataDTO
    {
        public long STT { get; set; }
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public long CheckingPlannedCounter => StoreCheckingPlannedIds?.Count() ?? 0;
        public long CheckingUnPlannedCounter => StoreCheckingUnPlannedIds?.Count() ?? 0;
        public string TotalCheckingTime { get; set; }
        public DateTime FirstChecking { get; set; }
        public DateTime LastChecking { get; set; }
        public long IndirectSalesOrderCounter => IndirectSalesOrderIds?.Count() ?? 0;
        public long SKUCounter => SKUItemIds?.Count() ?? 0;
        public decimal TotalRevenue { get; set; }
        public DateTime LastOrder { get; set; }
        public string LastOrderDisplay => LastOrder == DateTime.MinValue ? "" : LastOrder.ToLongDateString();
        internal HashSet<long> StoreCheckingPlannedIds { get; set; }
        internal HashSet<long> StoreCheckingUnPlannedIds { get; set; }
        internal HashSet<long> SKUItemIds { get; set; }
        internal HashSet<long> IndirectSalesOrderIds { get; set; }
    }
}
