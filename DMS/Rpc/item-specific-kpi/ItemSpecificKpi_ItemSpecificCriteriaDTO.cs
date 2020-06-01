using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.item_specific_kpi
{
    public class ItemSpecificKpi_ItemSpecificCriteriaDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        

        public ItemSpecificKpi_ItemSpecificCriteriaDTO() {}
        public ItemSpecificKpi_ItemSpecificCriteriaDTO(ItemSpecificCriteria ItemSpecificCriteria)
        {
            
            this.Id = ItemSpecificCriteria.Id;
            
            this.Code = ItemSpecificCriteria.Code;
            
            this.Name = ItemSpecificCriteria.Name;
            
            this.Errors = ItemSpecificCriteria.Errors;
        }
    }

    public class ItemSpecificKpi_ItemSpecificCriteriaFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public ItemSpecificCriteriaOrder OrderBy { get; set; }
    }
}