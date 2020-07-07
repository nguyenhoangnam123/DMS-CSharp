using Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DMS.Rpc.reports.report_store_checker
{
    public class ReportStoreChecker_StoreCheckingDTO : DataDTO
    {
        public string StoreCode { get; set; }
        public string StoreName { get; set; }
        public string StoreAddress { get; set; }
        public long SaleEmployeeId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public DateTime Duaration { get; set; }
        public string DeviceName { get; set; }
        public long ImageCounter { get; set; }
        public bool Planned { get; set; }
        public bool SalesOrder { get; set; }
    }

    public class ReportStoreChecker_StoreCheckingGroupByDateDTO : DataDTO
    {
        public DateTime Date { get; set; }
        public List<ReportStoreChecker_StoreCheckingDTO> StoreCheckings { get; set; }
    }
}
