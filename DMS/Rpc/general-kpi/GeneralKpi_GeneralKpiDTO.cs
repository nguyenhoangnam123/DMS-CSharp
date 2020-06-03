using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.general_kpi
{
    public class GeneralKpi_GeneralKpiDTO : DataDTO
    {
        public long Id { get; set; }
        public long OrganizationId { get; set; }
        public long EmployeeId { get; set; }
        public long KpiPeriodId { get; set; }
        public long StatusId { get; set; }
        public long CreatorId { get; set; }
        public List<long> EmployeeIds { get; set; }
        public GeneralKpi_AppUserDTO Creator { get; set; }
        public GeneralKpi_AppUserDTO Employee { get; set; }
        public GeneralKpi_KpiPeriodDTO KpiPeriod { get; set; }
        public GeneralKpi_OrganizationDTO Organization { get; set; }
        public GeneralKpi_StatusDTO Status { get; set; }
        public List<GeneralKpi_GeneralCriteriaDTO> GeneralCriterias { get; set; }
        public List<GeneralKpi_GeneralKpiCriteriaMappingDTO> GeneralKpiCriteriaMappings { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public GeneralKpi_GeneralKpiDTO() {}
        public GeneralKpi_GeneralKpiDTO(GeneralKpi GeneralKpi)
        {
            this.Id = GeneralKpi.Id;
            this.OrganizationId = GeneralKpi.OrganizationId;
            this.EmployeeId = GeneralKpi.EmployeeId;
            this.KpiPeriodId = GeneralKpi.KpiPeriodId;
            this.StatusId = GeneralKpi.StatusId;
            this.CreatorId = GeneralKpi.CreatorId;
            this.Creator = GeneralKpi.Creator == null ? null : new GeneralKpi_AppUserDTO(GeneralKpi.Creator);
            this.Employee = GeneralKpi.Employee == null ? null : new GeneralKpi_AppUserDTO(GeneralKpi.Employee);
            this.KpiPeriod = GeneralKpi.KpiPeriod == null ? null : new GeneralKpi_KpiPeriodDTO(GeneralKpi.KpiPeriod);
            this.Organization = GeneralKpi.Organization == null ? null : new GeneralKpi_OrganizationDTO(GeneralKpi.Organization);
            this.Status = GeneralKpi.Status == null ? null : new GeneralKpi_StatusDTO(GeneralKpi.Status);
            this.GeneralKpiCriteriaMappings = GeneralKpi.GeneralKpiCriteriaMappings?.Select(x => new GeneralKpi_GeneralKpiCriteriaMappingDTO(x)).ToList();
            this.CreatedAt = GeneralKpi.CreatedAt;
            this.UpdatedAt = GeneralKpi.UpdatedAt;
            this.Errors = GeneralKpi.Errors;
        }
    }

    public class GeneralKpi_GeneralKpiFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter OrganizationId { get; set; }
        public IdFilter EmployeeId { get; set; }
        public IdFilter KpiPeriodId { get; set; }
        public IdFilter StatusId { get; set; }
        public IdFilter CreatorId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public GeneralKpiOrder OrderBy { get; set; }
    }
}
