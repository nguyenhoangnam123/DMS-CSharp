using DMS.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace DMS.Entities
{
    public class NationType : DataEntity, IEquatable<NationType>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public bool Equals(NationType other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class NationTypeFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public List<NationTypeFilter> OrFilter { get; set; }
        public NationTypeOrder OrderBy { get; set; }
        public NationTypeSelect Selects { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum NationTypeOrder
    {
        Id = 0,
        Code = 1,
        Name = 2,
    }

    [Flags]
    public enum NationTypeSelect : long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        Name = E._2,
    }
}
