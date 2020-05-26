using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class GeneralKpiCriteriaMappingDAO
    {
        public long GeneralKpiId { get; set; }
        public long GeneralCriteriaId { get; set; }
        public long? M01 { get; set; }
        public long? M02 { get; set; }
        public long? M03 { get; set; }
        public long? M04 { get; set; }
        public long? M05 { get; set; }
        public long? M06 { get; set; }
        public long? M07 { get; set; }
        public long? M08 { get; set; }
        public long? M09 { get; set; }
        public long? M10 { get; set; }
        public long? M11 { get; set; }
        public long? M12 { get; set; }
        public long? Q01 { get; set; }
        public long? Q02 { get; set; }
        public long? Q03 { get; set; }
        public long? Q04 { get; set; }
        public long? Y01 { get; set; }
        public long StatusId { get; set; }

        public virtual GeneralCriteriaDAO GeneralCriteria { get; set; }
        public virtual GeneralKpiDAO GeneralKpi { get; set; }
        public virtual StatusDAO Status { get; set; }
    }
}
