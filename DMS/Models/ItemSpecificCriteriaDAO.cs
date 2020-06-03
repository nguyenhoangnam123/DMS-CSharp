using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class ItemSpecificCriteriaDAO
    {
        public ItemSpecificCriteriaDAO()
        {
            ItemSpecificKpiContentItemSpecificKpiCriteriaMappings = new HashSet<ItemSpecificKpiContentItemSpecificKpiCriteriaMappingDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ItemSpecificKpiContentItemSpecificKpiCriteriaMappingDAO> ItemSpecificKpiContentItemSpecificKpiCriteriaMappings { get; set; }
    }
}
