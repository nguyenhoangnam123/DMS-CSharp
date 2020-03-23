using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class StoreGrouping : DataEntity,  IEquatable<StoreGrouping>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long? ParentId { get; set; }
        public string Path { get; set; }
        public long Level { get; set; }
        public bool IsActive { get; set; }
        public List<Store> Stores { get; set; }

        public bool Equals(StoreGrouping other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class StoreGroupingFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter ParentId { get; set; }
        public StringFilter Path { get; set; }
        public LongFilter Level { get; set; }
        public List<StoreGroupingFilter> OrFilter { get; set; }
        public StoreGroupingOrder OrderBy {get; set;}
        public StoreGroupingSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum StoreGroupingOrder
    {
        Id = 0,
        Code = 1,
        Name = 2,
        Parent = 3,
        Path = 4,
        Level = 5,
        IsActive = 6,
    }

    [Flags]
    public enum StoreGroupingSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        Name = E._2,
        Parent = E._3,
        Path = E._4,
        Level = E._5,
        IsActive = E._6,
    }
}
