using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class DirectPriceListStoreMappingDAO
    {
        public long DirectPriceListId { get; set; }
        public long StoreId { get; set; }

        public virtual DirectPriceListDAO DirectPriceList { get; set; }
        public virtual StoreDAO Store { get; set; }
    }
}
