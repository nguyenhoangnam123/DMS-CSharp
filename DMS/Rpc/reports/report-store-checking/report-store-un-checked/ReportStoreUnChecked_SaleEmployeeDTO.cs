using Common;
using System;
using System.Collections.Generic;

namespace DMS.Rpc.reports.report_store_checking.report_store_un_checked
{
    public class ReportStoreUnChecked_SaleEmployeeDTO : DataDTO
    {
        public long SaleEmployeeId { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public DateTime Date { get; set; }
        public string ERouteCode { get; set; }
        public string StoreCode { get; set; }
        public string StoreName { get; set; }
        public string StoreTypeName { get; set; }
        public string StorePhone { get; set; }
        public string StoreAddress { get; set; }
    }
}
