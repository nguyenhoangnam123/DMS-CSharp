using DMS.Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DMS.Rpc.direct_sales_order
{
    public class DirectSalesOrder_DirectSalesOrderDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public long BuyerStoreId { get; set; }
        public string PhoneNumber { get; set; }
        public string StoreAddress { get; set; }
        public string DeliveryAddress { get; set; }
        public long SaleEmployeeId { get; set; }
        public long OrganizationId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public long RequestStateId { get; set; }
        public long EditedPriceStatusId { get; set; }
        public string Note { get; set; }
        public decimal SubTotal { get; set; }
        public decimal? GeneralDiscountPercentage { get; set; }
        public decimal? GeneralDiscountAmount { get; set; }
        public string PromotionCode { get; set; }
        public decimal? PromotionValue { get; set; }
        public decimal TotalTaxAmount { get; set; }
        public decimal TotalAfterTax { get; set; }
        public decimal Total { get; set; }
        public Guid RowId { get; set; }
        public DirectSalesOrder_StoreDTO BuyerStore { get; set; }
        public DirectSalesOrder_EditedPriceStatusDTO EditedPriceStatus { get; set; }
        public DirectSalesOrder_RequestStateDTO RequestState { get; set; }
        public DirectSalesOrder_AppUserDTO SaleEmployee { get; set; }
        public DirectSalesOrder_OrganizationDTO Organization { get; set; }
        public List<DirectSalesOrder_DirectSalesOrderContentDTO> DirectSalesOrderContents { get; set; }
        public List<DirectSalesOrder_DirectSalesOrderPromotionDTO> DirectSalesOrderPromotions { get; set; }
        public List<DirectSalesOrder_RequestWorkflowStepMappingDTO> RequestWorkflowStepMappings { get; set; }
        public DirectSalesOrder_DirectSalesOrderDTO() { }
        public DirectSalesOrder_DirectSalesOrderDTO(DirectSalesOrder DirectSalesOrder)
        {
            this.Id = DirectSalesOrder.Id;
            this.Code = DirectSalesOrder.Code;
            this.BuyerStoreId = DirectSalesOrder.BuyerStoreId;
            this.PhoneNumber = DirectSalesOrder.PhoneNumber;
            this.StoreAddress = DirectSalesOrder.StoreAddress;
            this.DeliveryAddress = DirectSalesOrder.DeliveryAddress;
            this.SaleEmployeeId = DirectSalesOrder.SaleEmployeeId;
            this.OrganizationId = DirectSalesOrder.OrganizationId;
            this.OrderDate = DirectSalesOrder.OrderDate;
            this.DeliveryDate = DirectSalesOrder.DeliveryDate;
            this.RequestStateId = DirectSalesOrder.RequestStateId;
            this.EditedPriceStatusId = DirectSalesOrder.EditedPriceStatusId;
            this.Note = DirectSalesOrder.Note;
            this.SubTotal = DirectSalesOrder.SubTotal;
            this.GeneralDiscountPercentage = DirectSalesOrder.GeneralDiscountPercentage;
            this.GeneralDiscountAmount = DirectSalesOrder.GeneralDiscountAmount;
            this.PromotionCode = DirectSalesOrder.PromotionCode;
            this.PromotionValue = DirectSalesOrder.PromotionValue;
            this.TotalTaxAmount = DirectSalesOrder.TotalTaxAmount;
            this.TotalAfterTax = DirectSalesOrder.TotalAfterTax;
            this.RowId = DirectSalesOrder.RowId;
            this.Total = Math.Round(DirectSalesOrder.Total, 0);
            this.BuyerStore = DirectSalesOrder.BuyerStore == null ? null : new DirectSalesOrder_StoreDTO(DirectSalesOrder.BuyerStore);
            this.EditedPriceStatus = DirectSalesOrder.EditedPriceStatus == null ? null : new DirectSalesOrder_EditedPriceStatusDTO(DirectSalesOrder.EditedPriceStatus);
            this.RequestState = DirectSalesOrder.RequestState == null ? null : new DirectSalesOrder_RequestStateDTO(DirectSalesOrder.RequestState);
            this.SaleEmployee = DirectSalesOrder.SaleEmployee == null ? null : new DirectSalesOrder_AppUserDTO(DirectSalesOrder.SaleEmployee);
            this.Organization = DirectSalesOrder.Organization == null ? null : new DirectSalesOrder_OrganizationDTO(DirectSalesOrder.Organization);
            this.DirectSalesOrderContents = DirectSalesOrder.DirectSalesOrderContents?.Select(x => new DirectSalesOrder_DirectSalesOrderContentDTO(x)).ToList();
            this.DirectSalesOrderPromotions = DirectSalesOrder.DirectSalesOrderPromotions?.Select(x => new DirectSalesOrder_DirectSalesOrderPromotionDTO(x)).ToList();
            this.RequestWorkflowStepMappings = DirectSalesOrder.RequestWorkflowStepMappings?.Select(x => new DirectSalesOrder_RequestWorkflowStepMappingDTO(x)).ToList();
            this.Errors = DirectSalesOrder.Errors;
        }
    }

    public class DirectSalesOrder_DirectSalesOrderFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public IdFilter OrganizationId { get; set; }
        public StringFilter Code { get; set; }
        public IdFilter BuyerStoreId { get; set; }
        public StringFilter PhoneNumber { get; set; }
        public StringFilter StoreAddress { get; set; }
        public StringFilter DeliveryAddress { get; set; }
        public IdFilter AppUserId { get; set; }
        public DateFilter OrderDate { get; set; }
        public DateFilter DeliveryDate { get; set; }
        public IdFilter RequestStateId { get; set; }
        public IdFilter EditedPriceStatusId { get; set; }
        public StringFilter Note { get; set; }
        public DecimalFilter SubTotal { get; set; }
        public DecimalFilter GeneralDiscountPercentage { get; set; }
        public DecimalFilter GeneralDiscountAmount { get; set; }
        public DecimalFilter TotalTaxAmount { get; set; }
        public DecimalFilter Total { get; set; }
        public IdFilter StoreStatusId { get; set; }
        public DirectSalesOrderOrder OrderBy { get; set; }
    }
}
