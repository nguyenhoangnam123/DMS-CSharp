using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class Combo : DataEntity,  IEquatable<Combo>
    {
        public long Id { get; set; }
        public long PromotionComboId { get; set; }
        public string Name { get; set; }
        public long PromotionDiscountTypeId { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountValue { get; set; }
        public PromotionCombo PromotionCombo { get; set; }
        public PromotionDiscountType PromotionDiscountType { get; set; }
        public List<ComboInItemMapping> ComboInItemMappings { get; set; }
        public List<ComboOutItemMapping> ComboOutItemMappings { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public bool Equals(Combo other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class ComboFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter PromotionComboId { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter PromotionDiscountTypeId { get; set; }
        public DecimalFilter DiscountPercentage { get; set; }
        public DecimalFilter DiscountValue { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<ComboFilter> OrFilter { get; set; }
        public ComboOrder OrderBy {get; set;}
        public ComboSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ComboOrder
    {
        Id = 0,
        PromotionCombo = 1,
        Name = 2,
        PromotionDiscountType = 3,
        DiscountPercentage = 4,
        DiscountValue = 5,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum ComboSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        PromotionCombo = E._1,
        Name = E._2,
        PromotionDiscountType = E._3,
        DiscountPercentage = E._4,
        DiscountValue = E._5,
    }
}
