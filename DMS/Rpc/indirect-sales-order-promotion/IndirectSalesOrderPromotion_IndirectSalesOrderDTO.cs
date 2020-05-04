using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.indirect_sales_order_promotion
{
    public class IndirectSalesOrderPromotion_IndirectSalesOrderDTO : DataDTO
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
        
        public long? GeneralDiscountPercentage { get; set; }
        
        public long? GeneralDiscountAmount { get; set; }
        
        public long TotalTaxAmount { get; set; }
        
        public long Total { get; set; }
        

        public IndirectSalesOrderPromotion_IndirectSalesOrderDTO() {}
        public IndirectSalesOrderPromotion_IndirectSalesOrderDTO(IndirectSalesOrder IndirectSalesOrder)
        {
            
            this.Id = IndirectSalesOrder.Id;
            
            this.Code = IndirectSalesOrder.Code;
            
            this.BuyerStoreId = IndirectSalesOrder.BuyerStoreId;
            
            this.PhoneNumber = IndirectSalesOrder.PhoneNumber;
            
            this.StoreAddress = IndirectSalesOrder.StoreAddress;
            
            this.DeliveryAddress = IndirectSalesOrder.DeliveryAddress;
            
            this.SellerStoreId = IndirectSalesOrder.SellerStoreId;
            
            this.SaleEmployeeId = IndirectSalesOrder.SaleEmployeeId;
            
            this.OrderDate = IndirectSalesOrder.OrderDate;
            
            this.DeliveryDate = IndirectSalesOrder.DeliveryDate;
            
            this.IndirectSalesOrderStatusId = IndirectSalesOrder.IndirectSalesOrderStatusId;
            
            this.EditedPriceStatusId = IndirectSalesOrder.EditedPriceStatusId;
            
            this.Note = IndirectSalesOrder.Note;
            
            this.SubTotal = IndirectSalesOrder.SubTotal;
            
            this.GeneralDiscountPercentage = IndirectSalesOrder.GeneralDiscountPercentage;
            
            this.GeneralDiscountAmount = IndirectSalesOrder.GeneralDiscountAmount;
            
            this.TotalTaxAmount = IndirectSalesOrder.TotalTaxAmount;
            
            this.Total = IndirectSalesOrder.Total;
            
            this.Errors = IndirectSalesOrder.Errors;
        }
    }

    public class IndirectSalesOrderPromotion_IndirectSalesOrderFilterDTO : FilterDTO
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
        
        public IndirectSalesOrderOrder OrderBy { get; set; }
    }
}