using DMS.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.kpi_tracking.kpi_product_grouping_report
{
    public class KpiProductGroupingReport_KpiProductGroupingTypeDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        

        public KpiProductGroupingReport_KpiProductGroupingTypeDTO() {}
        public KpiProductGroupingReport_KpiProductGroupingTypeDTO(KpiProductGroupingType KpiProductGroupingType)
        {
            
            this.Id = KpiProductGroupingType.Id;
            
            this.Code = KpiProductGroupingType.Code;
            
            this.Name = KpiProductGroupingType.Name;
            
            this.Errors = KpiProductGroupingType.Errors;
        }
    }

    public class KpiProductGroupingReport_KpiProductGroupingTypeFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public KpiProductGroupingTypeOrder OrderBy { get; set; }
    }
}