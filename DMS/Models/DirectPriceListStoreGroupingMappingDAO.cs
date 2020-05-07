using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class DirectPriceListStoreGroupingMappingDAO
    {
        public long DirectPriceListId { get; set; }
        public long StoreGroupingId { get; set; }

        public virtual DirectPriceListDAO DirectPriceList { get; set; }
        public virtual StoreGroupingDAO StoreGrouping { get; set; }
    }
}
