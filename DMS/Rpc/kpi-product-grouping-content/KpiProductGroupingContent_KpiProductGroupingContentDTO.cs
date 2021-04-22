using DMS.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.kpi_product_grouping_content
{
    public class KpiProductGroupingContent_KpiProductGroupingContentDTO : DataDTO
    {
        public long Id { get; set; }
        public long KpiProductGroupingId { get; set; }
        public long ProductGroupingId { get; set; }
        public Guid RowId { get; set; }
        public KpiProductGroupingContent_KpiProductGroupingDTO KpiProductGrouping { get; set; }
        public KpiProductGroupingContent_KpiProductGroupingContentDTO() {}
        public KpiProductGroupingContent_KpiProductGroupingContentDTO(KpiProductGroupingContent KpiProductGroupingContent)
        {
            this.Id = KpiProductGroupingContent.Id;
            this.KpiProductGroupingId = KpiProductGroupingContent.KpiProductGroupingId;
            this.ProductGroupingId = KpiProductGroupingContent.ProductGroupingId;
            this.RowId = KpiProductGroupingContent.RowId;
            this.KpiProductGrouping = KpiProductGroupingContent.KpiProductGrouping == null ? null : new KpiProductGroupingContent_KpiProductGroupingDTO(KpiProductGroupingContent.KpiProductGrouping);
            this.Errors = KpiProductGroupingContent.Errors;
        }
    }

    public class KpiProductGroupingContent_KpiProductGroupingContentFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter KpiProductGroupingId { get; set; }
        public IdFilter ProductGroupingId { get; set; }
        public GuidFilter RowId { get; set; }
        public KpiProductGroupingContentOrder OrderBy { get; set; }
    }
}
