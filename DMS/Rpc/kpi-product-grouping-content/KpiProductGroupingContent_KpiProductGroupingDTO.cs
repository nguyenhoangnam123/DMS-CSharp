using DMS.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.kpi_product_grouping_content
{
    public class KpiProductGroupingContent_KpiProductGroupingDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public long OrganizationId { get; set; }
        
        public long KpiYearId { get; set; }
        
        public long KpiPeriodId { get; set; }
        
        public long KpiProductGroupingTypeId { get; set; }
        
        public long StatusId { get; set; }
        
        public long EmployeeId { get; set; }
        
        public long CreatorId { get; set; }
        
        public Guid RowId { get; set; }
        

        public KpiProductGroupingContent_KpiProductGroupingDTO() {}
        public KpiProductGroupingContent_KpiProductGroupingDTO(KpiProductGrouping KpiProductGrouping)
        {
            
            this.Id = KpiProductGrouping.Id;
            
            this.OrganizationId = KpiProductGrouping.OrganizationId;
            
            this.KpiYearId = KpiProductGrouping.KpiYearId;
            
            this.KpiPeriodId = KpiProductGrouping.KpiPeriodId;
            
            this.KpiProductGroupingTypeId = KpiProductGrouping.KpiProductGroupingTypeId;
            
            this.StatusId = KpiProductGrouping.StatusId;
            
            this.EmployeeId = KpiProductGrouping.EmployeeId;
            
            this.CreatorId = KpiProductGrouping.CreatorId;
            
            this.RowId = KpiProductGrouping.RowId;
            
            this.Errors = KpiProductGrouping.Errors;
        }
    }

    public class KpiProductGroupingContent_KpiProductGroupingFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public IdFilter OrganizationId { get; set; }
        
        public IdFilter KpiYearId { get; set; }
        
        public IdFilter KpiPeriodId { get; set; }
        
        public IdFilter KpiProductGroupingTypeId { get; set; }
        
        public IdFilter StatusId { get; set; }
        
        public IdFilter EmployeeId { get; set; }
        
        public IdFilter CreatorId { get; set; }
        
        public GuidFilter RowId { get; set; }
        
        public KpiProductGroupingOrder OrderBy { get; set; }
    }
}