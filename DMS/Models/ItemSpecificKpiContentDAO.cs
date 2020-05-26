using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class ItemSpecificKpiContentDAO
    {
        public long Id { get; set; }
        public long ItemSpecificKpiId { get; set; }
        public long ItemSpecificCriteriaId { get; set; }
        public long ItemId { get; set; }
        public long Value { get; set; }

        public virtual ItemDAO Item { get; set; }
        public virtual ItemSpecificCriteriaDAO ItemSpecificCriteria { get; set; }
        public virtual ItemSpecificKpiDAO ItemSpecificKpi { get; set; }
    }
}
