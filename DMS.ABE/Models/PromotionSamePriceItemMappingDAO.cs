using System;
using System.Collections.Generic;

namespace DMS.ABE.Models
{
    public partial class PromotionSamePriceItemMappingDAO
    {
        public long PromotionSamePriceId { get; set; }
        public long ItemId { get; set; }
        public string Quantity { get; set; }

        public virtual ItemDAO Item { get; set; }
        public virtual PromotionSamePriceDAO PromotionSamePrice { get; set; }
    }
}
