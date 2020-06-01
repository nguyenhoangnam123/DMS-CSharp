using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.item_specific_kpi
{
    public class ItemSpecificKpi_TotalItemSpecificCriteriaDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        

        public ItemSpecificKpi_TotalItemSpecificCriteriaDTO() {}
        public ItemSpecificKpi_TotalItemSpecificCriteriaDTO(TotalItemSpecificCriteria TotalItemSpecificCriteria)
        {
            
            this.Id = TotalItemSpecificCriteria.Id;
            
            this.Code = TotalItemSpecificCriteria.Code;
            
            this.Name = TotalItemSpecificCriteria.Name;
            
            this.Errors = TotalItemSpecificCriteria.Errors;
        }
    }

    public class ItemSpecificKpi_TotalItemSpecificCriteriaFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public TotalItemSpecificCriteriaOrder OrderBy { get; set; }
    }
}