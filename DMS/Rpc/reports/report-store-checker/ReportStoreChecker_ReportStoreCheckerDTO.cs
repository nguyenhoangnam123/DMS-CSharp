using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace DMS.Rpc.reports.report_store_checker
{
    public class ReportStoreChecker_ReportStoreCheckerDTO : DataDTO
    {
        public string OrganizationName { get; set; }
        public List<ReportStoreChecker_SaleEmployeeDTO> SaleEmployees { get; set; }
    }


    public class ReportStoreChecker_ReportStoreCheckerFilterDTO : FilterDTO
    {
        public IdFilter OrganizationId { get; set; }
        public IdFilter AppUserId { get; set; }
        public IdFilter StoreId { get; set; }
        public IdFilter StoreTypeId { get; set; }
        public IdFilter StoreGroupingId { get; set; }
        public DateFilter CheckIn { get; set; }
        public List<ReportStoreChecker_ReportStoreCheckerFilterDTO> OrFilters { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ReportStoreCheckerOrder
    {
        Username = 1,
        DisplayName = 2,
        Organization = 3,
    }
}
