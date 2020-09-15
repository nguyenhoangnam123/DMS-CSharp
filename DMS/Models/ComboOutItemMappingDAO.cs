using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class ComboOutItemMappingDAO
    {
        public long ComboId { get; set; }
        public long ItemId { get; set; }
        public long Quantity { get; set; }

        public virtual ComboDAO Combo { get; set; }
        public virtual ItemDAO Item { get; set; }
    }
}
