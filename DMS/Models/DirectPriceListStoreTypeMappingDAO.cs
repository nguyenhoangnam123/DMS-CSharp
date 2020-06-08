using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class DirectPriceListStoreTypeMappingDAO
    {
        public long DirectPriceListId { get; set; }
        public long StoreTypeId { get; set; }

        public virtual DirectPriceListDAO DirectPriceList { get; set; }
        public virtual StoreTypeDAO StoreType { get; set; }
    }
}
