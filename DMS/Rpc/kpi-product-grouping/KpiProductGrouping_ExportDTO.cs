using DMS.Common;
using System.Collections.Generic;

namespace DMS.Rpc.kpi_product_grouping
{
    public class KpiProductGrouping_ExportDTO : DataDTO
    {
        public string OrganizationName { get; set; }
        public List<KpiProductGrouping_AppUserExportDTO> Employees { get; set; }
    }

    public class KpiProductGrouping_AppUserExportDTO : DataDTO
    {
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public List<KpiProductGrouping_KpiProductGroupingExportDTO> KpiProductGroupings { get; set; }
    }

    public class KpiProductGrouping_KpiProductGroupingExportDTO : DataDTO
    {
        public long Id { get; set; }
        public string KpiProductGroupingTypeName { get; set; }
        public List<KpiProductGrouping_ContentExportDTO> Contents { get; set; }
    }


    public class KpiProductGrouping_ContentExportDTO : DataDTO
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public long ItemCount { get; set; }
        public List<KpiProductGrouping_ExportItemDTO> Items { get; set; }
    }

    public class KpiProductGrouping_ExportItemDTO : DataDTO
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal? IndirectRevenue { get; set; }
        public long? IndirectStoreCounter { get; set; }
    }
}

