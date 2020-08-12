﻿using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class PriceListItemHistoryDAO
    {
        public long Id { get; set; }
        public long PriceListId { get; set; }
        public long ItemId { get; set; }
        public long OldPrice { get; set; }
        public long NewPrice { get; set; }
        public long ModifierId { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Source { get; set; }

        public virtual ItemDAO Item { get; set; }
        public virtual AppUserDAO Modifier { get; set; }
        public virtual PriceListDAO PriceList { get; set; }
    }
}
