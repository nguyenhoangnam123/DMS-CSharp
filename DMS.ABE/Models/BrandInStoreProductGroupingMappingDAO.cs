using System;
using System.Collections.Generic;

namespace DMS.ABE.Models
{
    public partial class BrandInStoreProductGroupingMappingDAO
    {
        public long BrandInStoreId { get; set; }
        public long ProductGroupingId { get; set; }

        public virtual BrandInStoreDAO BrandInStore { get; set; }
        public virtual ProductGroupingDAO ProductGrouping { get; set; }
    }
}
