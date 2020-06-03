using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DMS.Entities
{
    public class KpiItemContentKpiCriteriaItemMapping : DataEntity,  IEquatable<KpiItemContentKpiCriteriaItemMapping>
    {
        public long KpiItemContentId { get; set; }
        public long KpiCriteriaItemId { get; set; }
        public long Value { get; set; }
        public KpiCriteriaItem KpiItemCriteria { get; set; }
        public KpiItemContent KpiItemContent { get; set; }

        public bool Equals(KpiItemContentKpiCriteriaItemMapping other)
        {
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
