using DMS.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.posm.posm_report
{
    public class POSMReport_ShowingOrderContentDTO : DataDTO
    {
        public long Id { get; set; }
        public long ShowingOrderId { get; set; }
        public long ShowingItemId { get; set; }
        public long UnitOfMeasureId { get; set; }
        public decimal SalePrice { get; set; }
        public long Quantity { get; set; }
        public decimal Amount { get; set; }
        public POSMReport_ShowingItemDTO ShowingItem { get; set; }   
        public POSMReport_UnitOfMeasureDTO UnitOfMeasure { get; set; }   

        public POSMReport_ShowingOrderContentDTO() {}
        public POSMReport_ShowingOrderContentDTO(ShowingOrderContent ShowingOrderContent)
        {
            this.Id = ShowingOrderContent.Id;
            this.ShowingOrderId = ShowingOrderContent.ShowingOrderId;
            this.ShowingItemId = ShowingOrderContent.ShowingItemId;
            this.UnitOfMeasureId = ShowingOrderContent.UnitOfMeasureId;
            this.SalePrice = ShowingOrderContent.SalePrice;
            this.Quantity = ShowingOrderContent.Quantity;
            this.Amount = ShowingOrderContent.Amount;
            this.ShowingItem = ShowingOrderContent.ShowingItem == null ? null : new POSMReport_ShowingItemDTO(ShowingOrderContent.ShowingItem);
            this.UnitOfMeasure = ShowingOrderContent.UnitOfMeasure == null ? null : new POSMReport_UnitOfMeasureDTO(ShowingOrderContent.UnitOfMeasure);
            this.Errors = ShowingOrderContent.Errors;
        }
    }

    public class POSMReport_ShowingOrderContentFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public IdFilter ShowingOrderId { get; set; }
        
        public IdFilter ShowingItemId { get; set; }
        
        public IdFilter UnitOfMeasureId { get; set; }
        
        public DecimalFilter SalePrice { get; set; }
        
        public LongFilter Quantity { get; set; }
        
        public DecimalFilter Amount { get; set; }
        
        public ShowingOrderContentOrder OrderBy { get; set; }
    }
}