using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace DMS.Rpc.reports.report_store_checking.report_store_un_checked
{
    public class ReportStoreUnChecked_ReportStoreUnCheckedDTO : DataDTO
    {
        public string OrganizationName { get; set; }
        public List<ReportStoreUnChecked_SaleEmployeeDTO> SaleEmployees { get; set; }
    }


    public class ReportStoreChecked_ReportStoreCheckedFilterDTO : FilterDTO
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
