using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.promotion
{
    public class Promotion_PromotionStoreDTO : DataDTO
    {
        public long Id { get; set; }
        public long PromotionId { get; set; }
        public string Note { get; set; }
        public decimal FromValue { get; set; }
        public decimal ToValue { get; set; }
        public long PromotionDiscountTypeId { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountValue { get; set; }
        public Promotion_PromotionDiscountTypeDTO PromotionDiscountType { get; set; }   
        
        public Promotion_PromotionStoreDTO() {}
        public Promotion_PromotionStoreDTO(PromotionStore PromotionStore)
        {
            this.Id = PromotionStore.Id;
            this.PromotionId = PromotionStore.PromotionId;
            this.Note = PromotionStore.Note;
            this.FromValue = PromotionStore.FromValue;
            this.ToValue = PromotionStore.ToValue;
            this.PromotionDiscountTypeId = PromotionStore.PromotionDiscountTypeId;
            this.DiscountPercentage = PromotionStore.DiscountPercentage;
            this.DiscountValue = PromotionStore.DiscountValue;
            this.PromotionDiscountType = PromotionStore.PromotionDiscountType == null ? null : new Promotion_PromotionDiscountTypeDTO(PromotionStore.PromotionDiscountType);
            this.Errors = PromotionStore.Errors;
        }
    }

    public class Promotion_PromotionStoreFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public IdFilter PromotionId { get; set; }
        
        public StringFilter Note { get; set; }
        
        public DecimalFilter FromValue { get; set; }
        
        public DecimalFilter ToValue { get; set; }
        
        public IdFilter PromotionDiscountTypeId { get; set; }
        
        public DecimalFilter DiscountPercentage { get; set; }
        
        public DecimalFilter DiscountValue { get; set; }
        
        public PromotionStoreOrder OrderBy { get; set; }
    }
}