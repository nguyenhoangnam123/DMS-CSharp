using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class DirectPriceListItemMappingDAO
    {
        public long DirectPriceListId { get; set; }
        public long ItemId { get; set; }
        public long Price { get; set; }

        public virtual DirectPriceListDAO DirectPriceList { get; set; }
        public virtual ItemDAO Item { get; set; }
    }
}
