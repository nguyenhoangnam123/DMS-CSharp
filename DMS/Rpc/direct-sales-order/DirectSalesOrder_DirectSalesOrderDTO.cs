using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.direct_sales_order
{
    public class DirectSalesOrder_DirectSalesOrderDTO : DataDTO
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
        public DirectSalesOrder_StoreDTO BuyerStore { get; set; }
        public DirectSalesOrder_EditedPriceStatusDTO EditedPriceStatus { get; set; }
        public DirectSalesOrder_RequestStateDTO RequestState { get; set; }
        public DirectSalesOrder_AppUserDTO SaleEmployee { get; set; }
        public List<DirectSalesOrder_DirectSalesOrderContentDTO> DirectSalesOrderContents { get; set; }
        public List<DirectSalesOrder_DirectSalesOrderPromotionDTO> DirectSalesOrderPromotions { get; set; }
        public DirectSalesOrder_DirectSalesOrderDTO() {}
        public DirectSalesOrder_DirectSalesOrderDTO(DirectSalesOrder DirectSalesOrder)
        {
            this.Id = DirectSalesOrder.Id;
            this.Code = DirectSalesOrder.Code;
            this.BuyerStoreId = DirectSalesOrder.BuyerStoreId;
            this.StorePhone = DirectSalesOrder.StorePhone;
            this.StoreAddress = DirectSalesOrder.StoreAddress;
            this.StoreDeliveryAddress = DirectSalesOrder.StoreDeliveryAddress;
            this.TaxCode = DirectSalesOrder.TaxCode;
            this.SaleEmployeeId = DirectSalesOrder.SaleEmployeeId;
            this.OrderDate = DirectSalesOrder.OrderDate;
            this.DeliveryDate = DirectSalesOrder.DeliveryDate;
            this.EditedPriceStatusId = DirectSalesOrder.EditedPriceStatusId;
            this.Note = DirectSalesOrder.Note;
            this.SubTotal = DirectSalesOrder.SubTotal;
            this.GeneralDiscountPercentage = DirectSalesOrder.GeneralDiscountPercentage;
            this.GeneralDiscountAmount = DirectSalesOrder.GeneralDiscountAmount;
            this.TotalTaxAmount = DirectSalesOrder.TotalTaxAmount;
            this.Total = DirectSalesOrder.Total;
            this.RequestStateId = DirectSalesOrder.RequestStateId;
            this.BuyerStore = DirectSalesOrder.BuyerStore == null ? null : new DirectSalesOrder_StoreDTO(DirectSalesOrder.BuyerStore);
            this.EditedPriceStatus = DirectSalesOrder.EditedPriceStatus == null ? null : new DirectSalesOrder_EditedPriceStatusDTO(DirectSalesOrder.EditedPriceStatus);
            this.RequestState = DirectSalesOrder.RequestState == null ? null : new DirectSalesOrder_RequestStateDTO(DirectSalesOrder.RequestState);
            this.SaleEmployee = DirectSalesOrder.SaleEmployee == null ? null : new DirectSalesOrder_AppUserDTO(DirectSalesOrder.SaleEmployee);
            this.DirectSalesOrderContents = DirectSalesOrder.DirectSalesOrderContents?.Select(x => new DirectSalesOrder_DirectSalesOrderContentDTO(x)).ToList();
            this.DirectSalesOrderPromotions = DirectSalesOrder.DirectSalesOrderPromotions?.Select(x => new DirectSalesOrder_DirectSalesOrderPromotionDTO(x)).ToList();
            this.Errors = DirectSalesOrder.Errors;
        }
    }

    public class DirectSalesOrder_DirectSalesOrderFilterDTO : FilterDTO
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
        public DirectSalesOrderOrder OrderBy { get; set; }
    }
}
