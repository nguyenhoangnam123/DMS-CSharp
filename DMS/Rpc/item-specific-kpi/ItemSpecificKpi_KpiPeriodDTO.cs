using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.item_specific_kpi
{
    public class ItemSpecificKpi_KpiPeriodDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        

        public ItemSpecificKpi_KpiPeriodDTO() {}
        public ItemSpecificKpi_KpiPeriodDTO(KpiPeriod KpiPeriod)
        {
            
            this.Id = KpiPeriod.Id;
            
            this.Code = KpiPeriod.Code;
            
            this.Name = KpiPeriod.Name;
            
            this.Errors = KpiPeriod.Errors;
        }
    }

    public class ItemSpecificKpi_KpiPeriodFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public KpiPeriodOrder OrderBy { get; set; }
    }
}