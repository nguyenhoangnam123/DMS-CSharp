using Common;
using System;

namespace DMS.Rpc.monitor.monitor_store_checker
{
    public class MonitorStoreChecker_MonitorStoreCheckerDetailDTO : DataDTO
    {
        public string StoreCode { get; set; }
        public string StoreName { get; set; }
        public string IndirectSalesOrderCode { get; set; }
        public long Sales { get; set; }
        public string ImagePath { get; set; }
        public long StoreProblemId { get; set; }
        public long CompetitorProblemId { get; set; }
    }

    public class MonitorStoreChecker_MonitorStoreCheckerDetailFilterDTO : FilterDTO
    {
        public long SaleEmployeeId { get; set; }
        public DateTime Date { get; set; }
    }
}
