using DMS.Common;
using System;
using System.Collections.Generic;

namespace DMS.Rpc.monitor.monitor_store_checker
{
    public class MonitorStoreChecker_MonitorStoreCheckerDetailDTO : DataDTO
    {
        public long StoreId { get; set; }
        public string StoreCode { get; set; }
        public string StoreName { get; set; }
        public long ImageCounter { get; set; }
        public List<MonitorStoreChecker_MonitorStoreCheckerDetailInfoDTO> Infoes { get; set; }
    }

    public class MonitorStoreChecker_MonitorStoreCheckerDetailInfoDTO : DataDTO
    {
        public string IndirectSalesOrderCode { get; set; }
        public decimal Sales { get; set; }
        public string ImagePath { get; set; }
        public string ProblemCode { get; set; }
        public long ProblemId { get; set; }
    }
    public class MonitorStoreChecker_MonitorStoreCheckerDetailFilterDTO : FilterDTO
    {
        public long SaleEmployeeId { get; set; }
        public DateTime Date { get; set; }
    }
}
