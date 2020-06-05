using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class KpiItemContent : DataEntity,  IEquatable<KpiItemContent>
    {
        public long Id { get; set; }
        public long KpiItemId { get; set; }
        public long ItemId { get; set; }
        public Guid RowId { get; set; }
        public Item Item { get; set; }
        public KpiItem KpiItem { get; set; }
        public List<KpiItemContentKpiCriteriaItemMapping> KpiItemContentKpiCriteriaItemMappings { get; set; }
        public bool Equals(KpiItemContent other)
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
        public IdFilter ItemId { get; set; }
        public List<ItemSpecificKpiContentFilter> OrFilter { get; set; }
        public ItemSpecificKpiContentOrder OrderBy {get; set;}
        public ItemSpecificKpiContentSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ItemSpecificKpiContentOrder
    {
        Id = 0,
        ItemSpecificKpi = 1,
        Item = 2,
    }

    [Flags]
    public enum ItemSpecificKpiContentSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        ItemSpecificKpi = E._1,
        Item = E._2,
    }
}
