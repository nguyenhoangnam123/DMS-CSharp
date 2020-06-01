using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class ItemSpecificKpiContent : DataEntity,  IEquatable<ItemSpecificKpiContent>
    {
        public long Id { get; set; }
        public long ItemSpecificKpiId { get; set; }
        public long ItemSpecificCriteriaId { get; set; }
        public long ItemId { get; set; }
        public long Value { get; set; }
        public Item Item { get; set; }
        public ItemSpecificCriteria ItemSpecificCriteria { get; set; }
        public ItemSpecificKpi ItemSpecificKpi { get; set; }

        public bool Equals(ItemSpecificKpiContent other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class ItemSpecificKpiContentFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter ItemSpecificKpiId { get; set; }
        public IdFilter ItemSpecificCriteriaId { get; set; }
        public IdFilter ItemId { get; set; }
        public LongFilter Value { get; set; }
        public List<ItemSpecificKpiContentFilter> OrFilter { get; set; }
        public ItemSpecificKpiContentOrder OrderBy {get; set;}
        public ItemSpecificKpiContentSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ItemSpecificKpiContentOrder
    {
        Id = 0,
        ItemSpecificKpi = 1,
        ItemSpecificCriteria = 2,
        Item = 3,
        Value = 4,
    }

    [Flags]
    public enum ItemSpecificKpiContentSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        ItemSpecificKpi = E._1,
        ItemSpecificCriteria = E._2,
        Item = E._3,
        Value = E._4,
    }
}
