﻿using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace DMS.Rpc.kpi_tracking.kpi_general_period_report
{
    public class KpiGeneralPeriodReport_KpiGeneralPeriodReportDTO : DataDTO
    {
        public string OrganizationName { get; set; }
        public List<KpiGeneralPeriodReport_SaleEmployeeDTO> SaleEmployees { get; set; }
    }


    public class KpiGeneralPeriodReport_KpiGeneralPeriodReportFilterDTO : FilterDTO
    {
        public IdFilter OrganizationId { get; set; }
        public IdFilter SaleEmployeeId { get; set; }
        public IdFilter KpiPeriodId { get; set; }
        public IdFilter KpiYearId { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum KpiGeneralPeriodReportOrder
    {
        Username = 1,
        DisplayName = 2,
        Organization = 3,
    }
}
