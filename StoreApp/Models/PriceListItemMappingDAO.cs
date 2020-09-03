﻿using System;
using System.Collections.Generic;

namespace StoreApp.Models
{
    public partial class PriceListItemMappingDAO
    {
        public long PriceListId { get; set; }
        public long ItemId { get; set; }
        public long Price { get; set; }

        public virtual ItemDAO Item { get; set; }
        public virtual PriceListDAO PriceList { get; set; }
    }
}
