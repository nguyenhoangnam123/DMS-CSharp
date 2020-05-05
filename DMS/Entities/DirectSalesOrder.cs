using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class DirectSalesOrder : DataEntity,  IEquatable<DirectSalesOrder>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public long BuyerStoreId { get; set; }
        public string StorePhone { get; set; }
        public string StoreAddress { get; set; }
        public string StoreDeliveryAddress { get; set; }
        public string TaxCode { get; set; }
        public long SaleEmployeeId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public long EditedPriceStatusId { get; set; }
        public string Note { get; set; }
        public long SubTotal { get; set; }
        public decimal? GeneralDiscountPercentage { get; set; }
        public long? GeneralDiscountAmount { get; set; }
        public long TotalTaxAmount { get; set; }
        public long Total { get; set; }
        public long RequestStateId { get; set; }
        public Store BuyerStore { get; set; }
        public EditedPriceStatus EditedPriceStatus { get; set; }
        public RequestState RequestState { get; set; }
        public AppUser SaleEmployee { get; set; }
        List<DirectSalesOrderContent> DirectSalesOrderContents { get; set; }
        List<DirectSalesOrderPromotion> DirectSalesOrderPromotion { get; set; }
        public bool Equals(DirectSalesOrder other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class DirectSalesOrderFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public IdFilter BuyerStoreId { get; set; }
        public StringFilter StorePhone { get; set; }
        public StringFilter StoreAddress { get; set; }
        public StringFilter StoreDeliveryAddress { get; set; }
        public StringFilter TaxCode { get; set; }
        public IdFilter SaleEmployeeId { get; set; }
        public DateFilter OrderDate { get; set; }
        public DateFilter DeliveryDate { get; set; }
        public IdFilter EditedPriceStatusId { get; set; }
        public StringFilter Note { get; set; }
        public LongFilter SubTotal { get; set; }
        public DecimalFilter GeneralDiscountPercentage { get; set; }
        public LongFilter GeneralDiscountAmount { get; set; }
        public LongFilter TotalTaxAmount { get; set; }
        public LongFilter Total { get; set; }
        public IdFilter RequestStateId { get; set; }
        public List<DirectSalesOrderFilter> OrFilter { get; set; }
        public DirectSalesOrderOrder OrderBy {get; set;}
        public DirectSalesOrderSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DirectSalesOrderOrder
    {
        Id = 0,
        Code = 1,
        BuyerStore = 2,
        StorePhone = 3,
        StoreAddress = 4,
        StoreDeliveryAddress = 5,
        TaxCode = 6,
        SaleEmployee = 7,
        OrderDate = 8,
        DeliveryDate = 9,
        EditedPriceStatus = 10,
        Note = 11,
        SubTotal = 12,
        GeneralDiscountPercentage = 13,
        GeneralDiscountAmount = 14,
        TotalTaxAmount = 15,
        Total = 16,
        RequestState = 17,
    }

    [Flags]
    public enum DirectSalesOrderSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        BuyerStore = E._2,
        StorePhone = E._3,
        StoreAddress = E._4,
        StoreDeliveryAddress = E._5,
        TaxCode = E._6,
        SaleEmployee = E._7,
        OrderDate = E._8,
        DeliveryDate = E._9,
        EditedPriceStatus = E._10,
        Note = E._11,
        SubTotal = E._12,
        GeneralDiscountPercentage = E._13,
        GeneralDiscountAmount = E._14,
        TotalTaxAmount = E._15,
        Total = E._16,
        RequestState = E._17,
    }
}
