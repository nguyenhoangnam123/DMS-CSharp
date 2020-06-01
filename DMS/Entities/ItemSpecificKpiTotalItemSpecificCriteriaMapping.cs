using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class ItemSpecificKpiTotalItemSpecificCriteriaMapping : DataEntity,  IEquatable<ItemSpecificKpiTotalItemSpecificCriteriaMapping>
    {
        public long ItemSpecificKpiId { get; set; }
        public long TotalItemSpecificCriteriaId { get; set; }
        public long Value { get; set; }
        public ItemSpecificKpi ItemSpecificKpi { get; set; }
        public TotalItemSpecificCriteria TotalItemSpecificCriteria { get; set; }

        public bool Equals(ItemSpecificKpiTotalItemSpecificCriteriaMapping other)
        {
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class ItemSpecificKpiTotalItemSpecificCriteriaMappingFilter : FilterEntity
    {
        public IdFilter ItemSpecificKpiId { get; set; }
        public IdFilter TotalItemSpecificCriteriaId { get; set; }
        public LongFilter Value { get; set; }
        public List<ItemSpecificKpiTotalItemSpecificCriteriaMappingFilter> OrFilter { get; set; }
        public ItemSpecificKpiTotalItemSpecificCriteriaMappingOrder OrderBy {get; set;}
        public ItemSpecificKpiTotalItemSpecificCriteriaMappingSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ItemSpecificKpiTotalItemSpecificCriteriaMappingOrder
    {
        ItemSpecificKpi = 0,
        TotalItemSpecificCriteria = 1,
        Value = 2,
    }

    [Flags]
    public enum ItemSpecificKpiTotalItemSpecificCriteriaMappingSelect:long
    {
        ALL = E.ALL,
        ItemSpecificKpi = E._0,
        TotalItemSpecificCriteria = E._1,
        Value = E._2,
    }
}
