using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.item_specific_kpi_content
{
    public class ItemSpecificKpiContent_ItemSpecificCriteriaDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        

        public ItemSpecificKpiContent_ItemSpecificCriteriaDTO() {}
        public ItemSpecificKpiContent_ItemSpecificCriteriaDTO(ItemSpecificCriteria ItemSpecificCriteria)
        {
            
            this.Id = ItemSpecificCriteria.Id;
            
            this.Code = ItemSpecificCriteria.Code;
            
            this.Name = ItemSpecificCriteria.Name;
            
            this.Errors = ItemSpecificCriteria.Errors;
        }
    }

    public class ItemSpecificKpiContent_ItemSpecificCriteriaFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public ItemSpecificCriteriaOrder OrderBy { get; set; }
    }
}