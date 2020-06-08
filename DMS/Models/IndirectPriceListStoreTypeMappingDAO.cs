using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class IndirectPriceListStoreTypeMappingDAO
    {
        public long IndirectPriceListId { get; set; }
        public long StoreTypeId { get; set; }

        public virtual IndirectPriceListDAO IndirectPriceList { get; set; }
        public virtual StoreTypeDAO StoreType { get; set; }
    }
}
