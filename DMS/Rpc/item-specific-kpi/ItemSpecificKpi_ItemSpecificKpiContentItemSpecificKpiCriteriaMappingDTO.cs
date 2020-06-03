using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.item_specific_kpi
{
    public class ItemSpecificKpi_ItemSpecificKpiContentItemSpecificKpiCriteriaMappingDTO : DataDTO
    {
        public long ItemSpecificKpiContentId { get; set; }
        public long ItemSpecificCriteriaId { get; set; }
        public long Value { get; set; }
        public ItemSpecificKpi_ItemSpecificKpiContentDTO ItemSpecificKpiContent { get; set; }
        public ItemSpecificKpi_ItemSpecificCriteriaDTO ItemSpecificCriteria { get; set; }
        public ItemSpecificKpi_ItemSpecificKpiContentItemSpecificKpiCriteriaMappingDTO() { }
        public ItemSpecificKpi_ItemSpecificKpiContentItemSpecificKpiCriteriaMappingDTO(ItemSpecificKpiContentItemSpecificKpiCriteriaMapping ItemSpecificKpiContentItemSpecificKpiCriteriaMapping)
        {
            this.ItemSpecificKpiContentId = ItemSpecificKpiContentItemSpecificKpiCriteriaMapping.ItemSpecificCriteriaId;
            this.ItemSpecificCriteriaId = ItemSpecificKpiContentItemSpecificKpiCriteriaMapping.ItemSpecificCriteriaId;
            this.Value = ItemSpecificKpiContentItemSpecificKpiCriteriaMapping.Value;
            this.ItemSpecificKpiContent = ItemSpecificKpiContentItemSpecificKpiCriteriaMapping.ItemSpecificKpiContent == null ? null : new ItemSpecificKpi_ItemSpecificKpiContentDTO(ItemSpecificKpiContentItemSpecificKpiCriteriaMapping.ItemSpecificKpiContent);
            this.ItemSpecificCriteria = ItemSpecificKpiContentItemSpecificKpiCriteriaMapping.ItemSpecificCriteria == null ? null : new ItemSpecificKpi_ItemSpecificCriteriaDTO(ItemSpecificKpiContentItemSpecificKpiCriteriaMapping.ItemSpecificCriteria);
        }
    }
}
