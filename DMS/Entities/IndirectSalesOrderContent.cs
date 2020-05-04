using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class IndirectSalesOrderContent : DataEntity,  IEquatable<IndirectSalesOrderContent>
    {
        public long Id { get; set; }
        public long IndirectSalesOrderId { get; set; }
        public long ItemId { get; set; }
        public long UnitOfMeasureId { get; set; }
        public long Quantity { get; set; }
        public long PrimaryUnitOfMeasureId { get; set; }
        public long RequestedQuantity { get; set; }
        public long SalePrice { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public long? DiscountAmount { get; set; }
        public decimal? GeneralDiscountPercentage { get; set; }
        public long? GeneralDiscountAmount { get; set; }
        public long Amount { get; set; }
        public decimal? TaxPercentage { get; set; }
        public long? TaxAmount { get; set; }
        public IndirectSalesOrder IndirectSalesOrder { get; set; }
        public UnitOfMeasure PrimaryUnitOfMeasure { get; set; }
        public UnitOfMeasure UnitOfMeasure { get; set; }

        public bool Equals(IndirectSalesOrderContent other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class IndirectSalesOrderContentFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter IndirectSalesOrderId { get; set; }
        public IdFilter ItemId { get; set; }
        public IdFilter UnitOfMeasureId { get; set; }
        public LongFilter Quantity { get; set; }
        public IdFilter PrimaryUnitOfMeasureId { get; set; }
        public LongFilter RequestedQuantity { get; set; }
        public LongFilter SalePrice { get; set; }
        public DecimalFilter DiscountPercentage { get; set; }
        public LongFilter DiscountAmount { get; set; }
        public DecimalFilter GeneralDiscountPercentage { get; set; }
        public LongFilter GeneralDiscountAmount { get; set; }
        public LongFilter Amount { get; set; }
        public DecimalFilter TaxPercentage { get; set; }
        public LongFilter TaxAmount { get; set; }
        public List<IndirectSalesOrderContentFilter> OrFilter { get; set; }
        public IndirectSalesOrderContentOrder OrderBy {get; set;}
        public IndirectSalesOrderContentSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum IndirectSalesOrderContentOrder
    {
        Id = 0,
        IndirectSalesOrder = 1,
        Item = 2,
        UnitOfMeasure = 3,
        Quantity = 4,
        PrimaryUnitOfMeasure = 5,
        RequestedQuantity = 6,
        SalePrice = 7,
        DiscountPercentage = 8,
        DiscountAmount = 9,
        GeneralDiscountPercentage = 10,
        GeneralDiscountAmount = 11,
        Amount = 12,
        TaxPercentage = 13,
        TaxAmount = 14,
    }

    [Flags]
    public enum IndirectSalesOrderContentSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        IndirectSalesOrder = E._1,
        Item = E._2,
        UnitOfMeasure = E._3,
        Quantity = E._4,
        PrimaryUnitOfMeasure = E._5,
        RequestedQuantity = E._6,
        SalePrice = E._7,
        DiscountPercentage = E._8,
        DiscountAmount = E._9,
        GeneralDiscountPercentage = E._10,
        GeneralDiscountAmount = E._11,
        Amount = E._12,
        TaxPercentage = E._13,
        TaxAmount = E._14,
    }
}
