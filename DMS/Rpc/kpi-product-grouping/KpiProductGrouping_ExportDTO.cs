using DMS.Common;
using System.Collections.Generic;

namespace DMS.Rpc.kpi_product_grouping
{
    public class KpiProductGrouping_ExportDTO : DataDTO
    {
        public long OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public List<KpiProductGrouping_KpiProductGroupingExportDTO> Kpis { get; set; }
    }

    public class KpiProductGrouping_KpiProductGroupingExportDTO : DataDTO
    {
        public long STT { get; set; }
        public long Id { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string KpiProductGroupingTypeName { get; set; }
        public List<KpiProductGrouping_ProductGroupingExportDTO> ProductGroupings { get; set; }
    }

    public class KpiProductGrouping_ProductGroupingExportDTO : DataDTO
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public long ItemCount { get; set; }
        public decimal? IndirectRevenue { get; set; }
        public long? IndirectStoreCounter { get; set; }
        public List<KpiProductGrouping_ExportItemDTO> Items { get; set; }
    }

    public class KpiProductGrouping_ExportItemDTO : DataDTO
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}

