using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class IndirectSalesOrder : DataEntity,  IEquatable<IndirectSalesOrder>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public long BuyerStoreId { get; set; }
        public string PhoneNumber { get; set; }
        public string StoreAddress { get; set; }
        public string DeliveryAddress { get; set; }
        public long SellerStoreId { get; set; }
        public long SaleEmployeeId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public long IndirectSalesOrderStatusId { get; set; }
        public long EditedPriceStatusId { get; set; }
        public string Note { get; set; }
        public long SubTotal { get; set; }
        public decimal? GeneralDiscountPercentage { get; set; }
        public long? GeneralDiscountAmount { get; set; }
        public long TotalTaxAmount { get; set; }
        public long Total { get; set; }
        public Store BuyerStore { get; set; }
        public EditedPriceStatus EditedPriceStatus { get; set; }
        public AppUser SaleEmployee { get; set; }
        public Store SellerStore { get; set; }
        public List<IndirectSalesOrderContent> IndirectSalesOrderContents { get; set; }
        public List<IndirectSalesOrderPromotion> IndirectSalesOrderPromotions { get; set; }

        public bool Equals(IndirectSalesOrder other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class IndirectSalesOrderFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public IdFilter BuyerStoreId { get; set; }
        public StringFilter PhoneNumber { get; set; }
        public StringFilter StoreAddress { get; set; }
        public StringFilter DeliveryAddress { get; set; }
        public IdFilter SellerStoreId { get; set; }
        public IdFilter SaleEmployeeId { get; set; }
        public DateFilter OrderDate { get; set; }
        public DateFilter DeliveryDate { get; set; }
        public IdFilter IndirectSalesOrderStatusId { get; set; }
        public IdFilter EditedPriceStatusId { get; set; }
        public StringFilter Note { get; set; }
        public LongFilter SubTotal { get; set; }
        public LongFilter GeneralDiscountPercentage { get; set; }
        public LongFilter GeneralDiscountAmount { get; set; }
        public LongFilter TotalTaxAmount { get; set; }
        public LongFilter Total { get; set; }
        public List<IndirectSalesOrderFilter> OrFilter { get; set; }
        public IndirectSalesOrderOrder OrderBy {get; set;}
        public IndirectSalesOrderSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum IndirectSalesOrderOrder
    {
        Id = 0,
        Code = 1,
        BuyerStore = 2,
        PhoneNumber = 3,
        StoreAddress = 4,
        DeliveryAddress = 5,
        SellerStore = 6,
        SaleEmployee = 7,
        OrderDate = 8,
        DeliveryDate = 9,
        IndirectSalesOrderStatus = 10,
        EditedPriceStatus = 11,
        Note = 12,
        SubTotal = 13,
        GeneralDiscountPercentage = 14,
        GeneralDiscountAmount = 15,
        TotalTaxAmount = 16,
        Total = 17,
    }

    [Flags]
    public enum IndirectSalesOrderSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        BuyerStore = E._2,
        PhoneNumber = E._3,
        StoreAddress = E._4,
        DeliveryAddress = E._5,
        SellerStore = E._6,
        SaleEmployee = E._7,
        OrderDate = E._8,
        DeliveryDate = E._9,
        IndirectSalesOrderStatus = E._10,
        EditedPriceStatus = E._11,
        Note = E._12,
        SubTotal = E._13,
        GeneralDiscountPercentage = E._14,
        GeneralDiscountAmount = E._15,
        TotalTaxAmount = E._16,
        Total = E._17,
    }
}
