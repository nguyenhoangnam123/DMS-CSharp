using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.promotion
{
    public class Promotion_PromotionDirectSalesOrderDTO : DataDTO
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
        public Promotion_PromotionDiscountTypeDTO PromotionDiscountType { get; set; }   
        
        public Promotion_PromotionDirectSalesOrderDTO() {}
        public Promotion_PromotionDirectSalesOrderDTO(PromotionDirectSalesOrder PromotionDirectSalesOrder)
        {
            this.Id = PromotionDirectSalesOrder.Id;
            this.Name = PromotionDirectSalesOrder.Name;
            this.PromotionId = PromotionDirectSalesOrder.PromotionId;
            this.Note = PromotionDirectSalesOrder.Note;
            this.FromValue = PromotionDirectSalesOrder.FromValue;
            this.ToValue = PromotionDirectSalesOrder.ToValue;
            this.PromotionDiscountTypeId = PromotionDirectSalesOrder.PromotionDiscountTypeId;
            this.DiscountPercentage = PromotionDirectSalesOrder.DiscountPercentage;
            this.DiscountValue = PromotionDirectSalesOrder.DiscountValue;
            this.PromotionDiscountType = PromotionDirectSalesOrder.PromotionDiscountType == null ? null : new Promotion_PromotionDiscountTypeDTO(PromotionDirectSalesOrder.PromotionDiscountType);
            this.Errors = PromotionDirectSalesOrder.Errors;
        }
    }

    public class Promotion_PromotionDirectSalesOrderFilterDTO : FilterDTO
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
        
        public PromotionDirectSalesOrderOrder OrderBy { get; set; }
    }
}