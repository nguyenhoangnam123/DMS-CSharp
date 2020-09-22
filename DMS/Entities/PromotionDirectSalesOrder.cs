using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class PromotionDirectSalesOrder : DataEntity,  IEquatable<PromotionDirectSalesOrder>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long PromotionId { get; set; }
        public string Note { get; set; }
        public decimal FromValue { get; set; }
        public decimal ToValue { get; set; }
        public long PromotionDiscountTypeId { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountValue { get; set; }
        public Promotion Promotion { get; set; }
        public PromotionDiscountType PromotionDiscountType { get; set; }
        public List<PromotionDirectSalesOrderItemMapping> PromotionDirectSalesOrderItemMappings { get; set; }

        public bool Equals(PromotionDirectSalesOrder other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class PromotionDirectSalesOrderFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter PromotionId { get; set; }
        public StringFilter Note { get; set; }
        public DecimalFilter FromValue { get; set; }
        public DecimalFilter ToValue { get; set; }
        public IdFilter PromotionDiscountTypeId { get; set; }
        public DecimalFilter DiscountPercentage { get; set; }
        public DecimalFilter DiscountValue { get; set; }
        public List<PromotionDirectSalesOrderFilter> OrFilter { get; set; }
        public PromotionDirectSalesOrderOrder OrderBy {get; set;}
        public PromotionDirectSalesOrderSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PromotionDirectSalesOrderOrder
    {
        Id = 0,
        Name = 1,
        Promotion = 2,
        Note = 3,
        FromValue = 4,
        ToValue = 5,
        PromotionDiscountType = 6,
        DiscountPercentage = 7,
        DiscountValue = 8,
    }

    [Flags]
    public enum PromotionDirectSalesOrderSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Name = E._1,
        Promotion = E._2,
        Note = E._3,
        FromValue = E._4,
        ToValue = E._5,
        PromotionDiscountType = E._6,
        DiscountPercentage = E._7,
        DiscountValue = E._8,
    }
}
