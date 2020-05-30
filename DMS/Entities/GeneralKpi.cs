using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class GeneralKpi : DataEntity,  IEquatable<GeneralKpi>
    {
        public long Id { get; set; }
        public long OrganizationId { get; set; }
        public long EmployeeId { get; set; }
        public long KpiPeriodId { get; set; }
        public long StatusId { get; set; }
        public long CreatorId { get; set; }
        public KpiPeriod KpiPeriod { get; set; }
        public Organization Organization { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public bool Equals(GeneralKpi other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class GeneralKpiFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter OrganizationId { get; set; }
        public IdFilter EmployeeId { get; set; }
        public IdFilter KpiPeriodId { get; set; }
        public IdFilter StatusId { get; set; }
        public IdFilter CreatorId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<GeneralKpiFilter> OrFilter { get; set; }
        public GeneralKpiOrder OrderBy {get; set;}
        public GeneralKpiSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum GeneralKpiOrder
    {
        Id = 0,
        Organization = 1,
        Employee = 2,
        KpiPeriod = 3,
        Status = 4,
        Creator = 5,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum GeneralKpiSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Organization = E._1,
        Employee = E._2,
        KpiPeriod = E._3,
        Status = E._4,
        Creator = E._5,
    }
}
