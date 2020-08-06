using Common;
using System;
using System.Collections.Generic;

namespace DMS.Rpc.monitor.monitor_store_checker
{
    public class MonitorStoreChecker_MonitorStoreCheckerDetailDTO : DataDTO
    {
        public string StoreCode { get; set; }
        public string StoreName { get; set; }
        public List<MonitorStoreChecker_MonitorStoreCheckerDetailInfoDTO> Infoes { get; set; }
    }

    public class MonitorStoreChecker_MonitorStoreCheckerDetailInfoDTO : DataDTO
    {
        public string IndirectSalesOrderCode { get; set; }
        public decimal Sales { get; set; }
        public string ImagePath { get; set; }
        public string StoreProblemCode { get; set; }
        public long StoreProblemId { get; set; }
        public long CompetitorProblemId { get; set; }
        public string CompetitorProblemCode { get; set; }
    }
    public class MonitorStoreChecker_MonitorStoreCheckerDetailFilterDTO : FilterDTO
    {
        public long SaleEmployeeId { get; set; }
        public DateTime Date { get; set; }
    }
}
