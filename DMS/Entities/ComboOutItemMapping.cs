using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class ComboOutItemMapping : DataEntity,  IEquatable<ComboOutItemMapping>
    {
        public long ComboId { get; set; }
        public long ItemId { get; set; }
        public long Quantity { get; set; }
        public Combo Combo { get; set; }
        public Item Item { get; set; }

        public bool Equals(ComboOutItemMapping other)
        {
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class ComboOutItemMappingFilter : FilterEntity
    {
        public IdFilter ComboId { get; set; }
        public IdFilter ItemId { get; set; }
        public LongFilter Quantity { get; set; }
        public List<ComboOutItemMappingFilter> OrFilter { get; set; }
        public ComboOutItemMappingOrder OrderBy {get; set;}
        public ComboOutItemMappingSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ComboOutItemMappingOrder
    {
        Combo = 0,
        Item = 1,
        Quantity = 2,
    }

    [Flags]
    public enum ComboOutItemMappingSelect:long
    {
        ALL = E.ALL,
        Combo = E._0,
        Item = E._1,
        Quantity = E._2,
    }
}
