using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.promotion
{
    public class Promotion_PromotionPolicyDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        

        public Promotion_PromotionPolicyDTO() {}
        public Promotion_PromotionPolicyDTO(PromotionPolicy PromotionPolicy)
        {
            
            this.Id = PromotionPolicy.Id;
            
            this.Code = PromotionPolicy.Code;
            
            this.Name = PromotionPolicy.Name;
            
            this.Errors = PromotionPolicy.Errors;
        }
    }

    public class Promotion_PromotionPolicyFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public PromotionPolicyOrder OrderBy { get; set; }
    }
}