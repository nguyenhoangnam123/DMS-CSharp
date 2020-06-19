using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace DMS.Rpc.kpi_tracking.kpi_period_report
{
    public class KpiPeriodReport_KpiPeriodReportDTO : DataDTO
    {
        public string OrganizationName { get; set; }
        public List<KpiPeriodReport_SaleEmployeeDTO> SaleEmployees { get; set; }
    }


    public class KpiPeriodReport_KpiPeriodReportFilterDTO : FilterDTO
    {
        public IdFilter OrganizationId { get; set; }
        public IdFilter SaleEmployeeId { get; set; }
        public DateFilter KpiPeriod { get; set; }
        public IdFilter KpiPeriodId { get; set; }
        public DateFilter KpiYear { get; set; }
        public IdFilter KpiYearId { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum KpiPeriodReportOrder
    {
        Username = 1,
        DisplayName = 2,
        Organization = 3,
    }
}
