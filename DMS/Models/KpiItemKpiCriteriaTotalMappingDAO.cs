using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class KpiItemKpiCriteriaTotalMappingDAO
    {
        public long KpiItemId { get; set; }
        public long KpiCriteriaTotalId { get; set; }
        public long Value { get; set; }

        public virtual KpiCriteriaTotalDAO KpiCriteriaTotal { get; set; }
        public virtual KpiItemDAO KpiItem { get; set; }
    }
}
