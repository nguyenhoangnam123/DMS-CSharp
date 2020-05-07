using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class IndirectPriceListItemMappingDAO
    {
        public long IndirectPriceListId { get; set; }
        public long ItemId { get; set; }
        public long Price { get; set; }

        public virtual IndirectPriceListDAO IndirectPriceList { get; set; }
        public virtual ItemDAO Item { get; set; }
    }
}
