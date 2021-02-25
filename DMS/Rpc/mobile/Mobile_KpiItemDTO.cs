using DMS.Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DMS.Rpc.mobile
{
    public class Mobile_KpiItemDTO : DataDTO
    {
        public long Id { get; set; }
        public long OrganizationId { get; set; }
        public long KpiYearId { get; set; }
        public long KpiPeriodId { get; set; }
        public long StatusId { get; set; }
        public long EmployeeId { get; set; }
        public long CreatorId { get; set; }
        public bool ReadOnly { get; set; }
        public GenericEnum CurrentMonth { get; set; }
        public GenericEnum CurrentQuarter { get; set; }
        public GenericEnum CurrentYear { get; set; }
        public Mobile_AppUserDTO Creator { get; set; }
        public Mobile_AppUserDTO Employee { get; set; }
        public Mobile_KpiYearDTO KpiYear { get; set; }
        public Mobile_KpiPeriodDTO KpiPeriod { get; set; }
        public Mobile_OrganizationDTO Organization { get; set; }
        public Mobile_StatusDTO Status { get; set; }
        public List<Mobile_KpiItemContentDTO> KpiItemContents { get; set; }
        public List<Mobile_KpiCriteriaItemDTO> KpiCriteriaItems { get; set; }
        public List<long> EmployeeIds { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Mobile_KpiItemDTO() { }
        public Mobile_KpiItemDTO(KpiItem KpiItem)
        {
            this.Id = KpiItem.Id;
            this.OrganizationId = KpiItem.OrganizationId;
            this.KpiPeriodId = KpiItem.KpiPeriodId;
            this.KpiYearId = KpiItem.KpiYearId;
            this.StatusId = KpiItem.StatusId;
            this.EmployeeId = KpiItem.EmployeeId;
            this.CreatorId = KpiItem.CreatorId;
            this.ReadOnly = KpiItem.ReadOnly;
            this.Creator = KpiItem.Creator == null ? null : new Mobile_AppUserDTO(KpiItem.Creator);
            this.Employee = KpiItem.Employee == null ? null : new Mobile_AppUserDTO(KpiItem.Employee);
            this.KpiPeriod = KpiItem.KpiPeriod == null ? null : new Mobile_KpiPeriodDTO(KpiItem.KpiPeriod);
            this.KpiYear = KpiItem.KpiYear == null ? null : new Mobile_KpiYearDTO(KpiItem.KpiYear);
            this.Organization = KpiItem.Organization == null ? null : new Mobile_OrganizationDTO(KpiItem.Organization);
            this.Status = KpiItem.Status == null ? null : new Mobile_StatusDTO(KpiItem.Status);
            this.KpiItemContents = KpiItem.KpiItemContents?.Select(x => new Mobile_KpiItemContentDTO(x)).ToList();
            this.EmployeeIds = KpiItem.EmployeeIds; // to do
            this.CreatedAt = KpiItem.CreatedAt;
            this.UpdatedAt = KpiItem.UpdatedAt;
            this.Errors = KpiItem.Errors;
        }
    }

    public class KpiItem_KpiItemFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter OrganizationId { get; set; }
        public IdFilter KpiYearId { get; set; }
        public IdFilter KpiPeriodId { get; set; }
        public IdFilter StatusId { get; set; }
        public IdFilter AppUserId { get; set; }
        public IdFilter CreatorId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public KpiItemOrder OrderBy { get; set; }
    }
}
