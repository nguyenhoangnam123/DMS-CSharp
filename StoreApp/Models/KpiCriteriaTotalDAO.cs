using System;
using System.Collections.Generic;

namespace StoreApp.Models
{
    public partial class KpiCriteriaTotalDAO
    {
        public KpiCriteriaTotalDAO()
        {
            KpiItemKpiCriteriaTotalMappings = new HashSet<KpiItemKpiCriteriaTotalMappingDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<KpiItemKpiCriteriaTotalMappingDAO> KpiItemKpiCriteriaTotalMappings { get; set; }
    }
}
