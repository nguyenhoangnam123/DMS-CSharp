using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class PromotionStoreGroupingItemMappingDAO
    {
        public long PromotionStoreGroupingId { get; set; }
        public long itemId { get; set; }
        public string Quantity { get; set; }

        public virtual PromotionStoreGroupingDAO PromotionStoreGrouping { get; set; }
        public virtual ItemDAO item { get; set; }
    }
}
