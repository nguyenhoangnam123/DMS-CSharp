using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class ItemSpecificKpiTotalItemSpecificCriteriaMappingDAO
    {
        public long ItemSpecificKpiId { get; set; }
        public long TotalItemSpecificCriteriaId { get; set; }
        public long Value { get; set; }

        public virtual ItemSpecificKpiDAO ItemSpecificKpi { get; set; }
        public virtual TotalItemSpecificCriteriaDAO TotalItemSpecificCriteria { get; set; }
    }
}
