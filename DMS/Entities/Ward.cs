using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace DMS.Entities
{
    public class Ward : DataEntity, IEquatable<Ward>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long? Priority { get; set; }
        public long DistrictId { get; set; }
        public long StatusId { get; set; }
        public District District { get; set; }
        public Status Status { get; set; }

        public bool Equals(Ward other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class WardFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public LongFilter Priority { get; set; }
        public IdFilter DistrictId { get; set; }
        public IdFilter StatusId { get; set; }
        public List<WardFilter> OrFilter { get; set; }
        public WardOrder OrderBy { get; set; }
        public WardSelect Selects { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum WardOrder
    {
        Id = 0,
        Name = 1,
        Priority = 2,
        District = 3,
        Status = 4,
    }

    [Flags]
    public enum WardSelect : long
    {
        ALL = E.ALL,
        Id = E._0,
        Name = E._1,
        Priority = E._2,
        District = E._3,
        Status = E._4,
    }
}
