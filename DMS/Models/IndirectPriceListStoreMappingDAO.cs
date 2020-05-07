using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class IndirectPriceListStoreMappingDAO
    {
        public long IndirectPriceListId { get; set; }
        public long StoreId { get; set; }

        public virtual IndirectPriceListDAO IndirectPriceList { get; set; }
        public virtual StoreDAO Store { get; set; }
    }
}
