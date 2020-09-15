using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class ComboDAO
    {
        public ComboDAO()
        {
            ComboInItemMappings = new HashSet<ComboInItemMappingDAO>();
            ComboOutItemMappings = new HashSet<ComboOutItemMappingDAO>();
        }

        public long Id { get; set; }
        public long PromotionComboId { get; set; }
        public string Name { get; set; }
        public long PromotionDiscountTypeId { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountValue { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid RowId { get; set; }

        public virtual PromotionComboDAO PromotionCombo { get; set; }
        public virtual PromotionDiscountTypeDAO PromotionDiscountType { get; set; }
        public virtual ICollection<ComboInItemMappingDAO> ComboInItemMappings { get; set; }
        public virtual ICollection<ComboOutItemMappingDAO> ComboOutItemMappings { get; set; }
    }
}
