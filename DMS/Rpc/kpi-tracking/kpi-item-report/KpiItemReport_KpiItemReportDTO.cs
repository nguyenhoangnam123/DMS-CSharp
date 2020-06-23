using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace DMS.Rpc.kpi_tracking.kpi_item_report
{
    public class KpiItemReport_KpiItemReportDTO : DataDTO
    {
        public string OrganizationName { get; set; }
        public List<KpiItemReport_SaleEmployeeDTO> SaleEmployees { get; set; }
    }


    public class KpiItemReport_KpiItemReportFilterDTO : FilterDTO
    {
        public IdFilter OrganizationId { get; set; }
        public IdFilter SaleEmployeeId { get; set; }
        public IdFilter KpiPeriodId { get; set; }
        public IdFilter KpiYearId { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum KpiItemReportOrder
    {
        Username = 1,
        DisplayName = 2,
        Organization = 3,
    }
}
