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
        public string Name { get; set; }
        public long PromotionId { get; set; }
        
        public Promotion_PromotionComboDTO() {}
        public Promotion_PromotionComboDTO(PromotionCombo PromotionCombo)
        {
            this.Id = PromotionCombo.Id;
            this.Note = PromotionCombo.Note;
            this.Name = PromotionCombo.Name;
            this.PromotionId = PromotionCombo.PromotionId;
            this.Errors = PromotionCombo.Errors;
        }
    }

    public class Promotion_PromotionComboFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Note { get; set; }
        
        public StringFilter Name { get; set; }
        
        public IdFilter PromotionId { get; set; }
        
        public PromotionComboOrder OrderBy { get; set; }
    }
}