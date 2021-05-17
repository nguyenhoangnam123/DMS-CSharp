using DMS.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace DMS.Rpc.kpi_tracking.kpi_product_grouping_report
{
    public class KpiProductGroupingReport_KpiProductGroupingReportDTO : DataDTO
    {
        public long OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public List<KpiProductGroupingReport_KpiSaleEmployeetDTO> SaleEmployees { get; set; }
    }

    public class KpiProductGroupingReport_KpiSaleEmployeetDTO : DataDTO
    {
        public long STT { get; set; }
        public long Id { get; set; }
        public long OrganizationId { get; set; }
        public string UserName {get; set; }
        public string DisplayName { get; set; }
        public List<KpiProductGroupingReport_KpiProductGroupingContentDTO> Contents { get; set; }
    }

    public class KpiProductGroupingReport_KpiProductGroupingReportFilterDTO : FilterDTO
    {
        public IdFilter OrganizationId { get; set; }
        public IdFilter AppUserId { get; set; }
        public IdFilter KpiPeriodId { get; set; }
        public IdFilter KpiYearId { get; set; }
        public IdFilter KpiProductGroupingTypeId { get; set; }
        public IdFilter ProductGroupingId { get; set; }
    }
}
