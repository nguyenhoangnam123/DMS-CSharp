using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class KpiPeriodDAO
    {
        public KpiPeriodDAO()
        {
            GeneralKpis = new HashSet<GeneralKpiDAO>();
            ItemSpecificKpis = new HashSet<ItemSpecificKpiDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<GeneralKpiDAO> GeneralKpis { get; set; }
        public virtual ICollection<ItemSpecificKpiDAO> ItemSpecificKpis { get; set; }
    }
}
