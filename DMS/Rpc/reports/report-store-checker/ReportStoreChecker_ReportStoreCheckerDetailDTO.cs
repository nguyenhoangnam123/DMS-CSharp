using Common;
using System;

namespace DMS.Rpc.reports.report_store_checker
{
    public class ReportStoreChecker_ReportStoreCheckerDetailDTO : DataDTO
    {
        public string StoreCode { get; set; }
        public string StoreName { get; set; }
        public string IndirectSalesOrderCode { get; set; }
        public decimal Sales { get; set; }
        public string ImagePath { get; set; }
        public string StoreProblemCode { get; set; }
        public string CompetitorProblemCode { get; set; }
    }

    public class ReportStoreChecker_ReportStoreCheckerDetailFilterDTO : FilterDTO
    {
        public long AppUserId { get; set; }
        public DateTime Date { get; set; }
    }
}
