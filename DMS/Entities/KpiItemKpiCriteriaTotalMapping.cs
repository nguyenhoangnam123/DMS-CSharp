using Common;
using System;

namespace DMS.Entities
{
    public class KpiItemKpiCriteriaTotalMapping : DataEntity, IEquatable<KpiItemKpiCriteriaTotalMapping>
    {
        public long KpiItemId { get; set; }
        public long KpiCriteriaTotalId { get; set; }
        public long Value { get; set; }
        public KpiItem KpiItem { get; set; }
        public KpiCriteriaTotal KpiCriteriaTotal { get; set; }

        public bool Equals(KpiItemKpiCriteriaTotalMapping other)
        {
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
