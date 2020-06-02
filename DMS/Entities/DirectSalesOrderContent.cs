using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class DirectSalesOrderContent : DataEntity,  IEquatable<DirectSalesOrderContent>
    {
        public long Id { get; set; }
        public long DirectSalesOrderId { get; set; }
        public long ItemId { get; set; }
        public long UnitOfMeasureId { get; set; }
        public long Quantity { get; set; }
        public long PrimaryUnitOfMeasureId { get; set; }
        public long RequestedQuantity { get; set; }
        public long Price { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public long? DiscountAmount { get; set; }
        public decimal? GeneralDiscountPercentage { get; set; }
        public long? GeneralDiscountAmount { get; set; }
        public decimal? TaxPercentage { get; set; }
        public long? TaxAmount { get; set; }
        public long Amount { get; set; }
        public long? Factor { get; set; }
        public DirectSalesOrder DirectSalesOrder { get; set; }
        public Item Item { get; set; }
        public UnitOfMeasure PrimaryUnitOfMeasure { get; set; }
        public UnitOfMeasure UnitOfMeasure { get; set; }
        
        public bool Equals(DirectSalesOrderContent other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class DirectSalesOrderContentFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter DirectSalesOrderId { get; set; }
        public IdFilter ProductId { get; set; }
        public IdFilter ItemId { get; set; }
        public IdFilter UnitOfMeasureId { get; set; }
        public LongFilter Quantity { get; set; }
        public IdFilter PrimaryUnitOfMeasureId { get; set; }
        public LongFilter RequestedQuantity { get; set; }
        public LongFilter Price { get; set; }
        public DecimalFilter DiscountPercentage { get; set; }
        public LongFilter DiscountAmount { get; set; }
        public DecimalFilter GeneralDiscountPercentage { get; set; }
        public LongFilter GeneralDiscountAmount { get; set; }
        public DecimalFilter TaxPercentage { get; set; }
        public LongFilter TaxAmount { get; set; }
        public LongFilter Amount { get; set; }
        public List<DirectSalesOrderContentFilter> OrFilter { get; set; }
        public DirectSalesOrderContentOrder OrderBy {get; set;}
        public DirectSalesOrderContentSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DirectSalesOrderContentOrder
    {
        Id = 0,
        DirectSalesOrder = 1,
        Item = 2,
        UnitOfMeasure = 3,
        Quantity = 4,
        PrimaryUnitOfMeasure = 5,
        RequestedQuantity = 6,
        Price = 7,
        DiscountPercentage = 8,
        DiscountAmount = 9,
        GeneralDiscountPercentage = 10,
        GeneralDiscountAmount = 11,
        TaxPercentage = 12,
        TaxAmount = 13,
        Amount = 14,
        Factor = 15,
    }

    [Flags]
    public enum DirectSalesOrderContentSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        DirectSalesOrder = E._1,
        Item = E._2,
        UnitOfMeasure = E._3,
        Quantity = E._4,
        PrimaryUnitOfMeasure = E._5,
        RequestedQuantity = E._6,
        Price = E._7,
        DiscountPercentage = E._8,
        DiscountAmount = E._9,
        GeneralDiscountPercentage = E._10,
        GeneralDiscountAmount = E._11,
        TaxPercentage = E._12,
        TaxAmount = E._13,
        Amount = E._14,
        Factor = E._15
    }
}
