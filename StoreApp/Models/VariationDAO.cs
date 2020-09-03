using System;
using System.Collections.Generic;

namespace StoreApp.Models
{
    public partial class VariationDAO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long VariationGroupingId { get; set; }

        public virtual VariationGroupingDAO VariationGrouping { get; set; }
    }
}
