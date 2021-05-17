using DMS.Common;
using System.Collections.Generic;

namespace DMS.Rpc.kpi_tracking.kpi_product_grouping_report
{
    public class KpiProductGroupingReport_KpiProductGroupingContentCriteriaMappingDTO : DataDTO
    {
        public long SaleEmployeeId { get; set; }
        public long ProductGroupingId { get; set; }
        public long KpiProductGroupingContentId { get; set; }
        public long KpiProductGroupingCriteriaId { get; set; }
        public long? Value { get; set; }
    }
}
