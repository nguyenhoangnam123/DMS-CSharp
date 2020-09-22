using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class PromotionStore : DataEntity,  IEquatable<PromotionStore>
    {
        public long Id { get; set; }
        public long PromotionId { get; set; }
        public string Note { get; set; }
        public decimal FromValue { get; set; }
        public decimal ToValue { get; set; }
        public long PromotionDiscountTypeId { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountValue { get; set; }
        public Promotion Promotion { get; set; }
        public PromotionDiscountType PromotionDiscountType { get; set; }
        public List<PromotionStoreItemMapping> PromotionStoreItemMappings { get; set; }

        public bool Equals(PromotionStore other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class PromotionStoreFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter PromotionId { get; set; }
        public StringFilter Note { get; set; }
        public DecimalFilter FromValue { get; set; }
        public DecimalFilter ToValue { get; set; }
        public IdFilter PromotionDiscountTypeId { get; set; }
        public DecimalFilter DiscountPercentage { get; set; }
        public DecimalFilter DiscountValue { get; set; }
        public List<PromotionStoreFilter> OrFilter { get; set; }
        public PromotionStoreOrder OrderBy {get; set;}
        public PromotionStoreSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PromotionStoreOrder
    {
        Id = 0,
        Promotion = 1,
        Note = 2,
        FromValue = 3,
        ToValue = 4,
        PromotionDiscountType = 5,
        DiscountPercentage = 6,
        DiscountValue = 7,
    }

    [Flags]
    public enum PromotionStoreSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Promotion = E._1,
        Note = E._2,
        FromValue = E._3,
        ToValue = E._4,
        PromotionDiscountType = E._5,
        DiscountPercentage = E._6,
        DiscountValue = E._7,
    }
}
