using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.item_specific_kpi
{
    public class ItemSpecificKpi_ItemSpecificKpiTotalItemSpecificCriteriaMappingDTO : DataDTO
    {
        public long ItemSpecificKpiId { get; set; }
        public long TotalItemSpecificCriteriaId { get; set; }
        public long Value { get; set; }
        public ItemSpecificKpi_TotalItemSpecificCriteriaDTO TotalItemSpecificCriteria { get; set; }   
        
        public ItemSpecificKpi_ItemSpecificKpiTotalItemSpecificCriteriaMappingDTO() {}
        public ItemSpecificKpi_ItemSpecificKpiTotalItemSpecificCriteriaMappingDTO(ItemSpecificKpiTotalItemSpecificCriteriaMapping ItemSpecificKpiTotalItemSpecificCriteriaMapping)
        {
            this.ItemSpecificKpiId = ItemSpecificKpiTotalItemSpecificCriteriaMapping.ItemSpecificKpiId;
            this.TotalItemSpecificCriteriaId = ItemSpecificKpiTotalItemSpecificCriteriaMapping.TotalItemSpecificCriteriaId;
            this.Value = ItemSpecificKpiTotalItemSpecificCriteriaMapping.Value;
            this.TotalItemSpecificCriteria = ItemSpecificKpiTotalItemSpecificCriteriaMapping.TotalItemSpecificCriteria == null ? null : new ItemSpecificKpi_TotalItemSpecificCriteriaDTO(ItemSpecificKpiTotalItemSpecificCriteriaMapping.TotalItemSpecificCriteria);
            this.Errors = ItemSpecificKpiTotalItemSpecificCriteriaMapping.Errors;
        }
    }

    public class ItemSpecificKpi_ItemSpecificKpiTotalItemSpecificCriteriaMappingFilterDTO : FilterDTO
    {
        
        public IdFilter ItemSpecificKpiId { get; set; }
        
        public IdFilter TotalItemSpecificCriteriaId { get; set; }
        
        public LongFilter Value { get; set; }
        
        public ItemSpecificKpiTotalItemSpecificCriteriaMappingOrder OrderBy { get; set; }
    }
}