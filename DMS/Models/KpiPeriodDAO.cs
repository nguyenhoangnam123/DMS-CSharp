using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class KpiPeriodDAO
    {
        public KpiPeriodDAO()
        {
            GeneralKpis = new HashSet<GeneralKpiDAO>();
            KpiItems = new HashSet<KpiItemDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<GeneralKpiDAO> GeneralKpis { get; set; }
        public virtual ICollection<KpiItemDAO> KpiItems { get; set; }
    }
}
