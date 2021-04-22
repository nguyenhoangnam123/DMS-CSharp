using System;
using System.Collections.Generic;
using DMS.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class KpiProductGroupingType : DataEntity,  IEquatable<KpiProductGroupingType>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        
        public bool Equals(KpiProductGroupingType other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.Code != other.Code) return false;
            if (this.Name != other.Name) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class KpiProductGroupingTypeFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public List<KpiProductGroupingTypeFilter> OrFilter { get; set; }
        public KpiProductGroupingTypeOrder OrderBy {get; set;}
        public KpiProductGroupingTypeSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum KpiProductGroupingTypeOrder
    {
        Id = 0,
        Code = 1,
        Name = 2,
    }

    [Flags]
    public enum KpiProductGroupingTypeSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        Name = E._2,
    }
}
