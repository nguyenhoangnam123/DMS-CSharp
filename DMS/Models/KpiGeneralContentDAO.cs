using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class KpiGeneralContentDAO
    {
        public KpiGeneralContentDAO()
        {
            KpiGeneralContentKpiPeriodMappings = new HashSet<KpiGeneralContentKpiPeriodMappingDAO>();
        }

        public long Id { get; set; }
        public long GeneralKpiId { get; set; }
        public long GeneralCriteriaId { get; set; }
        public long StatusId { get; set; }

        public virtual KpiCriteriaGeneralDAO GeneralCriteria { get; set; }
        public virtual KpiGeneralDAO GeneralKpi { get; set; }
        public virtual StatusDAO Status { get; set; }
        public virtual ICollection<KpiGeneralContentKpiPeriodMappingDAO> KpiGeneralContentKpiPeriodMappings { get; set; }
    }
}
