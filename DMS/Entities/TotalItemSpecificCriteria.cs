using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class TotalItemSpecificCriteria : DataEntity,  IEquatable<TotalItemSpecificCriteria>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public List<ItemSpecificKpiTotalItemSpecificCriteriaMapping> ItemSpecificKpiTotalItemSpecificCriteriaMappings { get; set; }

        public bool Equals(TotalItemSpecificCriteria other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class TotalItemSpecificCriteriaFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public List<TotalItemSpecificCriteriaFilter> OrFilter { get; set; }
        public TotalItemSpecificCriteriaOrder OrderBy {get; set;}
        public TotalItemSpecificCriteriaSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TotalItemSpecificCriteriaOrder
    {
        Id = 0,
        Code = 1,
        Name = 2,
    }

    [Flags]
    public enum TotalItemSpecificCriteriaSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        Name = E._2,
    }
}
