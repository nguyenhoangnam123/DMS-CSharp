using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.promotion
{
    public class Promotion_PromotionComboDTO : DataDTO
    {
        public long Id { get; set; }
        public string Note { get; set; }
        public long PromotionPolicyId { get; set; }
        public long PromotionId { get; set; }
        public Promotion_PromotionPolicyDTO PromotionPolicy { get; set; }   
        
        public Promotion_PromotionComboDTO() {}
        public Promotion_PromotionComboDTO(PromotionCombo PromotionCombo)
        {
            this.Id = PromotionCombo.Id;
            this.Note = PromotionCombo.Note;
            this.PromotionPolicyId = PromotionCombo.PromotionPolicyId;
            this.PromotionId = PromotionCombo.PromotionId;
            this.PromotionPolicy = PromotionCombo.PromotionPolicy == null ? null : new Promotion_PromotionPolicyDTO(PromotionCombo.PromotionPolicy);
            this.Errors = PromotionCombo.Errors;
        }
    }

    public class Promotion_PromotionComboFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Note { get; set; }
        
        public IdFilter PromotionPolicyId { get; set; }
        
        public IdFilter PromotionId { get; set; }
        
        public PromotionComboOrder OrderBy { get; set; }
    }
}