using DMS.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.promotion
{
    public class Promotion_PromotionDiscountTypeDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        

        public Promotion_PromotionDiscountTypeDTO() {}
        public Promotion_PromotionDiscountTypeDTO(PromotionDiscountType PromotionDiscountType)
        {
            
            this.Id = PromotionDiscountType.Id;
            
            this.Code = PromotionDiscountType.Code;
            
            this.Name = PromotionDiscountType.Name;
            
            this.Errors = PromotionDiscountType.Errors;
        }
    }

    public class Promotion_PromotionDiscountTypeFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public PromotionDiscountTypeOrder OrderBy { get; set; }
    }
}