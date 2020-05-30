using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.general_criteria
{
    public class GeneralCriteria_GeneralKpiDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public long OrganizationId { get; set; }
        
        public long EmployeeId { get; set; }
        
        public long KpiPeriodId { get; set; }
        
        public long StatusId { get; set; }
        
        public long CreatorId { get; set; }
        

        public GeneralCriteria_GeneralKpiDTO() {}
        public GeneralCriteria_GeneralKpiDTO(GeneralKpi GeneralKpi)
        {
            
            this.Id = GeneralKpi.Id;
            
            this.OrganizationId = GeneralKpi.OrganizationId;
            
            this.EmployeeId = GeneralKpi.EmployeeId;
            
            this.KpiPeriodId = GeneralKpi.KpiPeriodId;
            
            this.StatusId = GeneralKpi.StatusId;
            
            this.CreatorId = GeneralKpi.CreatorId;
            
            this.Errors = GeneralKpi.Errors;
        }
    }

    public class GeneralCriteria_GeneralKpiFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public IdFilter OrganizationId { get; set; }
        
        public IdFilter EmployeeId { get; set; }
        
        public IdFilter KpiPeriodId { get; set; }
        
        public IdFilter StatusId { get; set; }
        
        public IdFilter CreatorId { get; set; }
        
        public GeneralKpiOrder OrderBy { get; set; }
    }
}