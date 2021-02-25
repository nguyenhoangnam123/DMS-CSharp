using DMS.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.mobile
{
    public class Mobile_KpiGeneralDTO : DataDTO
    {
        public long Id { get; set; }
        public long OrganizationId { get; set; }
        public long EmployeeId { get; set; }
        public long KpiYearId { get; set; }
        public GenericEnum CurrentMonth { get; set; }
        public GenericEnum CurrentQuarter { get; set; }
        public GenericEnum CurrentYear { get; set; }
        public long StatusId { get; set; }
        public long CreatorId { get; set; }
        public Mobile_AppUserDTO Creator { get; set; }
        public Mobile_AppUserDTO Employee { get; set; }
        public Mobile_KpiYearDTO KpiYear { get; set; }
        public Mobile_OrganizationDTO Organization { get; set; }
        public Mobile_StatusDTO Status { get; set; }
        public List<Mobile_KpiGeneralContentDTO> KpiGeneralContents { get; set; }
        public List<Mobile_KpiPeriodDTO> KpiPeriods { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<long> EmployeeIds { get; set; }
        public Mobile_KpiGeneralDTO() {}
        public Mobile_KpiGeneralDTO(KpiGeneral KpiGeneral)
        {
            this.Id = KpiGeneral.Id;
            this.OrganizationId = KpiGeneral.OrganizationId;
            this.EmployeeId = KpiGeneral.EmployeeId;
            this.EmployeeIds = KpiGeneral.EmployeeIds;
            this.KpiYearId = KpiGeneral.KpiYearId;
            this.StatusId = KpiGeneral.StatusId;
            this.CreatorId = KpiGeneral.CreatorId;
            this.Creator = KpiGeneral.Creator == null ? null : new Mobile_AppUserDTO(KpiGeneral.Creator);
            this.Employee = KpiGeneral.Employee == null ? null : new Mobile_AppUserDTO(KpiGeneral.Employee);
            this.KpiYear = KpiGeneral.KpiYear == null ? null : new Mobile_KpiYearDTO(KpiGeneral.KpiYear);
            this.Organization = KpiGeneral.Organization == null ? null : new Mobile_OrganizationDTO(KpiGeneral.Organization);
            this.Status = KpiGeneral.Status == null ? null : new Mobile_StatusDTO(KpiGeneral.Status);

            this.KpiGeneralContents = KpiGeneral.KpiGeneralContents?.Select(x => new Mobile_KpiGeneralContentDTO(x)).ToList();
            this.CreatedAt = KpiGeneral.CreatedAt;
            this.UpdatedAt = KpiGeneral.UpdatedAt;
            this.Errors = KpiGeneral.Errors;
        }
    }

    public class KpiGeneral_KpiGeneralFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter OrganizationId { get; set; }
        public IdFilter AppUserId { get; set; }
        public IdFilter KpiYearId { get; set; }
        public IdFilter StatusId { get; set; }
        public IdFilter CreatorId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public KpiGeneralOrder OrderBy { get; set; }
    }
}
