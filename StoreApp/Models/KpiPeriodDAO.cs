using System;
using System.Collections.Generic;

namespace StoreApp.Models
{
    public partial class KpiPeriodDAO
    {
        public KpiPeriodDAO()
        {
            KpiGeneralContentKpiPeriodMappings = new HashSet<KpiGeneralContentKpiPeriodMappingDAO>();
            KpiItems = new HashSet<KpiItemDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<KpiGeneralContentKpiPeriodMappingDAO> KpiGeneralContentKpiPeriodMappings { get; set; }
        public virtual ICollection<KpiItemDAO> KpiItems { get; set; }
    }
}
