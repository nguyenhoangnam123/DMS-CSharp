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
        public long BuyerStoreId { get; set; }
        public string PhoneNumber { get; set; }
        public string StoreAddress { get; set; }
        public string DeliveryAddress { get; set; }
        public long SellerStoreId { get; set; }
        public long SaleEmployeeId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public long IndirectSalesOrderStatusId { get; set; }
        public bool IsEditedPrice { get; set; }
        public string Note { get; set; }
        public long SubTotal { get; set; }
        public long? GeneralDiscountPercentage { get; set; }
        public long? GeneralDiscountAmount { get; set; }
        public long TotalTaxAmount { get; set; }
        public long Total { get; set; }
        public Store BuyerStore { get; set; }
        public IndirectSalesOrderStatus IndirectSalesOrderStatus { get; set; }
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
        public IdFilter BuyerStoreId { get; set; }
        public StringFilter PhoneNumber { get; set; }
        public StringFilter StoreAddress { get; set; }
        public StringFilter DeliveryAddress { get; set; }
        public IdFilter SellerStoreId { get; set; }
        public IdFilter SaleEmployeeId { get; set; }
        public DateFilter OrderDate { get; set; }
        public DateFilter DeliveryDate { get; set; }
        public IdFilter IndirectSalesOrderStatusId { get; set; }
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
        BuyerStore = 1,
        PhoneNumber = 2,
        StoreAddress = 3,
        DeliveryAddress = 4,
        SellerStore = 5,
        SaleEmployee = 6,
        OrderDate = 7,
        DeliveryDate = 8,
        IndirectSalesOrderStatus = 9,
        IsEditedPrice = 10,
        Note = 11,
        SubTotal = 12,
        GeneralDiscountPercentage = 13,
        GeneralDiscountAmount = 14,
        TotalTaxAmount = 15,
        Total = 16,
    }

    [Flags]
    public enum IndirectSalesOrderSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        BuyerStore = E._1,
        PhoneNumber = E._2,
        StoreAddress = E._3,
        DeliveryAddress = E._4,
        SellerStore = E._5,
        SaleEmployee = E._6,
        OrderDate = E._7,
        DeliveryDate = E._8,
        IndirectSalesOrderStatus = E._9,
        IsEditedPrice = E._10,
        Note = E._11,
        SubTotal = E._12,
        GeneralDiscountPercentage = E._13,
        GeneralDiscountAmount = E._14,
        TotalTaxAmount = E._15,
        Total = E._16,
    }
}
