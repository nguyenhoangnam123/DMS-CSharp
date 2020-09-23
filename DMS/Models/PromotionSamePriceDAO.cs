﻿using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class PromotionSamePriceDAO
    {
        public PromotionSamePriceDAO()
        {
            PromotionSamePriceItemMappings = new HashSet<PromotionSamePriceItemMappingDAO>();
        }

        public long Id { get; set; }
        public string Note { get; set; }
        public long PromotionPolicyId { get; set; }
        public long PromotionId { get; set; }
        public decimal Price { get; set; }
        public Guid RowId { get; set; }

        public virtual PromotionDAO Promotion { get; set; }
        public virtual ICollection<PromotionSamePriceItemMappingDAO> PromotionSamePriceItemMappings { get; set; }
    }
}
