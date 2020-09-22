using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class ComboInItemMapping : DataEntity,  IEquatable<ComboInItemMapping>
    {
        public long ComboId { get; set; }
        public long ItemId { get; set; }
        public long From { get; set; }
        public long? To { get; set; }
        public Combo Combo { get; set; }
        public Item Item { get; set; }

        public bool Equals(ComboInItemMapping other)
        {
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class ComboInItemMappingFilter : FilterEntity
    {
        public IdFilter ComboId { get; set; }
        public IdFilter ItemId { get; set; }
        public LongFilter From { get; set; }
        public LongFilter To { get; set; }
        public List<ComboInItemMappingFilter> OrFilter { get; set; }
        public ComboInItemMappingOrder OrderBy {get; set;}
        public ComboInItemMappingSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ComboInItemMappingOrder
    {
        Combo = 0,
        Item = 1,
        From = 2,
        To = 3,
    }

    [Flags]
    public enum ComboInItemMappingSelect:long
    {
        ALL = E.ALL,
        Combo = E._0,
        Item = E._1,
        From = E._2,
        To = E._3,
    }
}
