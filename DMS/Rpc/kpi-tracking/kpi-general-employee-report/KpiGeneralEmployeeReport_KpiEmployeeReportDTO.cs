using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace DMS.Rpc.kpi_tracking.kpi_general_employee_report
{
    public class KpiGeneralEmployeeReport_KpiGeneralEmployeeReportDTO : DataDTO
    {
        public string OrganizationName { get; set; }
        public List<KpiGeneralEmployeeReport_SaleEmployeeDTO> SaleEmployees { get; set; }
    }


    public class KpiGeneralEmployeeReport_KpiGeneralEmployeeReportFilterDTO : FilterDTO
    {
        public IdFilter OrganizationId { get; set; }
        public IdFilter SaleEmployeeId { get; set; }
        public IdFilter KpiPeriodId { get; set; }
        public IdFilter KpiYearId { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum KpiGeneralEmployeeReportOrder
    {
        Username = 1,
        DisplayName = 2,
        Organization = 3,
    }
}
