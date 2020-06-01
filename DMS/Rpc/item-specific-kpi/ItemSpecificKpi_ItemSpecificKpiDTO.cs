using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.item_specific_kpi
{
    public class ItemSpecificKpi_ItemSpecificKpiDTO : DataDTO
    {
        public long Id { get; set; }
        public long OrganizationId { get; set; }
        public long KpiPeriodId { get; set; }
        public long StatusId { get; set; }
        public long EmployeeId { get; set; }
        public long CreatorId { get; set; }
        public ItemSpecificKpi_AppUserDTO Creator { get; set; }
        public ItemSpecificKpi_AppUserDTO Employee { get; set; }
        public ItemSpecificKpi_KpiPeriodDTO KpiPeriod { get; set; }
        public ItemSpecificKpi_OrganizationDTO Organization { get; set; }
        public ItemSpecificKpi_StatusDTO Status { get; set; }
        public List<ItemSpecificKpi_ItemSpecificKpiContentDTO> ItemSpecificKpiContents { get; set; }
        public List<ItemSpecificKpi_ItemSpecificKpiTotalItemSpecificCriteriaMappingDTO> ItemSpecificKpiTotalItemSpecificCriteriaMappings { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ItemSpecificKpi_ItemSpecificKpiDTO() {}
        public ItemSpecificKpi_ItemSpecificKpiDTO(ItemSpecificKpi ItemSpecificKpi)
        {
            this.Id = ItemSpecificKpi.Id;
            this.OrganizationId = ItemSpecificKpi.OrganizationId;
            this.KpiPeriodId = ItemSpecificKpi.KpiPeriodId;
            this.StatusId = ItemSpecificKpi.StatusId;
            this.EmployeeId = ItemSpecificKpi.EmployeeId;
            this.CreatorId = ItemSpecificKpi.CreatorId;
            this.Creator = ItemSpecificKpi.Creator == null ? null : new ItemSpecificKpi_AppUserDTO(ItemSpecificKpi.Creator);
            this.Employee = ItemSpecificKpi.Employee == null ? null : new ItemSpecificKpi_AppUserDTO(ItemSpecificKpi.Employee);
            this.KpiPeriod = ItemSpecificKpi.KpiPeriod == null ? null : new ItemSpecificKpi_KpiPeriodDTO(ItemSpecificKpi.KpiPeriod);
            this.Organization = ItemSpecificKpi.Organization == null ? null : new ItemSpecificKpi_OrganizationDTO(ItemSpecificKpi.Organization);
            this.Status = ItemSpecificKpi.Status == null ? null : new ItemSpecificKpi_StatusDTO(ItemSpecificKpi.Status);
            this.ItemSpecificKpiContents = ItemSpecificKpi.ItemSpecificKpiContents?.Select(x => new ItemSpecificKpi_ItemSpecificKpiContentDTO(x)).ToList();
            this.ItemSpecificKpiTotalItemSpecificCriteriaMappings = ItemSpecificKpi.ItemSpecificKpiTotalItemSpecificCriteriaMappings?.Select(x => new ItemSpecificKpi_ItemSpecificKpiTotalItemSpecificCriteriaMappingDTO(x)).ToList();
            this.CreatedAt = ItemSpecificKpi.CreatedAt;
            this.UpdatedAt = ItemSpecificKpi.UpdatedAt;
            this.Errors = ItemSpecificKpi.Errors;
        }
    }

    public class ItemSpecificKpi_ItemSpecificKpiFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter OrganizationId { get; set; }
        public IdFilter KpiPeriodId { get; set; }
        public IdFilter StatusId { get; set; }
        public IdFilter EmployeeId { get; set; }
        public IdFilter CreatorId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public ItemSpecificKpiOrder OrderBy { get; set; }
    }
}
