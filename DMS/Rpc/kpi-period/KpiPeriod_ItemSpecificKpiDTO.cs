using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.kpi_period
{
    public class KpiPeriod_ItemSpecificKpiDTO : DataDTO
    {
        public long Id { get; set; }
        public long OrganizationId { get; set; }
        public long KpiPeriodId { get; set; }
        public long StatusId { get; set; }
        public long EmployeeId { get; set; }
        public long CreatorId { get; set; }
        public KpiPeriod_OrganizationDTO Organization { get; set; }   
        public KpiPeriod_StatusDTO Status { get; set; }   
        
        public KpiPeriod_ItemSpecificKpiDTO() {}
        public KpiPeriod_ItemSpecificKpiDTO(ItemSpecificKpi ItemSpecificKpi)
        {
            this.Id = ItemSpecificKpi.Id;
            this.OrganizationId = ItemSpecificKpi.OrganizationId;
            this.KpiPeriodId = ItemSpecificKpi.KpiPeriodId;
            this.StatusId = ItemSpecificKpi.StatusId;
            this.EmployeeId = ItemSpecificKpi.EmployeeId;
            this.CreatorId = ItemSpecificKpi.CreatorId;
            this.Organization = ItemSpecificKpi.Organization == null ? null : new KpiPeriod_OrganizationDTO(ItemSpecificKpi.Organization);
            this.Status = ItemSpecificKpi.Status == null ? null : new KpiPeriod_StatusDTO(ItemSpecificKpi.Status);
            this.Errors = ItemSpecificKpi.Errors;
        }
    }

    public class KpiPeriod_ItemSpecificKpiFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public IdFilter OrganizationId { get; set; }
        
        public IdFilter KpiPeriodId { get; set; }
        
        public IdFilter StatusId { get; set; }
        
        public IdFilter EmployeeId { get; set; }
        
        public IdFilter CreatorId { get; set; }
        
        public ItemSpecificKpiOrder OrderBy { get; set; }
    }
}