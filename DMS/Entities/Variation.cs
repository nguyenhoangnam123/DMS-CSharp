using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace DMS.Entities
{
    public class Variation : DataEntity, IEquatable<Variation>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long VariationGroupingId { get; set; }
        public VariationGrouping VariationGrouping { get; set; }

        public bool Equals(Variation other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class VariationFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter VariationGroupingId { get; set; }
        public List<VariationFilter> OrFilter { get; set; }
        public VariationOrder OrderBy { get; set; }
        public VariationSelect Selects { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum VariationOrder
    {
        Id = 0,
        Code = 1,
        Name = 2,
        VariationGrouping = 3,
    }

    [Flags]
    public enum VariationSelect : long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        Name = E._2,
        VariationGrouping = E._3,
    }
}
