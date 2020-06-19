using Common;
using System;

namespace DMS.Rpc.monitor.monitor_store_checker
{
    public class MonitorStoreChecker_MonitorStoreCheckerDetailDTO : DataDTO
    {
        public string StoreCode { get; set; }
        public string StoreName { get; set; }
        public string IndirectSalesOrderCode { get; set; }
        public decimal Sales { get; set; }
        public string ImagePath { get; set; }
        public string StoreProblemCode { get; set; }
        public string CompetitorProblemCode { get; set; }
    }

    public class MonitorStoreChecker_MonitorStoreCheckerDetailFilterDTO : FilterDTO
    {
        public long SaleEmployeeId { get; set; }
        public DateTime Date { get; set; }
    }
}
