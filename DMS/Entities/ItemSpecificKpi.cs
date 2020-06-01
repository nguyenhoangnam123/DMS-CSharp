using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class ItemSpecificKpi : DataEntity,  IEquatable<ItemSpecificKpi>
    {
        public long Id { get; set; }
        public long OrganizationId { get; set; }
        public long KpiPeriodId { get; set; }
        public long StatusId { get; set; }
        public long EmployeeId { get; set; }
        public long CreatorId { get; set; }
        public AppUser Creator { get; set; }
        public AppUser Employee { get; set; }
        public KpiPeriod KpiPeriod { get; set; }
        public Organization Organization { get; set; }
        public Status Status { get; set; }
        public List<long> EmployeeIds { get; set; }
        public List<ItemSpecificKpiContent> ItemSpecificKpiContents { get; set; }
        public List<ItemSpecificKpiTotalItemSpecificCriteriaMapping> ItemSpecificKpiTotalItemSpecificCriteriaMappings { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid RowId { get; set; }
        public bool Equals(ItemSpecificKpi other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class ItemSpecificKpiFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter OrganizationId { get; set; }
        public IdFilter KpiPeriodId { get; set; }
        public IdFilter StatusId { get; set; }
        public IdFilter EmployeeId { get; set; }
        public IdFilter CreatorId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<ItemSpecificKpiFilter> OrFilter { get; set; }
        public ItemSpecificKpiOrder OrderBy {get; set;}
        public ItemSpecificKpiSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ItemSpecificKpiOrder
    {
        Id = 0,
        Organization = 1,
        KpiPeriod = 2,
        Status = 3,
        Employee = 4,
        Creator = 5,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum ItemSpecificKpiSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Organization = E._1,
        KpiPeriod = E._2,
        Status = E._3,
        Employee = E._4,
        Creator = E._5,
    }
}
