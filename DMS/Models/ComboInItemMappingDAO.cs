using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class ComboInItemMappingDAO
    {
        public long ComboId { get; set; }
        public long ItemId { get; set; }
        public long From { get; set; }
        public long? To { get; set; }

        public virtual ComboDAO Combo { get; set; }
        public virtual ItemDAO Item { get; set; }
    }
}
