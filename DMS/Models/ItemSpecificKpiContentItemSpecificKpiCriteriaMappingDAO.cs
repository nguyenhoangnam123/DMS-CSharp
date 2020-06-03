using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class ItemSpecificKpiContentItemSpecificKpiCriteriaMappingDAO
    {
        public long ItemSpecificKpiContentId { get; set; }
        public long ItemSpecificCriteriaId { get; set; }
        public long Value { get; set; }

        public virtual ItemSpecificCriteriaDAO ItemSpecificCriteria { get; set; }
        public virtual ItemSpecificKpiContentDAO ItemSpecificKpiContent { get; set; }
    }
}
