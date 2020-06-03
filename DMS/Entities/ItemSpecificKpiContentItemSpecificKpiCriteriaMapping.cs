using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class ItemSpecificKpiContentItemSpecificKpiCriteriaMapping : DataEntity,  IEquatable<ItemSpecificKpiContentItemSpecificKpiCriteriaMapping>
    {
        public long ItemSpecificKpiContentId { get; set; }
        public long ItemSpecificCriteriaId { get; set; }
        public long Value { get; set; }
        public ItemSpecificCriteria ItemSpecificCriteria { get; set; }
        public ItemSpecificKpiContent ItemSpecificKpiContent { get; set; }

        public bool Equals(ItemSpecificKpiContentItemSpecificKpiCriteriaMapping other)
        {
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class ItemSpecificKpiContentItemSpecificKpiCriteriaMappingFilter : FilterEntity
    {
        public IdFilter ItemSpecificKpiContentId { get; set; }
        public IdFilter ItemSpecificCriteriaId { get; set; }
        public LongFilter Value { get; set; }
        public List<ItemSpecificKpiContentItemSpecificKpiCriteriaMappingFilter> OrFilter { get; set; }
        public ItemSpecificKpiContentItemSpecificKpiCriteriaMappingOrder OrderBy {get; set;}
        public ItemSpecificKpiContentItemSpecificKpiCriteriaMappingSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ItemSpecificKpiContentItemSpecificKpiCriteriaMappingOrder
    {
        ItemSpecificKpiContent = 0,
        ItemSpecificCriteria = 1,
        Value = 2,
    }

    [Flags]
    public enum ItemSpecificKpiContentItemSpecificKpiCriteriaMappingSelect:long
    {
        ALL = E.ALL,
        ItemSpecificKpiContent = E._0,
        ItemSpecificCriteria = E._1,
        Value = E._2,
    }
}
