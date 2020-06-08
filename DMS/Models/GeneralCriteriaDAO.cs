using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class GeneralCriteriaDAO
    {
        public GeneralCriteriaDAO()
        {
            GeneralKpiCriteriaMappings = new HashSet<GeneralKpiCriteriaMappingDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<GeneralKpiCriteriaMappingDAO> GeneralKpiCriteriaMappings { get; set; }
    }
}
