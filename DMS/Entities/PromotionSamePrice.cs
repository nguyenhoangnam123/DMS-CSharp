using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class PromotionSamePrice : DataEntity,  IEquatable<PromotionSamePrice>
    {
        public long Id { get; set; }
        public string Note { get; set; }
        public string Name { get; set; }
        public long PromotionId { get; set; }
        public decimal Price { get; set; }
        public Promotion Promotion { get; set; }

        public bool Equals(PromotionSamePrice other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class PromotionSamePriceFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Note { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter PromotionId { get; set; }
        public DecimalFilter Price { get; set; }
        public List<PromotionSamePriceFilter> OrFilter { get; set; }
        public PromotionSamePriceOrder OrderBy {get; set;}
        public PromotionSamePriceSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PromotionSamePriceOrder
    {
        Id = 0,
        Note = 1,
        Name = 2,
        Promotion = 3,
        Price = 4,
    }

    [Flags]
    public enum PromotionSamePriceSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Note = E._1,
        Name = E._2,
        Promotion = E._3,
        Price = E._4,
    }
}
