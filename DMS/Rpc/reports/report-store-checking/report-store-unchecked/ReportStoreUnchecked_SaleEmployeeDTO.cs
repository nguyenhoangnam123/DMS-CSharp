using Common;
using System;
using System.Collections.Generic;

namespace DMS.Rpc.reports.report_store_checking.report_store_unchecked
{
    public class ReportStoreUnchecked_SaleEmployeeDTO : DataDTO
    {
        public long SaleEmployeeId { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public List<ReportStoreUnchecked_StoreDTO> Stores { get; set; }
    }

    public class ReportStoreUnchecked_StoreDTO : DataDTO
    {
        public DateTime Date { get; set; }
        public string StoreCode { get; set; }
        public string StoreName { get; set; }
        public string StoreTypeName { get; set; }
        public string StorePhone { get; set; }
        public string StoreAddress { get; set; }
    }
}
