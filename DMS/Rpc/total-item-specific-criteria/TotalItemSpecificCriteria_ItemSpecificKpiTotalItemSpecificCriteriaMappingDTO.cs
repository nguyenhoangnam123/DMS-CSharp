using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.total_item_specific_criteria
{
    public class TotalItemSpecificCriteria_ItemSpecificKpiTotalItemSpecificCriteriaMappingDTO : DataDTO
    {
        public long ItemSpecificKpiId { get; set; }
        public long TotalItemSpecificCriteriaId { get; set; }
        public long Value { get; set; }
        public TotalItemSpecificCriteria_ItemSpecificKpiDTO ItemSpecificKpi { get; set; }   
        
        public TotalItemSpecificCriteria_ItemSpecificKpiTotalItemSpecificCriteriaMappingDTO() {}
        public TotalItemSpecificCriteria_ItemSpecificKpiTotalItemSpecificCriteriaMappingDTO(ItemSpecificKpiTotalItemSpecificCriteriaMapping ItemSpecificKpiTotalItemSpecificCriteriaMapping)
        {
            this.ItemSpecificKpiId = ItemSpecificKpiTotalItemSpecificCriteriaMapping.ItemSpecificKpiId;
            this.TotalItemSpecificCriteriaId = ItemSpecificKpiTotalItemSpecificCriteriaMapping.TotalItemSpecificCriteriaId;
            this.Value = ItemSpecificKpiTotalItemSpecificCriteriaMapping.Value;
            this.ItemSpecificKpi = ItemSpecificKpiTotalItemSpecificCriteriaMapping.ItemSpecificKpi == null ? null : new TotalItemSpecificCriteria_ItemSpecificKpiDTO(ItemSpecificKpiTotalItemSpecificCriteriaMapping.ItemSpecificKpi);
            this.Errors = ItemSpecificKpiTotalItemSpecificCriteriaMapping.Errors;
        }
    }

    public class TotalItemSpecificCriteria_ItemSpecificKpiTotalItemSpecificCriteriaMappingFilterDTO : FilterDTO
    {
        
        public IdFilter ItemSpecificKpiId { get; set; }
        
        public IdFilter TotalItemSpecificCriteriaId { get; set; }
        
        public LongFilter Value { get; set; }
        
        public ItemSpecificKpiTotalItemSpecificCriteriaMappingOrder OrderBy { get; set; }
    }
}