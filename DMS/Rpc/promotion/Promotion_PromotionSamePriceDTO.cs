using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.promotion
{
    public class Promotion_PromotionSamePriceDTO : DataDTO
    {
        public long Id { get; set; }
        public string Note { get; set; }
        public string Name { get; set; }
        public long PromotionId { get; set; }
        public decimal Price { get; set; }
        
        public Promotion_PromotionSamePriceDTO() {}
        public Promotion_PromotionSamePriceDTO(PromotionSamePrice PromotionSamePrice)
        {
            this.Id = PromotionSamePrice.Id;
            this.Note = PromotionSamePrice.Note;
            this.Name = PromotionSamePrice.Name;
            this.PromotionId = PromotionSamePrice.PromotionId;
            this.Price = PromotionSamePrice.Price;
            this.Errors = PromotionSamePrice.Errors;
        }
    }

    public class Promotion_PromotionSamePriceFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Note { get; set; }
        
        public StringFilter Name { get; set; }
        
        public IdFilter PromotionId { get; set; }
        
        public DecimalFilter Price { get; set; }
        
        public PromotionSamePriceOrder OrderBy { get; set; }
    }
}