using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class GeneralCriteria : DataEntity,  IEquatable<GeneralCriteria>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public List<GeneralKpiCriteriaMapping> GeneralKpiCriteriaMappings { get; set; }

        public bool Equals(GeneralCriteria other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class GeneralCriteriaFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public List<GeneralCriteriaFilter> OrFilter { get; set; }
        public GeneralCriteriaOrder OrderBy {get; set;}
        public GeneralCriteriaSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum GeneralCriteriaOrder
    {
        Id = 0,
        Code = 1,
        Name = 2,
    }

    [Flags]
    public enum GeneralCriteriaSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        Name = E._2,
    }
}
