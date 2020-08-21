using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace DMS.Rpc.reports.report_store_checking.report_store_unchecked
{
    public class ReportStoreUnchecked_ReportStoreUncheckedDTO : DataDTO
    {
        public string OrganizationName { get; set; }
        public string OrganizationPath { get; set; }
        public List<ReportStoreUnchecked_SaleEmployeeDTO> SaleEmployees { get; set; }
    }


    public class ReportStoreUnchecked_ReportStoreUncheckedFilterDTO : FilterDTO
    {
        public IdFilter OrganizationId { get; set; }
        public IdFilter AppUserId { get; set; }
        public IdFilter ERouteId { get; set; }
        public DateFilter Date { get; set; }

        internal bool HasValue => (OrganizationId != null && OrganizationId.HasValue) ||
            (AppUserId != null && AppUserId.HasValue) ||
            (ERouteId != null && ERouteId.HasValue) ||
            (Date != null && Date.HasValue);
    }

}
