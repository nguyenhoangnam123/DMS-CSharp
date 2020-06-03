using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class ItemSpecificKpiContentDAO
    {
        public ItemSpecificKpiContentDAO()
        {
            ItemSpecificKpiContentItemSpecificKpiCriteriaMappings = new HashSet<ItemSpecificKpiContentItemSpecificKpiCriteriaMappingDAO>();
        }

        public long Id { get; set; }
        public long ItemSpecificKpiId { get; set; }
        public long ItemId { get; set; }

        public virtual ItemDAO Item { get; set; }
        public virtual ItemSpecificKpiDAO ItemSpecificKpi { get; set; }
        public virtual ICollection<ItemSpecificKpiContentItemSpecificKpiCriteriaMappingDAO> ItemSpecificKpiContentItemSpecificKpiCriteriaMappings { get; set; }
    }
}
