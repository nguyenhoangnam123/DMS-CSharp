using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class TotalItemSpecificCriteriaDAO
    {
        public TotalItemSpecificCriteriaDAO()
        {
            ItemSpecificKpiTotalItemSpecificCriteriaMappings = new HashSet<ItemSpecificKpiTotalItemSpecificCriteriaMappingDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ItemSpecificKpiTotalItemSpecificCriteriaMappingDAO> ItemSpecificKpiTotalItemSpecificCriteriaMappings { get; set; }
    }
}
