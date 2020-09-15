﻿using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class PromotionStoreGroupingDAO
    {
        public PromotionStoreGroupingDAO()
        {
            PromotionStoreGroupingItemMappings = new HashSet<PromotionStoreGroupingItemMappingDAO>();
        }

        public long Id { get; set; }
        public long PromotionId { get; set; }
        public string Note { get; set; }
        public decimal FromValue { get; set; }
        public decimal ToValue { get; set; }
        public long PromotionDiscountTypeId { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountValue { get; set; }
        public Guid RowId { get; set; }

        public virtual PromotionDAO Promotion { get; set; }
        public virtual PromotionDiscountTypeDAO PromotionDiscountType { get; set; }
        public virtual ICollection<PromotionStoreGroupingItemMappingDAO> PromotionStoreGroupingItemMappings { get; set; }
    }
}
