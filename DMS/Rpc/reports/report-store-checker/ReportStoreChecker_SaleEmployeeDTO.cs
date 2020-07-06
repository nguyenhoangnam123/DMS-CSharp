using Common;
using System.Collections.Generic;

namespace DMS.Rpc.reports.report_store_checker
{
    public class ReportStoreChecker_SaleEmployeeDTO : DataDTO
    {
        public long SaleEmployeeId { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string OrganizationName { get; set; }
        public List<ReportStoreChecker_StoreCheckingGroupByDateDTO> StoreCheckingGroupByDates { get; set; }
    }
}
